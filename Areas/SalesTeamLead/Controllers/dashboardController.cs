using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DashTechCRM.Models;
using DashTechCRM.Models.User;

namespace DashTechCRM.Areas.SalesTeamLead.Controllers
{
    public class dashboardController : Controller
    {
        // GET: SalesTeamLead/dashboard
        public ActionResult Index()
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
            UserObject user = Session["userInfo"] as UserObject;
            string query = string.Format(@"select CandidateMaster.*, TeamLeadUser.UserId, TeamLeadUser.FullName as TeamLeadName, 
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

where TeamDetails.TeamLead = {0} {1} order by PODate desc", user.UserId,condition);
            dbConnectionModel directDb = new dbConnectionModel();
            directDb.query = query;
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}