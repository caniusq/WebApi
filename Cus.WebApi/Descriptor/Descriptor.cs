using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    class Descriptor
    {
        private readonly string _name;

        public Descriptor(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Documentation { get; set; }

        [JsonIgnore]
        public TypeDefinitionContainer Container { get; set; }

        [JsonIgnore]
        public bool SupportRecursive
        {
            get
            {
                return Container.SupportRecursive;
            }
        }
    }
}
