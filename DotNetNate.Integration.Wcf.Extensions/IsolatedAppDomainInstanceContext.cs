using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{

    /// <summary>
    /// Provides InstanceContext information about the hosting application domain.
    /// </summary>
    public class IsolatedAppDomainInstanceContext 
        : IExtension<InstanceContext>
    {              
        /// <summary>
        /// Creates a new instance of <see cref="IsolatedAppDomainInstanceContext"/>.
        /// </summary>
        private IsolatedAppDomainInstanceContext()
        {

        }
        /// <summary>
        /// Gets or sets the hosting domain for the context.
        /// </summary>
        public AppDomain HostingDomain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current <see cref="IsolatedAppDomainInstanceContext"/>. 
        /// </summary>
        public static IsolatedAppDomainInstanceContext Current
        {
            get
            {
                IsolatedAppDomainInstanceContext retVal;

                retVal = OperationContext.Current.InstanceContext.Extensions.Find<IsolatedAppDomainInstanceContext>();

                if (retVal == null)
                {
                    retVal = new IsolatedAppDomainInstanceContext();

                    OperationContext.Current.InstanceContext.Extensions.Add(retVal);
                }

                return retVal;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IExtension{InstanceContext}.Attach(InstanceContext)"/>
        /// </summary>
        /// <param name="owner"></param>
        public void Attach(InstanceContext owner)
        {
            //no-op
        }
        /// <summary>
        /// Implementation of <see cref="IExtension{InstanceContext}.Detach(InstanceContext)"/>
        /// </summary>
        /// <param name="owner"></param>
        public void Detach(InstanceContext owner)
        {
            //no-op
        }
        
    }
}