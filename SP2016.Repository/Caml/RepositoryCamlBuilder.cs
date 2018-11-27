using Microsoft.SharePoint;
using SP2016.Repository.Caml;
using SP2016.Repository.Mapping;
using System;

namespace SP2016.Repository.Repository
{
    /// <summary>
    /// Построитель caml выражений с учетом отображения столбцов списка на свойства сущностей
    /// </summary>
    public class RepositoryCamlBuilder<TEntity> : SimpleCamlBuilder
    {
        public SPFieldToPropertyMapper ListItemFieldMapper { get; set; }
        public SPFieldCollection FieldCollection { get; set; }

        public override string BuildCaml(FieldReference fieldReference)
        {
            FieldReference modifiedReference = (FieldReference)fieldReference.Clone();
            modifiedReference.FieldInternalName = ReplacePropertyNameWithFieldInternalName(fieldReference.FieldInternalName);
            return base.BuildCaml(modifiedReference);
        }

        private string ReplacePropertyNameWithFieldInternalName(string propertyName)
        {
            string displayName = ListItemFieldMapper.GetFieldDisplayName(propertyName);
            if (FieldCollection.ContainsField(displayName))
                return FieldCollection.GetField(displayName).InternalName;
            else
                throw new InvalidOperationException(string.Format("Поле {0} не найдено.", displayName));
        }
    }
}
