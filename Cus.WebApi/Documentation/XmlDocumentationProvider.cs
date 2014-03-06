using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Globalization;
using System.Reflection;

namespace Cus.WebApi
{
    /// <summary>
    /// xml文档提供者
    /// </summary>
    class XmlDocumentationProvider
    {
        private XPathNavigator _documentNavigator;
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";

        public XmlDocumentationProvider(System.IO.Stream stream)
        {
            var xpath = new XPathDocument(stream);
            _documentNavigator = xpath.CreateNavigator();
        }

        public XmlDocumentationProvider(string documentPath)
        {
            var xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        public void SetDocumentation(ApiDescriptor apiDescriptor, bool classDocOnly = false)
        {
            if (apiDescriptor.Documentation == null)
                apiDescriptor.Documentation = GetApiDocumentation(apiDescriptor);

            if (classDocOnly) return;

            foreach (var methodDescriptor in apiDescriptor.Methods)
            {
                if (methodDescriptor.Documentation == null)
                    methodDescriptor.Documentation = GetMethodDocumentation(methodDescriptor);

                foreach (var paramDescriptor in methodDescriptor.Params)
                {
                    if (paramDescriptor.Documentation == null)
                        paramDescriptor.Documentation = GetParamDocumentation(methodDescriptor, paramDescriptor);

                    ScanParamDocumentation(paramDescriptor);
                }

                if (methodDescriptor.ResponseParam.Documentation == null)
                    methodDescriptor.ResponseParam.Documentation = GetResponseDocumentation(methodDescriptor);

                ScanParamDocumentation(methodDescriptor.ResponseParam);
            }
        }

        private string GetApiDocumentation(ApiDescriptor apiDescriptor)
        {
            XPathNavigator typeNode = GetTypeNode(apiDescriptor.ApiType);
            return GetTagValue(typeNode, "summary");
        }

        private string GetMethodDocumentation(MethodDescriptor methodDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(methodDescriptor);
            return GetTagValue(methodNode, "summary");
        }

        private string GetParamDocumentation(MethodDescriptor methodDescriptor, ParamDescriptor paramDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(methodDescriptor);
            if (methodNode != null)
            {
                string parameterName = paramDescriptor.ParameterInfo.Name;
                XPathNavigator parameterNode = methodNode.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                if (parameterNode != null)
                {
                    return parameterNode.InnerXml.Trim();
                }
            }
            return null;
        }

        private string GetResponseDocumentation(MethodDescriptor methodDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(methodDescriptor);
            return GetTagValue(methodNode, "returns");
        }

        private void ScanParamDocumentation(TypeDescriptor typeDescriptor)
        {
            if (!typeDescriptor.Def.IsLeaf && typeDescriptor.Properties != null)
            {
                foreach (var property in typeDescriptor.Properties)
                {
                    ScanPropertyDocumentation(property);
                }
            }
        }

        private void ScanPropertyDocumentation(PropertyDescriptor propertyDescriptor)
        {
            XPathNavigator propertyNode = GetPropertyNode(propertyDescriptor);

            if (propertyDescriptor.Documentation == null)
                propertyDescriptor.Documentation = GetTagValue(propertyNode, "summary");

            if (propertyDescriptor.Def.IsLeaf) return;
            if (propertyDescriptor.Def.IsArray && propertyDescriptor.SubType != null && propertyDescriptor.SubType.Def.IsObject)
            {
                foreach (PropertyDescriptor sub in propertyDescriptor.SubType.Properties)
                {
                    ScanPropertyDocumentation(sub);
                }
            }
            else if (propertyDescriptor.Def.IsObject)
            {
                foreach (PropertyDescriptor sub in propertyDescriptor.Properties)
                {
                    ScanPropertyDocumentation(sub);
                }
            }
        }

        private XPathNavigator GetPropertyNode(PropertyDescriptor propertyDescriptor)
        {
            string selectExpression = String.Format(CultureInfo.InvariantCulture, PropertyExpression, GetPropertyName(propertyDescriptor.PropertyInfo));
            return _documentNavigator.SelectSingleNode(selectExpression);
        }

         private static string GetPropertyName(PropertyInfo property)
         {
             var dtype = property.DeclaringType;
             if (dtype.IsGenericType) dtype = dtype.GetGenericTypeDefinition();
             return String.Format(CultureInfo.InvariantCulture, "{0}.{1}", dtype.FullName, property.Name);
         }

        private XPathNavigator GetMethodNode(MethodDescriptor methodDescriptor)
        {
            string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(methodDescriptor.MethodInfo));
            return _documentNavigator.SelectSingleNode(selectExpression);
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", method.DeclaringType.FullName, method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.SelectSingleNode(tagName);
                if (node != null)
                {
                    return node.InnerXml.Trim();
                }
            }

            return null;
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string typeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                return String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", typeName, String.Join(",", argumentTypeNames));
            }

            return type.FullName;
        }

        private XPathNavigator GetTypeNode(Type type)
        {
            string typeName = type.FullName;
            if (type.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                typeName = typeName.Replace("+", ".");
            }
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, typeName);
            return _documentNavigator.SelectSingleNode(selectExpression);
        }
    }
}
