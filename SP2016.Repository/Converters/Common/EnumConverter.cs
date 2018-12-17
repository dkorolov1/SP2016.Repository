using System;
using System.ComponentModel;
using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class EnumConverter : FieldConverter
    {
        public override object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            if (propertyValue == null)
                return null;

            Type type = propertyValue.GetType();
            string name = Enum.GetName(type, propertyValue);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                        Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;
                    else
                        return name;
                }
            }

            return null;
        }

        public override object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            if (fieldValue == null)
                return null;

            var isNullable = Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
            var enumType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            return ParseEnum(fieldValue.ToString(), enumType, isNullable);
        }

        private object ParseEnum(string fieldValue, Type enumType, bool isNullable)
        {
            string str = fieldValue.ToString();
            Array enumValues = Enum.GetValues(enumType);

            foreach (var enumValue in enumValues)
            {
                string name = Enum.GetName(enumType, enumValue);
                var field = enumType.GetField(name);
                DescriptionAttribute attr =
                       Attribute.GetCustomAttribute(field,
                         typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null && attr.Description != null && attr.Description == str) return enumValue;
                if (name == str) return enumValue;
            }

            return isNullable ? null : (object)0;
        }
    }
}
