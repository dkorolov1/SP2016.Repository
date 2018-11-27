using System;

namespace SP2016.Repository.Caml
{
    public class ParameterBindingValue
    {
        public string Value { get; set; }

        public override string ToString()
        {
            if (Value != null)
                return Value;
            else
                return "";
        }
    }
}
