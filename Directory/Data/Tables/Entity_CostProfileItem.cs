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
    
    public partial class Entity_CostProfileItem
    {
        public int Id { get; set; }
        public int CostProfileId { get; set; }
        public int CostTypeId { get; set; }
        public string CostTypeOther { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public string PaymentPattern { get; set; }
        public Nullable<System.Guid> PayeeUid { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> LastUpdatedById { get; set; }
        public string OtherResidencyType { get; set; }
        public string OtherEnrollmentType { get; set; }
        public string OtherApplicableAudienceType { get; set; }
        public System.Guid RowId { get; set; }
        public string ProfileName { get; set; }
    
        public virtual Codes_PropertyValue Codes_PropertyValue { get; set; }
        public virtual Entity_CostProfile Entity_CostProfile { get; set; }
    }
}
