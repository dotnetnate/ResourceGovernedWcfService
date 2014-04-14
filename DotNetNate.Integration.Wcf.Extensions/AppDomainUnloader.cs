using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Manages the unloading of application domains.
    /// </summary>
    internal static class AppDomainUnloader
    {
        // maintains the strategy chain list
        private static List<IAppDomainUnloadStrategy> _strategyChain = new List<IAppDomainUnloadStrategy>
        {
            new SingleAttemptAppDomainUnloadStrategy(),
            new ExponentialBackoffAppDomainUnloadStrategy()
        };

        /// <summary>
        /// Unloads an application domain by attempting the execution of each unload strategy, in order.
        /// </summary>
        /// <param name="domain">The application domain to unload.</param>
        /// <returns>Returns <c>true</c> if the application domain was unloaded, <c>false</c> otherwise.</returns>
        public static bool Unload(AppDomain domain)
        {
            foreach (IAppDomainUnloadStrategy strategy in _strategyChain)
            {
                if (strategy.Execute(domain))
                {
                    return true;
                }
            }

            return false;
        }
    }
}