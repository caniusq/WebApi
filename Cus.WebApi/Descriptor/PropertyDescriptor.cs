using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class PropertyDescriptor : TypeDescriptor
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyDescriptor(PropertyInfo info,int depth)
            : base(info.Name, info.PropertyType, depth)
        {
            _propertyInfo = info;
        }

        [JsonIgnore]
        public PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }
    }
}
