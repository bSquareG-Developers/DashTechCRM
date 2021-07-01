using DashTechCRM.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DashTechCRM.Areas.User.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class UserAccountDetailsController : Controller
    {
        private dashTech_crm_Entities db = new dashTech_crm_Entities();
        ConnectionDB dl = new ConnectionDB();
        DataTable dt = new DataTable();
        // GET: User/UserAccountDetails
        public ActionResult Index()
        {
            var userAccountDetails = db.UserAccountDetails.Include(u => u.LocationMaster).Include(u => u.RoleMaster);
            return View(userAccountDetails.ToList());
        }




        // GET: User/UserAccountDetails/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                UserAccountDetail userAccountDetail = db.UserAccountDetails.Find(id);
                if (userAccountDetail == null)
                    return HttpNotFound();
                return View(userAccountDetail);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/Details");
                throw;
            }
        }

        // GET: User/UserAccountDetails/Create
        public ActionResult Create()
        {
            ViewBag.RefLocationId = new SelectList(db.LocationMasters, "LocationId", "LocationName");
            ViewBag.RefRoleId = new SelectList(db.RoleMasters, "RoleId", "RoleTitle");
            return View();
        }

        // POST: User/UserAccountDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FullName,RocketName,EmailId,Password,RefLocationId,RefRoleId,UserImageUrl,IsActive,LastLogin,CompanyName,JoiningDate")] UserAccountDetail userAccountDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.UserAccountDetails.Add(userAccountDetail);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.RefLocationId = new SelectList(db.LocationMasters, "LocationId", "LocationName", userAccountDetail.RefLocationId);
                ViewBag.RefRoleId = new SelectList(db.RoleMasters, "RoleId", "RoleTitle", userAccountDetail.RefRoleId);
                return View(userAccountDetail);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/Create");
                throw;
            }
        }

        // GET: User/UserAccountDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                UserAccountDetail userAccountDetail = db.UserAccountDetails.Find(id);
                if (userAccountDetail == null)
                    return HttpNotFound();
                ViewBag.RefLocationId = new SelectList(db.LocationMasters, "LocationId", "LocationName", userAccountDetail.RefLocationId);
                ViewBag.RefRoleId = new SelectList(db.RoleMasters, "RoleId", "RoleTitle", userAccountDetail.RefRoleId);
                return View(userAccountDetail);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/Edit");
                throw;
            }
        }

        // POST: User/UserAccountDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FullName,RocketName,EmailId,Password,RefLocationId,RefRoleId,UserImageUrl,IsActive,LastLogin,CompanyName,JoiningDate")] UserAccountDetail userAccountDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(userAccountDetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.RefLocationId = new SelectList(db.LocationMasters, "LocationId", "LocationName", userAccountDetail.RefLocationId);
                ViewBag.RefRoleId = new SelectList(db.RoleMasters, "RoleId", "RoleTitle", userAccountDetail.RefRoleId);
                return View(userAccountDetail);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/Edit");
                throw;
            }
        }

        // GET: User/UserAccountDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                UserAccountDetail userAccountDetail = db.UserAccountDetails.Find(id);
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                if (userAccountDetail == null)
                    return HttpNotFound();
                return View(userAccountDetail);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/Delete");
                throw;
            }
        }

        // POST: User/UserAccountDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                UserAccountDetail userAccountDetail = db.UserAccountDetails.Find(id);
                db.UserAccountDetails.Remove(userAccountDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/DeleteConfirmed");
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region User Account Details

        public ActionResult UserAccountDetailsGet()
        {
            return View();
        }

        public string TeamDetailsGet()
        {
            try
            {
                dt = dl.GetDataTable("TeamDetailsGet");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsGet");
                throw;
            }
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
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/GetUserAccountDetails");
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
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/GetLocationMaster");
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
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/GetRoleMaster");
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
                    //SendConfirmationMailUserAccountCreation(Convert.ToString(prm.fullName), Convert.ToString(prm.rocketName), Convert.ToString(prm.emailId), Convert.ToString(prm.password));
                }
                return result.ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/UserAccountDetailsInsertUpdate");
                throw;
            }
        }

        public string SendConfirmationMailUserAccountCreation(string UserName, string RocketName, string EmailId, string Password)
        {
            try
            {
                string EmailBody = "<p>Hello <b>{UserName}</b>,</p><p> Thank you for join Dash Technologies.Here , we are sending you your details kindly see below.</p><p> Rocket Name : {RocketName}</p><p> Email Id: {EmailId}</p><p> Password : {Passwpord}</p><p> Thanks you.</p> ";

                EmailBody = EmailBody.Replace("{UserName}", UserName);
                EmailBody = EmailBody.Replace("{RocketName}", RocketName);
                EmailBody = EmailBody.Replace("{EmailId}", EmailId);
                EmailBody = EmailBody.Replace("{Passwpord}", Password);
                //bool result = SMTPEmailSendingModel.Send(EmailId, EmailBody, "Dash Technologies Join", "", "");
                return "";
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/SendConfirmationMailUserAccountCreation");
                throw;
            }
        }
        [HttpPost]
        public bool UserAccountUserDelete(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@UserId", Convert.ToString(prm.UserId)));
                bool result = dl.Execute_NonQuery("UserAccountDetailsDelete", p.ToArray());
                return result;
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/UserAccountUserDelete");
                throw;
            }
        }

        #endregion

        #region Team Details

        public ActionResult GetTeamDetails()
        {
            return View();
        }

        public string TeamDetailsGetDepartment()
        {
            try
            {
                dt = dl.GetDataTable("TeamDetailsGetDepartment");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsGetDepartment");
                throw;
            }
        }

        public string TeamDetailsMembersRemaining(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.flag)));
                dt = dl.GetDataTable("TeamDetailsMembersGet", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsMembersRemaining");
                throw;
            }
        }

        public string TeamDetailsGetTeamManagerByDepartment(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@Department", Convert.ToString(prm.department)));
                dt = dl.GetDataTable("TeamDetailsGetTeamManagerByDepartment", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsGetTeamManagerByDepartment");
                throw;
            }
        }

        public string TeamDetailsGetTeamLeadByTeamManagerAndDepartment(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@Department", Convert.ToString(prm.department)));
                p.Add(new SqlParameter("@TeamManager", Convert.ToString(prm.teamManager)));
                dt = dl.GetDataTable("TeamDetailsGetTeamLeadByTeamManagerAndDepartment", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsGetTeamLeadByTeamManagerAndDepartment");
                throw;
            }
        }

        public string TeamDetailsInsertUpdate(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@Member", Convert.ToString(prm.member)));
                p.Add(new SqlParameter("@TeamLead", Convert.ToString(prm.teamLead)));
                p.Add(new SqlParameter("@TeamManager", Convert.ToString(prm.teamManager)));
                p.Add(new SqlParameter("@Department", Convert.ToString(prm.department)));
                p.Add(new SqlParameter("@IsEnabledTeam", Convert.ToString(prm.isEnabledTeam)));
                p.Add(new SqlParameter("@TeamId", Convert.ToString(prm.teamId)));
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.flag)));
                object result = dl.Execute_Scaler("TeamDetailsInsertUpdate", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "User/UserAccountDetails/TeamDetailsInsertUpdate");
                throw;
            }
        }
        #endregion
    }
}
