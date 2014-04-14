using DotNetNate.Integration.Wcf.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Constrains an operation to operate within specified resource constraints.
    /// Must be used in conjunction with <see cref="IsolatedAppDomainBehavior"/> at the service level.
    /// </summary>
    public class ResourceConstraintOperationBehavior
        : Attribute, IOperationBehavior
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ResourceConstraintOperationBehavior"/>.
        /// </summary>
        /// <param name="memoryUsageLimit">The memory usage limit, in bytes, of the operation.</param>
        /// <param name="processorUsageLimit">The processor usage limit, in milliseconds, of the operation.</param>
        /// <param name="maximumAllowedExecutionTime"></param>
        public ResourceConstraintOperationBehavior(long memoryUsageLimit, double processorUsageLimit, double maximumAllowedExecutionTime )
        {
            MemoryUsageLimit = memoryUsageLimit;
            ProcessorUsageLimit = processorUsageLimit;
            MaximumAllowedExecutionTime = TimeSpan.FromMilliseconds(maximumAllowedExecutionTime);
        }
        public ResourceConstraintOperationBehavior(string policyName)
        {
            PolicyName = policyName;
        }
        public ResourceConstraintOperationBehavior()
        {
            // this utilizes the mapping constructs
        }
        #endregion

        #region Methods
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           // not implemented
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
           // not implemented
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new ResourceConstraintOperationInvoker(dispatchOperation.Invoker, GetResourceConstraintPolicyForOperation(operationDescription));               
        }

        public void Validate(OperationDescription operationDescription)
        {
           // not implemented
        }

        private ResourceConstraintPolicy GetResourceConstraintPolicyForOperation(OperationDescription operationDescription)
        {            
            string                      serviceContractTypeName;
            string                      operationName;
            string                      selectedPolicyName;
          
            if( MemoryUsageLimit != null && ProcessorUsageLimit != null && MaximumAllowedExecutionTime != TimeSpan.MaxValue && MaximumAllowedExecutionTime >= TimeSpan.Zero )
            {
                return new ResourceConstraintPolicy {   CpuUsageConstraint = ProcessorUsageLimit.HasValue ? (double)ProcessorUsageLimit : double.MaxValue
                                                        , MemoryUsageConstraint = MemoryUsageLimit.HasValue ? (long)MemoryUsageLimit : long.MaxValue 
                                                        , ExecutionTimeConstraint = MaximumAllowedExecutionTime};
            }

            if (PolicyName != null)
            {
                selectedPolicyName = PolicyName;
            }
            else
            {
                serviceContractTypeName = operationDescription.DeclaringContract.ContractType.FullName;
                operationName = operationDescription.Name;

                var policyMapping = Configuration.ConfigurationSettings.Current.PolicyMap.Cast<PolicyMappingConfigurationElement>()
                    .Where(e => e.OperationName == operationName && e.ServiceContractType == serviceContractTypeName).FirstOrDefault();

                selectedPolicyName = policyMapping != null ? policyMapping.PolicyName : Configuration.ConfigurationSettings.Current.DefaultPolicy;               
            }



            return GetPolicyFromConfiguration(selectedPolicyName);
         
        }

        private ResourceConstraintPolicy GetPolicyFromConfiguration(string policyName)
        {
            ResourceConstraintPolicyConfigurationElement policyConfiguration;

            policyConfiguration = Configuration.ConfigurationSettings.Current.Policies.Cast<ResourceConstraintPolicyConfigurationElement>()
                    .Where(e => e.Name == policyName).FirstOrDefault();

            if (policyConfiguration == null)
            {
                throw new ConfigurationErrorsException(string.Format("The referenced policy, {0}, was not found in configuration.", policyName));
            }

            return new ResourceConstraintPolicy { CpuUsageConstraint = policyConfiguration.CpuConstraint, MemoryUsageConstraint = policyConfiguration.MemoryConstraint, ExecutionTimeConstraint = TimeSpan.FromMilliseconds(policyConfiguration.ExecutionTimeConstraint) };
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the memory usage limit on the operation, in bytes.
        /// </summary>
        public long?    MemoryUsageLimit
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the processor usage limit on the operation, in milliseconds.
        /// </summary>
        public double?  ProcessorUsageLimit
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum allowed execution time. This will cover scenarios where the processor
        /// is idle during blocking or sleeping.
        /// </summary>
        public TimeSpan MaximumAllowedExecutionTime
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the policy name to be used.
        /// </summary>
        public string   PolicyName
        {
            get;
            set;
        }
        #endregion
    }

  
}