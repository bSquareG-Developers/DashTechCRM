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
    
    public partial class CandidateAssign_Insert_Result
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> RefSalesAssociate { get; set; }
        public int RefRecurringTypeId { get; set; }
        public int RefServiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public System.DateTime Date { get; set; }
        public string PaymentStatus { get; set; }
        public string CandidateStatus { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> MarketingStartDate { get; set; }
        public string VisaStatus { get; set; }
        public int TechnologyId { get; set; }
        public Nullable<bool> AgreementSent { get; set; }
        public Nullable<decimal> Agreement { get; set; }
        public string AgreementLink { get; set; }
    }
}
