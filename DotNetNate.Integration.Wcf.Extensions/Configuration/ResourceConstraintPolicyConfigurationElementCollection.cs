using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions.Configuration
{
    /// <summary>
    /// Provides configuration information for a collection of <see cref="ResourceConstraintPolicy"/> instances.
    /// </summary>
    public class ResourceConstraintPolicyConfigurationElementCollection
        : ConfigurationElementCollection
    {
        /// <summary>
        /// Implementation of <see cref="ConfigurationElementCollection.CreateNewElement"/>.
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {         
            return new ResourceConstraintPolicyConfigurationElement();
        }
        /// <summary>
        /// Implementation of <see cref="ConfigurationElementCollection.GetElementKey"/>.
        /// </summary>
        /// <param name="element">The element to inspect.</param>
        /// <returns>Returns the key of the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ResourceConstraintPolicyConfigurationElement)element).Name;          
        }
    }
}