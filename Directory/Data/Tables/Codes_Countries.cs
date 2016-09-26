//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Tables
{
    using System;
    using System.Collections.Generic;
    
    public partial class Codes_Countries
    {
        public int Id { get; set; }
        public int CountryNumber { get; set; }
        public string CommonName { get; set; }
        public string FormalName { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Sovereignty { get; set; }
        public string Capital { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CommonCurrencyName { get; set; }
        public Nullable<double> TelephoneCode { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public string IANACountryCodeTLD { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}
