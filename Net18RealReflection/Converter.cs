using Net18RealReflection.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Net18RealReflection
{
    public static class Converter
    {
        public static void Initialize(string data, object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (data[0] == ';')
            {
                data = data.Remove(0, 1);
            }
            if (data[^1] == ';')
            {
                data = data.Remove(data.Length - 1);
            }

            var splitted = data.Split(';');
            var propertiesData = new PropertyData[splitted.Length];
            for (int i = 0; i < splitted.Length; i++)
            {
                var property = splitted[i].Split(':').Select(v => v.Trim().ToLower()).ToArray();
                propertiesData[i] = new PropertyData
                {
                    PropertyName = property[0],
                    Value = property[1]
                };
            }

            var objType = obj.GetType();
            var properties = objType.GetProperties();

            foreach (var prop in properties)
            {
                var propName = GetPropertyName(prop)?.ToLower();
                var propertyData = propertiesData.FirstOrDefault(d => d.PropertyName.Equals(propName));

                if (propertyData == null)
                {
                    continue;
                }

                var value = new object();
                var propType = prop.PropertyType;
                var propValue = propertyData.Value;

                if (propType == typeof(string))
                {
                    value = propValue;
                }
                else if (propType == typeof(bool))
                {
                    value = Convert.ToBoolean(propValue);
                }
                else if (propType == typeof(int))
                {
                    value = Convert.ToInt32(propValue);
                }
                else if (propType == typeof(decimal))
                {
                    value = Convert.ToDecimal(propValue);
                }
                else
                {
                    throw new InvalidCastException($"Type {propType} is not supported.");
                }

                prop.SetValue(obj, value);
            }
        }

        public static string GetString(object obj)
        {
            var result = new StringBuilder();

            var objType = obj.GetType();
            var properties = objType.GetProperties();

            foreach (var prop in properties)
            {
                var propName = GetPropertyName(prop);

                if (string.IsNullOrEmpty(propName))
                {
                    continue;
                }

                result.Append($"{propName}:{prop.GetValue(obj) ?? "null"};");
            }

            result.Remove(result.Length - 1, 1);

            return result.ToString();
        }

        private static string GetPropertyName(PropertyInfo info) =>
            (info.GetCustomAttribute(typeof(PropertyNameAttribute)) as PropertyNameAttribute)?.PropertyName;
    }
}
