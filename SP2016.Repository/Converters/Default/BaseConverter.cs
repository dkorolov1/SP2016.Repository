using System.Reflection;

namespace SP2016.Repository.Converters.Default
{
    public class BaseConverter
    {
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Конвертируем значение свойства в значение поля
        /// </summary>
        /// <param name="propertyValue">Значение свойства</param>
        /// <returns>Значение поля</returns>
        public virtual object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            return propertyValue;
        }

        /// <summary>
        /// Конвертируем значение поля в значение свойства
        /// </summary>
        /// <param name="propertyValue">Значение свойства</param>
        /// <returns>Значение поля</returns>
        public virtual object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            return fieldValue;
        }
    }
}
