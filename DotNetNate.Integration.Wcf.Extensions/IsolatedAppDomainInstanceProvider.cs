using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Implementation of <see cref="IInstanceProvider"/> that hosts each service call in its own ApplicationDomain.
    /// Note that this should be judiciously used for services that utilize static members.
    /// </summary>
    public class IsolatedAppDomainInstanceProvider
        : IInstanceProvider
    {
        private readonly Type _serviceType;     
        
        /// <summary>
        /// Creates a new instance of <see cref="IsolatedAppDomainInstanceProvider"/>.
        /// </summary>
        /// <param name="serviceType"></param>
        public IsolatedAppDomainInstanceProvider(Type serviceType)
        {
            AppDomain.MonitoringIsEnabled = true;

            _serviceType = serviceType;
        }
        /// <summary>
        /// Implementation of <see cref="IInstanceProvider.GetInstance(InstanceContext,Message)"/>
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            AppDomain   hostDomain;
            object      retVal;
            
            hostDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(),
                AppDomain.CurrentDomain.Evidence,
                new AppDomainSetup
                {
                    ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    CachePath = AppDomain.CurrentDomain.SetupInformation.CachePath,
                    DynamicBase = AppDomain.CurrentDomain.SetupInformation.DynamicBase,
                    PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath,
                    PrivateBinPathProbe = AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe,
                });

            
            retVal = hostDomain.CreateInstanceFromAndUnwrap(_serviceType.Assembly.CodeBase, _serviceType.FullName);

            IsolatedAppDomainInstanceContext.Current.HostingDomain = hostDomain;

            return retVal;
        }
        /// <summary>
        /// Implementation of <see cref="IInstanceProvider.GetInstance(InstanceContext)"/>
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }
        /// <summary>
        /// Implementation of <see cref="IInstanceProvider.ReleaseInstance"/>.
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <param name="instance"></param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            ResourceConstraintOperationInvoker.RemoveDomainFromCache(IsolatedAppDomainInstanceContext.Current.HostingDomain.FriendlyName);

            AppDomain.Unload(IsolatedAppDomainInstanceContext.Current.HostingDomain);

            IsolatedAppDomainInstanceContext.Current.HostingDomain = null;
        }
    }




}