using System.Collections.Generic;

namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Запрос на языке Caml
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Создать запрос на языке Caml
        /// </summary>
        public Query()
        {
            OrderBy = new List<FieldReference>();
            GroupBy = new List<FieldReference>();
            Collapse = true;
            GroupLimit = 30;
        }

        /// <summary>
        /// Создать запрос на языке Caml
        /// </summary>
        /// <param name="expression">Выражение для фильтрации</param>
        public Query(IExpression expression) 
            : this()
        {
            Where = expression;
        }

        /// <summary>
        /// Создать запрос на языке Caml
        /// </summary>
        /// <param name="expression">Выражение для фильтрации</param>
        /// <param name="recursive">Рекурсивно для папок</param>
        public Query(IExpression expression, bool recursive)
            : this(expression)
        {
            Recursive = recursive;
        }

        /// <summary>
        /// Фильтр выражения
        /// </summary>
        public IExpression Where { get; set; }

        /// <summary>
        /// Порядок вывода результатов
        /// </summary>
        public IList<FieldReference> OrderBy { get; private set; }

        /// <summary>
        /// Группировка результатов
        /// </summary>
        public IList<FieldReference> GroupBy { get; private set; }

        /// <summary>
        /// Получить элементы рекурсивно из всех папок
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Показывать группы свернутыми
        /// </summary>
        public bool Collapse { get; set; }
        /// <summary>
        /// Максимальное количество групп на странице
        /// </summary>
        public int GroupLimit { get; set; }

        public override string ToString()
        {
            return SimpleCamlBuilder.Instance.BuildCaml(this);
        }    
    }
}
