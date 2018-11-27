namespace SP2016.Repository.Caml
{
    public abstract class FieldReferenceBase
    {
        /// <summary>
        /// Внутренне имя поля
        /// </summary>
        /// 
        public string FieldInternalName { get; set; }

        public FieldReferenceBase(string fieldInternalName)
        {
            this.FieldInternalName = fieldInternalName;
        }

        public FieldReferenceBase() { }

    }
}
