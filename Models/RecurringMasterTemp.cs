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
    
    public partial class RecurringMasterTemp
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> CandidateId { get; set; }
        public string ReceivedIn { get; set; }
        public string SendRemainderEmail { get; set; }
        public string PaymentStatus { get; set; }
        public string Remarks { get; set; }
        public string InstallmentNumber { get; set; }
    }
}
