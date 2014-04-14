using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DotNetNate.Integration.Wcf.Extensions.Configuration
{
    public class PolicyMappingConfigurationElementCollection
        : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PolicyMappingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PolicyMappingConfigurationElement)element).Name;
        }
    }
}