namespace SP2016.Repository.Caml
{
    public class SiteQueryFieldReference : FieldReferenceBase
    {
        public bool Nulleable { get; set; }

        public SiteQueryFieldReference(string fieldInternalName)
            : base(fieldInternalName)
        {
            FieldInternalName = fieldInternalName;
        }

        public SiteQueryFieldReference(string fieldInternalName, bool nulleable = false)
            : base(fieldInternalName)
        {
            FieldInternalName = fieldInternalName;
            Nulleable = nulleable;
        }
    }
}
