using DashTechCRM.Models;
using DashTechCRM.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.Recruiter.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class SubmissionsManageController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        ConnectionDB dl = new ConnectionDB();
        // GET: Recruiter/SubmissionsManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Interviews()
        {
            return View();
        }


        #region Json Result
        //public JsonResult GetTodaySubmissionList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"select SD.SubmissionId,CA.AssignedId,CM.CandidateName,SD.VendorName,SD.VendorContactNumber,SD.VendorEmailId,SD.Date as SubmissionDate,SD.FollowUpDate,SD.ClientName,SD.JobTitle from SubmissionDetails SD INNER JOIN CandidateAssign CA on CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD on CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM on CM.CandidateId = CMD.RefCandidateId INNER join TeamDetails TD on TD.TeamId = CA.RefTeamId INNER join UserAccountDetails as RecruiterUser on RecruiterUser.UserId = TD.Member where CA.refAssignRecruiter = {0} and SD.Date = '{1}' order by SD.SubmissionId desc", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetTodaySubmissionList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            p.Add(new SqlParameter("@Date", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetTodaySubmissionList", p.ToArray()));
        }
        //public JsonResult GetSubmissionList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"select SD.SubmissionId,CA.AssignedId,CM.CandidateName,SD.VendorName,SD.VendorContactNumber,SD.VendorEmailId,SD.Date as SubmissionDate,SD.FollowUpDate,SD.ClientName,SD.JobTitle from SubmissionDetails SD INNER JOIN CandidateAssign CA on CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD on CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM on CM.CandidateId = CMD.RefCandidateId INNER join TeamDetails TD on TD.TeamId = CA.RefTeamId INNER join UserAccountDetails as RecruiterUser on RecruiterUser.UserId = TD.Member where CA.refAssignRecruiter = {0} and SD.Date < '{1}' order by SD.SubmissionId desc", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetSubmissionList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            p.Add(new SqlParameter("@Date", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetSubmissionList", p.ToArray()));
        }
        public string GetTodayInterviewList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            p.Add(new SqlParameter("@Date", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetTodayInterviewList", p.ToArray()));
        }


        //public JsonResult GetTodayInterviewList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"select IMD.ModeOfInterviewTitle,ID.RoundOfInterv,ID.InterviewSupportName,SD.vendorname,SD.vendorcontactnumber,CM.CandidateName,SD.Date,ID.DateOfInterview  from InterviewDetails ID INNER JOIN InterviewMode IMD on IMD.ModeId = ID.RefModeOfInterview INNER JOIN SubmissionDetails SD on SD.SubmissionId= ID.RefSumissionId INNER JOIN CandidateAssign CA ON CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId WHERE CA.refAssignRecruiter = {0} and ID.DateOfInterview = '{1}' order by ID.InteviewId desc", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetInterviewList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetInterviewList", p.ToArray()));
        }
        //public JsonResult GetInterviewList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"select IMD.ModeOfInterviewTitle,ID.RoundOfInterv,ID.InterviewSupportName,SD.vendorname,SD.vendorcontactnumber,CM.CandidateName,SD.Date,ID.DateOfInterview  from InterviewDetails ID INNER JOIN InterviewMode IMD on IMD.ModeId = ID.RefModeOfInterview INNER JOIN SubmissionDetails SD on SD.SubmissionId= ID.RefSumissionId INNER JOIN CandidateAssign CA ON CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId WHERE CA.refAssignRecruiter = {0} order by ID.InteviewId desc", user.UserId);
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Submission Create
        [ValidateInput(false)]

        public ActionResult NewSubmission(SubmissionDetail model)
        {
            try
            {
                model.Date = DateTime.Now.Date;
                db.SubmissionDetails.Add(model);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "New Submission Submitted!" };
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Interview Create
        [ValidateInput(false)]

        public ActionResult NewInterview(InterviewDetail model, int aid)
        {
            try
            {
                //model.DateOfInterview = DateTime.Now.Date;
                if (model.RefSumissionId == null)
                {
                    SubmissionDetail submission = new SubmissionDetail();
                    submission.ClientName = "";
                    submission.Date = DateTime.Now.Date;
                    submission.FollowUpDate = DateTime.Now.Date;
                    submission.JobLocation = "";
                    submission.JobPortal = "";
                    submission.JobTitle = "";
                    submission.PerHourRate = 0;
                    submission.RefAssignedId = aid;
                    submission.Remarks = "";
                    submission.StatusOfSubmission = "Interview  Scheduled";
                    submission.VendorCompanyName = "";
                    submission.VendorContactNumber = "";
                    submission.VendorEmailId = "";
                    submission.VendorName = "";
                    db.SubmissionDetails.Add(submission);
                    db.SaveChanges();
                    model.RefSumissionId = submission.SubmissionId;
                }
                model.Feedback = "";
                model.Status = "Scheduled";
                model.InterviewSupportFeedback = "";
                model.PreviousInterview = null;
                db.InterviewDetails.Add(model);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "New Interview Scheduled!" };
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
            }
            return RedirectToAction("Index");
        }
        #endregion

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
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@UserId", userId));
                DataTable dt = dl.GetDataTable("GetInterviewSubmissionList", p.ToArray());
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
