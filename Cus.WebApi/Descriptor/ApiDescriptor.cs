using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class ApiDescriptor : Descriptor
    {
        private readonly Type _apiType;
        private readonly List<MethodDescriptor> _methods;
        private readonly Dictionary<string, MethodDescriptor> _methodsDic;

        public ApiDescriptor(Type apiType)
            : base(apiType.Name)
        {
            _apiType = apiType;
            var list = GetMethodInfos(apiType);
            _methods = new List<MethodDescriptor>(list.Count);
            _methodsDic = new Dictionary<string, MethodDescriptor>(list.Count, StringComparer.CurrentCultureIgnoreCase);

            var defaultCodes = new HashSet<int>();
            foreach (ApiCodeAttribute attr in typeof(ApiHandler).GetCustomAttributes(typeof(ApiCodeAttribute), true))
            {
                defaultCodes.Add(attr.Code);
            }

            var attrs = new List<ApiCodeAttribute>();
            foreach (ApiCodeAttribute attr in apiType.GetCustomAttributes(typeof(ApiCodeAttribute),true))
            {
                if (defaultCodes.Contains(attr.Code)) attr.Category = 1;
                attrs.Add(attr);
            }

            var authAttr = (ApiAuthAttribute)Attribute.GetCustomAttribute(apiType, typeof(ApiAuthAttribute));
            bool auth = authAttr != null && authAttr.NeedAuth;

            foreach (var info in list)
            {
                if (null != Attribute.GetCustomAttribute(info, typeof(ApiIgnoreAttribute))) continue;
                var methodAuthAttr = (ApiAuthAttribute)Attribute.GetCustomAttribute(info, typeof(ApiAuthAttribute));
                bool needAuth = methodAuthAttr != null ? methodAuthAttr.NeedAuth : auth;
                var item = new MethodDescriptor(info, new List<ApiCodeAttribute>(attrs), needAuth);
                _methods.Add(item);
                if (_methodsDic.ContainsKey(info.Name)) throw new ApplicationException("不支持重载");
                _methodsDic.Add(info.Name, item);
            }
        }

        public void ScanTypeDefinitions()
        {
            foreach (var method in _methods)
            {
                method.Container = Container;
                foreach (var param in method.Params)
                {
                    param.Container = Container;
                    param.ScanTypeDefinition(new HashSet<Type>());
                }
                method.ResponseParam.Container = Container;
                method.ResponseParam.ScanTypeDefinition(new HashSet<Type>());
            }
        }

        private List<MethodInfo> GetMethodInfos(Type t)
        {
            var list = new List<MethodInfo>();
            var infos = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in infos)
            {
                if (info.DeclaringType == typeof(Object) || info.DeclaringType == typeof(ApiHandler)) continue;
                if ((info.Attributes & MethodAttributes.SpecialName) != 0) continue;
                list.Add(info);
            }
            return list;
        }

        [JsonIgnore]
        public Type ApiType
        {
            get
            {
                return _apiType;
            }
        }

        public IEnumerable<MethodDescriptor> Methods
        {
            get
            {
                return _methods;
            }
        }

        [JsonIgnore]
        public IDictionary<string, MethodDescriptor> MethodsDic
        {
            get
            {
                return _methodsDic;

            }
        }
    }
}
