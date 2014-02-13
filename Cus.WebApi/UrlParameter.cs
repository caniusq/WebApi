using System;
using System.Diagnostics.CodeAnalysis;

namespace Cus.WebApi
{
    public sealed class UrlParameter
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This type is immutable.")]
        public static readonly UrlParameter Optional = new UrlParameter();

        private UrlParameter()
        {
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}
