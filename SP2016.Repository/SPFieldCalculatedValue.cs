namespace SP2016.Repository
{
    public class SPFieldCalculatedValue
    {
        public SPFieldCalculatedValue() { }

        public SPFieldCalculatedValue(string type, string value)
        {
            Type = type;
            Value = value;
        }
            
        public string Value { get; protected set; }
        
        public string Type { get; protected set; }

        public override string ToString()
        {
            return Type + ";#" +this.Value;
        }
    }
}
