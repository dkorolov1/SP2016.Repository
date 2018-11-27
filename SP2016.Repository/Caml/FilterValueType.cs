namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Тип значения Caml-запроса
    /// </summary>
    public enum FilterValueType
    {
        /// <summary>
        /// Тип значения отсутствует
        /// </summary>
        None = 0,

        /// <summary>
        /// Тип значения "Текст"
        /// </summary>
        Text,

        /// <summary>
        /// Тип значения "Счетчик"
        /// </summary>
        /// <remarks>Используется при указании значения колонки типа ID</remarks>
        Counter,

        /// <summary>
        /// Тип значения "Логическое значение"
        /// </summary>
        Boolean,
 
        /// <summary>
        /// Тип значения "Подстановка"
        /// </summary>
        Lookup,

        /// <summary>
        /// Тип значения "Дата"
        /// </summary>
        Date,

        /// <summary>
        /// Тип значения "Дата и время"
        /// </summary>
        DateTime,

        /// <summary>
        /// Тип значения "Число"
        /// </summary>
        Number,

        /// <summary>
        /// Тип значения "Выбор"
        /// </summary>
        Choice, 

        /// <summary>
        /// Тип значения "Целое"
        /// </summary>
        Integer,

        /// <summary>
        /// Тип значения "Идентификатор типа содержимого"
        /// </summary>
        ContentTypeId,

        /// <summary>
        /// Тип значения "Множественная подстановка"
        /// </summary>
        MultiLookup,

        /// <summary>
        /// ИД пользователя
        /// </summary>
        UserId,

        /// <summary>
        /// Тип значения "Подстановка" по значению
        /// </summary>
        LookupByValue,

        /// <summary>
        /// Тип значения "Гиперссылка или рисунок"
        /// </summary>
        URL,

        /// <summary>
        /// Тип значения "GUID"
        /// </summary>
        GUID,

        /// <summary>
        /// Тип значения "Управляемые метаданные (WssId)"
        /// </summary>
        Taxonomy,

        /// <summary>
        /// Тип значения "Управляемые метаданные (WssId)"
        /// </summary>
        MultiTaxonomy,

        /// <summary>
        /// Тип значения "Управляемые метаданные (по значению)"
        /// </summary>
        TaxonomyByValue
    }
}
