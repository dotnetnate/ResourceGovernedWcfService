using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions.Configuration
{
    /// <summary>
    /// Defines the resource constraint configuration for a policy.
    /// </summary>
    public class ResourceConstraintPolicyConfigurationElement
        : ConfigurationElement
    {

        private const double MAX_TIMESPAN_VALUE_IN_MILLISECONDS = 922337203685477.0;
        private const string CPU_CONSTRAINT_PROPERTY_NAME = "cpuConstraint";
        private const string MEMORY_CONSTRAINT_PROPERTY_NAME = "memoryConstraint";
        private const string EXECUTION_TIME_CONSTRAINT_PROPERTY_NAME = "executionTimeConstraint";
        private const string POLICY_NAME_PROPERTY_NAME = "name";

        /// <summary>
        /// Gets or sets the name of the policy.
        /// </summary>
        [ConfigurationProperty(POLICY_NAME_PROPERTY_NAME, IsRequired = false)]
        public string Name
        {
            get { return (string)this[POLICY_NAME_PROPERTY_NAME]; }
            set { this[POLICY_NAME_PROPERTY_NAME] = value; }
        }
        /// <summary>
        /// Gets or sets the CPU constraint for the policy.
        /// </summary>
        [ConfigurationProperty(CPU_CONSTRAINT_PROPERTY_NAME, IsRequired = false, DefaultValue = double.MaxValue)]
        public double CpuConstraint
        {
            get { return (double)this[CPU_CONSTRAINT_PROPERTY_NAME]; }
            set { this[CPU_CONSTRAINT_PROPERTY_NAME] = value; }
        }
        /// <summary>
        /// Gets or sets the memory constraint for the policy.
        /// </summary>
        [ConfigurationProperty(MEMORY_CONSTRAINT_PROPERTY_NAME, IsRequired = false, DefaultValue = long.MaxValue )]
        public long MemoryConstraint
        {
            get { return (long)this[MEMORY_CONSTRAINT_PROPERTY_NAME]; }
            set { this[MEMORY_CONSTRAINT_PROPERTY_NAME] = value; }
        }
        /// <summary>
        /// Gets or sets the execution time constraint for the policy.
        /// </summary>
        [ConfigurationProperty(EXECUTION_TIME_CONSTRAINT_PROPERTY_NAME, IsRequired = false, DefaultValue = MAX_TIMESPAN_VALUE_IN_MILLISECONDS)]
        public double ExecutionTimeConstraint
        {            
            get { return (double)this[EXECUTION_TIME_CONSTRAINT_PROPERTY_NAME]; }
            set { this[EXECUTION_TIME_CONSTRAINT_PROPERTY_NAME] = value; }
        }
    }
}