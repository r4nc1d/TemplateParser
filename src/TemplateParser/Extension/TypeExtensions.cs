using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TemplateParser.Extension
{
    public static class TypeExtensions
    {
        private static readonly Type[] KnownPrimitiveTypes =
        {
            typeof (string), typeof (decimal), typeof (DateTime),
            typeof (DateTimeOffset), typeof (Guid), typeof (Type)
        };

        public static IDictionary<string, PropertyMetaData> GetProperties<T>(this T source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var state = new VisitorState();
            return GetProperties(state, typeof(T), source);
        }

        private static IDictionary<string, PropertyMetaData> GetProperties(VisitorState state, IReflect classType, object source)
        {
            var className = classType.UnderlyingSystemType.Name;
            foreach (var property in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(source);
                var key = className + "." + property.Name;

                if (property.PropertyType.IsPrimtiveType())
                {
                    if (state.CanVisit(key))
                        state.Placeholders.Add(key, GetPropertyMetaData(property, value));

                }
                else if (state.CanVisit(value))
                {
                    state.Visited.Add(value);
                    GetProperties(state, property.PropertyType, value);
                }
            }

            return state.Placeholders;
        }

        private static PropertyMetaData GetPropertyMetaData(PropertyInfo property, object value)
        {
            return new PropertyMetaData
            {
                Type = property.PropertyType,
                Value = value
            };
        }

        private static bool IsPrimtiveType(this Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type) ?? type;
            return actualType.IsPrimitive || actualType.IsValueType || actualType.IsEnum || KnownPrimitiveTypes.Contains(actualType);
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException(@"Unable to convert object to a dictionary. The source object is null.");
        }

        private class VisitorState
        {
            public readonly IList<object> Visited = new List<object>();
            public readonly IDictionary<string, PropertyMetaData> Placeholders = new Dictionary<string, PropertyMetaData>();

            public bool CanVisit(string key)
            {
                return !Placeholders.ContainsKey(key);
            }

            public bool CanVisit(object value)
            {
                return value != null && !IsOfType<IEnumerable>(value) && !Visited.Contains(value);
            }

            private static bool IsOfType<T>(object value)
            {
                return value is T;
            }
        }
    }
}
