using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class ParamDescriptor : TypeDescriptor
    {
        private readonly ParameterInfo _paramInfo;

        public ParamDescriptor(ParameterInfo info)
            : base(info.Name, info.ParameterType, 0)
        {
            _paramInfo = info;
        }
 
        [JsonIgnore]
        public ParameterInfo ParameterInfo
        {
            get
            {
                return _paramInfo;
            }
        }
    }
}
