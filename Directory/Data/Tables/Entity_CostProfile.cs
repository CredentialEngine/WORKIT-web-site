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
    
    public partial class Entity_CostProfile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Entity_CostProfile()
        {
            this.Entity_CostProfileItem = new HashSet<Entity_CostProfileItem>();
        }
    
        public int Id { get; set; }
        public System.Guid ParentUid { get; set; }
        public int ParentTypeId { get; set; }
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateEffective { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string DetailsUrl { get; set; }
        public Nullable<int> CurrencyTypeId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> LastUpdatedById { get; set; }
        public System.Guid RowId { get; set; }
        public Nullable<int> EntityId { get; set; }
        public string Currency { get; set; }
    
        public virtual Codes_Currency Codes_Currency { get; set; }
        public virtual Entity Entity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Entity_CostProfileItem> Entity_CostProfileItem { get; set; }
    }
}
