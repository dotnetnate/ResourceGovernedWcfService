using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Implementation of <see cref="IAppDomainUnloadStrategy"/> that attempts a single, 
    /// low-overhead unload.
    /// </summary>
    public class SingleAttemptAppDomainUnloadStrategy
        : IAppDomainUnloadStrategy
    {
        /// <summary>
        /// Implementation of <see cref="IAppDomainUnloadStrategy.Execute"/>.
        /// </summary>
        /// <param name="domain">The application domain to unload.</param>
        /// <returns>Returns <c>true</c> if the application domain was successfully unloaded, <c>false</c> otherwise.</returns>
        public bool Execute(AppDomain domain)
        {
            try
            {
                AppDomain.Unload(domain);

                return true;
            }
            catch(AppDomainUnloadedException)
            {
                return false;
            }
        }
    }
}