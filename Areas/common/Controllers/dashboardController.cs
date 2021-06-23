using DashTechCRM.Models;
using DashTechCRM.Models.Reports;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace DashTechCRM.Areas.common.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        // GET: common/dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult candidatestatus()
        {

            TempData["st"] = false;
            UserObject user = Session["userInfo"] as UserObject;
            if (user.Role.Contains("Manager"))
            {
                TempData["st"] = true;
            }
            return View();
        }

        #region Json Results

        public string GetShortCode(string status)
        {
            switch (status)
            {

                case "Deleted": return "DLT";
                case "Dropped": return "DRP";
                case "Expert": return "EXP";
                case "Expret:Resume Call Done": return "EXPCALL";
                case "Expret:Resume Preperation": return "EXPPRE";
                case "In Marketing": return "MKT";
                case "In Training": return "TRN";
                case "Not Responding To Resume Team": return "EXPNOT";
                case "Expert:Not Responding To Resume Team": return "EXPNOT";
                case "On Hold": return "ONH";
                case "Placed": return "PLD";
                case "Resume Preparation": return "EXPPRE";
                case "Resume Verification": return "EXPVER";
                case "Expert:Resume Preparation": return "EXPPRE";
                case "Expert:Resume Verification": return "EXPVER";
                case "RUC Done": return "RUD";
                case "RUC Pending": return "RUP";
                case "Expert:RUC Pending": return "RUP";
                case "Sales": return "SAL";
                default: return "N/A";

            }
        }
        public JsonResult GetInterviews(DateTime? from, DateTime? to)
        {
            DateTime dtF, dtT;
            dtF = DateTime.Now.Date;
            dtT = DateTime.Now.Date;

            if (from != null || to != null)
            {
                dtF = (DateTime)from;
                dtT = (DateTime)to;
            }

            UserObject user = Session["userInfo"] as UserObject;
            int userId = user.UserId;
            if (user.RocketName.Contains("Recruiter"))
            {

                string mQuery = string.Format(@"
select 
MemberUser.FullName as RecruiterName, TeamLeadUser.FullName as TeamLeadName, ManagerUser.FullName as ManagerName,
Count(SubmissionDetails.SubmissionId) as TotalSubmission
from CandidateMaster 
inner join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
inner join CandidateAssign on CandidateAssign.RefMarketingId = CandidateMarketingDetails.MarketingId
inner join TeamDetails on TeamDetails.TeamId = CandidateAssign.RefTeamId
inner join UserAccountDetails as MemberUser on MemberUser.UserId = TeamDetails.Member
inner join UserAccountDetails as TeamLeadUser on TeamLeadUser.UserId = TeamDetails.TeamLead
inner join UserAccountDetails as ManagerUser on ManagerUser.UserId = TeamDetails.TeamManager
inner join SubmissionDetails on RefAssignedId = AssignedId
where TeamDetails.TeamManager = {0} and (SubmissionDetails.Date between '{1}' and '{2}')
group by ManagerUser.FullName, TeamLeadUser.FullName, MemberUser.FullName
order by Count(SubmissionDetails.SubmissionId) desc
", userId, dtF.ToString("yyyy-MM-dd"), dtT.ToString("yyyy-MM-dd"));
                dbConnectionModel DirectDb = new dbConnectionModel();
                DirectDb.query = mQuery;
                var data = DirectDb.GetDictionary();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (user.RocketName.Contains("Marketing Team Lead"))
            {

                string mQuery = string.Format(@"
select 
MemberUser.FullName as RecruiterName, TeamLeadUser.FullName as TeamLeadName, ManagerUser.FullName as ManagerName,
Count(SubmissionDetails.SubmissionId) as TotalSubmission
from CandidateMaster 
inner join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
inner join CandidateAssign on CandidateAssign.RefMarketingId = CandidateMarketingDetails.MarketingId
inner join TeamDetails on TeamDetails.TeamId = CandidateAssign.RefTeamId
inner join UserAccountDetails as MemberUser on MemberUser.UserId = TeamDetails.Member
inner join UserAccountDetails as TeamLeadUser on TeamLeadUser.UserId = TeamDetails.TeamLead
inner join SubmissionDetails on RefAssignedId = AssignedId
where TeamDetails.TeamManager = {0} and (SubmissionDetails.Date between '{1}' and '{2}')
group by ManagerUser.FullName, TeamLeadUser.FullName, MemberUser.FullName
order by Count(SubmissionDetails.SubmissionId) desc
", userId, dtF.ToString("yyyy-MM-dd"), dtT.ToString("yyyy-MM-dd"));
                dbConnectionModel DirectDb = new dbConnectionModel();
                DirectDb.query = mQuery;
                var data = DirectDb.GetDictionary();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (user.RocketName.Contains("Marketing Manager"))
            {

                string mQuery = string.Format(@"
                select 
                MemberUser.FullName as RecruiterName, TeamLeadUser.FullName as TeamLeadName, ManagerUser.FullName as ManagerName,
                Count(SubmissionDetails.SubmissionId) as TotalSubmission
                from CandidateMaster 
                inner join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
                inner join CandidateAssign on CandidateAssign.RefMarketingId = CandidateMarketingDetails.MarketingId
                inner join TeamDetails on TeamDetails.TeamId = CandidateAssign.RefTeamId
                inner join UserAccountDetails as MemberUser on MemberUser.UserId = TeamDetails.Member
                inner join UserAccountDetails as TeamLeadUser on TeamLeadUser.UserId = TeamDetails.TeamLead
                inner join UserAccountDetails as ManagerUser on ManagerUser.UserId = TeamDetails.TeamManager
                inner join SubmissionDetails on RefAssignedId = AssignedId
                where TeamDetails.TeamManager = {0} and (SubmissionDetails.Date between '{1}' and '{2}')
                group by ManagerUser.FullName, TeamLeadUser.FullName, MemberUser.FullName
                order by Count(SubmissionDetails.SubmissionId) desc
                ", userId, dtF.ToString("yyyy-MM-dd"), dtT.ToString("yyyy-MM-dd"));
                dbConnectionModel DirectDb = new dbConnectionModel();
                DirectDb.query = mQuery;
                var data = DirectDb.GetDictionary();
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubmissions()
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public JsonResult TimelineJsonData()
        {
            CandidateDetailBoardLogic detailBoardLogic = new CandidateDetailBoardLogic();

            return Json(detailBoardLogic.GetDetails().Select(a => new
            {
                CandidateId = a.CandidateId,
                LocationName = a.LocationName,
                CandidateName = a.CandidateName,
                OnBoardingDate = a.OnBoardingDate.ToString("MM-dd-yyyy"),
                SalesAssocaites = a.SalesAssocaites,
                ExpertCVStatus = string.Format("St.={0}<br>Ed.={1}<br>Days={3}", a.ResumeProcessStarted == null ? "N/A" : ((DateTime)a.ResumeProcessStarted).ToString("MM-dd-yyyy"), a.ResumeProcessEnded == null ? "N/A" : ((DateTime)a.ResumeProcessEnded).ToString("MM-dd-yyyy"), a.ResumeUnderstandingDate == null ? "N/A" : ((DateTime)a.ResumeUnderstandingDate).ToString("MM-dd-yyyy"), a.TotalResumeDays),
                CurrentStatus = GetShortCode(a.CurrentStatus),
                MarketingStatus = string.Format("St.={0}<br>Ed.={1}<br>Days={2}", a.MarketingStartDate == null ? "N/A" : ((DateTime)a.MarketingStartDate).ToString("MM-dd-yyyy"), a.PODate == null ? "N/A" : ((DateTime)a.PODate).ToString("MM-dd-yyyy"), a.TotalMarketingDays),
                TrainingStatus = string.Format("St.={0}<br>Ed.={1}<br>Days={2}", a.TrainingStartDate == null ? "N/A" : ((DateTime)a.TrainingStartDate).ToString("MM-dd-yyyy"), a.TrainingEndDate == null ? "N/A" : ((DateTime)a.TrainingEndDate).ToString("MM-dd-yyyy"), a.TotalTrainingDays),
                PODate = a.PODate != null ? a.PODate.ToString() : "N/A",
                TechnicalStatus = a.ResumeUnderstandingBy,
                OnboardingStatus = "https://localhost:44372/Content/images/details.png",
                AccountStatus = "https://localhost:44372/Content/images/details.png",
                DisputeStatus = "https://localhost:44372/Content/images/details.png"
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCandidateModelData(Parameter parameter)
        {
            dynamic prm = JObject.Parse(parameter.JsonData);
            int candidateId = Convert.ToInt32(Convert.ToString(prm.CandidateId));
            string Status = Convert.ToString(prm.Status);

            if (Status == "Sales")
            {
                Status = "Account";
            }
            var data = db.FollowUpMasters.Where(a => a.FollowUpStatus.Contains(Status) && a.RefCandidateId == candidateId).ToList();

            List<Dictionary<string, string>> details = new List<Dictionary<string, string>>();

            foreach (var d in data)
            {
                var a = new Dictionary<string, string>();
                a.Add("FollowUpStatus", d.FollowUpStatus);
                a.Add("FollowUpBy", d.FollowUpBy);
                a.Add("Date", d.FollowUpDate.ToString("MM/dd/yyyy"));
                a.Add("Time", d.FollowUpTime.ToString());
                a.Add("FollowUpMessage", d.FollowUpMessage);
                details.Add(a);
            }

            return Json(details, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetcountCandidateTotalByStatus()
        {
            string Query = "select Count(*) as CountNumber,CandidateStatus from CandidateMaster Group By CandidateStatus";
            dbConnectionModel sqlObj = new dbConnectionModel();

            sqlObj.adpt = new System.Data.SqlClient.SqlDataAdapter(Query, sqlObj.conn);
            sqlObj.dt = new System.Data.DataTable();
            sqlObj.adpt.Fill(sqlObj.dt);

            List<Dictionary<string, string>> details = new List<Dictionary<string, string>>();
            foreach (DataRow d in sqlObj.dt.Rows)
            {
                var newData = new Dictionary<string, string>();
                newData.Add("CountNumber", d["CountNumber"].ToString());
                newData.Add("CandidateStatus", d["CandidateStatus"].ToString());
                details.Add(newData);
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
