using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashTechCRM.Models.Reports
{
    public class FollowUpLogic
    {
        //FollowUpId	
        public static void setFollowUp(string FollowUpStatus, string FollowUpMessage, string FollowUpBy, int RefCandidateId, string Department)
        {
            FollowUpMaster follow = new FollowUpMaster();
            follow.Department = Department;
            follow.FollowUpBy = FollowUpBy;
            follow.FollowUpDate = DateTime.Now.Date;
            follow.FollowUpTime = DateTime.Now.TimeOfDay;
            follow.RefCandidateId = RefCandidateId;
            follow.FollowUpMessage = FollowUpMessage;
            dashTech_crm_Entities db = new dashTech_crm_Entities();
            db.FollowUpMasters.Add(follow);
            db.SaveChanges();
        }
    }
}