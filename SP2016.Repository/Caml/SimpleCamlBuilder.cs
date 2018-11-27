using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.SharePoint;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using Microsoft.SharePoint.Taxonomy;
using SP2016.Repository.Enums;

namespace SP2016.Repository.Caml
{
    public class SimpleCamlBuilder
    {
        public static readonly SimpleCamlBuilder Instance = new SimpleCamlBuilder();

        /// <summary>
        /// Словарь соответствия типа фильтра и его caml эквивалента
        /// </summary>
        private static IDictionary<FilterType, string> filterTypeToCamlFilterDictionary = null;

        /// <summary>
        /// Статический конструктор для заполнения словаря соответствия типа фильтра и его caml эквивалента
        /// </summary>
        static SimpleCamlBuilder()
        {
            FillOperationTypeToCamlOperationDictionary();
        }

        /// <summary>
        /// Получение caml эквививалента типа фильтра
        /// </summary>
        /// <param name="filterType">Тип фильтра</param>
        /// <returns>Caml значение для фильтра</returns>
        protected static string GetCamlRepresentation(FilterType filterType)
        {
            if (!filterTypeToCamlFilterDictionary.ContainsKey(filterType))
                throw new ArgumentOutOfRangeException("filterType", filterType, "Неизвестный или недопустимый тип фильтра");
            return filterTypeToCamlFilterDictionary[filterType];
        }

        /// <summary>
        /// Заполнение словаря соответствия типа фильтра и его caml эквивалента
        /// </summary>
        private static void FillOperationTypeToCamlOperationDictionary()
        {
            filterTypeToCamlFilterDictionary = new Dictionary<FilterType, string>();

            filterTypeToCamlFilterDictionary.Add(FilterType.Equal, "Eq");
            filterTypeToCamlFilterDictionary.Add(FilterType.NotEqual, "Neq");
            filterTypeToCamlFilterDictionary.Add(FilterType.GreaterThan, "Gt");
            filterTypeToCamlFilterDictionary.Add(FilterType.GreaterThanOrEqual, "Geq");
            filterTypeToCamlFilterDictionary.Add(FilterType.LowerThan, "Lt");
            filterTypeToCamlFilterDictionary.Add(FilterType.LowerThanOrEqual, "Leq");
            filterTypeToCamlFilterDictionary.Add(FilterType.IsNull, "IsNull");
            filterTypeToCamlFilterDictionary.Add(FilterType.IsNotNull, "IsNotNull");
            filterTypeToCamlFilterDictionary.Add(FilterType.BeginsWith, "BeginsWith");
            filterTypeToCamlFilterDictionary.Add(FilterType.Contains, "Contains");
            filterTypeToCamlFilterDictionary.Add(FilterType.DateRangesOverlap, "DateRangesOverlap");
            filterTypeToCamlFilterDictionary.Add(FilterType.In, "In");
        }

        public virtual SPSiteDataQuery BuildCaml(SiteQuery query)
        {
            SPSiteDataQuery resultQuery = new SPSiteDataQuery();

            resultQuery.Webs = BuildCaml(query.WebScope);
            resultQuery.Lists = BuildCaml(query.ListsReference);
            resultQuery.Query = BuildCaml(query.Query);
            resultQuery.ViewFields = BuildCaml(query.ViewFields.ToArray());

            return resultQuery;
        }

        /// <summary>
        /// Получение Caml-запроса соответствующего выражения
        /// </summary>
        public virtual string BuildCaml(Query query)
        {
            string appendFormat = "<{0}>{1}</{0}>";
            StringBuilder sb = new StringBuilder();

            if (query.Where != null)
                sb.AppendFormat(appendFormat, "Where", BuildCaml(query.Where));

            if (query.OrderBy.Count > 0)
                sb.AppendFormat(appendFormat, "OrderBy", GenerateCaml(query.OrderBy));

            if (query.GroupBy.Count > 0)
            {
                string groupByFormat = "<{0} Collapse=\"{2}\" GroupLimit=\"{3}\">{1}</{0}>";
                string collapse = query.Collapse ? "TRUE" : "FALSE";
                sb.AppendFormat(groupByFormat, "GroupBy", GenerateCaml(query.GroupBy), collapse, query.GroupLimit);
            }
            return sb.ToString();
        }

