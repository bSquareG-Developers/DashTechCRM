﻿
using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.Recruiter.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();
        CommonHelperClass cs = new CommonHelperClass();

        // GET: Recruiter/dashboard
        public ActionResult Index()
        {
            return View();
        }
        public string BindFollowUpStatusRecruiter()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(cs.GetfollowupStatusByDepartment("Recruiter"));
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #region Followup save
        [HttpPost]
        public ActionResult SaveFollowup(string remarks, int candidateId, string status)
        {
            try
            {
                ConnectionDB dl = new ConnectionDB();
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = remarks;
                followUp.FollowUpStatus = status;
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidateId;
                followUp.Department = "Marketing";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();
                p.Add(new SqlParameter("@FollowUpMessage", "Status : " + status + " Remarks : " + remarks));
                p.Add(new SqlParameter("@FollowUpBy", user.UserId));
                p.Add(new SqlParameter("@CandidateId", candidateId));
                dl.Execute_NonQuery("FollowUpMasterLog_Insert", p.ToArray());
            }
            catch (Exception ex)
            {


            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Get Candidate List

        public string GetCandidatesJson()
        {
            UserObject user = Session["userInfo"] as UserObject;
            string query = string.Format(@"SELECT CA.AssignedId,CM.CandidateId,UADRecruiter.FullName,SrSalesRecruiter.FullName AS SeniorSalesRecruiter,JrBatchRecruiter.FullName AS JuniorSalesRecruiter,TraineeBatchRecruiter.FullName AS TraineeSalesRecruiter,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date AS EnrolledDate,CM.MarketingStartDate,CMD.MarketingId,CMD.MarketingEmailId,CMD.MarketingContactNumber, (SELECT COUNT(SubmissionId) FROM SubmissionDetails SD WHERE CA.AssignedId = SD.RefAssignedId AND SD.Date = '{0}') AS TodaySubmissionCount,(SELECT COUNT(SubmissionId) FROM SubmissionDetails SD WHERE CA.AssignedId = SD.RefAssignedId) AS TotalSubmissionCount,(SELECT COUNT(InteviewId) FROM InterviewDetails ID INNER JOIN SubmissionDetails SD ON SD.SubmissionId = ID.RefSumissionId AND CA.AssignedId = SD.RefAssignedId) AS TotalInterviewCount,(SELECT COUNT(InteviewId) FROM InterviewDetails ID INNER JOIN SubmissionDetails SD ON SD.SubmissionId = ID.RefSumissionId AND CA.AssignedId = SD.RefAssignedId AND SD.Date = '{0}')AS TodayInterviewCount,TM.TechTitle as [Technology],PO.PODate FROM CandidateAssign CA INNER JOIN UserAccountDetails UADRecruiter ON CA.refAssignRecruiter = UADRecruiter.UserId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId LEFT JOIN UserAccountDetails SrSalesRecruiter ON SrSalesRecruiter.UserId = CA.SrBatchRecruiter LEFT JOIN UserAccountDetails JrBatchRecruiter ON JrBatchRecruiter.UserId = CA.JrBatchRecruiter LEFT JOIN UserAccountDetails TraineeBatchRecruiter ON TraineeBatchRecruiter.UserId = CA.TraineeBatchRecruiter Inner join technologyMaster TM on TM.TechId = CM.TechnologyId LEFT join PODetails PO on PO.Candidateid = CM.CandidateId WHERE CA.refAssignRecruiter = {1} ORDER BY CA.AssignedId DESC", DateTime.Now.Date.ToString("yyyy-MM-dd"), user.UserId);
            return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));
            //var data = directDb.GetDictionary();
            //return Json(data, JsonRequestBehavior.AllowGet);
        }
        public string GetNewCandidatesJson()
        {
            UserObject user = Session["userInfo"] as UserObject;
            string query = string.Format(@"select CA.AssignedId,CM.CandidateId,UADRecruiter.FullName,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date as EnrolledDate,CM.MarketingStartDate,CMD.MarketingId,CMD.MarketingEmailId,CMD.MarketingContactNumber from CandidateAssign CA Inner join UserAccountDetails UADRecruiter on CA.refAssignRecruiter = UADRecruiter.UserId INNER JOIN CandidateMarketingDetails CMD on CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM on CM.CandidateId = CMD.RefCandidateId where CA.refAssignRecruiter={0} and CM.MarketingStartDate = '{1}' order by CA.AssignedId desc", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));
        }
        #endregion

        public string InsertFollowUpDetails(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                ConnectionDB dl = new ConnectionDB();
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string rocketName = "";
                if (user != null)
                {
                    rocketName = user.RocketName;
                }
                p.Add(new SqlParameter("@FollowUpStatus", "marketing closed"));
                p.Add(new SqlParameter("@FollowUpDate", Convert.ToString(DateTime.Now.Date)));
                p.Add(new SqlParameter("@FollowUpTime", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@FollowUpBy", Convert.ToString(rocketName)));
                p.Add(new SqlParameter("@Department", "Marketing"));
                p.Add(new SqlParameter("@FollowUpMessage", Convert.ToString(prm.marketingEndReason)));
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                object result = dl.Execute_NonQuery("FollowUpMasterInsert", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string InsertCandidateMarketingDetails(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                ConnectionDB dl = new ConnectionDB();
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string rocketName = "";
                if (user != null)
                {
                    rocketName = user.RocketName;
                }
                p.Add(new SqlParameter("@MarketingId", Convert.ToString(prm.MarketingId)));
                p.Add(new SqlParameter("@InsertedBy", Convert.ToString(rocketName)));
                p.Add(new SqlParameter("@EntryDate", Convert.ToString(DateTime.Now.Date)));
                p.Add(new SqlParameter("@FollowUpTime", Convert.ToString(DateTime.Now.TimeOfDay)));

                p.Add(new SqlParameter("@PassportNumber", Convert.ToString(prm.passportNumber)));
                p.Add(new SqlParameter("@SSN", Convert.ToString(prm.ssn)));
                p.Add(new SqlParameter("@EAD", Convert.ToString(prm.ead)));
                p.Add(new SqlParameter("@VISA", Convert.ToString(prm.visa)));
                p.Add(new SqlParameter("@i20", Convert.ToString(prm.i20)));
                p.Add(new SqlParameter("@workAuthorization", Convert.ToString(prm.workAuthorization)));
                p.Add(new SqlParameter("@DrivingLicence", Convert.ToString(prm.drivingLicence)));

                p.Add(new SqlParameter("@MarketingStatus", "In Marketing"));
                p.Add(new SqlParameter("@FollowUpStatus", "Marketing"));
                p.Add(new SqlParameter("@FollowUpMessage", "Hi, Candidate Assigned to Recruiter"));
                p.Add(new SqlParameter("@Department", "Marketing"));

                object result = dl.Execute_Scaler("CandidateMarketingDetails_AddByRecruiter", p.ToArray());
                return result.ToString();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string AddSubmissionDetailsDefault(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            p.Add(new SqlParameter("@AssignedId", Convert.ToString(prm.assignedId)));
            return dl.Execute_NonQuery("AddSubmissionDetailsDefault", p.ToArray()).ToString();
        }
        public string AddInterviewDetailsDefault(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            p.Add(new SqlParameter("@AssignedId", Convert.ToString(prm.assignedId)));
            return dl.Execute_NonQuery("AddInterviewDetailsDefault", p.ToArray()).ToString();
        }

        public string GetInterviewSubmissionCount()
        {
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            UserObject user = Session["userInfo"] as UserObject;
            int UserId = user != null ? user.UserId : 0;
            p.Add(new SqlParameter("@RecruiterId", UserId));
            p.Add(new SqlParameter("@Date", Convert.ToString(DateTime.Now.Date)));
            dt = dl.GetDataTable("GetInterviewSubmissionCount", p.ToArray());
            return CommonHelperClass._serializeDatatable(dt);
        }
        public string BindAssignTo()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
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
        public string SaveCandidateAssignToSeniorRecruiter(string parameter)
        {
            try
            {
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string AssignFrom = "";
                if (user != null)
                    AssignFrom = user.UserId.ToString();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@SrBatchRecruiter", Convert.ToString(prm.srBatchRecruiter)));
                p.Add(new SqlParameter("@Date", Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"))));
                p.Add(new SqlParameter("@Time", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@IsActive", true));
                p.Add(new SqlParameter("@MarketingId", Convert.ToString(prm.marketingId)));
                p.Add(new SqlParameter("@AssignFrom", AssignFrom));
                p.Add(new SqlParameter("@AssignedId", Convert.ToString(prm.AssignedId)));
                object result = dl.Execute_Scaler("CandidateAssign_SrBatchRecruiter", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetPoDetailsByCandidateId(string parameter)
        {
            try
            {
                List<SqlParameter> p = new List<SqlParameter>();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.candidateId)));
                dt = dl.GetDataTable("GetRemainingPODetailsByCandidateId", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Recruiter/dashboard/GetPoDetailsByCandidateId");
                throw;
            }
        }

        public string PODetailsSave(string parameter)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                List<SqlParameter> p = new List<SqlParameter>();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@TVPPOInstallment", (DataTable)JsonConvert.DeserializeObject(Convert.ToString(prm._amountInstallmentsArr), (typeof(DataTable)))));
                p.Add(new SqlParameter("@PayableAmount", Convert.ToString(prm.payableToDash)));
                p.Add(new SqlParameter("@CandidateName", Convert.ToString(prm.candidateName)));
                p.Add(new SqlParameter("@RefOfCandidate", Convert.ToString(prm.refOfCandidate)));
                p.Add(new SqlParameter("@RefSaleId", Convert.ToString(prm.refSaleId)));
                p.Add(new SqlParameter("@JobType", Convert.ToString(prm.jobType)));
                p.Add(new SqlParameter("@AppliedPosition", Convert.ToString(prm.appliedPosition)));
                p.Add(new SqlParameter("@InterviewSupport", Convert.ToString(prm.interviewSupport)));
                p.Add(new SqlParameter("@JobLocation", Convert.ToString(prm.jobLocation)));
                p.Add(new SqlParameter("@EndClient", Convert.ToString(prm.endClient)));
                p.Add(new SqlParameter("@VendorDetails", Convert.ToString(prm.vendorDetails)));
                p.Add(new SqlParameter("@RecruiterName", Convert.ToString(prm.recruiterName)));
                p.Add(new SqlParameter("@TeamLeadName", Convert.ToString(prm.teamLeadName)));
                p.Add(new SqlParameter("@Rate", Convert.ToString(prm.rate)));
                p.Add(new SqlParameter("@SignUpDate", Convert.ToString(prm.signUpDate)));
                p.Add(new SqlParameter("@TraineeName", Convert.ToString(prm.traineeName)));
                p.Add(new SqlParameter("@TrainingStartDate", Convert.ToString(prm.trainingStartDate)));
                p.Add(new SqlParameter("@TrainingEndDate", Convert.ToString(prm.trainingEndDate)));
                p.Add(new SqlParameter("@MarketingStartDate", Convert.ToString(prm.marketingStartDate)));
                p.Add(new SqlParameter("@MarketingEndDate", Convert.ToString(prm.marketingEndDate)));
                p.Add(new SqlParameter("@AgreementPercentage", Convert.ToString(prm.agreementPercentage)));
                p.Add(new SqlParameter("@AgreementMonth", Convert.ToString(prm.agreementMonth)));
                p.Add(new SqlParameter("@JobDescription", Convert.ToString(prm.jobDescription)));
                p.Add(new SqlParameter("@OtherDetails", Convert.ToString(prm.otherDetails)));
                p.Add(new SqlParameter("@PODate", Convert.ToString(prm.poDate)));
                p.Add(new SqlParameter("@JoiningDate", Convert.ToString(prm.joiningDate)));
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.candidateId)));
                p.Add(new SqlParameter("@AddedBy", Convert.ToString(user.RocketName)));
                p.Add(new SqlParameter("@EntryDate", Convert.ToString(DateTime.Now.Date)));
                p.Add(new SqlParameter("@IsDeleted", Convert.ToString(false)));
                p.Add(new SqlParameter("@ConfirmPO", "Pending"));
                var result = dl.Execute_NonQuery("PODetailsSave", p.ToArray()).ToString();
                return result;
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "Recruiter/dashboard/PODetailsSave");
                throw;
            }
        }
    }
}
