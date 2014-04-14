using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions
{
    /// <summary>
    /// Provides <see cref="AppDomain"/> level isolation between service calls to facilitate the 
    /// governing of resources and possible termination of calls.
    /// </summary>
    public class IsolatedAppDomainBehavior
        : Attribute, IServiceBehavior
    {
        /// <summary>
        /// Implementation of <see cref="IServiceBehavior.AddBindingParameters"/>
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // no implementation
        }
        /// <summary>
        /// Implementation of <see cref="IServiceBehavior.ApplyDispatchBehavior"/>
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var currentChannelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                var dispatcher = currentChannelDispatcher as ChannelDispatcher;

                if (dispatcher != null)
                {
                    foreach (var endpoint in dispatcher.Endpoints)
                    {
                        endpoint.DispatchRuntime.InstanceProvider = new IsolatedAppDomainInstanceProvider(serviceDescription.ServiceType);
                    }
                }
            }

        }
        /// <summary>
        /// Implementation of <see cref="IServiceBehavior.Validate"/>
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // no implementation
        }
    }
}