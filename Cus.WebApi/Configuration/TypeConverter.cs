using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cus.WebApi.Configuration
{
    sealed class TypeConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            if (value == null)
                return string.Empty;

            return ((Type)value).ToString();
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            if (string.IsNullOrEmpty((string)data))
                return null;

            return Type.GetType((string)data, true);
        }
    }
}
