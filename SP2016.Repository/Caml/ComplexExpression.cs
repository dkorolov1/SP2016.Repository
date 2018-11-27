using System.Collections.Generic;
using System.Linq;

namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Выражение на языке Caml
    /// </summary>
    public class ComplexExpression : IExpression
    {
        /// <summary>
        /// Конструктор выражения
        /// </summary>
        public ComplexExpression()
        {
            this.Expressions = new List<IExpression>();
        }

        /// <summary>
        /// Конструктор выражения
        /// </summary>
        /// <param name="op">Оператор</param>
        public ComplexExpression(Operator op)
            : this()
        {
            Operator = op;
        }

        /// <summary>
        /// Конструктор выражения
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <param name="expressions">Фильтры</param>
        public ComplexExpression(Operator op, IEnumerable<IExpression> expressions)
            : this(op)
        {
            this.Expressions = expressions.ToList();
        }
        
        /// <summary>
        /// Конструктор выражения
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <param name="expressions">Фильтры</param>
        public ComplexExpression(Operator op, params IExpression[] expressions)
            : this(op)
        {
            this.Expressions = expressions.ToList();
        }

        /// <summary>
        /// Оператор для связи выражений
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// Список выражений для связи
        /// </summary>
        public IList<IExpression> Expressions { get; private set; }

        public void Add(IExpression expression)
        {
            Expressions.Add(expression);
        }

        public void Add(IEnumerable<IExpression> expressions) 
        {
            foreach (IExpression expression in expressions)
            {
                Expressions.Add(expression);
            }
        }

        public static ComplexExpression operator +(ComplexExpression expr1, ComplexExpression expr2)
        {
            return CreateExpression(expr1, expr2, Operator.Or);
        }

        public static ComplexExpression operator *(ComplexExpression expr1, ComplexExpression expr2)
        {
            return CreateExpression(expr1, expr2, Operator.And);
        }

        public static ComplexExpression operator +(ComplexExpression expr1, Filter filter2)
        {
            return CreateExpression(expr1, filter2, Operator.Or);
        }

        public static ComplexExpression operator *(ComplexExpression expr1, Filter filter2)
        {
            return CreateExpression(expr1, filter2, Operator.And);
        }

        public static ComplexExpression operator *(Filter filter1, ComplexExpression expr2) 
        {
            return CreateExpression(expr2, filter1, Operator.And);
        }

        public static ComplexExpression operator +(Filter filter1, ComplexExpression expr2)
        {
            return CreateExpression(filter1, expr2, Operator.Or);
        }

        public static ComplexExpression CreateExpression(IExpression expr1, IExpression expr2, Operator op)
        {
            ComplexExpression expr = new ComplexExpression();
            expr.Operator = op;
            expr.Expressions.Add(expr1);
            expr.Expressions.Add(expr2);
            return expr;
        }
    }
}
