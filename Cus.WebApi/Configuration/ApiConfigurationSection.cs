using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cus.WebApi.Configuration
{
    class ApiConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("authetication")]
        public AutheticationType AutheticationType
        {
            get { return (AutheticationType)base["authetication"]; }
            set { base["authetication"] = value; }
        }

        //[ConfigurationProperty("autheticationTypes", IsDefaultCollection = true)]
        //public AutheticationTypeElementCollection AutheticationTypes
        //{
        //    get { return (AutheticationTypeElementCollection)base["autheticationTypes"]; }
        //    set { base["autheticationTypes"] = value; }
        //}
    }
}
