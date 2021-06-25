using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DashTechCRM.Areas.User.Controllers
{
    public class LoginController : Controller
    {
        // GET: User/Login
        #region Login Action
        ConnectionDB dl = new ConnectionDB();
        List<SqlParameter> p = new List<SqlParameter>();
        DataTable dt = new DataTable();
        //this will use as login action for all
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Index(string emailId, string password)
        {
            try
            {
                UserLogic userLogic = new UserLogic();
                UserObject user = userLogic.LoginProcess(emailId, password);
                if (user == null)
                {
                    TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = "Email Id or Password is wrong!" };

                    return View();
                }
                Session["userInfo"] = user;
                return RedirectToAction("RoleBasedRedirection", new { Role = user.Role });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            //return View();
        }
        #endregion

        #region Manage Redirection after login
        public ActionResult RoleBasedRedirection(string Role)
        {
            try
            {
                string defUrl = ConfigurationManager.AppSettings["defUrl"].ToString();
                string areaName = Role.Replace(" ", "");
                if (areaName.ToLower() == "on boarding" || areaName.ToLower() == "account")
                {
                    areaName = "otherdept";
                }
                string defController = "dashboard";
                if (Role == "Test")
                {
                    defController = "ctest";
                    areaName = "commontest";
                }
                string rediretUrl = string.Format("{0}/{1}/{2}", defUrl, areaName, defController);
                UserObject user = Session["userInfo"] as UserObject;
                user.DefUrl = rediretUrl;
                Session["userInfo"] = user;
                return Redirect(rediretUrl);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                throw;
            }


        }
        #endregion

        public ActionResult ProfileManage()
        {
            return View();
        }

        public string getUserDetails()
        {
            try
            {
                DataTable dt = new DataTable();
                ConnectionDB dl = new ConnectionDB();
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user != null ? user.UserId.ToString() : "";
                string query = "select * from UserAccountDetails where UserId = " + userId;
                dt = dl.GetDataTable(query);
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string ChangePassword(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@UserId", userId));
                p.Add(new SqlParameter("@UserPassword", Convert.ToString(prm.newPassword)));
                object result = dl.Execute_NonQuery("UserAccountDetails_ChangePassword", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetUserDetailsByUserId()
        {
            try
            {
                p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                string userId = user != null ? user.UserId.ToString() : "";
                p.Add(new SqlParameter("@userId", userId));
                dt = dl.GetDataTable("UserAccountDetailsGetByUserId", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string ForgetPassword(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string rocketName = Convert.ToString(prm.rocketName);
            string emailId = Convert.ToString(prm.emailId);
            p.Add(new SqlParameter("@RocketName", rocketName));
            p.Add(new SqlParameter("@EmailId", emailId));
            dt = dl.GetDataTable("UserAccountDetails_UpdateDefaultPassword", p.ToArray());
            return CommonHelperClass._serializeDatatable(dt);
        }

        public string SendMailUpdatedForgotPassword(string parameter)
        {
            dynamic prm = JObject.Parse(parameter);
            string emailId = Convert.ToString(prm.EmailId);
            string password = Convert.ToString(prm.Password);
            string fullName = Convert.ToString(prm.FullName);
            string emailBody = "<p>Hello {username} ,</p><p><br></p><p> Your password has been updated successfully here are your credentials for login </p><p> Email : {Email} </p><p> Password : {Password}</p><p><br></p><p>Thanks you</p>";
            emailBody = emailBody.Replace("{username}", fullName);
            emailBody = emailBody.Replace("{Email}", emailId);
            emailBody = emailBody.Replace("{Password}", password);
            bool result = SMTPEmailSendingModel.Send(emailId, emailBody, "Forgot Password", "", "");
            return result.ToString();
        }
    }
}
