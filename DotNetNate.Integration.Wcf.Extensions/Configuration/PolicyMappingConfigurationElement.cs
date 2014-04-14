using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions.Configuration
{
    /// <summary>
    /// Provides configuration information for mapping service operations to resource policies.
    /// </summary>
    public class PolicyMappingConfigurationElement
        : ConfigurationElement
    {
        private const string NAME_PROPERTY_NAME = "name";
        private const string CONTRACT_TYPE_PROPERTY_NAME = "serviceContract";
        private const string OPERATION_NAME_PROPERTY_NAME = "operationName";
        private const string POLICY_PROPERTY_NAME = "policyName";

        /// <summary>
        /// Gets or sets the name of the policy.
        /// </summary>
        [ConfigurationProperty(NAME_PROPERTY_NAME)]
        public string Name
        {
            get
            {
                return (string)this[NAME_PROPERTY_NAME];
            }
            set
            {
                this[NAME_PROPERTY_NAME] = value;
            }
        }
        /// <summary>
        /// Gets or sets the full type name of the service contract type the policy applies to.
        /// </summary>
        [ConfigurationProperty(CONTRACT_TYPE_PROPERTY_NAME)]
        public string ServiceContractType
        {
            get
            {
                return (string)this[CONTRACT_TYPE_PROPERTY_NAME];
            }
            set
            {
                this[CONTRACT_TYPE_PROPERTY_NAME] = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the operation the policy is to be applied to.
        /// </summary>
        [ConfigurationProperty(OPERATION_NAME_PROPERTY_NAME)]
        public string OperationName
        {
            get
            {
                return (string)this[OPERATION_NAME_PROPERTY_NAME];
            }
            set
            {
                this[OPERATION_NAME_PROPERTY_NAME] = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the policy being applied to the operation.
        /// </summary>
        [ConfigurationProperty(POLICY_PROPERTY_NAME)]
        public string PolicyName
        {
            get { return (string)this[POLICY_PROPERTY_NAME]; }
            set { this[POLICY_PROPERTY_NAME] = value; }
        }
    }
}
