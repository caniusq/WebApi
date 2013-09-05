using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class MethodDescriptor : Descriptor
    {
        private readonly MethodInfo _info;
        private readonly List<ParamDescriptor> _params;
        private readonly Dictionary<string, ParamDescriptor> _paramsDic;
        private readonly TypeDescriptor _responseParam;
        private readonly ParameterInfo _responseParameterInfo;
        private readonly IEnumerable<ApiCodeAttribute> _responseCodes;
        private readonly bool _needAuth;

        public MethodDescriptor(MethodInfo info, List<ApiCodeAttribute> attrs,bool needAuth)
            : base(info.Name)
        {
            _info = info;
            var paras = info.GetParameters();
            _params = new List<ParamDescriptor>(paras.Length);
            _paramsDic = new Dictionary<string, ParamDescriptor>(paras.Length, StringComparer.CurrentCultureIgnoreCase);
            foreach (var paramInfo in paras)
            {
                var item = new ParamDescriptor(paramInfo);
                _params.Add(item);
                _paramsDic.Add(paramInfo.Name, item);
            }

            _responseParameterInfo = info.ReturnParameter;

            Type t;
            if (info.ReturnParameter.ParameterType == typeof(void)) t = typeof(Response);
            else t = typeof(Response<>).MakeGenericType(info.ReturnParameter.ParameterType);

            _responseParam = new TypeDescriptor(null, t, -1);
            _responseParam.Container = Container;

            foreach (ApiCodeAttribute attr in info.GetCustomAttributes(typeof(ApiCodeAttribute), true))
            {
                attrs.Add(attr);
            }

            _responseCodes = from attr in attrs orderby attr.Category,attr.Code select attr;
            _needAuth = needAuth;
        }

        [JsonIgnore]
        public MethodInfo MethodInfo
        {
            get
            {
                return _info;
            }
        }

        public bool NeedAuth
        {
            get
            {
                return _needAuth;
            }
        }

        public IEnumerable<ParamDescriptor> Params
        {
            get
            {
                return _params;
            }
        }

        public TypeDescriptor ResponseParam
        {
            get
            {
                return _responseParam;
            }
        }

        [JsonIgnore]
        public ParameterInfo ResponseParameterInfo
        {
            get
            {
                return _responseParameterInfo;
            }
        }

        public bool IsResponseVoid
        {
            get
            {
                return _responseParam.Type == typeof(void);
            }
        }

        [JsonIgnore]
        public IDictionary<string, ParamDescriptor> ParamsDic
        {
            get
            {
                return _paramsDic;
            }
        }

        public IEnumerable<ApiCodeAttribute> ResponseCodes
        {
            get
            {
                return _responseCodes;
            }
        }

    }
}
