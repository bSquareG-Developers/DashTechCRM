using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.ExpertCVCoach.Controllers
{
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db
             = new dashTech_crm_Entities();
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();
        CommonHelperClass cs = new CommonHelperClass();
        // GET: ExpertCVCoach/dashboard
        public ActionResult Index()
        {
            return View();
        }

        #region Json Result
        //        public JsonResult GetCandidateList()
        //        {
        //            UserObject user = Session["userInfo"] as UserObject;
        //            dbConnectionModel directDb = new dbConnectionModel();
        //            directDb.query = string.Format(@"select CM.CandidateId, CM.CandidateName, CM.CandidateStatus,cm.MobileNumber,cm.EmailId,cm.AgreementSent,cm.Agreement,CM.Date as EnrolledDate,CM.TotalAmount , cm.PaidAmount, SSM.ServiceName from CandidateMaster CM inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster LM on LM.LocationId = UAD.RefLocationId inner join SalesServiceMaster SSM on SSM.ServiceId = CM.RefServiceId where (CM.CandidateStatus = 'Sales' OR CM.CandidateStatus like '%Expert%') and CM.Date < '{0}'
        //", DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //            var data = directDb.GetDictionary();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }

        public string GetCandidateList()
        {
            string query = string.Format
                (@"SELECT CM.CandidateId,CM.CandidateName,CM.CandidateStatus,cm.MobileNumber,cm.EmailId,cm.AgreementSent,cm.Agreement,CM.Date AS EnrolledDate,CM.TotalAmount,cm.PaidAmount,SSM.ServiceName,TM.TechTitle,CVF.ProcessStart,CVF.ResumePreparation,CVF.ResumeVerification,CVF.ResumeModification,CVF.RUC,CVF.ProcessEnd FROM CandidateMaster CM INNER JOIN UserAccountDetails UAD ON UAD.UserId = CM.RefSalesAssociate INNER JOIN LocationMaster LM ON LM.LocationId = UAD.RefLocationId INNER JOIN SalesServiceMaster SSM ON SSM.ServiceId = CM.RefServiceId INNER JOIN TechnologyMaster TM ON TM.TechId = CM.TechnologyId LEFT JOIN CandidateExpertCVFollowupDetails CVF on CVF.CandidateId = CM.CandidateId where (CM.CandidateStatus = 'Sales' OR CM.CandidateStatus like '%Expert%') and CM.Date < '{0}' order by CM.CandidateId desc", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));
        }

        public string GetNewCandidateList()
        {
            string query = string.Format(@"SELECT CM.CandidateId,CM.CandidateName,CM.CandidateStatus,cm.MobileNumber,cm.EmailId,cm.AgreementSent,cm.Agreement,CM.Date AS EnrolledDate,CM.TotalAmount,cm.PaidAmount,SSM.ServiceName,TM.TechTitle,CVF.ProcessStart,CVF.ResumePreparation,CVF.ResumeVerification,CVF.ResumeModification,CVF.RUC,CVF.ProcessEnd FROM CandidateMaster CM INNER JOIN UserAccountDetails UAD ON UAD.UserId = CM.RefSalesAssociate INNER JOIN LocationMaster LM ON LM.LocationId = UAD.RefLocationId INNER JOIN SalesServiceMaster SSM ON SSM.ServiceId = CM.RefServiceId INNER JOIN TechnologyMaster TM ON TM.TechId = CM.TechnologyId INNER JOIN CandidateExpertCVFollowupDetails CVF on CVF.CandidateId = CM.CandidateId where CM.Date = '{0}' order by CM.CandidateId desc", DateTime.Now.Date.ToString("yyyy-MM-dd"));

            return CommonHelperClass._serializeDatatable(dl.GetDataTable(query));
        }

        //public JsonResult GetNewCandidateList()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"SELECT CM.CandidateId,CM.CandidateName,CM.EmailId,CM.CandidateStatus,CM.MobileNumber,CM.AgreementSent,CM.Agreement,CM.Date as EnrolledDate,SM.ServiceName,CM.TotalAmount,CM.PaidAmount from CandidateMaster CM inner join UserAccountDetails UAD on UAD.UserId = CM.RefSalesAssociate inner join LocationMaster on LocationMaster.LocationId = UAD.RefLocationId inner join SalesServiceMaster SM on SM.ServiceId = CM.RefServiceId where CM.Date = '{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Followup save
        [HttpPost]
        public ActionResult SaveFollowup(string remarks, int candidateId, string status)
        {
            try
            {

                List<SqlParameter> p = new List<SqlParameter>();

                UserObject user = Session["userInfo"] as UserObject;
                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = status + ", " + remarks;
                followUp.FollowUpStatus = "ExpertCv";
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidateId;
                followUp.Department = "ExpertCv";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();

                var c = db.CandidateMasters.Find(candidateId);
                c.CandidateStatus = status;
                //db.CandidateMasters.Add(c);
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

        public string GetexpertcvCandidateDetailsByCandidatId(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string CandidateId = Convert.ToString(prm.CandidatId);
            dt = dl.GetDataTable(
                "select CONVERT(varchar,FM.FollowUpDate,106)  as FollowUpDate,Convert(varchar(8),FM.FollowUpTime) as FollowUpTime,FM.FollowUpMessage,FM.FollowUpStatus, FM.Department, CM.CandidateName, CM.MobileNumber, CM.EmailId from FollowUpMaster FM inner join CandidateMaster CM on CM.CandidateId = FM.RefCandidateId where  FM.Department = 'ExpertCv' and FM.RefCandidateId = " + CandidateId);
            return CommonHelperClass._serializeDatatable(dt);
        }
        public string InsertFollowUpDetails(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string rocketName = "";
                if (user != null)
                {
                    rocketName = user.RocketName;
                }
                p.Add(new SqlParameter("@FollowUpStatus", Convert.ToString(prm.FollowUpStatus)));
                p.Add(new SqlParameter("@FollowUpDate", Convert.ToString(DateTime.Now.Date)));
                p.Add(new SqlParameter("@FollowUpTime", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@FollowUpBy", Convert.ToString(rocketName)));
                p.Add(new SqlParameter("@Department", "ExpertCv"));
                p.Add(new SqlParameter("@FollowUpMessage", Convert.ToString(prm.followUpMessage)));
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                p.Add(new SqlParameter("@processStatus", Convert.ToString(prm.processStatus)));
                object result = dl.Execute_NonQuery("FollowUpMasterInsert", p.ToArray());
                var c = db.CandidateMasters.Find(Convert.ToInt32(prm.CandidateId));
                c.CandidateStatus = "ExpertCv: " + Convert.ToString(prm.FollowUpStatus);
                //db.CandidateMasters.Add(c);
                db.SaveChanges();

                p = new List<SqlParameter>();
                p.Add(new SqlParameter("@FollowUpMessage", "Status : " + Convert.ToString(prm.FollowUpStatus) + " Remarks : " + Convert.ToString(prm.followUpMessage)));
                p.Add(new SqlParameter("@FollowUpBy", user.UserId));
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                dl.Execute_NonQuery("FollowUpMasterLog_Insert", p.ToArray());

                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string BindFollowUpStatusExpertCv()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(cs.GetfollowupStatusByDepartment("ExpertCv"));
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public string GetTechnology()
        {

            try
            {
                dt = dl.GetDataTable("select TechTitle from TechnologyMaster");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public string changeTechnology(string parameter)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                string UserId = user != null ? user.UserId.ToString() : "";

                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@technologyName", Convert.ToString(prm.technologyName)));
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                p.Add(new SqlParameter("@UserId", UserId));
                object result = dl.Execute_NonQuery("changeTechnologyCandidate", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion
    }
}
