using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions.Configuration
{
    /// <summary>
    /// Provides configuration settings information.
    /// </summary>
    /// <remarks>
    /// Example configuration section:
    ///   &lt;constrainedServiceExecution defaultPolicy="Fast Execution Policy"&gt;
    ///     &lt;policies&gt;
    ///         &lt;add policyName="Fast Execution Policy" cpuConstraint="1" memoryConstraint="1073741824" executionTimeConstraint="1" /&gt;
    ///     &lt;/policies&gt;
    ///     &lt;policyMap&gt;
    ///         &lt;add name="Operation3 Mapping" serviceContract="MyOutOfControlService.IService1" operationName="Operation3" policy="Fast Execution Policy" /&gt;
    ///     &lt;/policyMap&gt;
    /// &lt;/constrainedServiceExecution&gt;
    /// 
    /// </remarks>
    public class ConfigurationSettings
        : ConfigurationSection
    {
        private const string CONFIG_SECTION_NAME = "constrainedServiceExecution";
        private const string POLICIES_PROPERTY_NAME = "policies";
        private const string DEFAULT_POLICY_PROPERTY_NAME = "defaultPolicy";
        private const string POLICY_MAP_PROPERTY_NAME = "policyMap";

        public static ConfigurationSettings Current
        {
            get
            {
                return (ConfigurationSettings)ConfigurationManager.GetSection(CONFIG_SECTION_NAME);
            }
        }

        /// <summary>
        /// Gets or sets the name of the default policy.
        /// </summary>
        [ConfigurationProperty(DEFAULT_POLICY_PROPERTY_NAME)]
        public string DefaultPolicy
        {
            get { return (string)this[DEFAULT_POLICY_PROPERTY_NAME]; }
            set { this[DEFAULT_POLICY_PROPERTY_NAME] = value; }
        }
        /// <summary>
        /// Gets or sets the collection of resource constraint policies.
        /// </summary>
        [ConfigurationProperty(POLICIES_PROPERTY_NAME)]
        [ConfigurationCollection(typeof(ResourceConstraintPolicyConfigurationElement))]
        public ResourceConstraintPolicyConfigurationElementCollection Policies
        {
            get{
                return (ResourceConstraintPolicyConfigurationElementCollection)this[POLICIES_PROPERTY_NAME];
            }
        }
        /// <summary>
        /// Gets the list of mappings of service operations to policies.
        /// </summary>
        [ConfigurationProperty(POLICY_MAP_PROPERTY_NAME)]
        [ConfigurationCollection(typeof(PolicyMappingConfigurationElement))]
        public PolicyMappingConfigurationElementCollection PolicyMap
        {
            get
            {
                return (PolicyMappingConfigurationElementCollection)this[POLICY_MAP_PROPERTY_NAME];
            }
        }
    }
}