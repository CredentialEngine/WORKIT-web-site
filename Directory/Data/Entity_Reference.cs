//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Entity_Reference
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string TextValue { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> LastUpdatedById { get; set; }
        public Nullable<int> PropertyValueId { get; set; }
    
        public virtual Codes_PropertyCategory Codes_PropertyCategory { get; set; }
        public virtual Codes_PropertyValue Codes_PropertyValue { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
