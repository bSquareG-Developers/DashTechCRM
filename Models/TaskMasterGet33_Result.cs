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
    
    public partial class TaskMasterGet33_Result
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string TaskDetails { get; set; }
        public string AssignedByName { get; set; }
        public string AssignedToName { get; set; }
        public string AssignDate { get; set; }
        public string AssignTime { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Nullable<System.DateTime> defaultEndDate { get; set; }
        public Nullable<System.TimeSpan> defaultEndTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
        public string FeedBack { get; set; }
        public Nullable<int> TaskTitleId { get; set; }
        public string AssignTo { get; set; }
    }
}
