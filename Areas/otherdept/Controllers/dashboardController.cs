using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DashTechCRM.Models.Reports;
using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System.Data;

namespace DashTechCRM.Areas.otherdept.Controllers
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
        #region Json Result
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
                ExpertCVStatus = string.Format("St.={0} to Ed.={1}<br>Days={3}", a.ResumeProcessStarted == null ? "N/A" : a.ResumeProcessStarted.ToString(), a.ResumeProcessEnded == null ? "N/A" : a.ResumeProcessEnded.ToString(), a.ResumeUnderstandingDate == null ? "N/A" : a.ResumeUnderstandingDate.ToString(), a.TotalResumeDays),
                CurrentStatus = a.CurrentStatus,
                MarketingStatus = string.Format("St.={0} to Ed.={1}<br>Days={2}", a.MarketingStartDate == null ? "N/A" : a.MarketingStartDate.ToString(), a.PODate == null ? "N/A" : a.PODate.ToString(), a.TotalMarketingDays),
                TrainingStatus = string.Format("St.={0} to Ed.={1}<br>Days={2}", a.TrainingStartDate == null ? "N/A" : a.TrainingStartDate.ToString(), a.TrainingEndDate == null ? "N/A" : a.TrainingEndDate.ToString(), a.TotalTrainingDays),
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
                if (d.FollowUpStatus.Contains(":"))
                {
                    a.Add("Department", d.FollowUpStatus.Split(':')[0]);
                }
                else
                {
                    a.Add("Department", d.Department);
                }
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
                if (model.FollowUpStatus.Contains(":"))
                    model.Department = status.Split(':')[0];
                else
                    model.Department = status;
                #region Comments

                //if (status.Contains("Sale"))
                //{
                //    model.Department = "Sales";
                //}
                //else if (status.Contains("Resume"))
                //{
                //    model.Department = "Expert CV";
                //}
                //else if (status.Contains("Account"))
                //{
                //    model.Department = "Account";
                //}
                //else if (status.Contains("Dispute"))
                //{
                //    model.Department = "Account";
                //}
                //else if (status.Contains("Marketing"))
                //{
                //    model.Department = "Marketing";
                //}
                //else if (status.Contains("Onboarding"))
                //{
                //    model.Department = "Account";
                //}
                //else
                //{
                //    model.Department = "Admin";
                //}
                #endregion

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

    }
}