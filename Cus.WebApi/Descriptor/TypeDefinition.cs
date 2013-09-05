using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class TypeDefinition
    {
        private readonly Type _type;
        private readonly JsonSchema _schema;

        private readonly string _id;
        private readonly string _name;
        private readonly bool _canNull;
        private readonly bool _isArray;
        private readonly bool _isObject;
        private readonly bool _isCustomObject;
        private readonly bool _isLeaf;
        private readonly Type _subType;
        private readonly object _demoValue;

        public TypeDefinition(Type type,JsonSchemaGenerator generator,TypeDefinitionContainer container)
        {
            _type = type;
            _schema = generator.Generate(type, true);
            _canNull = HasFlag(_schema.Type, JsonSchemaType.Null);
            _isArray = HasFlag(_schema.Type, JsonSchemaType.Array);
            _isObject = HasFlag(_schema.Type, JsonSchemaType.Object);

            if (_isArray && _isObject)
                throw new ApplicationException(string.Format("Type {0} is both array and object.", type.FullName));

            if (_isArray)
            {
                _subType = GetCollectionItemType(_type);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                _subType = type.GetGenericArguments()[0];
                var subDef = container.GetOrCreate(_subType);
                _name = subDef.Name;
                _demoValue = subDef.DemoValue;
            }
            else if (type == typeof(DateTime))
            {
                _name = "dateTime";
                _demoValue = JsonConvert.SerializeObject(DateTime.Now).Trim('\"');
            }
            else if (type == typeof(byte[]))
            {
                _name = "base64binary";
                _demoValue = JsonConvert.SerializeObject(Encoding.UTF8.GetBytes("Cus.WebApi")).Trim('\"');
            }

            _isLeaf = !(_isArray || _isObject);

            _id = Guid.NewGuid().ToString("N");

            if (_name == null)
            {
                if (IsArray)
                {
                    _name = "array";
                    _demoValue = "[]";
                }
                else if (IsObject)
                {
                    _name = "object";
                    _demoValue = "{}";
                    if (_schema.Id != null)
                        _isCustomObject = true;
                }
                else if (HasFlag(_schema.Type, JsonSchemaType.String))
                {
                    _name = "string";
                    _demoValue = "string1";
                }
                else if (HasFlag(_schema.Type, JsonSchemaType.Boolean))
                {
                    _name = "boolean";
                    _demoValue = true;
                }
                else if (HasFlag(_schema.Type, JsonSchemaType.Float))
                {
                    _name = "float";
                    _demoValue = 1.1f;
                }
                else if (HasFlag(_schema.Type, JsonSchemaType.Integer))
                {
                    _name = "integer";
                    _demoValue = 1;
                }
                else if (_schema.Type == JsonSchemaType.Any) _name = "any";
                else if (_schema.Type == JsonSchemaType.Null) _name = "null";
                else _name = "unknown";
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return _isLeaf;
            }
        }

        public bool IsArray
        {
            get
            {
                return _isArray;
            }
        }

        public bool IsObject
        {
            get
            {
                return _isObject;
            }
        }

        public bool IsCustomObject
        {
            get
            {
                return _isCustomObject;
            }
        }

        public bool CanNull
        {
            get
            {
                return _canNull;
            }
        }

        public Type SubType
        {
            get
            {
                return _subType;
            }
        }

        public object DemoValue
        {
            get
            {
                return _demoValue;
            }
        }

        private static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
        {
            // default value is Any
            if (value == null)
                return true;

            bool match = ((value & flag) == flag);
            if (match)
                return true;

            // integer is a subset of float
            if (value == JsonSchemaType.Float && flag == JsonSchemaType.Integer)
                return true;

            return false;
        }

        //private static Type GetArraySubType(Type type)
        //{
        //    if (type.IsArray && type.GetArrayRank() == 1)
        //    {
        //        return type.GetElementType();
        //    }
        //    if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.GetInterface((typeof(IEnumerable<>).FullName)) != null)
        //    {
        //        return type.GetGenericArguments()[0];
        //    }

        //    return null;
        //}

        public static bool IsDictionaryType(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type)) return true;
            Type genericDicType;
            if (ImplementsGenericDefinition(type, typeof(IDictionary<,>), out genericDicType)) return true;
            return false;
        }

        public static Type GetCollectionItemType(Type type)
        {
            Type genericListType;

            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out genericListType))
            {
                if (genericListType.IsGenericTypeDefinition)
                    throw new Exception(string.Format("Type {0} is not a collection.", type));

                return genericListType.GetGenericArguments()[0];
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }
            else
            {
                throw new Exception(string.Format("Type {0} is not a collection.", type));
            }
        }

        private static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type i in type.GetInterfaces())
            {
                if (i.IsGenericType)
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }
    }
}
