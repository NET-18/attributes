using System;

namespace Net18RealReflection.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyNameAttribute : Attribute
    {
        private string _propertyName;

        internal string PropertyName => _propertyName;

        public PropertyNameAttribute(string propertyName)
        {
            _propertyName = propertyName;
        }
    }
}
