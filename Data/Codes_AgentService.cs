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
    
    public partial class Codes_AgentService
    {
        public Codes_AgentService()
        {
            this.Organization_Service = new HashSet<Organization_Service>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SchemaTag { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsQAService { get; set; }
        public Nullable<bool> IsCredentialingService { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> Totals { get; set; }
    
        public virtual ICollection<Organization_Service> Organization_Service { get; set; }
    }
}
