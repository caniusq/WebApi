using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    class TypeDefinitionContainer
    {
        JsonSchemaGenerator _generator;
        Dictionary<Type, TypeDefinition> _cache;
        private readonly bool _supportRecursive;

        public TypeDefinitionContainer(bool supportRecursive)
        {
            _supportRecursive = supportRecursive;
        }

        public bool SupportRecursive
        {
            get
            {
                return _supportRecursive;
            }
        }

        public TypeDefinition GetOrCreate(Type type)
        {
            TypeDefinition def;

            if (_cache == null)
            {
                _generator = new JsonSchemaGenerator();
                if (_supportRecursive) _generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseAssemblyQualifiedName;
                _cache = new Dictionary<Type, TypeDefinition>();
            }

            if (!_cache.TryGetValue(type, out def))
            {
                def = new TypeDefinition(type, _generator, this);
                _cache.Add(type, def);
            }

            return def;
        }

        public IEnumerable<TypeDefinition> CustomDefs
        {
            get
            {
                return _cache.Values.Where(d => d.IsCustomObject);
            }
        }
    }
}
