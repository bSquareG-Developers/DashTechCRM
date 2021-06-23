using DashTechCRM.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.test.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: test/DashBoard
        ConnectionDB dl = new ConnectionDB();
        DataTable dt = new DataTable();
        public ActionResult Index()
        {
            return View();
        }
        public string TeamDetailsGet()
        {
            dt = dl.GetDataTable("TeamDetailsGet");
            return CommonHelperClass._serializeDatatable(dt);
        }
        [HttpGet]
        public string GetUserAccountDetails()
        {
            try
            {
                dt = dl.GetDataTable("UserAccountDetailsGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpGet]
        public string GetLocationMaster()
        {
            try
            {
                dt = dl.GetDataTable("LocationMasterGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpGet]
        public string GetRoleMaster()
        {
            try
            {
                dt = dl.GetDataTable("RoleMasterGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        [HttpPost]
        public string UserAccountDetailsInsertUpdate(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@UserId", Convert.ToString(prm.UserId)));
                p.Add(new SqlParameter("@FullName", Convert.ToString(prm.fullName)));
                p.Add(new SqlParameter("@RocketName", Convert.ToString(prm.rocketName)));
                p.Add(new SqlParameter("@EmailId", Convert.ToString(prm.emailId)));
                p.Add(new SqlParameter("@Password", Convert.ToString(prm.password)));
                p.Add(new SqlParameter("@RefLocationId", Convert.ToString(prm.location)));
                p.Add(new SqlParameter("@RefRoleId", Convert.ToString(prm.role)));
                p.Add(new SqlParameter("@UserImageUrl", Convert.ToString(prm.imagefile)));
                p.Add(new SqlParameter("@IsActive", Convert.ToString(prm.Active)));
                p.Add(new SqlParameter("@LastLogin", Convert.ToString(DateTime.Now)));
                p.Add(new SqlParameter("@CompanyName", Convert.ToString(prm.companyName)));
                p.Add(new SqlParameter("@JoiningDate", Convert.ToString(DateTime.Now)));
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.Flag)));
                object result = dl.Execute_Scaler("Useraccountdetails_insert_update", p.ToArray());
                if (result.ToString() == "0")
                {
                    SendConfirmationMailUserAccountCreation(Convert.ToString(prm.fullName), Convert.ToString(prm.rocketName), Convert.ToString(prm.emailId), Convert.ToString(prm.password));
                }
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string SendConfirmationMailUserAccountCreation(string UserName, string RocketName, string EmailId, string Password)
        {
            string EmailBody = "<p>Hello <b>{UserName}</b>,</p><p> Thank you for join Dash Technologies.Here , we are sending you your details kindly see below.</p><p> Rocket Name : {RocketName}</p><p> Email Id: {EmailId}</p><p> Password : {Passwpord}</p><p> Thanks you.</p> ";

            EmailBody = EmailBody.Replace("{UserName}", UserName);
            EmailBody = EmailBody.Replace("{RocketName}", RocketName);
            EmailBody = EmailBody.Replace("{EmailId}", EmailId);
            EmailBody = EmailBody.Replace("{Passwpord}", Password);
            bool result = SMTPEmailSendingModel.Send(EmailId, EmailBody, "Dash Technologies Join", "", "");
            return "";
        }

    }
}
