using System.Diagnostics;

namespace SP2016.Repository.Mapping
{
    /// <summary>
    /// Маппинг между полями SPListItem и свойствами сущности
    /// </summary>
    [DebuggerDisplay("F:{FieldName}, P:{EntityPropertyName}")]
    public class FieldToPropertyMapping
    {
        #region Конструкторы
        
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public FieldToPropertyMapping() { }
     
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="name">Название поля списка и свойства сущности</param>
        public FieldToPropertyMapping(string name)
            : this(name, name) { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="name">Название поля списка и свойства сущности</param>
        /// <param name="readOnly">Разрешать ли сохранять значения свойства при сохранении сущности</param>
        public FieldToPropertyMapping(string name, bool readOnly)
            : this(name, name, readOnly) { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fieldName">Название поля списка</param>
        /// <param name="entityPropertyName">Название свойства сущности</param>
        public FieldToPropertyMapping(string fieldName, string entityPropertyName)
        {
            FieldName = fieldName;
            EntityPropertyName = entityPropertyName;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fieldName">Название поля списка</param>
        /// <param name="entityPropertyName">Название свойства сущности</param>
        /// <param name="readOnly">Разрешать ли сохранять значения свойства при сохранении сущности</param>
        public FieldToPropertyMapping(string fieldName, string entityPropertyName, bool readOnly)
            : this(fieldName, entityPropertyName)
        {
            ReadOnly = readOnly;
        }

        #endregion

        /// <summary>
        /// Название поля
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Название свойства сущности
        /// </summary>
        public string EntityPropertyName { get; set; }

        /// <summary>
        /// Признак поля "только для чтения"
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}
