////===============================================================================
//// Microsoft patterns & practices
//// SharePoint Guidance version 2.0
////===============================================================================
//// Copyright (c) Microsoft Corporation.  All rights reserved.
//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//// FITNESS FOR A PARTICULAR PURPOSE.
////===============================================================================
//// The example companies, organizations, products, domain names,
//// e-mail addresses, logos, people, places, and events depicted
//// herein are fictitious.  No association with any real company,
//// organization, product, domain name, email address, logo, person,
//// places, or events is intended or should be inferred.
////===============================================================================

namespace SP2016.Repository.Caml
{
    /// <summary>
    /// Class that holds filter expressions that can be used by the <see cref="CAMLQueryBuilder"/>. 
    /// </summary>
    public class CamlFilter
    {
        /// <summary>
        /// The filter expression to use when building a query. 
        /// </summary>
        public string FilterExpression { get; set; }
    }
}
