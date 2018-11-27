namespace SP2016.Repository.Constants
{
    public static class SPConstants
    {
        public static class CAML_QUERY
        {
            public const string COMMON_EQ = "<Where><Eq><FieldRef Name='{0}' /><Value Type='{1}'>{2}</Value></Eq></Where>";
            public const string COMMON_NULL = "<Where><IsNull><FieldRef Name='{0}' /></IsNull></Where>";
            public const string COMMON_LOOKUP_ID_EQ = "<Where><Eq><FieldRef Name='{0}' LookupId='true'/><Value Type='Lookup'>{1}</Value></Eq></Where>";
            public const string COMMON_NE = "<Where><Neq><FieldRef Name='{0}' /><Value Type='{1}'>{2}</Value></Neq></Where>";
            public const string ORDER_BY = "<OrderBy><FieldRef Name='{0}' Ascending='{1}' /></OrderBy>";
            public const string RECURSIVE_SCOPE = "Scope=\"RecursiveAll\"";
        }

        public static class LIST
        {
            public static class URL
            {
                /// <summary>
                /// "Lists/"
                /// </summary>
                public const string PREFIX = "Lists/";
            }

            public static class FIELD
            {
                public static class TYPE
                {
                    /// <summary>
                    /// Text
                    /// </summary>
                    public const string TEXT = "Text";

                    /// <summary>
                    /// Counter
                    /// </summary>
                    public const string COUNTER = "Counter";
                }
            }
        }
    }
}
