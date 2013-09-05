using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cus.WebApi.Configuration
{
    class AutheticationTypeElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AutheticationType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AutheticationType)element).Name;
        }
    }
}
