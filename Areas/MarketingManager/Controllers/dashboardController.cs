using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace DashTechCRM.Areas.MarketingManager.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
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

            directDb.query = string.Format(@"select CM.Date as EnrolledDate,CM.CandidateName,CM.CandidateStatus,CM.MarketingStartDate,CM.VisaStatus,CMD.MarketingContactNumber,CMD.MarketingEmailId from CandidateMaster CM inner join CandidateMarketingDetails CMD on CMD.refCandidateId = CM.CandidateId inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster on LocationMaster.LocationId = UAD.RefLocationId where CM.CandidateStatus  not in ('Deleted') and (CM.CandidateStatus like '%Expert%') and CM.Date = '{0}' and UAD.RefLocationId = {1} order by CM.CandidateId desc", DateTime.Now.Date.ToString("yyyy-MM-dd"), account.RefLocationId);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetCandidateList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
        //    directDb.query = string.Format(@"
        //    select CM.CandidateName,CM.MobileNumber , CM.VisaStatus,CM.Date,CM.MarketingStartDate,UAD.EmailId ,CM.CandidateStatus,UAD.EmailId as MarketingEmailId,
        //    UAD.FullName as MarketingName
        //    from CandidateMaster CM
        //    inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate
        //    inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId 
        //    where  (CM.CandidateStatus in ('In Marketing','On Hold','Placed') OR (CM.CandidateStatus like '%Expert%')) and LocationId = {0} and CM.Date < '{1}'", account.RefLocationId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            string query = string.Format(@"select CMD.MarketingId,CM.CandidateName,CM.MobileNumber , CM.VisaStatus,CM.Date,CM.MarketingStartDate,UAD.EmailId ,CM.CandidateStatus,UAD.EmailId as MarketingEmailId,UAD.FullName as MarketingName from CandidateMaster CM inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId inner join CandidateMarketingDetails CMD on CMD.RefCandidateId = CM.CandidateId where  (CM.CandidateStatus in ('In Marketing','On Hold','Placed') OR (CM.CandidateStatus like '%Expert%')) and LocationId = {0} and CM.Date < '{1}' order by CM.CandidateId desc", account.RefLocationId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
            DataTable dt = dl.GetDataTable(query);
            return CommonHelperClass._serializeDatatable(dt);
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
            where CandidateMaster.CandidateStatus  in ('In Marketing','In Training','On Hold') and LocationId = {0} order by CandidateMaster.CandidateId desc", account.RefLocationId);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetPendingMarketingCandidateList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
        //    directDb.query = string.Format(@"select CMD.MarketingId,CM.CandidateId,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date as EnrolledDate,CM.MarketingStartDate,CMD.MarketingContactNumber,CMD.MarketingEmailId from CandidateMaster CM inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId left join CandidateMarketingDetails CMD on CMD.RefCandidateId = CM.CandidateId where CM.CandidateStatus not in ('In Marketing','On Hold') and LM.LocationId = {0} and CMD.RefCandidateId is null", account.RefLocationId);
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetPendingMarketingCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            string query = string.Format(@"select CMD.MarketingId,CM.CandidateId,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date as EnrolledDate,CM.MarketingStartDate,CMD.MarketingContactNumber,CMD.MarketingEmailId,CMD.MktEmailContactStatusFlag from CandidateMaster CM inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId left join CandidateMarketingDetails CMD on CMD.RefCandidateId = CM.CandidateId where CM.CandidateStatus not in ('In Marketing','On Hold') and LM.LocationId = {0} order by CM.CandidateId desc", account.RefLocationId);
            DataTable dt = dl.GetDataTable(query);
            return CommonHelperClass._serializeDatatable(dt);
        }

        #endregion
        //[HttpPost]
        //public ActionResult AssignCandidate(CandidateMarketingDetail candidateMarketing, int teamid)
        //{
        //    try
        //    {
        //        UserObject user = Session["userInfo"] as UserObject;
        //        ConnectionDB dl = new ConnectionDB();
        //        List<SqlParameter> p = new List<SqlParameter>();
        //        candidateMarketing.InsertedBy = user.RocketName;
        //        candidateMarketing.EntryDate = DateTime.Now.Date;
        //        candidateMarketing.MarketingStatus = "In Marketing";
        //        db.CandidateMarketingDetails.Add(candidateMarketing);
        //        db.SaveChanges();

        //        var candidate = db.CandidateMasters.Find(candidateMarketing.RefCandidateId);
        //        candidate.MarketingStartDate = DateTime.Now.Date;
        //        db.SaveChanges();

        //        CandidateAssign candidateAssign = new CandidateAssign();
        //        candidateAssign.Date = DateTime.Now.Date;
        //        candidateAssign.IsActive = true;
        //        candidateAssign.refMarketingId = candidateMarketing.MarketingId;
        //        candidateAssign.RefTeamId = teamid;
        //        db.CandidateAssigns.Add(candidateAssign);
        //        db.SaveChanges();

        //        FollowUpMaster followUp = new FollowUpMaster();
        //        followUp.FollowUpBy = user.RocketName;
        //        followUp.FollowUpDate = DateTime.Now.Date;
        //        followUp.FollowUpMessage = "Hi, Candidate Assigned to Recruiter";
        //        followUp.FollowUpStatus = "Marketing";
        //        followUp.FollowUpTime = DateTime.Now.TimeOfDay;
        //        followUp.RefCandidateId = candidateMarketing.RefCandidateId;
        //        followUp.Department = "Marketing";
        //        db.FollowUpMasters.Add(followUp);
        //        db.SaveChanges();

        //        p.Add(new SqlParameter("@FollowUpMessage", "Status : Hi, Candidate Assigned to Recruiter" + " Remarks : Marketing "));
        //        p.Add(new SqlParameter("@FollowUpBy", user.UserId));
        //        p.Add(new SqlParameter("@CandidateId", candidateMarketing.RefCandidateId));
        //        dl.Execute_NonQuery("FollowUpMasterLog_Insert", p.ToArray());
        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //    return RedirectToAction("AssignedCandidate");
        //}

        public string AssignCandidateAdd(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            UserObject user = Session["userInfo"] as UserObject;
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@MarketingId", Convert.ToString(prm.MarketingId)));
            p.Add(new SqlParameter("@OriginalResumeLink", Convert.ToString(prm.originalResume)));
            p.Add(new SqlParameter("@MarketingResumeLink", Convert.ToString(prm.marketingResume)));
            p.Add(new SqlParameter("@InsertedBy", Convert.ToString(user.RocketName)));
            p.Add(new SqlParameter("@OtherRemarks", Convert.ToString(prm.otherRemarks)));
            p.Add(new SqlParameter("@MarketingStatus", "In Marketing"));
            p.Add(new SqlParameter("@LocationCorncern", Convert.ToString(prm.locationConcern)));
            p.Add(new SqlParameter("@RequiredLocationList", ""));
            p.Add(new SqlParameter("@EntryDate", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
            p.Add(new SqlParameter("@TeamLeadId", Convert.ToString(prm.teamLeadId)));
            p.Add(new SqlParameter("@TeamManagerId", Convert.ToString(user.UserId)));
            p.Add(new SqlParameter("@IsActive", "true"));
            p.Add(new SqlParameter("@FollowUpStatus", "Marketing"));
            p.Add(new SqlParameter("@FollowUpTime", Convert.ToString(DateTime.Now)));
            p.Add(new SqlParameter("@FollowUpMessage", "Hi, Candidate Assigned to Recruiter"));
            p.Add(new SqlParameter("@Department", "Marketing"));
            object result = dl.Execute_Scaler("CandidateMarketingDetails_StartMarketing", p.ToArray());
            return result.ToString();
        }

        public string ReAssignCandidate(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            UserObject user = Session["userInfo"] as UserObject;
            string userId = user == null ? "" : Convert.ToString(user.UserId);

            p.Add(new SqlParameter("@MarketingId", Convert.ToString(prm.MarketingId)));
            p.Add(new SqlParameter("@teamLeadId", Convert.ToString(prm.teamLeadId)));
            p.Add(new SqlParameter("@userId", userId));
            return dl.Execute_NonQuery("MktTeamCandidateReassign", p.ToArray()).ToString();
        }
        //[HttpPost]
        //public ActionResult ReAssignCandidate(int MarketingId, int teamid)
        //{
        //    try
        //    {
        //        var candidateMarketing = db.CandidateMarketingDetails.FirstOrDefault(a => a.MarketingId == MarketingId);
        //        UserObject user = Session["userInfo"] as UserObject;
        //        var oldAssigned = db.CandidateAssigns.Where(a => a.refMarketingId == MarketingId).ToList();
        //        for (int i = 0; i < oldAssigned.Count; i++)
        //        {
        //            oldAssigned[i].IsActive = false;
        //        }
        //        db.SaveChanges();


        //        CandidateAssign candidateAssign = new CandidateAssign();
        //        candidateAssign.Date = DateTime.Now.Date;
        //        candidateAssign.IsActive = true;
        //        candidateAssign.refMarketingId = MarketingId;
        //        candidateAssign.RefTeamId = teamid;
        //        db.CandidateAssigns.Add(candidateAssign);
        //        db.SaveChanges();


        //        var team = db.TeamDetails.Find(teamid);

        //        FollowUpMaster followUp = new FollowUpMaster();
        //        followUp.FollowUpBy = user.RocketName;
        //        followUp.FollowUpDate = DateTime.Now.Date;
        //        followUp.FollowUpMessage = string.Format("Hi, Candidate Re-Assigned to Recruiter, {0}", team.UserAccountDetail.FullName);
        //        followUp.FollowUpStatus = "Marketing";
        //        followUp.FollowUpTime = DateTime.Now.TimeOfDay;
        //        followUp.RefCandidateId = candidateMarketing.RefCandidateId;
        //        followUp.Department = "Marketing";
        //        db.FollowUpMasters.Add(followUp);
        //        db.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //    return RedirectToAction("AssignedCandidate");
        //}
        public ActionResult AssignedCandidate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangeStatus(string status, string remarks, int candidateId)
        {
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            UserObject user = Session["userInfo"] as UserObject;

            CandidateMaster candidate = db.CandidateMasters.Find(candidateId);
            candidate.CandidateStatus = status;
            db.SaveChanges();

            CandidateMarketingDetail marketing = db.CandidateMarketingDetails.FirstOrDefault(a => a.RefCandidateId == candidateId);
            marketing.MarketingStatus = status;
            db.SaveChanges();

            FollowUpMaster follow = new FollowUpMaster();
            follow.Department = "Marketing";
            follow.FollowUpBy = user.RocketName;
            follow.FollowUpDate = DateTime.Now.Date;
            follow.FollowUpMessage = remarks;
            follow.FollowUpStatus = status;
            follow.FollowUpTime = DateTime.Now.TimeOfDay;
            follow.RefCandidateId = candidate.CandidateId;
            db.FollowUpMasters.Add(follow);
            db.SaveChanges();
            p.Add(new SqlParameter("@FollowUpMessage", "Status : " + status + " Remarks : " + remarks));
            p.Add(new SqlParameter("@FollowUpBy", user.UserId));
            p.Add(new SqlParameter("@CandidateId", candidateId));
            dl.Execute_NonQuery("FollowUpMasterLog_Insert", p.ToArray());
            return RedirectToAction("AssignedCandidate");
        }

        public string SendMailToRequest(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                UserObject user = Session["userInfo"] as UserObject;
                List<SqlParameter> p = new List<SqlParameter>();
                ConnectionDB dl = new ConnectionDB();
                bool result = false;
                string userName = user != null ? user.RocketName.ToString() : "";
                //string emailId = "kautilya.dashtechinc@gmail.com";
                string emailId = "rakesh.giri@dashtechinc.com";
                //string emailId = "kiran@dashtechinc.com";
                string candidateName = Convert.ToString(prm.CandidateName);
                string emailBody = "<p>Hello Mr.Rakesh,</p><p> Mr {UserName} have requested you to generate Email and contact number for the candidate : {CandidateName} for marketing purpose.</p><p><br></p><p>Thank You</p>";
                emailBody = emailBody.Replace("{UserName}", userName);
                emailBody = emailBody.Replace("{CandidateName}", candidateName);
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                p.Add(new SqlParameter("@InsertedBy", userName));
                p.Add(new SqlParameter("@MktEmailContactStatusFlag", 1));
                object changeRequestResult = dl.Execute_Scaler("CandidateMarketingDetails_InsertMktContactStatus", p.ToArray());
                if (changeRequestResult.ToString() == "1")
                    result = SMTPEmailSendingModel.Send(emailId, emailBody, "Request for Generate EmailId and Contact For Marketing of an Candidate", "", "");
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetTeamLeaders()
        {
            try
            {
                List<SqlParameter> p = new List<SqlParameter>();
                DataTable dt = new DataTable();
                ConnectionDB dl = new ConnectionDB();
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@Department", "Marketing"));
                p.Add(new SqlParameter("@TeamManager", userId));
                dt = dl.GetDataTable("TeamDetailsGetTeamLeadByTeamManagerAndDepartment", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
