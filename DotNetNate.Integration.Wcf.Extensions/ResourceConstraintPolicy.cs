using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Provides information about the resource budget for a service operation.
    /// </summary>
    public class ResourceConstraintPolicy
    {
        /// <summary>
        /// Gets or sets the name of the policy.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum execution time the policy may use.
        /// </summary>
        public TimeSpan ExecutionTimeConstraint
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum amount of processor time, in milliseconds,
        /// the call may use.
        /// </summary>
        public double CpuUsageConstraint
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum amount of memory, in bytes, the call may use.
        /// </summary>
        public long MemoryUsageConstraint
        {
            get;
            set;
        }
    }
}