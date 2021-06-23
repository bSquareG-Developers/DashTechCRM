using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace DashTechCRM.Areas.MarketingTeamLead.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        List<SqlParameter> p = new List<SqlParameter>();
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();

        // GET: MarketingManager/dashboard
        public ActionResult Index()
        {
            return View();
        }

        #region Json Results
        public JsonResult GetTodayCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);

            directDb.query = string.Format(@"
            select * from CandidateMaster   
            inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
            inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId 

            where CandidateMaster.CandidateStatus  not in ('Deleted') and CandidateMaster.Date = '{0}' and LocationId = {1}", DateTime.Now.Date.ToString("yyyy-MM-dd"), account.RefLocationId);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
            directDb.query = string.Format(@"
            select * from CandidateMaster
            inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
            inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId 
            where CandidateMaster.CandidateStatus in ('In Marketing','On Hold','Placed') and LocationId = {0} and CandidateMaster.Date < '{1}'", account.RefLocationId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMarketingCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
            directDb.query = string.Format(@"
            select CandidateMaster.*, 
            TeamLeadUser.FullName as TeamLeadName, ManagerUser.FullName as ManagerName, RecruiterUser.FullName as RecruiterName,
            CandidateMarketingDetails.*
            from CandidateMaster
            inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
            inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId
            inner join CandidateMarketingDetails on RefCandidateId = CandidateId
            inner join CandidateAssign on refMarketingId = MarketingId
            inner join TeamDetails on CandidateAssign.RefTeamId = TeamId
            inner join UserAccountDetails as TeamLeadUser  on TeamLeadUser.UserId = TeamDetails.TeamLead
            inner join UserAccountDetails as ManagerUser  on ManagerUser.UserId = TeamDetails.TeamManager
            inner join UserAccountDetails as RecruiterUser  on RecruiterUser.UserId = TeamDetails.Member
            where CandidateMaster.CandidateStatus  in ('In Marketing','In Training','On Hold') and LocationId = {0}", account.RefLocationId);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPendingMarketingCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
            directDb.query = string.Format(@"
            select CandidateMaster.*, CandidateMarketingDetails.*, UserAccountDetails.*
            from CandidateMaster
            inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
            inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId
            left join CandidateMarketingDetails on RefCandidateId = CandidateId
            where CandidateMaster.CandidateStatus not in ('In Marketing','On Hold') and LocationId = {0} and CandidateMarketingDetails.RefCandidateId is null", account.RefLocationId);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public ActionResult AssignCandidate(CandidateMarketingDetail candidateMarketing, int teamid)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                candidateMarketing.InsertedBy = user.RocketName;
                candidateMarketing.EntryDate = DateTime.Now.Date;
                candidateMarketing.MarketingStatus = "In Marketing";
                db.CandidateMarketingDetails.Add(candidateMarketing);
                db.SaveChanges();

                var candidate = db.CandidateMasters.Find(candidateMarketing.RefCandidateId);
                candidate.MarketingStartDate = DateTime.Now.Date;
                db.SaveChanges();

                CandidateAssign candidateAssign = new CandidateAssign();
                candidateAssign.Date = DateTime.Now.Date;
                candidateAssign.IsActive = true;
                candidateAssign.refMarketingId = candidateMarketing.MarketingId;
                candidateAssign.RefTeamId = teamid;
                db.CandidateAssigns.Add(candidateAssign);
                db.SaveChanges();

                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = "Hi, Candidate Assigned to Recruiter";
                followUp.FollowUpStatus = "Marketing";
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidateMarketing.RefCandidateId;
                followUp.Department = "Marketing";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();
            }
            catch (Exception ex)
            {


            }
            return RedirectToAction("AssignedCandidate");
        }

        public ActionResult AssignedCandidate()
        {
            return View();
        }

        public string GetCandidateDetails()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                TeamDetail team = db.TeamDetails.FirstOrDefault(x => x.TeamLead == user.UserId);
                if (team != null)
                {
                    string query = String.Format(@"select CA.AssignedId,CMD.MarketingId,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date as EnrolledDate,CM.MarketingStartDate,CMD.MarketingEmailId,CMD.MarketingContactNumber,UAD.FullName,uadRecruiter.FullName as AssignedTo from CandidateAssign CA INNER JOIN TeamDetails TD on CA.RefTeamId = TD.TeamId INNER JOIN UserAccountDetails UAD on UAD.UserId = TD.Member INNER JOIN CandidateMarketingDetails CMD on CMD.MarketingId = CA.refMarketingId Inner join CandidateMaster CM on CM.CandidateId = CMD.RefCandidateId LEFT JOIN UserAccountDetails uadRecruiter on uadRecruiter.UserId = CA.refAssignRecruiter where Ca.RefTeamId = " + team.TeamId + " Order By CA.AssignedId desc");
                    return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string BindAssignTo()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                //string query = String.Format(@"select UAD.FullName as [TEXT], TD.TeamId as [ID] from TeamDetails TD inner join UserAccountDetails UAD on TD.Member = UAD.UserId where TD.TeamLead =" + user.UserId);
                //dt = dl.GetDataTable(query);
                CommonHelperClass ch = new CommonHelperClass();
                dt = ch.GetTeamOfUser(user.UserId);
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string SaveCandidateAssign(string parameter)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                string AssignFrom = "";
                if (user != null)
                    AssignFrom = user.UserId.ToString();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@JrTeamLeadId", Convert.ToString(prm.jrTeamLeadId)));
                p.Add(new SqlParameter("@Date", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
                p.Add(new SqlParameter("@Time", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@IsActive", true));
                p.Add(new SqlParameter("@MarketingId", Convert.ToString(prm.marketingId)));
                p.Add(new SqlParameter("@AssignFrom", AssignFrom));
                p.Add(new SqlParameter("@AssignedId", Convert.ToString(prm.AssignedId)));
                object result = dl.Execute_Scaler("CandidateAssign_Insert", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public ActionResult SubmissionInterviewDetails()
        {
            return View();
        }

        public string GetSubmissionInterviewDetails()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user == null ? "" : user.UserId.ToString();
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("SELECT TeamManager.FullName AS MarketingManager,TeamLead.FullName AS MarketingTeamLead, UADMember.FullName AS RecruiterName, CM.CandidateName,COUNT(ID.InteviewId) AS Interviews, COUNT(SD.RefAssignedId) AS Submissions,CM.VisaStatus,TM.TechTitle FROM SubmissionDetails SD LEFT JOIN InterviewDetails ID ON ID.RefSumissionId = SD.SubmissionId INNER JOIN CandidateAssign CA ON CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId INNER JOIN TechnologyMaster TM ON TM.TechId = CM.TechnologyId INNER JOIN TeamDetails TD ON TD.TeamId = CA.RefTeamId LEFT JOIN UserAccountDetails UADMember ON CA.refAssignRecruiter = UADMember.UserId INNER JOIN UserAccountDetails TeamLead ON TD.TeamLead = TeamLead.UserId INNER JOIN UserAccountDetails TeamManager ON TD.TeamManager = TeamManager.UserId WHERE TeamLead.RefRoleId = 3 and TeamLead.UserId = " + userId + " GROUP BY CM.CandidateName,CM.VisaStatus,TM.TechTitle,UADMember.FullName,TeamLead.FullName,TeamManager.FullName"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
