//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DashTechCRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CandidateTechnicalExpertDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CandidateTechnicalExpertDetail()
        {
            this.BatchSessionDetails = new HashSet<BatchSessionDetail>();
            this.BatchSessionDetails1 = new HashSet<BatchSessionDetail>();
            this.TaskManageMasters = new HashSet<TaskManageMaster>();
            this.TaskManageMasters1 = new HashSet<TaskManageMaster>();
        }
    
        public int CTId { get; set; }
        public System.DateTime CTAssignDate { get; set; }
        public bool CTReassigned { get; set; }
        public string CTAssingedRemarks { get; set; }
        public string CTReassignedRemarks { get; set; }
        public int RefAssignedCandidateId { get; set; }
        public int RefAssignedExpertId { get; set; }
        public bool CTAssignIsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public string AssignedBy { get; set; }
        public Nullable<int> RefBatchId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchSessionDetail> BatchSessionDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchSessionDetail> BatchSessionDetails1 { get; set; }
        public virtual CandidateBatchDetail CandidateBatchDetail { get; set; }
        public virtual CandidateBatchDetail CandidateBatchDetail1 { get; set; }
        public virtual CandidateMaster CandidateMaster { get; set; }
        public virtual CandidateMaster CandidateMaster1 { get; set; }
        public virtual UserAccountDetail UserAccountDetail { get; set; }
        public virtual UserAccountDetail UserAccountDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskManageMaster> TaskManageMasters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskManageMaster> TaskManageMasters1 { get; set; }
    }
}
