using DashTechCRM.Models;
using DashTechCRM.Models.User;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DashTechCRM.Areas.SalesManager.Controllers
{
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        dbConnectionModel directDb = new dbConnectionModel();

        // GET: SalesManager/dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TeamRecurringList()
        {
            return View();
        }
        #region Json Result
        public JsonResult GetCandidate(DateTime? date = null)
        {
            DateTime condidateDate = DateTime.Now;
            string condition = "";
            if (date != null)
            {
                condidateDate = (DateTime)date;
                condition = string.Format(" and CandidateMaster.Date = '{0}'", condidateDate.ToString("yyyy-MM-dd"));
            }
            else
            {

                condition = string.Format(" and CandidateMaster.Date < '{0}'", condidateDate.ToString("yyyy-MM-dd"));
            }
            UserObject user = Session["userInfo"] as UserObject;
            string query = string.Format(@"select CandidateMaster.*, CandidateMaster.Date as EnrolledDate, TeamLeadUser.UserId, TeamLeadUser.FullName as TeamLeadName, 
ManagerUser.FullName as ManagerName , MemberUser.RocketName as SalesAssociateName, PODetails.PODate,
SalesServiceMaster.*,RecurringType.*
from CandidateMaster 
inner join TeamDetails on Member = RefSalesAssociate
inner join SalesServiceMaster on RefServiceId = ServiceId
inner join RecurringType on RecurringTypeId = RefRecurringTypeId
inner join UserAccountDetails as TeamLeadUser on TeamLeadUser.UserId = TeamLead
inner join UserAccountDetails as ManagerUser on ManagerUser.UserId = TeamManager
inner join UserAccountDetails as MemberUser on MemberUser.UserId = Member
left join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateId
left join CandidateAssign on CandidateAssign.refMarketingId = MarketingId
left join TeamDetails as MarketingTeam on MarketingTeam.TeamId = RefTeamId
left join UserAccountDetails as MarketingTeamLeadUser on MarketingTeamLeadUser.UserId =MarketingTeam.TeamLead
left join UserAccountDetails as MarketingManagerUser on MarketingManagerUser.UserId = MarketingTeam.Member
left join UserAccountDetails as MarketingMemberUser on MarketingMemberUser.UserId = MarketingTeam.TeamManager
left join PODetails on PODetails.CandidateId = CandidateMaster.CandidateId

where TeamDetails.TeamManager = {0} {1} order by PODate desc", user.UserId, condition);
            dbConnectionModel directDb = new dbConnectionModel();
            directDb.query = query;
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTeamRecurringJson()
        {
            UserObject user = Session["userInfo"] as UserObject;
            directDb.query = @"
select CandidateMaster.CandidateName, CandidateMaster.EmailId,AgreementPercentage,AgreementSent,CandidateMaster.Date as EnrolledDate, TotalAmount, PaidAmount,ServiceName, RecurringMaster.*,RecurringMaster.PaymentStatus from CandidateMaster 
inner join UserAccountDetails on RefSalesAssociate = UserId
inner join SalesServiceMaster on ServiceId = RefServiceId
inner join RecurringType on RecurringTypeId = RefRecurringTypeId
inner join RecurringMaster on CandidateMaster.CandidateId = RecurringMaster.RefCandidateId
inner join TeamDetails on Member = UserId
left join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
left join CandidateAssign on refMarketingId = MarketingId
left join PODetails on CandidateMaster.CandidateId = PODetails.CandidateId where TeamDetails.TeamManager = " + user.UserId;
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public ActionResult ChangeStatus(string status, string remarks, int candidateId)
        {
            UserObject user = Session["userInfo"] as UserObject;

            CandidateMaster candidate = db.CandidateMasters.Find(candidateId);
            candidate.CandidateStatus = status;
            db.SaveChanges();

            CandidateMarketingDetail marketing = db.CandidateMarketingDetails.FirstOrDefault(a => a.RefCandidateId == candidateId);
            marketing.MarketingStatus = status;
            db.SaveChanges();

            FollowUpMaster follow = new FollowUpMaster();
            follow.Department = "Sales";
            follow.FollowUpBy = user.RocketName;
            follow.FollowUpDate = DateTime.Now.Date;
            follow.FollowUpMessage = remarks;
            follow.FollowUpStatus = status;
            follow.FollowUpTime = DateTime.Now.TimeOfDay;
            follow.RefCandidateId = candidate.CandidateId;
            db.FollowUpMasters.Add(follow);
            db.SaveChanges();
            return RedirectToAction("AssignedCandidate");
        }
    }
}
