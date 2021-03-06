﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HillPigeon.ApplicationBuilder
{
    public static class CustomAttributeBuilderFactory
    {
        public static CustomAttributeBuilder Build(Attribute attribute)
        {
            ConstructorInfo constructorInfo = null;
            object[] values = null;
            if (attribute is AuthorizeAttribute || attribute is AllowAnonymousAttribute)
            {
                //无需赋值
            }
            else if (typeof(ApiVersionsBaseAttribute).IsAssignableFrom(attribute.GetType()))
            {
                var apiVerAttr = (ApiVersionsBaseAttribute)attribute;
                constructorInfo = attribute.GetType().GetConstructor(new Type[] { typeof(ApiVersion) });
                foreach (var versions in apiVerAttr.Versions)
                {
                    values = new object[] { versions };
                    return Build(attribute, constructorInfo, values);
                }
            }
            else if (attribute is RouteAttribute routeAttr)
            {
                constructorInfo = attribute.GetType().GetConstructor(new Type[] { typeof(string) });
                values = new object[] { routeAttr.Template };
            }
            else if (attribute is AcceptVerbsAttribute acceptVerbs)
            {
                constructorInfo = attribute.GetType().GetConstructor(new Type[] { typeof(string[]) });
                values = new object[] { acceptVerbs.HttpMethods.ToArray() };
            }
            else if (typeof(HttpMethodAttribute).IsAssignableFrom(attribute.GetType()))
            {
                var httpMethod = (HttpMethodAttribute)attribute;
                if (!string.IsNullOrEmpty(httpMethod.Template))
                {
                    constructorInfo = attribute.GetType().GetConstructor(new Type[] { typeof(string) });
                    values = new object[] { httpMethod.Template };
                }
            }
            else if (attribute.IsBindingSourceAttribute())
            {
                // 无需赋值
            }
            else
            {
                return null;
            }
            return Build(attribute, constructorInfo, values);
        }
        public static CustomAttributeBuilder Build(Attribute attribute, ConstructorInfo con, object[] constructorArgs)
        {
            var type = attribute.GetType();
            if (con == null)
            {
                con = type.GetConstructor(new Type[0]);
                constructorArgs = new object[0];
            }
            var properties = attribute.GetType().GetProperties().Where(f => f.CanWrite && f.CanRead).ToArray();
            object[] values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                values[i] = property.GetValue(attribute);
            }
            return new CustomAttributeBuilder(con, constructorArgs, properties, values);
        }

        public static bool IsBindingSourceAttribute(this Attribute attribute)
        {
            if (attribute is FromBodyAttribute || 
                attribute is FromFormAttribute || 
                attribute is FromHeaderAttribute || 
                attribute is FromQueryAttribute || 
                attribute is FromRouteAttribute || 
                attribute is FromServicesAttribute)
                return true;
            else
                return false;
        }
    }
}
