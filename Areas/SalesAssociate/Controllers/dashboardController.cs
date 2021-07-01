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
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/BindFollowUpStatusSales");
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

                db.FollowUpMasters.Add(model);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "Follow Up Message Saved!" };

            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
                CommonHelperClass.InsertErrorLog(ex.Message, "SalesAssociate/dashboard/ChangeStatus");
            }
            return RedirectToAction("Index");
        }

        #region JSON Results

        #region Get Associates Dashboard Current



        public string GetNewCandidateTimeLine()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@StartDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
                p.Add(new SqlParameter("@EndDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
                p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetTodayCandidatetimeLineSalesAssociate", p.ToArray()));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/GetNewCandidateTimeLine");
                throw;
            }
        }

        public string GetCandidateTimeLine()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@EndDate", DateTime.Now.Date.ToString("yyyy-MM-dd")));
                p.Add(new SqlParameter("@UserId", Convert.ToString(user.UserId)));
                return CommonHelperClass._serializeDatatable(dl.GetDataTable("GetCandidatetimeLineSalesAssociate", p.ToArray()));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/GetCandidateTimeLine");
                throw;
            }
        }

        #endregion

        #endregion
        #region Manage Recurring Details
        public string ShowRecurringDetails(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                DataSet ds = new DataSet();
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@CandidateId", Convert.ToString(prm.CandidateId)));
                ds = dl.GetDataSet("GetRecurringMasterTempByCandidateId", p.ToArray());
                return CommonHelperClass._serializeDataSet(ds);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/ShowRecurringDetails");
                throw;
            }
        }

        public string RecurringMasterTemp_Insert(string parameter)
        {
            try
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
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/RecurringMasterTemp_Insert");
                throw;
            }
        }

        public string RecurringMasterTemp_Update(string parameter)
        {
            try
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
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "SalesAssociate/dashboard/RecurringMasterTemp_Update");
                throw;
            }
        }
        #endregion

    }


}
