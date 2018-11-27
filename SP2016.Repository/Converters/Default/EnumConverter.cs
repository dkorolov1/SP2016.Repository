using System;
using System.ComponentModel;

namespace SP2016.Repository.Converters.Default
{
    public class EnumConverter : BaseConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
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

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            if (fieldValue == null)
                return null;

            var isNullable = Nullable.GetUnderlyingType(PropertyInfo.PropertyType) != null;
            var enumType = Nullable.GetUnderlyingType(PropertyInfo.PropertyType) ?? PropertyInfo.PropertyType;
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
