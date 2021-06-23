using DashTechCRM.Models;
using DashTechCRM.Models.Reports;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace DashTechCRM.Areas.admin.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        // GET: admin/dashboard
        public ActionResult Index()
        {
            return View();
        }
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
        #region Json Result
        public JsonResult TimelineJsonData()
        {
            UserObject user = Session["userInfo"] as UserObject;

            UserAccountDetail u = db.UserAccountDetails.Find(user.UserId);

            CandidateDetailBoardLogic detailBoardLogic = new CandidateDetailBoardLogic();

            return Json(detailBoardLogic.GetDetails().Select(a => new
            {
                CandidateId = a.CandidateId,
                LocationName = a.LocationName,
                CandidateName = a.CandidateName,
                OnBoardingDate = a.OnBoardingDate.ToString("MM-dd-yyyy"),
                SalesAssocaites = a.SalesAssocaites,
                ExpertCVStatus = string.Format("St.={0} <br> Ed.={1}<br>Days={3}", a.ResumeProcessStarted == null ? "N/A" : a.ResumeProcessStarted.ToString(), a.ResumeProcessEnded == null ? "N/A" : ((DateTime)a.ResumeProcessEnded).ToString("MM-dd-yyyy"), a.ResumeUnderstandingDate == null ? "N/A" : ((DateTime)a.ResumeUnderstandingDate).ToString("MM-dd-yyyy"), a.TotalResumeDays),
                CurrentStatus = CandidateStatusModel.GetShortCode(a.CurrentStatus),
                MarketingStatus = string.Format("St.={0} <br> Ed.={1}<br>Days={2}", a.MarketingStartDate == null ? "N/A" : ((DateTime)a.MarketingStartDate).ToString("MM-dd-yyyy"), a.PODate == null ? "N/A" : ((DateTime)a.PODate).ToString("MM-dd-yyyy"), a.TotalMarketingDays),
                TrainingStatus = string.Format("St.={0} <br> Ed.={1}<br>Days={2}", a.TrainingStartDate == null ? "N/A" : ((DateTime)a.TrainingStartDate).ToString("MM-dd-yyyy"), a.TrainingEndDate == null ? "N/A" : ((DateTime)a.TrainingEndDate).ToString("MM-dd-yyyy"), a.TotalTrainingDays),
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
            var data = db.FollowUpMasters.Where(a => a.Department == Status && a.RefCandidateId == candidateId).ToList();
            //var data = db.FollowUpMasters.Where(followup => followup.RefCandidateId == candidateId).ToList();
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

        public ActionResult ChangeStatus(string status, int cid, string message)
        {
            try
            {

                FollowUpMaster model = new FollowUpMaster();
                UserObject user = Session["userInfo"] as UserObject;
                model.RefCandidateId = cid;
                model.FollowUpBy = user.RocketName;
                model.FollowUpDate = DateTime.Now.Date;
                model.FollowUpTime = DateTime.Now.TimeOfDay;
                model.FollowUpMessage = message;
                model.FollowUpStatus = status;
                if (status.Contains("Sale"))
                {
                    model.Department = "Sales";
                }
                else if (status.Contains("Resume"))
                {
                    model.Department = "Expert CV";
                }
                else if (status.Contains("Account"))
                {
                    model.Department = "Account";
                }
                else if (status.Contains("Dispute"))
                {
                    model.Department = "Account";
                }
                else if (status.Contains("Marketing"))
                {
                    model.Department = "Marketing";
                }
                else if (status.Contains("Onboarding"))
                {
                    model.Department = "Account";
                }
                else
                {
                    model.Department = "Admin";
                }
                db.FollowUpMasters.Add(model);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "Follow Up Message Saved!" };

            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
            }
            return RedirectToAction("Index");
        }

        public ActionResult AdminDashboardDemo()
        {
            return View();
        }
        public string GetAdminDashBoard()
        {
            dbConnectionModel directDb = new dbConnectionModel();
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            string query = "CREATE TABLE #TEMP (CandidateId int,CandidateName varchar(100),OnBoardingDate DATETIME,SalesAssocaites varchar(100),ResumeProcessStarted DATETIME,ExpertCVMakerName  varchar(100),ResumeProcessEnded DATETIME,TotalResumeDays int,CurrentStatus varchar(50),TrainingStartDate DATETIME,TrainingEndDate DATETIME,TotalTrainingDays int,TrainerName varchar(50),ResumeUnderstandingDate DATETIME,ResumeUnderstandingBy varchar(50),MarketingStartDate DATETIME,TotalMarketingDays int,RecruiterName varchar(50),PODate DATETIME,LocationName varchar(50))INSERT INTO #TEMP(CandidateId,CandidateName,CurrentStatus,SalesAssocaites,OnBoardingDate,MarketingStartDate,PODate)SELECT CM.CandidateId,CM.CandidateName,CM.CandidateStatus,  FullName as SalesAssociateName,CM.Date,CM.MarketingStartDate,POData.PODate FROM CandidateMaster CM inner join UserAccountDetails on  UserId = RefSalesAssociate LEFT JOIN PODetails as POData on POData.CandidateId = CM.CandidateId WHERE CandidateStatus<> 'Deleted' UPDATE #TEMP set ExpertCVMakerName = UAD.FullName,ResumeProcessStarted = CTL.Date from CandidateTimeLine  CTL inner join UserAccountDetails UAD on UAD.UserId = CTL.ChangedBy Inner Join #TEMP t on t.CandidateId = CTL.RefCandidateId where  CTL.Status like '%Resume Call%' UPDATE #TEMP set ResumeProcessEnded = CTL.Date from CandidateTimeLine  CTL inner join UserAccountDetails UAD on UAD.UserId = CTL.ChangedBy Inner Join #TEMP t on t.CandidateId = CTL.RefCandidateId where  CTL.Status like '%Resume Verification%' Update #TEMP set ResumeUnderstandingBy = Expert.FullName,ResumeUnderstandingDate =tmng.TaskDate from CandidateTechnicalExpertDetails CTE inner join CandidateMaster CM on CM.CandidateId = RefAssignedCandidateId inner join UserAccountDetails  as Expert on RefAssignedExpertId = Expert.UserId inner join TaskManageMaster as tmng on  tmng.RefCTId = CTE.CTId left join CandidateBatchDetails CBD on  CBD.BatchId = RefBatchId left join TaskCategoryMaster as CatM on CatM.TaskCatId = tmng.RefTaskCatId Inner Join #TEMP t on t.CandidateId = CM.CandidateId where  RefTaskCatId = 2 Update #TEMP set TrainerName = Expert.FullName, TrainingStartDate = CandidateBatchDetails.BatchStartDate,TrainingEndDate = CandidateBatchDetails.BatchEndDate from CandidateTechnicalExpertDetails CTED inner join CandidateMaster CM on CM.CandidateId = CTED.RefAssignedCandidateId inner join UserAccountDetails as Expert on CTED.RefAssignedExpertId = Expert.UserId left join CandidateBatchDetails on  CandidateBatchDetails.BatchId = CTED.RefBatchId Inner Join #TEMP t on t.CandidateId = CM.CandidateId Update #TEMP set RecruiterName = UAD.FullName,LocationName = LM.LocationName from CandidateMaster CM inner join CandidateMarketingDetails CMD on CM.CandidateId = CMD.RefCandidateId inner join CandidateAssign CAA on CAA.refMarketingId = CMD.MarketingId inner join TeamDetails TD on CAA.RefTeamId = TD.TeamId inner join UserAccountDetails UAD on UAD.UserId = TD.Member inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId Inner Join #TEMP t on t.CandidateId = CM.CandidateId where  CAA.IsActive = 1 select CandidateId ,CandidateName,ISNULL(Convert(varchar,OnBoardingDate,106),'') as [OnBoardingDate],SalesAssocaites, ResumeProcessStarted, ExpertCVMakerName, ResumeProcessEnded, TotalResumeDays, CurrentStatus, TrainingStartDate, TrainingEndDate, TotalTrainingDays, TrainerName, ResumeUnderstandingDate, ResumeUnderstandingBy, MarketingStartDate, TotalMarketingDays, RecruiterName, PODate, LocationName,('st: '+ISNULL(CONVERT(varchar,MarketingStartDate,106),'N/A')+''+' Ed: '+ ISNULL(CONVERT(varchar,PODate,106),'N/A') + ' Days : '+ ISNULL(CONVERT(varchar,TotalMarketingDays),'N/A')) as MarketingStatus  from #TEMP  drop table #TEMP";
            return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));

            //var data = directDb.GetDictionary();
            //return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ManageMktEmailCntsRequests()
        {
            return View();
        }

        public string BindManageEmailContactRequests()
        {
            try
            {
                ConnectionDB dl = new ConnectionDB();
                UserObject user = Session["userInfo"] as UserObject;
                UserAccountDetail account = db.UserAccountDetails.Find(user.UserId);
                dbConnectionModel directDb = new dbConnectionModel();

                string query = string.Format(@"SELECT CM.CandidateId,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date AS EnrolledDate,CM.MarketingStartDate,CMD.MarketingContactNumber,    CMD.MarketingEmailId,CASE WHEN  CMD.MktEmailContactStatusFlag  = 1 then 'Pending' END as StatusFlag,CMD.InsertedBy,UADEmail.EmailId,UADEmail.FullName,CMD.MarketingId FROM CandidateMaster CM JOIN UserAccountDetails UAD ON UAD.UserId = CM.RefSalesAssociate JOIN LocationMaster LM ON LM.LocationId = UAD.RefLocationId JOIN CandidateMarketingDetails CMD ON CMD.RefCandidateId = CM.CandidateId JOIN UserAccountDetails UADEmail on CMD.InsertedBy = UADEmail.RocketName WHERE CM.CandidateStatus NOT IN('In Marketing', 'On Hold') AND LM.LocationId = {0} AND CMD.MktEmailContactStatusFlag = 1", account.RefLocationId);
                DataTable dt = dl.GetDataTable(query);
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string SaveMarketingContactDetails(string parameter)
        {
            try
            {
                string username = "";
                UserObject user = Session["userInfo"] as UserObject;
                if (user != null)
                {
                    username = user.FullName != null ? user.FullName.ToString() : "";
                }
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                ConnectionDB dl = new ConnectionDB();
                string email = Convert.ToString(prm.email);
                string phoneNo = Convert.ToString(prm.phoneNo);
                string MarketingId = Convert.ToString(prm.MarketingId);
                string candidateName = Convert.ToString(prm.CandidateName);
                string receiverName = Convert.ToString(prm.FullName);
                string receiverEmail = Convert.ToString(prm.EmailId);
                string query = "UPDATE CandidateMarketingDetails SET MarketingEmailId = '" + email + "' , MarketingContactNumber = " + phoneNo + ",MktEmailContactStatusFlag = 2 WHERE MarketingId =" + MarketingId;
                string result = dl.Execute_NonQuery(query).ToString();
                if (result == "True")
                {
                    string emailBody = "<p>Hello " + receiverName + "</p><p> Mr." + username + " Have successfully updated " + candidateName + "&apos; s Marketing Email - Id and Marketing Contact Number. Now you can start next process for the candidate successfully.</p><p> Thank You </p> ";
                    SMTPEmailSendingModel.Send("kautilya.dashtechinc@gmail.com", emailBody, "Response for Generate EmailId and Contact For Marketing of an Candidate", "", "");
                }
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #region Manage Visa Status
        public ActionResult ManageVisaStatus()
        {
            return View();
        }

        public string BindVisaStatus()
        {
            ConnectionDB dl = new ConnectionDB();
            DataTable dt = new DataTable();
            dt = dl.GetDataTable("select * FROM VisaTitleMaster");
            return CommonHelperClass._serializeDatatable(dt);
        }

        public string SaveorChangeVisaStatus(string parameter)
        {
            ConnectionDB dl = new ConnectionDB();
            List<SqlParameter> p = new List<SqlParameter>();
            dynamic prm = JObject.Parse(parameter);
            p.Add(new SqlParameter("@Id", Convert.ToString(prm.VisaId)));
            p.Add(new SqlParameter("@visaTitle", Convert.ToString(prm.VisaTitle)));
            p.Add(new SqlParameter("@flag", Convert.ToString(prm.flag)));
            object result = dl.Execute_Scaler("VisaTitleMasterInsertUpdate", p.ToArray());
            return result.ToString();
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
                ConnectionDB dl = new ConnectionDB();
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user == null ? "" : user.UserId.ToString();
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("SELECT TeamManager.FullName AS MarketingManager,TeamLead.FullName AS MarketingTeamLead, UADMember.FullName AS RecruiterName, CM.CandidateName,COUNT(ID.InteviewId) AS Interviews, COUNT(SD.RefAssignedId) AS Submissions,CM.VisaStatus,TM.TechTitle FROM SubmissionDetails SD LEFT JOIN InterviewDetails ID ON ID.RefSumissionId = SD.SubmissionId INNER JOIN CandidateAssign CA ON CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId INNER JOIN TechnologyMaster TM ON TM.TechId = CM.TechnologyId INNER JOIN TeamDetails TD ON TD.TeamId = CA.RefTeamId LEFT JOIN UserAccountDetails UADMember ON CA.refAssignRecruiter = UADMember.UserId INNER JOIN UserAccountDetails TeamLead ON TD.TeamLead = TeamLead.UserId INNER JOIN UserAccountDetails TeamManager ON TD.TeamManager = TeamManager.UserId WHERE TeamLead.RefRoleId = 3  GROUP BY CM.CandidateName,CM.VisaStatus,TM.TechTitle,UADMember.FullName,TeamLead.FullName,TeamManager.FullName"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult GetBulkCandidateRecord()
        {
            return View();
        }

        public string GetCandidateUploadTemp()
        {
            DataTable dt = new DataTable();
            ConnectionDB dl = new ConnectionDB();
            dt = dl.GetDataTable("select * from CandidateUploadTemp");
            return CommonHelperClass._serializeDatatable(dt);
        }
    }
}
