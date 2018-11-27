using SP2016.Repository.Enums;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Caml
{
    public class DateTimeValue
    {
        public string Value { get; private set; }

        public DateTimeValue(DateTimeValues valueType) 
        {
            switch ((DateTimeValues)valueType)
            {
                case DateTimeValues.Now:
                    Value = "<Now />";
                    break;
                case DateTimeValues.Today:
                    Value = "<Today />";
                    break;
                case DateTimeValues.Month:
                    Value = "<Month />";
                    break;
                default:
                    throw new ArgumentException("Неправильное значение для поля типа DateTimeValue", "valueType");
            }
        }

        public DateTimeValue(int offsetDays)
        {
            Value = string.Format("<Today OffsetDays=\"{0}\"/>", offsetDays);
        }
    }
}