        public virtual string BuildCaml(ListsReference listsReference)
        {
            StringBuilder listReferencesSB = new StringBuilder();

            foreach (Guid listID in listsReference.ListsIds)
            {
                listReferencesSB.AppendFormat("<List ID='{0}'/>", listID);
            }

            string listsAttrAppendFormat = "{0}='{1}' ";

            StringBuilder listsAttributesSB = new StringBuilder();

            if (listsReference.ListsServerTemplate.HasValue)
            {
                listsAttributesSB.AppendFormat(listsAttrAppendFormat, "ServerTemplate", (int)listsReference.ListsServerTemplate);
            }

            if (!String.IsNullOrEmpty(listsReference.ListsBaseType))
            {
                listsAttributesSB.AppendFormat(listsAttrAppendFormat, "BaseType", listsReference.ListsBaseType);
            }

            if (listsReference.MaxListsLimit != 0)
            {
                listsAttributesSB.AppendFormat(listsAttrAppendFormat, "MaxListsLimit", listsReference.MaxListsLimit);
            }

            if (listsReference.SearchInHiddenLists)
            {
                listsAttributesSB.AppendFormat(listsAttrAppendFormat, "Hidden", "TRUE");
            }

            return String.Format("<Lists {0}>{1}</Lists>", listsAttributesSB.ToString(), listReferencesSB.ToString());
        }

        public virtual string BuildCaml(SiteQueryWebScope scope)
        {
            string scopeValue = String.Empty;

            switch (scope)
            {
                case SiteQueryWebScope.Recursive:
                    {
                        scopeValue = "Recursive";
                    }
                    break;
                case SiteQueryWebScope.SiteCollection:
                    {
                        scopeValue = "SiteCollection";

                    }
                    break;
                 default:
                    {
                        scopeValue = String.Empty;
                    }
                    break;
            }

            return String.Format("<Webs Scope='{0}' />",scopeValue);
        }

        public virtual string BuildCaml(SiteQueryFieldReference[] fieldReferences)
        {
            StringBuilder sb = new StringBuilder();

            foreach (SiteQueryFieldReference fieldReference in fieldReferences)
            {
                sb.AppendFormat("<FieldRef Name='{0}' Nullable='{1}' />", fieldReference.FieldInternalName, 
                    fieldReference.Nulleable ? "TRUE" : "FALSE");                
            }

            return sb.ToString();
        }

        public virtual string BuildCaml(IExpression expression)
        {
            if (expression is ComplexExpression)
                return BuildCaml((ComplexExpression)expression);
            else if (expression is Filter)
                return BuildCaml((Filter)expression);
            else if (expression is MembershipFilter)
                return BuildCaml((MembershipFilter)expression);
            else
                throw new ArgumentException("Неизвестный тип выражения", "expression");
        }

        public virtual string BuildCaml(FieldReference fieldReference)
        {
            if (string.IsNullOrEmpty(fieldReference.FieldInternalName))
                throw new InvalidOperationException("Внутренее имя поля не может быть пустым");
            string customAttributes = BuildCustomAttributes(fieldReference.CustomAttributes);
            if (fieldReference.SortOrder == SortOrder.None)
                return string.Format(CultureInfo.CurrentCulture, "<FieldRef Name='{0}' {1}/>", fieldReference.FieldInternalName, customAttributes);
            else
                return string.Format(CultureInfo.CurrentCulture, "<FieldRef Name='{0}' Ascending='{1}' {2}/>", fieldReference.FieldInternalName, (fieldReference.SortOrder == SortOrder.Ascending).ToString().ToUpper(), customAttributes);
        }

        public virtual string BuildCaml(FieldReference[] fieldReferences)
        {
            if(fieldReferences == null || fieldReferences.Length == 0)
                throw new InvalidOperationException("Количество полей должно быть больше либо равно одному");
            StringBuilder sb = new StringBuilder();
            foreach (FieldReference fieldReference in fieldReferences)
            {
                sb.AppendFormat("{0}", BuildCaml(fieldReference));
            }
            return sb.ToString();
        }

        private string BuildCustomAttributes(IDictionary<string, string> customAttributes)
        {
            List<string> pairs = new List<string>();
            foreach (KeyValuePair<string, string> keyValuePair in customAttributes)
                pairs.Add(string.Format("{0}='{1}'", keyValuePair.Key, keyValuePair.Value));
            return string.Join(" ", pairs.ToArray());
        }

