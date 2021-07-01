using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.SalesAssociate.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();
        CommonHelperClass cs = new CommonHelperClass();
        // GET: SalesAssociate/dashboard
        public ActionResult Index()
        {
            return View();
        }
        public string BindFollowUpStatusSales()
        {
            try
            {
                return CommonHelperClass._serializeDatatable(cs.GetfollowupStatusByDepartment("Sales"));
            }
            catch (Exception e)
            {
                return "";
            }
        }
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

        #region JSON Results

        #region Get Associates Dashboard Current

        //public JsonResult GetNewCandidateTimeLine()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"SELECT  CM.CandidateId,CM.CandidateName, CM.CandidateStatus,CM.EmailId,UAD.FullName,PO.AgreementPercentage,CM.AgreementSent,CM.Date AS EnrolledDate, CM.TotalAmount, CM.PaidAmount,SSM.ServiceName,PO.PODate FROM CandidateMaster CM INNER JOIN UserAccountDetails UAD  ON CM.RefSalesAssociate = UAD.UserId INNER JOIN SalesServiceMaster SSM ON SSM.ServiceId = CM.RefServiceId LEFT JOIN CandidateMarketingDetails CMD ON CMD.RefCandidateId = CM.CandidateId LEFT JOIN CandidateAssign CA ON CA.refMarketingId = CMD.MarketingId LEFT JOIN TeamDetails ON TeamId = RefTeamId LEFT JOIN PODetails PO ON CM.CandidateId = PO.CandidateId where UserId = {0} and CM.Date between '{1}' and '{2}' and CM.CandidateId = ( CASE When CA.AssignedId is null then CM.CandidateId when CA.AssignedId is not null and CA.IsActive = 1 then   CM.CandidateId else -1 END)", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCandidateTimeLine()
        //{
        //    UserObject user = Session["userInfo"] as UserObject;
        //    dbConnectionModel directDb = new dbConnectionModel();
        //    directDb.query = string.Format(@"SELECT  CM.CandidateId,CM.CandidateName, CM.CandidateStatus, CM.EmailId,UAD.FullName,PO.AgreementPercentage,CM.AgreementSent,CM.Date AS EnrolledDate,CM.TotalAmount, CM.PaidAmount, SSM.ServiceName, PO.PODate FROM CandidateMaster CM INNER JOIN UserAccountDetails UAD  ON CM.RefSalesAssociate = UAD.UserId INNER JOIN SalesServiceMaster SSM ON SSM.ServiceId = CM.RefServiceId LEFT JOIN CandidateMarketingDetails CMD ON CMD.RefCandidateId = CM.CandidateId LEFT JOIN CandidateAssign CA ON CA.refMarketingId = CMD.MarketingId LEFT JOIN TeamDetails ON TeamId = RefTeamId LEFT JOIN PODetails PO ON CM.CandidateId = PO.CandidateId
        //        WHERE UserId = {0} AND CM.DATE < '{1}' and CM.CandidateId = (CASE WHEN CA.AssignedId IS NULL THEN CM.CandidateId WHEN CA.AssignedId IS NOT NULL AND CA.IsActive = 1 THEN CM.CandidateId ELSE -1 END)", user.UserId, DateTime.Now.Date.ToString("yyyy-MM-dd"));
        //    var data = directDb.GetDictionary();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public string GetNewCandidateTimeLine()
        {
            UserObject user = Session["userInfo"] as UserObject;
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@StartDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            p.Add(new SqlParameter("@EndDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetTodayCandidatetimeLineSalesAssociate", p.ToArray()));
        }

        public string GetCandidateTimeLine()
        {
            UserObject user = Session["userInfo"] as UserObject;
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@EndDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
            p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
            return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetCandidatetimeLineSalesAssociate", p.ToArray()));
        }

        #endregion

        #endregion
        #region Manage Recurring Details
        public string ShowRecurringDetails(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            DataSet ds = new DataSet();
            List<SqlParameter> p = new List<SqlParameter>();
            p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));

            ds = dl.GetDataSet("GetRecurringMasterTempByCandidateId", p.ToArray());
            return CommonHelperClass._serializeDataSet(ds);
        }

        public string RecurringMasterTemp_Insert(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            p.Add(new SqlParameter("@DueDate", Convert.ToString(prm.DueDate)));
            p.Add(new SqlParameter("@Amount", Convert.ToString(prm.Amount)));
            p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
            p.Add(new SqlParameter("@ReceivedIn", Convert.ToString(prm.ReceivedIn)));
            p.Add(new SqlParameter("@SendRemainderEmail", Convert.ToString(prm.SendRemainderEmail)));
            p.Add(new SqlParameter("@PaymentStatus", Convert.ToString(prm.PaymentStatus)));
            p.Add(new SqlParameter("@Remarks", Convert.ToString(prm.Remarks)));
            return dl.Execute_NonQuery("RecurringMasterTemp_Insert", p.ToArray()).ToString();
        }

        public string RecurringMasterTemp_Update(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            List<SqlParameter> p = new List<SqlParameter>();
            ConnectionDB dl = new ConnectionDB();
            string paymentStatus = Convert.ToString(prm.PaymentStatus);

            string sendReminder = paymentStatus == "Paid" ? "No" : "Yes";
            p.Add(new SqlParameter("@Id", Convert.ToString(prm.Id)));
            p.Add(new SqlParameter("@DueDate", Convert.ToString(prm.DueDate)));
            p.Add(new SqlParameter("@Amount", Convert.ToString(prm.Amount)));
            p.Add(new SqlParameter("@ReceivedIn", Convert.ToString(prm.ReceivedIn)));
            p.Add(new SqlParameter("@PaymentStatus", paymentStatus));
            p.Add(new SqlParameter("@Remarks", Convert.ToString(prm.Remarks)));
            return dl.Execute_NonQuery("RecurringMasterTemp_Update", p.ToArray()).ToString();
        }
        #endregion

    }


}
