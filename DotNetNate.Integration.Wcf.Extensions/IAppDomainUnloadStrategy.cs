using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Interface for strategies that deal with unloading application domains.
    /// </summary>
    public interface IAppDomainUnloadStrategy
    {
        /// <summary>
        /// Executes the strategy against the application domain.
        /// </summary>
        /// <param name="domain">The domain to act against.</param>
        /// <returns>Returns <c>true</c> if the application domain was unloaded, <c>false</c> otherwise.</returns>
        bool Execute(AppDomain domain);
    }
}