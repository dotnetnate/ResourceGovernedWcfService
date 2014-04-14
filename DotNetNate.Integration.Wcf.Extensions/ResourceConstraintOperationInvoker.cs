using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Implementation of <see cref="IOperationInvoker"/> that inspects operations for adherence
    /// to resource utilization constraints and terminates them if the constraints are exceeded.
    /// 
    /// Must be used in conjunction with the <see cref="IsolatedAppDomainBehavior"/> to provide 
    /// contextual information about the application domain that's hosting the operation.
    /// </summary>
    public class ResourceConstraintOperationInvoker
      : IOperationInvoker
    {
        private static ReaderWriterLockSlim                                                                         _cacheLock = new ReaderWriterLockSlim();
        private static object                                                                                       _syncObj = new object();
        private static bool                                                                                         _monitoringThreadInitialized = false;
        private static bool                                                                                         _monitoringEnabled = true;
        private IOperationInvoker                                                                                   _baseInvoker;
        private static Dictionary<string, Tuple<AppDomain, OperationContext, ResourceConstraintPolicy, DateTime>>   _cache = new Dictionary<string, Tuple<AppDomain, OperationContext, ResourceConstraintPolicy, DateTime>>();
        private ResourceConstraintPolicy                                                                            _policy;


        private const int MONITOR_THREAD_SCAN_INTERVAL = 1000;

        /// <summary>
        /// Creates a new instance of <see cref="ResourceConstraintOperationInvoker"/>.
        /// </summary>
        /// <param name="baseInvoker"></param>
        public ResourceConstraintOperationInvoker(IOperationInvoker baseInvoker, ResourceConstraintPolicy resourceConstraintPolicy)
        {
            _baseInvoker = baseInvoker;
            _policy = resourceConstraintPolicy;   
        }
        /// <summary>
        /// Initializes the monitoring thread for a given host.
        /// </summary>
        /// <param name="host"></param>
        private static void InitializeMonitoringThread(ServiceHostBase host)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteMonitor), host);
        }
        /// <summary>
        /// Evaluates the application domain to see if it has exceeded the defined usage constraints.
        /// </summary>
        /// <param name="targetDomain">The application domain to inspect.</param>
        /// <returns>Returns <c>true</c> if the constraints have been exceeded, <c>false</c> otherwise</returns>
        private static bool HasAppDomainExceededUsageConstraints(AppDomain targetDomain, ResourceConstraintPolicy policy, DateTime startTime)
        {
            bool retVal = false;
                       
            retVal |= targetDomain.MonitoringTotalProcessorTime > TimeSpan.FromMilliseconds(policy.CpuUsageConstraint);
            retVal |= targetDomain.MonitoringTotalAllocatedMemorySize > policy.MemoryUsageConstraint;
            retVal |= (DateTime.Now - startTime) > policy.ExecutionTimeConstraint;

            return retVal;
        }

        /// <summary>
        /// Monitor thread implementation.
        /// </summary>
        /// <param name="state">The state to provide to the monitor function, in this case the service host instance.</param>
        private static void ExecuteMonitor(object state)
        {
            ServiceHostBase stateAsServiceHost;

            stateAsServiceHost = (ServiceHostBase)state;

            stateAsServiceHost.Closing += delegate(object o, EventArgs e) { _monitoringEnabled = false; };

            while (_monitoringEnabled)
            {               
                _cacheLock.EnterUpgradeableReadLock();

                foreach (var item in _cache)
                {
                    if (HasAppDomainExceededUsageConstraints(item.Value.Item1, item.Value.Item3, item.Value.Item4))
                    {
                        _cacheLock.EnterWriteLock();

                        AppDomain currentDomain = _cache[item.Key].Item1;

                        item.Value.Item2.RequestContext.Abort();
                      
                        WriteEventLogInformationForShutdown(item.Value.Item1);

                        // naive implementation, we'll just let it process one every cycle, then go back to others; 
                        // generally you can copy the collection, then loop over that and remove items from the source
                        // or use PLINQ, which has other issues for consideration
                        _cache.Remove(item.Key); 

                        AppDomainUnloader.Unload(currentDomain);

                        _cacheLock.ExitWriteLock();

                        break;
                    }
                }

                _cacheLock.ExitUpgradeableReadLock();
                
                Thread.Sleep(MONITOR_THREAD_SCAN_INTERVAL);
            }
        }

        private static void WriteEventLogInformationForShutdown(AppDomain domain)
        {
            EventLog applicationLog;

            applicationLog = new EventLog("Application");

            applicationLog.Source = "Your Source Here";

            applicationLog.WriteEntry(
                string.Format("Application domain was shut down due to resource usage in violation of policy.\r\nThe application domain {0} was using {1} bytes of memory and consumed {2} CPU time", domain.FriendlyName, domain.MonitoringSurvivedMemorySize, domain.MonitoringTotalProcessorTime),                
                EventLogEntryType.Error);

            applicationLog.Dispose();
        }

        /// <summary>
        /// Initializes the monitor thread if it hasn't already been started.
        /// </summary>
        private static void EnsureMonitorThreadIsRunning()
        {
            if (!_monitoringThreadInitialized)
            {
                lock (_syncObj)
                {
                    if (!_monitoringThreadInitialized)
                    {
                        InitializeMonitoringThread(OperationContext.Current.Host);
                    }
                }
            }
        }
        /// <summary>
        /// Implementation of <see cref="IOperationInvoker.AllocateInputs"/>.
        /// </summary>
        /// <returns></returns>
        public object[] AllocateInputs()
        {
            EnsureMonitorThreadIsRunning();

            return this._baseInvoker.AllocateInputs();
        }
        /// <summary>
        /// Implementation of <see cref="IOperationInvoker.Invoke(string,object[],out object[])"/>.
        /// </summary>
        /// <param name="instance">The instance the operation is being invoked against.</param>
        /// <param name="inputs">The inputs for the operation.</param>
        /// <param name="outputs">The outputs of the operation.</param>
        /// <returns>Returns an <see cref="System.Object"/> containing the result of the call.</returns>
        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            try
            {
                _cacheLock.EnterWriteLock();

                _cache.Add(IsolatedAppDomainInstanceContext.Current.HostingDomain.FriendlyName, new Tuple<AppDomain, OperationContext, ResourceConstraintPolicy,DateTime>(IsolatedAppDomainInstanceContext.Current.HostingDomain, OperationContext.Current, _policy, DateTime.Now));
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }            

            return _baseInvoker.Invoke(instance, inputs, out  outputs);
        }


        /// <summary>
        /// Implementation of <see cref="IOperationInvoker.InvokeBegin"/>
        /// </summary>
        /// <param name="instance">The instance the operation is being invoked against.</param>
        /// <param name="inputs">The inputs for the operation.</param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return _baseInvoker.InvokeBegin(instance, inputs, callback, state);
        }
        /// <summary>
        /// Implementation of <see cref="IOperationInvoker.InvokeEnd"/>
        /// </summary>
        /// <param name="instance">The instance the operation is being invoked against.</param>
        /// <param name="outputs">The outputs of the operation.</param>
        /// <param name="result">The state container for of the async operation.</param>
        /// <returns></returns>
        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return _baseInvoker.InvokeEnd(instance, out outputs, result);
        }
        /// <summary>
        /// Implementation of <see cref="IOperationInvoker.IsSynchronous"/>.
        /// </summary>
        public bool IsSynchronous
        {
            get { return _baseInvoker.IsSynchronous; }
        }


        /// <summary>
        /// Handles the unload event of an app domain to remove it from the cache so it is
        /// no longer analyzed during the usage sweep.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void RemoveDomainFromCache(string domainFriendlyName)
        {
            try
            {
                _cacheLock.EnterWriteLock();

                _cache.Remove(domainFriendlyName);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
    }
}