using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cus.WebApi.Configuration
{
    static class ConfigManager
    {
        public static IAuthetication CreateAuthetication()
        {
            var item = GetAutheticationType();
            if (item == null || item.Type == null) return null;
            return (IAuthetication)Activator.CreateInstance(item.Type);
        }

        private static AutheticationType GetAutheticationType()
        {
            var section = (ApiConfigurationSection)ConfigurationManager.GetSection("cus.webApi");
            if (section == null) return null;
            return section.AutheticationType;
        }
    }
}
