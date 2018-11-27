using SP2016.Repository.Enums;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Ссылка на поле
    /// </summary>
    public class FieldReference : FieldReferenceBase, ICloneable
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public FieldReference() { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fieldInternalName">Внутренее имя поля</param>
        public FieldReference(string fieldInternalName)
            : base(fieldInternalName){ }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fieldInternalName">Внутренее имя поля</param>
        public FieldReference(string fieldInternalName, SortOrder sortOrder)
            : this(fieldInternalName)
        {
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Дополнительные атрибуты
        /// </summary>
        public readonly IDictionary<string, string> CustomAttributes = new Dictionary<string, string>();

        #region ICloneable Members

        public object Clone()
        {
            FieldReference newRefeerence = new FieldReference(this.FieldInternalName);
            newRefeerence.SortOrder = this.SortOrder;
            foreach (string key in this.CustomAttributes.Keys)
            {
                newRefeerence.CustomAttributes[key] = this.CustomAttributes[key];
            }
            return newRefeerence;
        }

        #endregion
    }
}
