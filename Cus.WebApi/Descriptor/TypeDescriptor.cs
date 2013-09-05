using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class TypeDescriptor : Descriptor
    {
        private readonly Type _type;
        private readonly int _depth;
        private TypeDefinition _def;

        private TypeDescriptor _subType;
        private List<PropertyDescriptor> _properties;
        private Dictionary<string, PropertyDescriptor> _propertiesDic;

        public TypeDescriptor(string name, Type type, int depth)
            : base(name)
        {
            _type = type;
            _depth = depth;
        }

        public void ScanTypeDefinition(HashSet<Type> parentCustomTypes)
        {
            
            _def = Container.GetOrCreate(_type);
            if (_def.IsObject)
            {
                var properties = _type.GetProperties();
                _properties = new List<PropertyDescriptor>(properties.Length);
                _propertiesDic = new Dictionary<string, PropertyDescriptor>(properties.Length, StringComparer.CurrentCultureIgnoreCase);

                if (_def.IsCustomObject)
                {
                    if (parentCustomTypes.Contains(_type)) return;
                    parentCustomTypes.Add(_type);
                }
                if (!TypeDefinition.IsDictionaryType(_type))
                {
                    foreach (var property in properties)
                    {
                        if (property.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Length > 0) continue;
                        var item = new PropertyDescriptor(property, _depth + 1);
                        item.Container = Container;
                        _properties.Add(item);
                        _propertiesDic.Add(property.Name, item);
                        item.ScanTypeDefinition(new HashSet<Type>(parentCustomTypes));
                    }
                }
            }
            else if (_def.IsArray && _def.SubType != null)
            {
                var subDef = Container.GetOrCreate(_def.SubType);
                _subType = new TypeDescriptor(_def.SubType.Name, _def.SubType, _depth);
                _subType.Container = Container;
                _subType.ScanTypeDefinition(parentCustomTypes);
            }
        }

        [JsonIgnore]
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public TypeDefinition Def
        {
            get
            {
                return _def;
            }
        }

        public TypeDescriptor SubType
        {
            get
            {
                return _subType;
            }
        }

        public List<PropertyDescriptor> Properties
        {

            get
            {
                return _properties;
            }
        }

        [JsonIgnore]
        public Dictionary<string, PropertyDescriptor> PropertiesDic
        {

            get
            {
                return _propertiesDic;
            }
        }

        public int Depth
        {
            get
            {
                return _depth;
            }
        }
    }
}
