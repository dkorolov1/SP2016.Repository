using System.Reflection;

namespace SP2016.Repository.Converters.Common
{
    public class SimpleConverter : IConverter
    {
        /// <summary>
        /// Конвертируем значение свойства в значение поля
        /// </summary>
        /// <param name="propertyValue">Значение свойства</param>
        /// <returns>Значение поля</returns>
        public virtual object ConvertPropertyValueToFieldValue(PropertyInfo propertyInfo, object propertyValue)
        {
            return propertyValue;
        }

        /// <summary>
        /// Конвертируем значение поля в значение свойства
        /// </summary>
        /// <param name="propertyValue">Значение свойства</param>
        /// <returns>Значение поля</returns>
        public virtual object ConvertFieldValueToPropertyValue(PropertyInfo propertyInfo, object fieldValue)
        {
            return fieldValue;
        }
    }
}
