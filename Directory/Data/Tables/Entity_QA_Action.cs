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
    
    public partial class Entity_QA_Action
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int RelationshipTypeId { get; set; }
        public System.Guid AgentUid { get; set; }
        public string Description { get; set; }
        public int IssuedCredentialId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> LastUpdatedById { get; set; }
        public System.Guid RowId { get; set; }
    
        public virtual Codes_CredentialAgentRelationship Codes_CredentialAgentRelationship { get; set; }
        public virtual Credential Credential { get; set; }
        public virtual Entity Entity { get; set; }
    }
}