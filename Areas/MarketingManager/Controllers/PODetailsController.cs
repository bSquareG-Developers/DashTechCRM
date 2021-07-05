using DashTechCRM.Models;
using DashTechCRM.Models.User;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DashTechCRM.Areas.MarketingManager.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class PODetailsController : Controller
    {
        private dashTech_crm_Entities db = new dashTech_crm_Entities();

        // GET: MarketingManager/PODetails
        public ActionResult Index()
        {
            return View();
        }

        #region JSON Result
        public JsonResult getIndex(string s = null)
        {
            UserObject user = Session["userInfo"] as UserObject;

            UserAccountDetail current = db.UserAccountDetails.Find(user.UserId);

            dbConnectionModel directDB = new dbConnectionModel();
            if (s != null)
            {
                s = string.Format(" and PODate < '{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }
            else
            {
                s = string.Format(" and PODate = '{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }
            directDB.query = @"SELECT * FROM PODetails INNER JOIN UserAccountDetails ON RocketName = PoDetails.AddBy WHERE UserAccountDetails.RefLocationId = " + current.RefLocationId + " and RefRoleId = 2 " + s + " and (ConfirmPO <> 'Deleted' or ConfirmPO is null)";
            var data = directDB.GetDictionary();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        #endregion

        [HttpPost]
        public ActionResult DeletePO(int id)
        {
            PODetail pODetail = db.PODetails.Find(id);
            pODetail.ConfirmPO = "Deleted";
            db.SaveChanges();
            var url = Url.Action("index", "PODetails", new { area = "MarketingManager" });
            return Redirect(url);
        }
        [HttpPost]
        public ActionResult UpdateDateOfJoining(int jd_po_id, DateTime joiningdate)
        {
            PODetail pODetail = db.PODetails.Find(jd_po_id);
            //pODetail.ConfirmPO = "Deleted";
            pODetail.JoiningDate = joiningdate;
            db.SaveChanges();
            var url = Url.Action("index", "PODetails", new { area = "MarketingManager" });
            return Redirect(url);
        }

        [HttpPost]
        public ActionResult SaveFollowup(string remarks, int candidateId, string status)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                FollowUpMaster followUp = new FollowUpMaster();
                followUp.FollowUpBy = user.RocketName;
                followUp.FollowUpDate = DateTime.Now.Date;
                followUp.FollowUpMessage = remarks;
                followUp.FollowUpStatus = status;
                followUp.FollowUpTime = DateTime.Now.TimeOfDay;
                followUp.RefCandidateId = candidateId;
                followUp.Department = "ExpertCv";
                db.FollowUpMasters.Add(followUp);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };

            }
            return RedirectToAction("Index");
        }
        // GET: MarketingManager/PODetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PODetail pODetail = db.PODetails.Find(id);
            if (pODetail == null)
            {
                return HttpNotFound();
            }
            return View(pODetail);
        }

        public ActionResult noneaog()
        {
            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName");
            return View();
        }

        // GET: MarketingManager/PODetails/Create
        public ActionResult Create()
        {
            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName");
            return View();
        }

        // POST: MarketingManager/PODetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PODetail pODetail)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                pODetail.AddBy = user.RocketName;
                pODetail.EnteryDate = DateTime.Now.Date;
                pODetail.IsDeleted = false;
                pODetail.UpdateBy = "";
                pODetail.UpdateDate = null;
                pODetail.ConfirmPO = "Pending";
                pODetail.CandidateName = db.CandidateMasters.Find(pODetail.CandidateId).CandidateName;
                db.PODetails.Add(pODetail);
                db.SaveChanges();
                var tlemailid = db.UserAccountDetails.FirstOrDefault(a => a.RocketName == pODetail.TeamLeadName).EmailId;
                var userDetails = db.UserAccountDetails.FirstOrDefault(a => a.RocketName == pODetail.TeamLeadName);
                var sendConfirmation = "";
                if (userDetails.RefLocationId == 1)
                {
                    tlemailid += ",po-amd@dashtechinc.com";
                    sendConfirmation = "devansh@dashtechinc.com";
                }
                else if (userDetails.RefLocationId == 2)
                {
                    tlemailid += ",po-vis@dashtechinc.com";
                    sendConfirmation = "nirav@dashtechinc.com";
                }


                string emailBody = string.Format(@"<div style=""width: 90%; margin: 50px auto; font-family: Calibri; "">
        <table
            style=""width: 100%;margin: 5px auto;box-shadow: 0px 0px 5px black;padding: 5px;border-radius: 10px;background-color: blanchedalmond;"">
            <tr style=""height: 50px; background-color: burlywood; font-size: 20px;"">
                <th colspan=""2"">PO Details</th>
            </tr>
            <tr>
                <th style=""text-align: left; width: 30%;"">Candidate Name</th>
                <td style=""text-align: left;width: 70%;"">{0}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">PO Date</th>
                <td>{1}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Source/Reference of Candidate</th>
                <td>{2}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Sales Associates</th>
                <td>{3}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Type</th>
                <td>{4}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Applied For Position</th>
                <td>{5}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Interview Support</th>
                <td>{6}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Location</th>
                <td>{7}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">End Client</th>
                <td>{8}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Vendor Details</th>
                <td>{9}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Rate</th>
                <td>$ {10}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Sign Up Date</th>
                <td>{11}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Training Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Technical Expert Name</th>
                <td>{12}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Training Dates</th>
                <td>{13} to {14}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Marketing Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Team Lead Name</th>
                <td>{15}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Recruiter Name</th>
                <td>{16}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Marketing Dates</th>
                <td>{17} to {18}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Agreement Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Months</th>
                <td>{19}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Percentages</th>
                <td>{20}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Other Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Description</th>
                <td><pre>{21}</pre></td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Remarks</th>
                <td><pre>{22}</pre></td>
            </tr>
        </table>
    </div>", pODetail.CandidateName,
               pODetail.PODate,
               pODetail.RefOfCandidate,
               db.UserAccountDetails.Find(pODetail.RefSaleId).FullName,
               pODetail.JobType,
               pODetail.AppliedPosition,
               pODetail.InterviewSupport, pODetail.JobLocation, pODetail.EndClient, pODetail.VendorDetails, pODetail.Rate,
               pODetail.SignUpDate, pODetail.TrainerName,
               pODetail.TrainingStartDate, pODetail.TrainingEndDate,
               pODetail.TeamLeadName, pODetail.RecruiterName,
               pODetail.MarketingStartDate, pODetail.MarketingEndDate,
               pODetail.AgreementMonths, pODetail.AgreementPercentage,
               pODetail.JobDescription, pODetail.OtherDetails);



                //SMTPEmailSendingModel.Send(user.EmailId, emailBody, "PO detail Form of (" + pODetail.CandidateName + ")", tlemailid, "bhadresh@dashtechinc.com");

                SMTPEmailSendingModel.Send("nirav@dashtechinc.com", emailBody, "PO detail Form of (" + pODetail.CandidateName + ")", "nirav@dashtechinc.com", "nirav@dashtechinc.com");


                SMTPEmailSendingModel.Send("nirav@dashtechinc.com", "Hi,<br>Confirmation for this PO is Pending in CRM.", "Pending PO Confirmation (" + pODetail.CandidateName + ")", "", "nirav@dashtechinc.com");

                // SMTPEmailSendingModel.Send(sendConfirmation, "Hi,<br>Confirmation for this PO is Pending in CRM.", "Pending PO Confirmation (" + pODetail.CandidateName + ")", "", "bhadresh@dashtechinc.com");


                FollowUpMaster follow = new FollowUpMaster()
                {
                    Department = "Marketing",
                    FollowUpBy = user.RocketName,
                    FollowUpDate = DateTime.Now.Date,
                    FollowUpMessage = "PO detail filled for (" + pODetail.CandidateName + ")",
                    FollowUpStatus = "Placed",
                    FollowUpTime = DateTime.Now.TimeOfDay,
                    RefCandidateId = (int)pODetail.CandidateId
                };
                db.FollowUpMasters.Add(follow);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "PO Email Sent!" };

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };

            }

            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName", pODetail.RefSaleId);
            return View(pODetail);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateAogDetails(PODetail pODetail)
        {
            try
            {
                UserObject user = Session["userInfo"] as UserObject;
                pODetail.AddBy = user.RocketName;
                pODetail.EnteryDate = DateTime.Now.Date;
                pODetail.IsDeleted = false;
                pODetail.UpdateBy = "";
                pODetail.UpdateDate = null;
                pODetail.ConfirmPO = "Pending";

                //pODetail.CandidateName = db.CandidateMasters.Find(pODetail.CandidateId).CandidateName;
                db.PODetails.Add(pODetail);
                db.SaveChanges();
                var tlemailid = db.UserAccountDetails.FirstOrDefault(a => a.RocketName == pODetail.TeamLeadName).EmailId;
                var userDetails = db.UserAccountDetails.FirstOrDefault(a => a.RocketName == pODetail.TeamLeadName);
                var sendConfirmation = "";
                if (userDetails.RefLocationId == 1)
                {
                    tlemailid += ",po-amd@dashtechinc.com";
                    sendConfirmation = "devansh@dashtechinc.com";
                }
                else if (userDetails.RefLocationId == 2)
                {
                    tlemailid += ",po-vis@dashtechinc.com";
                    sendConfirmation = "nirav@dashtechinc.com";
                }


                string emailBody = string.Format(@"<div style=""width: 90%; margin: 50px auto; font-family: Calibri; "">
        <table
            style=""width: 100%;margin: 5px auto;box-shadow: 0px 0px 5px black;padding: 5px;border-radius: 10px;background-color: blanchedalmond;"">
            <tr style=""height: 50px; background-color: burlywood; font-size: 20px;"">
                <th colspan=""2"">PO Details</th>
            </tr>
            <tr>
                <th style=""text-align: left; width: 30%;"">Candidate Name</th>
                <td style=""text-align: left;width: 70%;"">{0}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">PO Date</th>
                <td>{1}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Source/Reference of Candidate</th>
                <td>{2}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Sales Associates</th>
                <td>{3}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Type</th>
                <td>{4}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Applied For Position</th>
                <td>{5}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Interview Support</th>
                <td>{6}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Location</th>
                <td>{7}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">End Client</th>
                <td>{8}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Vendor Details</th>
                <td>{9}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Rate</th>
                <td>$ {10}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Sign Up Date</th>
                <td>{11}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Training Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Technical Expert Name</th>
                <td>{12}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Training Dates</th>
                <td>{13} to {14}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Marketing Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Team Lead Name</th>
                <td>{15}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Recruiter Name</th>
                <td>{16}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Marketing Dates</th>
                <td>{17} to {18}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Agreement Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Months</th>
                <td>{19}</td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Percentages</th>
                <td>{20}</td>
            </tr>
            <tr style=""height: 50px; background-color: burlywood;"">
                <th colspan=""2"">Other Details</th>
            </tr>
            <tr>
                <th style=""text-align: left;"">Job Description</th>
                <td><pre>{21}</pre></td>
            </tr>
            <tr>
                <th style=""text-align: left;"">Remarks</th>
                <td><pre>{22}</pre></td>
            </tr>
        </table>
    </div>", pODetail.CandidateName,
               pODetail.PODate,
               pODetail.RefOfCandidate,
               db.UserAccountDetails.Find(pODetail.RefSaleId).FullName,
               pODetail.JobType,
               pODetail.AppliedPosition,
               pODetail.InterviewSupport, pODetail.JobLocation, pODetail.EndClient, pODetail.VendorDetails, pODetail.Rate,
               pODetail.SignUpDate, pODetail.TrainerName,
               pODetail.TrainingStartDate, pODetail.TrainingEndDate,
               pODetail.TeamLeadName, pODetail.RecruiterName,
               pODetail.MarketingStartDate, pODetail.MarketingEndDate,
               pODetail.AgreementMonths, pODetail.AgreementPercentage,
               pODetail.JobDescription, pODetail.OtherDetails);



                SMTPEmailSendingModel.Send(user.EmailId, emailBody, "PO detail Form of (" + pODetail.CandidateName + ")", tlemailid, "bhadresh@dashtechinc.com");

                SMTPEmailSendingModel.Send(sendConfirmation, "Hi,<br>Confirmation for this PO is Pending in CRM.", "Pending PO Confirmation (" + pODetail.CandidateName + ")", "", "bhadresh@dashtechinc.com");


                FollowUpMaster follow = new FollowUpMaster()
                {
                    Department = "Marketing",
                    FollowUpBy = user.RocketName,
                    FollowUpDate = DateTime.Now.Date,
                    FollowUpMessage = "PO detail filled for (" + pODetail.CandidateName + ")",
                    FollowUpStatus = "Placed",
                    FollowUpTime = DateTime.Now.TimeOfDay,
                    RefCandidateId = (int)pODetail.CandidateId
                };
                db.FollowUpMasters.Add(follow);
                db.SaveChanges();
                TempData["alert"] = new AlertBoxModel() { Type = "Success", Message = "PO Email Sent!" };

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["alert"] = new AlertBoxModel() { Type = "Error", Message = ex.Message };

            }

            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName", pODetail.RefSaleId);
            return View(pODetail);
        }

        // GET: MarketingManager/PODetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PODetail pODetail = db.PODetails.Find(id);
            if (pODetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName", pODetail.RefSaleId);
            return View(pODetail);
        }

        // POST: MarketingManager/PODetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "POId,CandidateName,RefOfCandidate,RefSaleId,JobType,AppliedPosition,InterviewSupport,JobLocation,EndClient,VendorDetails,RecruiterName,TeamLeadName,Rate,SignUpDate,TrainerName,TrainingStartDate,TrainingEndDate,MarketingStartDate,MarketingEndDate,AgreementPercentage,AgreementMonths,JobDescription,OtherDetails,AddBy,EnteryDate,UpdateDate,UpdateBy,IsDeleted,PODate,JoiningDate,CandidateId")] PODetail pODetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pODetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefSaleId = new SelectList(db.UserAccountDetails, "UserId", "FullName", pODetail.RefSaleId);
            return View(pODetail);
        }

        // GET: MarketingManager/PODetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PODetail pODetail = db.PODetails.Find(id);
            if (pODetail == null)
            {
                return HttpNotFound();
            }
            return View(pODetail);
        }

        // POST: MarketingManager/PODetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PODetail pODetail = db.PODetails.Find(id);
            db.PODetails.Remove(pODetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
