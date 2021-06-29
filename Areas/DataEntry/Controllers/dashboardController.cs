using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.DataEntry.Controllers
{
    public class dashboardController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        // GET: DataEntry/dashboard
        public ActionResult Index()
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

                string query = string.Format(@"SELECT CM.CandidateId,CM.CandidateName,CM.CandidateStatus,CM.VisaStatus,CM.Date AS EnrolledDate,CM.MarketingStartDate,CMD.MarketingContactNumber, CMD.MarketingEmailId,CASE WHEN  CMD.MktEmailContactStatusFlag  = 1 then 'Pending' END as StatusFlag,CMD.InsertedBy,UADEmail.EmailId,UADEmail.FullName,CMD.MarketingId FROM CandidateMaster CM JOIN UserAccountDetails UAD ON UAD.UserId = CM.RefSalesAssociate JOIN LocationMaster LM ON LM.LocationId = UAD.RefLocationId JOIN CandidateMarketingDetails CMD ON CMD.RefCandidateId = CM.CandidateId JOIN UserAccountDetails UADEmail on CMD.InsertedBy = UADEmail.RocketName WHERE CM.CandidateStatus NOT IN('In Marketing', 'On Hold')  AND CMD.MktEmailContactStatusFlag = 1 and UADEmail.refRoleId=2");
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
                    if (user.FullName == null)
                    {
                        if (user.RocketName != null)
                            username = user.RocketName;
                        else
                            username = "";
                    }
                    else
                        username = user.FullName;
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
                    //SMTPEmailSendingModel.Send(receiverEmail, emailBody, "Response for Generate EmailId and Contact For Marketing of an Candidate", "", "");

                }
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
