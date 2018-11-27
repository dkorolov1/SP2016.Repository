namespace SP2016.Repository.Enums
{
    /// <summary>
    /// Тип фильтрации
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Тип фильтрации не указан
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Фильтр на равенство
        /// </summary>
        Equal,

        /// <summary>
        /// Фильтр на неравенство
        /// </summary>
        NotEqual,

        /// <summary>
        /// Фильтр "больше, чем"
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Фильтр "больше или равно"
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Фильтр "меньше чем"
        /// </summary>
        LowerThan,

        /// <summary>
        /// Фильтр "меньше или равно"
        /// </summary>
        LowerThanOrEqual,

        /// <summary>
        /// Фильтр на Null
        /// </summary>
        IsNull,

        /// <summary>
        /// Фильтр на Not Null
        /// </summary>
        IsNotNull,

        /// <summary>
        /// Фильтр "начинается с"
        /// </summary>
        BeginsWith,

        /// <summary>
        /// Фильтр "содержит"
        /// </summary>
        Contains,

        /// <summary>
        /// Фильтр "Попадает в диапазон дат"
        /// </summary>
        DateRangesOverlap,

        /// <summary>
        /// Фильтр "Содержится в перечисленных значениях"
        /// </summary>
        In
    }
}
