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
    
    public partial class TargetDetail
    {
        public int TargetId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public decimal Revenue { get; set; }
        public int PO { get; set; }
        public int Lead { get; set; }
        public int DailyCall { get; set; }
        public int Candidate { get; set; }
    }
}
