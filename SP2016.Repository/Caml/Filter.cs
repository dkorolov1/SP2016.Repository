using SP2016.Repository.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Caml выражение фильтрации 
    /// </summary>    
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Свойство используется в интерфейсе")]
    public class Filter : IExpression
    {

        /// <summary>
        /// Тип фильтра
        /// </summary>
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Ссылка на поле
        /// </summary>
        public FieldReference FieldReference
        {
            get
            {
                if (FieldReferences == null || FieldReferences.Length == 0)
                    return null;

                if (FieldReferences.Length > 1)
                    throw new InvalidOperationException("Нельзя использовать FieldReference при более чем 1 ссылке на поле, используйте FieldReferences");

                return FieldReferences[0];
            }
            set
            {
                FieldReferences = new FieldReference[] { value };
            }
        }

        /// <summary>
        /// Ссылка на несколько полей
        /// </summary>
        /// <remarks>Актуально при использовании DateRangesOverlap</remarks>
        public FieldReference[] FieldReferences { get; set; }

        /// <summary>
        /// Тип значения
        /// </summary>
        public FilterValueType ValueType { get; set; }

        /// <summary>
        /// Значение для фильтрации
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Конструктор фильтра
        /// </summary>
        /// <param name="filterType">Тип фильтра</param>
        /// <param name="fieldInternalNames">Внутренние имена полей для фильтрации</param>
        /// <param name="value">Значения поля</param>
        /// <param name="valueType">Тип значения</param>
        public Filter(FilterType filterType, string[] fieldInternalNames, object value, FilterValueType valueType)
        {
            if (fieldInternalNames == null || fieldInternalNames.Length == 0)
                throw new InvalidOperationException("Список полей для фильтрации не может быть пустым");

            FilterType = filterType;
            FieldReferences = new FieldReference[fieldInternalNames.Length];

            for (int i = 0; i < fieldInternalNames.Length; i++)
            {
                this.FieldReferences[i] = new FieldReference(fieldInternalNames[i]);
            }
            this.Value = value;
            this.ValueType = valueType;
        }

        /// <summary>
        /// Конструктор фильтра
        /// </summary>
        /// <param name="filterType">Тип фильтра</param>
        /// <param name="fieldInternalName">Внутреннее имя поля для фильтрации</param>
        /// <param name="value">Значения поля</param>
        /// <param name="valueType">Тип значения</param>
        public Filter(FilterType filterType, string fieldInternalName, object value, FilterValueType valueType)
        {
            this.FilterType = filterType;
            this.FieldReference = new FieldReference(fieldInternalName);
            this.Value = value;
            this.ValueType = valueType;
        }

        /// <summary>
        /// Конструктор фильтра
        /// </summary>
        /// <param name="filterType">Тип фильтра</param>
        /// <param name="fieldInternalName">Внутреннее имя поля для фильтрации</param>
        public Filter(FilterType filterType, string fieldInternalName)
        {
            this.FilterType = filterType;
            this.FieldReference = new FieldReference(fieldInternalName);
        }

        public static ComplexExpression operator +(Filter filter1, Filter filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.Or);
        }

        public static ComplexExpression operator *(Filter filter1, Filter filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.And);
        }
    }
}
