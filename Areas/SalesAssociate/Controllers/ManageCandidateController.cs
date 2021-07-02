using DashTechCRM.Models;
using DashTechCRM.Models.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashTechCRM.Areas.SalesAssociate.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class ManageCandidateController : Controller
    {
        // GET: SalesAssociate/ManageCandidate
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        dbConnectionModel directDb = new dbConnectionModel();
        ConnectionDB dl = new ConnectionDB();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RecurringList()
        {
            return View();
        }

        #region Get Data Json
        public JsonResult GetCandidateDetails(int id)
        {
            try
            {
                return Json(db.CandidateMasters.Find(id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public JsonResult GetFollowUpJson(int id)
        {
            UserObject user = Session["userInfo"] as UserObject;
            directDb.query = string.Format(@"select FollowUpBy , Convert(varchar,FollowUpDate,106) as FollowUpDate,FollowUpTime, FollowUpMessage,Department,FollowUpStatus from FollowUpMaster where RefCandidateId= {0} order by FollowUpDate desc ", id);
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIndexJson()
        {
            UserObject user = Session["userInfo"] as UserObject;
            directDb.query = @"
select CandidateMaster.CandidateId,CandidateMaster.CandidateName, CandidateMaster.EmailId,AgreementPercentage,AgreementSent,CandidateMaster.Date as EnrolledDate, TotalAmount, PaidAmount,ServiceName from CandidateMaster 
inner join UserAccountDetails on RefSalesAssociate = UserId
inner join SalesServiceMaster on ServiceId = RefServiceId
inner join RecurringType on RecurringTypeId = RefRecurringTypeId
left join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
left join CandidateAssign on refMarketingId = MarketingId
left join TeamDetails on TeamId = RefTeamId
left join PODetails on CandidateMaster.CandidateId = PODetails.CandidateId where  CandidateMaster.CandidateId = (
CASE When CandidateAssign.AssignedId is null then CandidateMaster.CandidateId
        when CandidateAssign.AssignedId is not null and CandidateAssign.IsActive = 1 then   CandidateMaster.CandidateId
            else -1
END
) and UserId = " + user.UserId + "order by CandidateMaster.Date desc";
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecurringJson()
        {
            UserObject user = Session["userInfo"] as UserObject;
            directDb.query = @"
            select CM.CandidateName, CM.EmailId,AgreementPercentage,AgreementSent,CM.Date as EnrolledDate, TotalAmount, PaidAmount,ServiceName, RM.* 
            from CandidateMaster CM
            inner join UserAccountDetails UAD on CM.RefSalesAssociate = UAD.UserId
            inner join SalesServiceMaster SSM on SSM.ServiceId = CM.RefServiceId
            inner join RecurringType RT on RT.RecurringTypeId = CM.RefRecurringTypeId
            inner join RecurringMaster RM on CM.CandidateId = RM.RefCandidateId
            left join CandidateMarketingDetails CMD on CMD.RefCandidateId = CM.CandidateId
            left join CandidateAssign CA on CA.refMarketingId = CMD.MarketingId
            left join TeamDetails TD on TD.TeamId = CA.RefTeamId
            left join PODetails PD on CM.CandidateId = PD.CandidateId where  CM.CandidateId = (
            CASE When CA.AssignedId is null then CM.CandidateId
                    when CA.AssignedId is not null and CA.IsActive = 1 then   CM.CandidateId
                        else -1
            END) and UserId=" + user.UserId + "order by CM.Date desc";
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecurringDetailsJson(int id)
        {
            UserObject user = Session["userInfo"] as UserObject;
            directDb.query = @"
            select CandidateMaster.CandidateId,CandidateMaster.CandidateName, TotalAmount, PaidAmount,ServiceName, RecurringMaster.* from CandidateMaster 
            inner join UserAccountDetails on RefSalesAssociate = UserId
            inner join SalesServiceMaster on ServiceId = RefServiceId
            inner join RecurringType on RecurringTypeId = RefRecurringTypeId
            inner join RecurringMaster on CandidateMaster.CandidateId = RecurringMaster.RefCandidateId
            left join CandidateMarketingDetails on CandidateMarketingDetails.RefCandidateId = CandidateMaster.CandidateId
            left join CandidateAssign on refMarketingId = MarketingId
            left join TeamDetails on TeamId = RefTeamId
            left join PODetails on CandidateMaster.CandidateId = PODetails.CandidateId where  CandidateMaster.CandidateId = (
            CASE When CandidateAssign.AssignedId is null then CandidateMaster.CandidateId
                    when CandidateAssign.AssignedId is not null and CandidateAssign.IsActive = 1 then   CandidateMaster.CandidateId
                        else -1
            END
            ) and CandidateMaster.CandidateId = " + id;
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecurringType(int id)
        {
            var d = db.RecurringTypes.Find(id);
            var dictionary = new Dictionary<string, dynamic>();
            dictionary.Add("Amount", d.Amount);
            dictionary.Add("Pay", d.Amount / d.Installment);
            dictionary.Add("Installment", d.Installment);
            return Json(dictionary, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Candidate CRUD
        [HttpPost]
        public ActionResult NewRecurring(RecurringMaster data)
        {
            data.PaymentStatus = "Paid";
            data.DueDate = DateTime.Now.Date;
            data.InstallmentNumber = "";
            data.SendReminderEmail = false;
            db.RecurringMasters.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteCandidate(int cid)
        {
            try
            {
                var followup_clist = db.FollowUpMasters.Where(a => a.RefCandidateId == cid).ToList();
                db.FollowUpMasters.RemoveRange(followup_clist);
                db.SaveChanges();
                var recurring_clist = db.RecurringMasters.Where(a => a.RefCandidateId == cid).ToList();
                db.RecurringMasters.RemoveRange(recurring_clist);
                db.SaveChanges();
                var candidate = db.CandidateMasters.Find(cid);
                db.CandidateMasters.Remove(candidate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public ActionResult AddCandidate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCandidate(CandidateMaster candidate, string recIn, HttpPostedFileBase AgreementFile, bool InTraining, bool ExpertCV, bool InMarketing, bool AgreementSent)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                RecurringType recurringType = db.RecurringTypes.Find(candidate.RefRecurringTypeId);
                //setting all default properties
                candidate.CandidateStatus = "Sales";
                candidate.MarketingStartDate = null;
                //if (recurringType.RecurringTitle.ToLower().Contains("one"))
                //{
                //    candidate.PaymentStatus = "Paid";
                //}
                //else
                //{
                candidate.PaymentStatus = "Installments";
                //}
                candidate.TotalAmount = recurringType.Amount;
                candidate.PaidAmount = 0;
                candidate.RefSalesAssociate = user.UserId;
                candidate.Date = DateTime.Now.Date;
                candidate.AgreementSent = AgreementSent;
                db.CandidateMasters.Add(candidate);
                db.SaveChanges();
                if (AgreementFile != null)
                {
                    string fileName = "";
                    string dir = Server.MapPath("/Content/agreements/");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    fileName = string.Format("{0}_agreement.pdf", candidate.CandidateId, AgreementFile.FileName.Split('.')[1]);
                    AgreementFile.SaveAs(dir + fileName);
                }
                DateTime currentDate = DateTime.Now.Date;
                string stringInstallment = "1st";
                decimal amount = candidate.TotalAmount / recurringType.Installment;
                string payStatus = "Paid";
                //recIn = "";
                directDb.query = string.Format(@"Insert into RecurringMaster (DueDate,PaidDate,Amount,RefCandidateId,ReceivedIn,SendReminderEmail,PaymentStatus,InstallmentNumber) values('{0}','{1}',{2},{3},'{4}',0,'{5}','{6}')", currentDate.Date.ToString("yyyy-MM-dd"), currentDate.Date.ToString("yyyy-MM-dd"), amount, candidate.CandidateId, recIn, payStatus, stringInstallment);

                directDb.runCmd(directDb.query);
                if (recurringType.RecurringTitle.Contains("Month"))
                {
                    currentDate = currentDate.AddMonths(1);
                }
                else
                {
                    currentDate = currentDate.AddDays(15);
                }

                decimal paidAmt = 0;
                foreach (RecurringMaster recurring in db.RecurringMasters.Where(a => a.RefCandidateId == candidate.CandidateId && a.PaymentStatus == "Paid").ToList())
                {
                    paidAmt += recurring.Amount;
                }

                candidate = db.CandidateMasters.Find(candidate.CandidateId);
                candidate.PaidAmount = paidAmt;
                db.SaveChanges();

                //setting followup details
                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = "Hi, New Candidate is Enrolled.";
                followUp.FollowUpStatus = candidate.CandidateStatus;
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidate.CandidateId;
                followUp.Department = "Sales";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "Candidate Inserted!" };

                string toEm = "";
                string ccEm = "";
                string body = @"Hi Team,";
                if (InTraining)
                {
                    body += "<br>Please Start Training for this Candidate:<br><br>";
                    body += string.Format(@"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}", candidate.CandidateName, candidate.EmailId, db.TechnologyMasters.Find(candidate.TechnologyId).TechTitle, candidate.MobileNumber, user.RocketName);
                    toEm = "training@dashtechinc.com";
                    if (db.UserAccountDetails.Find(candidate.RefSalesAssociate).RefLocationId == 1)
                    {
                        ccEm = "devansh@geta-job.com";
                    }
                    else
                    {
                        ccEm = "nirav@geta-job.com";
                    }
                    SMTPEmailSendingModel.Send(toEm, body, db.SalesServiceMasters.Find(candidate.RefServiceId).ServiceName + " Start Training:" + candidate.CandidateName, ccEm, "bhadresh@geta-job.com");
                }
                body = @"Hi Team,";
                if (InMarketing)
                {

                    body += "<br>Please Start Marketing for this Candidate:<br><br>";
                    body += string.Format(@"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}", candidate.CandidateName, candidate.EmailId, db.TechnologyMasters.Find(candidate.TechnologyId).TechTitle, candidate.MobileNumber, user.RocketName);
                    var u = db.UserAccountDetails.Find(candidate.RefSalesAssociate);
                    if (u.RefLocationId == 1)
                    {
                        toEm = "aog-ahm@dashtechinc.com";
                        ccEm = "devansh@geta-job.com";
                    }
                    else
                    {
                        toEm = "aog-vis@dashtechinc.com";
                        ccEm = "nirav@geta-job.com";
                    }
                    SalesServiceMaster salesService = db.SalesServiceMasters.Find(candidate.RefServiceId);
                    SMTPEmailSendingModel.Send(toEm, body, salesService.ServiceName + " Start Marketing:" + candidate.CandidateName, ccEm, "bhadresh@geta-job.com");
                }
                body = @"Hi Team,";
                if (ExpertCV)
                {

                    body += "<br>Please Start Resume Build Process for this Candidate:<br><br>";
                    body += string.Format(@"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}", candidate.CandidateName, candidate.EmailId, db.TechnologyMasters.Find(candidate.TechnologyId).TechTitle, candidate.MobileNumber, user.RocketName);
                    //toEm = "training@dashtechinc.com";
                    if (db.UserAccountDetails.Find(candidate.RefSalesAssociate).RefLocationId == 1)
                    {
                        toEm = "aog-ahm@dashtechinc.com";
                        ccEm = "devansh@geta-job.com,himanshu@expertcvcoach.com,jatin@dashtechinc.com";
                    }
                    else
                    {
                        toEm = "aog-vis@dashtechinc.com";
                        ccEm = "nirav@geta-job.com,himanshu@expertcvcoach.com,jatin@dashtechinc.com";
                    }
                    SMTPEmailSendingModel.Send(toEm, body, db.SalesServiceMasters.Find(candidate.RefServiceId).ServiceName + " Start Resume Build:" + candidate.CandidateName, ccEm, "bhadresh@geta-job.com");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ConnectionDB dl = new ConnectionDB();
                List<SqlParameter> p = new List<SqlParameter>();
                p.Add(new SqlParameter("@EmailId", Convert.ToString(candidate.EmailId)));
                p.Add(new SqlParameter("@MobileNo", Convert.ToString(candidate.MobileNumber)));
                object result = dl.Execute_Scaler("GetSalesAssociateNameByCandidateDetails", p.ToArray());
                result = result == null ? "" : result.ToString();
                //TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = "their are chance to have same details already present in our system with mobile Number or Email Id..!! Entry might be done by : " + result };
            }
            return View();
        }

        public string SaveNewCandidate(string parameter)
        {
            try
            {
                ConnectionDB dl = new ConnectionDB();
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                UserObject user = Session["userInfo"] as UserObject;
                p.Add(new SqlParameter("@CandidateName", Convert.ToString(prm.candidateName)));
                p.Add(new SqlParameter("@EmailId", Convert.ToString(prm.emailId)));
                p.Add(new SqlParameter("@MobileNumber", Convert.ToString(prm.mobileNumber)));

                p.Add(new SqlParameter("@RefSalesAssociate", Convert.ToString(user.UserId)));
                p.Add(new SqlParameter("@RefRecurringTypeId", Convert.ToString(prm.recurringTypeId)));

                p.Add(new SqlParameter("@RefServiceId", Convert.ToString(prm.serviceId)));
                p.Add(new SqlParameter("@TotalAmount", Convert.ToString(prm.totalAmmount)));
                p.Add(new SqlParameter("@PaidAmount", Convert.ToString(prm.enrollmentAmount)));

                p.Add(new SqlParameter("@Date", Convert.ToString(DateTime.Now.Date)));
                p.Add(new SqlParameter("@PaymentStatus", "Installments")); // Remaining
                p.Add(new SqlParameter("@CandidateStatus", "Sales"));
                p.Add(new SqlParameter("@Remarks", Convert.ToString(prm.remarks)));
                p.Add(new SqlParameter("@VisaStatus", Convert.ToString(prm.visaStatus)));

                p.Add(new SqlParameter("@TechnologyId", Convert.ToString(prm.technologyId)));
                p.Add(new SqlParameter("@AgreementSent", Convert.ToString(prm.agreementSent)));
                p.Add(new SqlParameter("@Agreement", Convert.ToString(prm.agreement)));
                p.Add(new SqlParameter("@AgreementLink", Convert.ToString(prm.agreementLink)));

                //Follow up details completed
                p.Add(new SqlParameter("@FollowUpStatus", "Sales"));
                p.Add(new SqlParameter("@FollowUpTime", Convert.ToString(DateTime.Now.TimeOfDay)));
                p.Add(new SqlParameter("@FollowUpMessage", "Hi, New Candidate is Enrolled."));
                p.Add(new SqlParameter("@FollowUpBy", Convert.ToString(user.RocketName)));
                p.Add(new SqlParameter("@Department", "Sales"));
                p.Add(new SqlParameter("@Installments", Convert.ToString(prm.recurringInstallment)));
                p.Add(new SqlParameter("@recIn", Convert.ToString(prm.recIn)));
                p.Add(new SqlParameter("@isJobGaruntee", Convert.ToString(prm.inJobGaruntee)));
                p.Add(new SqlParameter("@JobGarunteeLastDate", Convert.ToString(prm.JobGarunteeLastDate)));
                p.Add(new SqlParameter("@RePaymentMonths", Convert.ToString(prm.rePaymentMonths)));
                p.Add(new SqlParameter("@isRembursed", Convert.ToString(prm.rembursed)));
                p.Add(new SqlParameter("@ContractedAmount", Convert.ToString(prm.contractedAmount)));
                p.Add(new SqlParameter("@TVPRecurringMaster", (DataTable)JsonConvert.DeserializeObject(Convert.ToString(prm.recurringDataArr), (typeof(DataTable)))));
                object resultInsertCandidate = dl.Execute_Scaler("CandidateMaster_Insert", p.ToArray());

                if (resultInsertCandidate.ToString() == "1")
                {
                    string toEm = "";
                    string ccEm = "";
                    string body = @"Hi Team,";
                    string technologyName = db.TechnologyMasters.Find(Convert.ToInt32(prm.technologyId)).TechTitle;
                    if (Convert.ToString(prm.inTraining) == "1")
                    {
                        body += "<br>Please Start Training for this Candidate:<br><br>";
                        body += string.Format(
                            @"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}",
                            Convert.ToString(prm.candidateName), Convert.ToString(prm.emailId),
                            Convert.ToString(prm.mobileNumber), technologyName, user.RocketName);
                        toEm = "training@dashtechinc.com";
                        if (db.UserAccountDetails.Find(user.UserId).RefLocationId == 1)
                        {
                            ccEm = "devansh@geta-job.com";
                        }
                        else
                        {
                            ccEm = "nirav@geta-job.com";
                        }

                        string serviceName = db.SalesServiceMasters.Find(Convert.ToInt32(prm.serviceId)).ServiceName;

                        SMTPEmailSendingModel.Send(toEm, body, serviceName + " Start Training:" + Convert.ToString(prm.candidateName), ccEm, "bhadresh@geta-job.com");
                    }

                    body = @"Hi Team,";
                    if (Convert.ToString(prm.inMarketing) == "1")
                    {

                        body += "<br>Please Start Marketing for this Candidate:<br><br>";
                        body += string.Format(
                            @"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}",
                            Convert.ToString(prm.candidateName), Convert.ToString(prm.emailId),
                            Convert.ToString(prm.mobileNumber), technologyName, user.RocketName);

                        var u = db.UserAccountDetails.Find(user.UserId);
                        if (u.RefLocationId == 1)
                        {
                            toEm = "aog-ahm@dashtechinc.com";
                            ccEm = "devansh@geta-job.com";
                        }
                        else
                        {
                            toEm = "aog-vis@dashtechinc.com";
                            ccEm = "nirav@geta-job.com";
                        }

                        SalesServiceMaster salesService = db.SalesServiceMasters.Find(Convert.ToInt32(prm.serviceId));
                        string serviceName = salesService.ServiceName;
                        SMTPEmailSendingModel.Send(toEm, body, serviceName + " Start Marketing:" + Convert.ToString(prm.candidateName), ccEm, "bhadresh@geta-job.com");
                        //SMTPEmailSendingModel.Send("kautilya.v@dashtechinc.com", body, serviceName + " Start Marketing:" + Convert.ToString(prm.candidateName), ccEm, "bhadresh@geta-job.com");
                    }

                    body = @"Hi Team,";
                    if (Convert.ToString(prm.expertCv) == "1")
                    {

                        body += "<br>Please Start Resume Build Process for this Candidate:<br><br>";
                        body += string.Format(
                            @"Candidate Name: {0}<br>Email Address: {1}<br>Contact Number:{2}<br>Technology: {3}<br>Sales Person: {4}",
                            Convert.ToString(prm.candidateName), Convert.ToString(prm.emailId),
                            Convert.ToString(prm.mobileNumber), technologyName, user.RocketName);
                        //toEm = "training@dashtechinc.com";
                        if (db.UserAccountDetails.Find(user.UserId).RefLocationId == 1)
                        {
                            toEm = "aog-ahm@dashtechinc.com";
                            ccEm = "devansh@geta-job.com,himanshu@expertcvcoach.com,jatin@dashtechinc.com";
                        }
                        else
                        {
                            toEm = "aog-vis@dashtechinc.com";
                            ccEm = "nirav@geta-job.com,himanshu@expertcvcoach.com,jatin@dashtechinc.com";
                        }

                        string serviceName = db.SalesServiceMasters.Find(Convert.ToInt32(prm.serviceId)).ServiceName;
                        SMTPEmailSendingModel.Send(toEm, body, serviceName + " Start Resume Build:" + Convert.ToString(prm.candidateName), ccEm, "bhadresh@geta-job.com");
                        //SMTPEmailSendingModel.Send("kautilya.v@dashtechinc.com", body, serviceName + " Start Resume Build:" + Convert.ToString(prm.candidateName), ccEm, "bhadresh@geta-job.com");
                    }
                }

                return resultInsertCandidate.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
        public ActionResult UpdateCandidate(int id)
        {
            return View(db.CandidateMasters.Find(id));
        }
        [HttpPost]
        public ActionResult UpdateCandidate(CandidateMaster candidate, HttpPostedFileBase AgreementFile)
        {
            try
            {
                //getting sales person details
                UserObject user = Session["userInfo"] as UserObject;

                RecurringType recurringType = db.RecurringTypes.Find(candidate.RefRecurringTypeId);

                //setting all default properties
                db.Entry(candidate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (AgreementFile != null)
                {
                    string fileName = "";
                    string dir = Server.MapPath("/Content/agreements/");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    fileName = string.Format("{0}_agreement.pdf", candidate.CandidateId, AgreementFile.FileName.Split('.')[1]);
                    AgreementFile.SaveAs(dir + fileName);
                }

                decimal paidAmt = 0;
                foreach (RecurringMaster recurring in db.RecurringMasters.Where(a => a.RefCandidateId == candidate.CandidateId && a.PaymentStatus == "Paid").ToList())
                {
                    paidAmt += recurring.Amount;
                }

                candidate = db.CandidateMasters.Find(candidate.CandidateId);
                candidate.PaidAmount = paidAmt;
                db.SaveChanges();

                //setting followup details
                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = "Hi, Candidate Details is Updated!";
                followUp.FollowUpStatus = candidate.CandidateStatus;
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidate.CandidateId;
                followUp.Department = "Sales";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "Candidate Details Updated!" };
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
            }
            return View();
        }
        #endregion

        #region Manage Recurring
        public ActionResult ReccurringDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecurringPayment(RecurringMaster model, DateTime? RDate, decimal? RAmount)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                RecurringMaster updateRecurring = db.RecurringMasters.Find(model.RecurringId);
                updateRecurring.PaidDate = DateTime.Now.Date;
                updateRecurring.PaymentStatus = model.PaymentStatus;
                updateRecurring.ReceivedIn = model.ReceivedIn;
                updateRecurring.remarks = model.remarks;
                updateRecurring.Amount = model.Amount;
                db.SaveChanges();
                if (updateRecurring.PaymentStatus == "Paid")
                {
                    CandidateMaster candidate1 = db.CandidateMasters.Find(updateRecurring.RefCandidateId);
                    candidate1.PaidAmount += model.Amount;
                    db.SaveChanges();
                    if (RAmount != null && RDate != null)
                    {
                        DateTime dt = (DateTime)RDate;
                        RecurringMaster PartialRecurring = new RecurringMaster();
                        PartialRecurring.Amount = decimal.Parse(RAmount.ToString());
                        PartialRecurring.DueDate = dt;
                        PartialRecurring.InstallmentNumber = "PRT";
                        PartialRecurring.PaidDate = null;
                        PartialRecurring.PaymentStatus = "Un-Paid";
                        PartialRecurring.ReceivedIn = "";
                        PartialRecurring.RefCandidateId = candidate1.CandidateId;
                        PartialRecurring.remarks = model.remarks;
                        PartialRecurring.SendReminderEmail = false;
                        db.RecurringMasters.Add(PartialRecurring);
                        db.SaveChanges();
                    }
                }

                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = "Hi, Candidate Recurring Payment Update:" + updateRecurring.PaymentStatus;
                followUp.FollowUpStatus = "Sales";
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = updateRecurring.RefCandidateId;
                followUp.Department = "Sales";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();

                CandidateMaster candidate = db.CandidateMasters.Find(updateRecurring.RefCandidateId);
                string message = string.Format(@"
Hi Team,<br> We got Payment Update for <b>{0}</b><br><br>
<table  border=""1"" cellspacing=""0"" cellpadding=""0""
width=""444"" style='width:333.0pt;background:white;mso-background-themecolor:
background1;border-collapse:collapse;border:none;mso-border-alt:double windowtext 1.5pt;
mso-yfti-tbllook:1184;mso-padding-alt:0in 5.4pt 0in 5.4pt;mso-border-insideh:
1.5pt double windowtext;mso-border-insidev:1.5pt double windowtext'>
<tr style='mso-yfti-irow:-1;mso-yfti-firstrow:yes;mso-yfti-lastfirstrow:yes'>
 <td valign=top style='border:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:5'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Client Name<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border:double windowtext 1.5pt;border-left:none;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:1'><span class=SpellE><span style='font-size:12.0pt;
 font-family:""Calibri Light"",sans-serif;mso-ascii-theme-font:major-latin;
 mso-fareast-font-family:""Times New Roman"";mso-hansi-theme-font:major-latin;
 mso-bidi-theme-font:major-latin;color:black;mso-themecolor:text1;mso-bidi-font-weight:
 bold'></span></span><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1;mso-bidi-font-weight:bold'> <span class=SpellE>{0}</span><o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:0'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Contact Number<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{1}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:1'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Email Address<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{2}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:2'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Total Amount Paid (In USD)<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{3}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:3'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Amount Remaining (In USD)<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{4}<o:p></o:p></span></p>
 </td>
</tr>

<tr style='mso-yfti-irow:4'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Amount (In USD)<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{5}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:4'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Mode of Payment<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{6}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:5'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Subscription Date (Mandatory if payment mode is
 'Recurring')<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{7}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:6'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Sales Associate Name<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span class=SpellE><span style='font-size:12.0pt;
 font-family:""Calibri Light"",sans-serif;mso-ascii-theme-font:major-latin;
 mso-fareast-font-family:""Times New Roman"";mso-hansi-theme-font:major-latin;
 mso-bidi-theme-font:major-latin;color:black;mso-themecolor:text1'></span></span><span
 style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;mso-ascii-theme-font:
 major-latin;mso-fareast-font-family:""Times New Roman"";mso-hansi-theme-font:
 major-latin;mso-bidi-theme-font:major-latin;color:black;mso-themecolor:text1'>
 <span class=SpellE>{8}</span><o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:7'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Sales Associate Email Address<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{9}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:8'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:68'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Services<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:64'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{10}<o:p></o:p></span></p>
 </td>
</tr>
<tr style='mso-yfti-irow:9;mso-yfti-lastrow:yes'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Technology<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{11}<o:p></o:p></span></p>
 </td>
</tr>

<tr style='mso-yfti-irow:9;mso-yfti-lastrow:yes'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Status<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{12}<o:p></o:p></span></p>
 </td>
</tr>


<tr style='mso-yfti-irow:9;mso-yfti-lastrow:yes'>
 <td valign=top style='border:double windowtext 1.5pt;border-top:none;
 mso-border-top-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal;mso-yfti-cnfc:4'><b><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>Remakrs<o:p></o:p></span></b></p>
 </td>
 <td valign=top style='border-top:none;border-left:none;border-bottom:double windowtext 1.5pt;
 border-right:double windowtext 1.5pt;mso-border-top-alt:double windowtext 1.5pt;
 mso-border-left-alt:double windowtext 1.5pt;padding:0in 5.4pt 0in 5.4pt'>
 <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
 normal'><span style='font-size:12.0pt;font-family:""Calibri Light"",sans-serif;
 mso-ascii-theme-font:major-latin;mso-fareast-font-family:""Times New Roman"";
 mso-hansi-theme-font:major-latin;mso-bidi-theme-font:major-latin;color:black;
 mso-themecolor:text1'>{13}<o:p></o:p></span></p>
 </td>
</tr>

</table>

", candidate.CandidateName, candidate.MobileNumber, candidate.EmailId, candidate.PaidAmount, candidate.TotalAmount - candidate.PaidAmount, candidate.PaidAmount.ToString("0.00") + " (Paid)", model.ReceivedIn, candidate.Date.ToString("MMM, dd yyyy"), db.UserAccountDetails.Find(candidate.RefSalesAssociate).RocketName, db.UserAccountDetails.Find(candidate.RefSalesAssociate).EmailId, db.SalesServiceMasters.Find(candidate.RefServiceId).ServiceName, db.TechnologyMasters.Find(candidate.TechnologyId).TechTitle, candidate.PaymentStatus, candidate.Remarks);


                string toEmailId = "";

                string ccEmailId = "";

                var salesAssociate = db.UserAccountDetails.Find(candidate.RefSalesAssociate);
                var team = db.TeamDetails.Where(a => a.Member == candidate.RefSalesAssociate).ToList();
                if (team.Count == 1)
                {
                    var tlId = team[0].TeamLead;
                    var mangId = team[0].TeamManager;
                    var salesTL = db.UserAccountDetails.Single(a => a.UserId == tlId).EmailId;
                    var salesManager = db.UserAccountDetails.Single(a => a.UserId == mangId).EmailId;
                    ccEmailId = string.Format("{0},{1},{2}", salesTL, salesManager, salesAssociate.EmailId);
                }
                if (salesAssociate.RefLocationId == 2)
                {
                    toEmailId = "nirav@dashtechinc.com";
                }
                else if (salesAssociate.RefLocationId == 1)
                {
                    toEmailId = "devansh@dashtechinc.com";
                }
                else if (salesAssociate.RefLocationId == 3)
                {
                    toEmailId = "devansh@dashtechinc.com,nirav@dashtechinc.com";
                }

                SMTPEmailSendingModel.Send(toEmailId, message, "New Client:" + candidate.CandidateName, ccEmailId);


            }
            catch (Exception ex)
            {

                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };

            }

            return RedirectToAction("ReccurringDetails");
        }
        #endregion


        public ActionResult ChangeStatus(string status, int cid, string message)
        {
            try
            {
                ConnectionDB dl = new ConnectionDB();
                List<SqlParameter> p = new List<SqlParameter>();
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
                p.Add(new SqlParameter("@FollowUpMessage", "Status : " + status + " Remarks : " + message));
                p.Add(new SqlParameter("@FollowUpBy", user.UserId));
                p.Add(new SqlParameter("@CandidateId", cid));
                dl.Execute_NonQuery("FollowUpMasterLog_Insert", p.ToArray());
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "Follow Up Message Saved!" };

            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };
                CommonHelperClass.InsertErrorLog(ex.Message, "ManageCandidate/ChangeStatus");
            }
            return RedirectToAction("Index", "Dashboard");
        }

        public string CheckExistMobileorEmail(string parameter)
        {
            try
            {
                dynamic prm = JObject.Parse(parameter);
                List<SqlParameter> p = new List<SqlParameter>();
                ConnectionDB dl = new ConnectionDB();
                p.Add(new SqlParameter("@Flag", Convert.ToString(prm.flag)));
                p.Add(new SqlParameter("@Value", Convert.ToString(prm.value)));
                object result = dl.Execute_Scaler("CandidatemasterCheckExistingEmailOrMobile", p.ToArray());
                return result.ToString();
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "ManageCandidate/CheckExistMobileorEmail");
                throw;
            }
        }

        public string BindVisaStatus()
        {
            try
            {
                ConnectionDB dl = new ConnectionDB();
                DataTable dt = new DataTable();
                dt = dl.GetDataTable("select * FROM VisaTitleMaster");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "ManageCandidate/BindVisaStatus");
                throw;
            }
        }

        public ActionResult ViewInterviewSubmissions()
        {
            return View();
        }

        public string GetSubmissionInterviewDetails()
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                string UserId = user == null ? "" : user.UserId.ToString();
                return CommonHelperClass._serializeDatatable(dl.GetDataTable(
                    "SELECT TeamManager.FullName as MarketingManager ,TeamLead.FullName as MarketingTeamLead ,UADMember.FullName as RecruiterName ,CM.CandidateName ,COUNT(ID.InteviewId) AS Interviews ,COUNT(SD.RefAssignedId) AS Submissions ,CM.VisaStatus ,TM.TechTitle FROM SubmissionDetails SD LEFT JOIN InterviewDetails ID ON ID.RefSumissionId = SD.SubmissionId INNER JOIN CandidateAssign CA ON CA.AssignedId = SD.RefAssignedId INNER JOIN CandidateMarketingDetails CMD ON CMD.MarketingId = CA.refMarketingId INNER JOIN CandidateMaster CM ON CM.CandidateId = CMD.RefCandidateId INNER JOIN TechnologyMaster TM ON TM.TechId = CM.TechnologyId INNER JOIN TeamDetails TD on TD.TeamId = CA.RefTeamId LEFT JOIN UserAccountDetails UADMember ON CA.refAssignRecruiter = UADMember.UserId INNER JOIN UserAccountDetails TeamLead ON TD.TeamLead = TeamLead.UserId INNER JOIN UserAccountDetails TeamManager ON TD.TeamManager = TeamManager.UserId where TeamLead.RefRoleId = 3 and TeamManager.UserId =" + UserId + " GROUP BY CM.CandidateName,CM.VisaStatus,TM.TechTitle,UADMember.FullName,TeamLead.FullName,TeamManager.FullName"));
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "ManageCandidate/GetSubmissionInterviewDetails");
                throw;
            }
        }

        public string GetRecurringTypeBind(string parameter)
        {
            try
            {
                List<SqlParameter> p = new List<SqlParameter>();
                DataTable dt = new DataTable();
                dynamic prm = JObject.Parse(parameter);
                p.Add(new SqlParameter("@IsOneTime", Convert.ToString(prm.IsOneTime)));
                dt = dl.GetDataTable("GetRecurringType", p.ToArray());
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "ManageCandidate/GetRecurringType");
                throw;
            }
        }

    }
}