        public virtual string BuildCaml(ComplexExpression complexExpression)
        {
            if (complexExpression.Expressions.Count == 1)
                return BuildCaml(complexExpression.Expressions[0]);
            if (complexExpression.Expressions.Count == 0)
                throw new InvalidOperationException("Отсутствуют дочерние выражения");
            if (complexExpression.Operator == Operator.Nothing)
                throw new InvalidOperationException("Недопустимое значение оператора");

            string template = "<{0}>{1}{2}</{0}>";
            string result = BuildCaml(complexExpression.Expressions[0]);
            for (int index = 1; index < complexExpression.Expressions.Count; index++)
            {
                result = string.Format(CultureInfo.CurrentCulture, template, complexExpression.Operator.ToString(), result, BuildCaml(complexExpression.Expressions[index]));
            }

            return result;
        }

        public virtual string BuildCaml(Filter filter)
        {
            string camlOperationText = GetCamlRepresentation(filter.FilterType);

            if (filter.FilterType == FilterType.IsNull || filter.FilterType == FilterType.IsNotNull)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "<{0}>{1}</{0}>",
                    camlOperationText,
                    BuildCaml(filter.FieldReference));
            }
            else
            {
                if (filter.ValueType == FilterValueType.None)
                    throw new InvalidOperationException("Должен быть задан тип значения");

                object value = ApplyValuePolicy(filter.Value, filter.ValueType);

                if (filter.ValueType == FilterValueType.DateTime)
                    return string.Format(
                    CultureInfo.CurrentCulture,
                    "<{0}>{1}<Value Type=\"DateTime\" IncludeTimeValue=\"TRUE\">{3}</Value></{0}>",
                    camlOperationText,
                    BuildCaml(filter.FieldReferences),
                    filter.ValueType,
                    value);

               
                if (filter.ValueType == FilterValueType.Date)
                    return string.Format(
                    CultureInfo.CurrentCulture,
                    "<{0}>{1}<Value Type=\"DateTime\" IncludeTimeValue=\"FALSE\">{3}</Value></{0}>",
                    camlOperationText,
                    BuildCaml(filter.FieldReferences),
                    filter.ValueType,
                    value);

                if (filter.FilterType == FilterType.In)
                {
                    var values = new StringBuilder();

                    foreach(object val in (IEnumerable)filter.Value)
                    {
                        values.AppendFormat("<Value Type=\"{1}\">{0}</Value>", val.ToString(), filter.ValueType);
                    }

                    return string.Format(
                       CultureInfo.CurrentCulture,
                       "<{0}>{1}<Values>{2}</Values></{0}>",
                       camlOperationText,
                       BuildCaml(filter.FieldReferences),
                       values.ToString());
                }

                if (filter.ValueType == FilterValueType.Lookup        ||
                    filter.ValueType == FilterValueType.MultiLookup   ||
                    filter.ValueType == FilterValueType.LookupByValue ||
                    filter.ValueType == FilterValueType.Taxonomy      ||
                    filter.ValueType == FilterValueType.MultiTaxonomy ||
                    filter.ValueType == FilterValueType.TaxonomyByValue)
                {
                    FieldReference fieldReference = (FieldReference)filter.FieldReference.Clone();
                    if (filter.ValueType != FilterValueType.LookupByValue &&
                        filter.ValueType != FilterValueType.TaxonomyByValue)
                        fieldReference.CustomAttributes.Add("LookupId", "TRUE");

                    if (filter.ValueType == FilterValueType.Lookup        ||
                        filter.ValueType == FilterValueType.LookupByValue ||
                        filter.ValueType == FilterValueType.Taxonomy      ||
                        filter.ValueType == FilterValueType.TaxonomyByValue)
                        return string.Format(
                            CultureInfo.CurrentCulture,
                            "<{0}>{1}<Value Type=\"Lookup\">{3}</Value></{0}>",
                            camlOperationText,
                            BuildCaml(fieldReference),
                            filter.ValueType,
                            value);
                    else
                        return string.Format(
                            CultureInfo.CurrentCulture,
                            "<{0}>{1}<Value Type=\"LookupMulti\">{3}</Value></{0}>",
                            camlOperationText,
                            BuildCaml(fieldReference),
                            filter.ValueType,
                            value);
                }

                if (filter.ValueType == FilterValueType.UserId)
                    return string.Format(
                    CultureInfo.CurrentCulture,
                    "<{0}>{1}<Value Type='User'><UserID Type='Integer'>{2}</UserID></Value></{0}>",
                    camlOperationText,
                    BuildCaml(filter.FieldReferences),
                    value ?? "");

                if (filter.ValueType == FilterValueType.GUID)
                    return String.Format(
                        CultureInfo.CurrentCulture,
                        "<{0}>{1}<Value Type='{2}'>{3}</Value></{0}>",
                        camlOperationText,
                        BuildCaml(filter.FieldReference),
                        filter.ValueType,
                        value);

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "<{0}>{1}<Value Type='{2}'>{3}</Value></{0}>",
                    camlOperationText,
                    BuildCaml(filter.FieldReferences),
                    filter.ValueType,
                    value);
            }
        }

        public virtual string BuildCaml(MembershipFilter filter)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "<Membership Type='{1}' {2}>{0}</Membership>",
                BuildCaml(filter.FieldReference),
                GetMembershipFilterTypeDescription(filter.FilterType),
                filter.FilterType == MembershipFilterType.SPGroup ? " ID='" + filter.SPGroupId + "' " : string.Empty);
        }

        private string GetMembershipFilterTypeDescription(MembershipFilterType filterType)
        { 
            switch(filterType)
            {
                case MembershipFilterType.AllUsers: return "SPWeb.AllUsers";
                case MembershipFilterType.CurrentUserGroups: return "CurrentUserGroups";
                case MembershipFilterType.Groups: return "SPWeb.Groups";
                case MembershipFilterType.SPGroup: return "SPGroup";
                case MembershipFilterType.Users: return "SPWeb.Users";
            }
            return string.Empty;
        }

        private object ApplyValuePolicy(object value, FilterValueType valueType)
        {
            object result = value;
            if (valueType == FilterValueType.Boolean)
            {
                if (!(value is Boolean))
                    throw new ArgumentException("Неправильное значение для поля типа Boolean", "value");

                Boolean boolValue = (Boolean)value;

                result = boolValue ? 1 : 0;
            }
            else if (valueType == FilterValueType.Date || valueType == FilterValueType.DateTime)
            {
                if (!(value is DateTime) && !(value is DateTimeValues) && !(value is ParameterBindingValue) && !(value is SPContentTypeId) && !(value is DateTimeValue))
                    throw new ArgumentException("Неправильное значение для поля типа DateTime", "value");

                if (value is DateTime)
                {
                    DateTime dateTimeValue = (DateTime)value;
                    result = Microsoft.SharePoint.Utilities.SPUtility.CreateISO8601DateTimeFromSystemDateTime(dateTimeValue);
                }
                if (value is DateTimeValues)
                {
                    switch ((DateTimeValues)value)
                    {
                        case DateTimeValues.Now:
                            result = "<Now />";
                            break;
                        case DateTimeValues.Today:
                            result = "<Today />";
                            break;
                        case DateTimeValues.Month:
                            result = "<Month />";
                            break;
                        default:
                            throw new ArgumentException("Неправильное значение для поля типа DateTime", "value");
                    }
                }
                if (value is DateTimeValue)
                    return (value as DateTimeValue).Value;
                if (value is ParameterBindingValue)
                    return (value as ParameterBindingValue).Value;
                if (value is SPContentTypeId)
                    return value.ToString();
            }
            else if (valueType == FilterValueType.ContentTypeId && value is SPContentTypeId)
                return value.ToString();
            else if (valueType == FilterValueType.Choice || valueType == FilterValueType.Text)
            {
                Type type = value.GetType();
                if (type.IsEnum)
                {
                    string name = Enum.GetName(type, value);

                    if (name != null)
                    {
                        FieldInfo field = type.GetField(name);
                        if (field != null)
                        {
                            DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                            if (attr != null)
                                return attr.Description;
                            else
                                return value.ToString();
                        }
                    }
                }
            }
            else if (valueType == FilterValueType.Taxonomy || valueType == FilterValueType.MultiTaxonomy)
            {
                if (value is int)
                    return value;
                if (value is TaxonomyFieldValue)
                {
                    TaxonomyFieldValue taxonomyFieldValue = value as TaxonomyFieldValue;
                    return taxonomyFieldValue.WssId;
                }
            }
            else if (valueType == FilterValueType.TaxonomyByValue)
            {
                if (value is TaxonomyFieldValue)
                {
                    TaxonomyFieldValue taxonomyFieldValue = value as TaxonomyFieldValue;
                    return taxonomyFieldValue.Label;
                }
            }
            return result;
        }

        /// <summary>
        /// Генерирует Caml текст
        /// </summary>
        /// <param name="fieldReferenceList">Список ссылок на поля</param>
        /// <returns>Caml текст запроса</returns>
        private string GenerateCaml(IList<FieldReference> fieldReferenceList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FieldReference fieldReference in fieldReferenceList)
            {
                sb.Append(BuildCaml(fieldReference));
            }

            return sb.ToString();
        }
    }
}
