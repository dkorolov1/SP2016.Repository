using Microsoft.SharePoint;
using SP2016.Repository.Mapping;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SP2016.Repository.Utils;

namespace SP2016.Repository.Batch
{
    [Serializable]
    public class Operation
    {
        [XmlIgnore]
        public BaseEntity Entity { get; set; }
        [XmlAttribute("ID")]
        public int OperationID { get; set; }
        [XmlElement]
        public SetList SetList { get; set; }
        [XmlElement]
        public SetFieldValue[] SetVar { get; set; }

        public Operation() { }

        public Operation(SPWeb web, int operationId, SPList list, BaseEntity entity, Command command, ISPFieldToPropertyMapper mapper, string folderPath = null)
        {
            Entity = entity;
            OperationID = operationId;
            SetList = new SetList { ListId = list.ID.ToString("B") };
            var setVars = new List<SetFieldValue>(new[] { SetFieldValue.ForID(entity.ID), SetFieldValue.ForCmd(command) });

            if (string.IsNullOrEmpty(folderPath))
                setVars.Add(SetFieldValue.ForFolder(list, folderPath));

            foreach (var mapping in mapper.Mappings)
            {
                if (!mapping.ReadOnly && mapping.FieldName != "ID")
                    setVars.Add(GetSetVarEntry(web, list, entity, mapper, mapping));
            }
            SetVar = setVars.ToArray();
        }

        private SetFieldValue GetSetVarEntry(SPWeb web, SPList list, BaseEntity entity, ISPFieldToPropertyMapper mapper, FieldToEntityPropertyMapping mapping)
        {
            var entityType = entity.GetType();
            var propertyInfo = ReflectionUtil.GetPropertyInfo(entityType, mapping);
            SPField field = ItemFieldsChecking.EnsureListFieldID(list, entityType, mapping);

            object propertyValue = propertyInfo.GetValue(entity, null);
            object fieldValue;
            if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                fieldValue = null;
            else
                fieldValue = mapper.GetAfterPropertiesFieldValuesConverter(web, propertyInfo, field).ConvertPropertyValueToFieldValue(propertyValue);

            return new SetFieldValue(GetInternalName(list, mapping.FieldName), fieldValue);
        }

        private string GetInternalName(SPList list, string staticName)
        {
            SPField field;
            if ((field = list.Fields.TryGetFieldByStaticName(staticName)) != null)
                return field.InternalName;

            if (list.Fields.ContainsField(staticName))
                return list.Fields[staticName].InternalName;

            return null;
        }
    }
}