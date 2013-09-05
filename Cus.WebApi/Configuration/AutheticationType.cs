using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Cus.WebApi.Configuration
{
    class AutheticationType : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeConverter))]
        public Type Type
        {
            get { return (Type)this["type"]; }
            set { this["type"] = value; }
        }
    }
}
