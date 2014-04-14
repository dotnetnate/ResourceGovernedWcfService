using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Implementation of <see cref="IAppDomainUnloadStrategy"/> that executes
    /// an exponential backoff (up to about 30s) for uploading an application domain.
    /// </summary>
    public class ExponentialBackoffAppDomainUnloadStrategy
        : IAppDomainUnloadStrategy
    {
        /// <summary>
        /// Implementation of <see cref="IAppDomainUnloadStrategy"/>.
        /// </summary>
        /// <param name="domain">The application domain to unload.</param>
        /// <returns>Returns <c>true</c> if the application domain was successfully unloaded, <c>false</c> otherwise.</returns>
        public bool Execute(AppDomain domain)
        {
            bool retVal = false;

            for (int i = 0; i < 7; i++) {
              
                try
                {
                    if (retVal)
                    {
                        break;
                    }

                    Thread.Sleep((int)((1d / 2d) * (Math.Pow(2d, (double)i) - 1d)) * 1000);

                    AppDomain.Unload(domain);                   

                    retVal = true;
                }
                catch(CannotUnloadAppDomainException)
                {
                    // we're expecting this, any other exceptions should bubble up
                }                
            }
        
            return retVal;
        }     
    }
}