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
    
    public partial class CommentDetail
    {
        public int CommentId { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public string CommentBy { get; set; }
        public int RefCandidateId { get; set; }
        public string CommentText { get; set; }
    
        public virtual CandidateMaster CandidateMaster { get; set; }
        public virtual CandidateMaster CandidateMaster1 { get; set; }
    }
}
