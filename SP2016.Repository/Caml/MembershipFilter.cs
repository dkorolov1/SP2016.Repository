
using SP2016.Repository.Enums;

namespace SP2016.Repository.Caml
{
    public class MembershipFilter : IExpression
    {
        public int SPGroupId { get; set; }

        public FieldReference FieldReference { get; set; }

        public MembershipFilterType FilterType { get; set; }

        /// <summary>
        /// Конструктор фильтра
        /// </summary>
        /// <param name="membershipFilterType">Тип фильтра</param>
        /// <param name="fieldInternalName">Внутреннее имя поля</param>
        /// <param name="spGroupId">Ид группы. Требуется указать при использовании фильтра SPGroup</param>
        public MembershipFilter(MembershipFilterType membershipFilterType, string fieldInternalName, int spGroupId = 0)
        {
            FilterType = membershipFilterType;
            FieldReference = new FieldReference(fieldInternalName);
            SPGroupId = spGroupId;
        }

        public static ComplexExpression operator +(IExpression filter1, MembershipFilter filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.Or);
        }

        public static ComplexExpression operator *(IExpression filter1, MembershipFilter filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.And);
        }

        public static ComplexExpression operator +(MembershipFilter filter1, IExpression filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.Or);
        }

        public static ComplexExpression operator *(MembershipFilter filter1, IExpression filter2)
        {
            return ComplexExpression.CreateExpression(filter1, filter2, Operator.And);
        }
    }
}
