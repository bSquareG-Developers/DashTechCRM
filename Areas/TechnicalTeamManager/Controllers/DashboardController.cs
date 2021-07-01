using DashTechCRM.Models;
using DashTechCRM.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashTechCRM.Areas.TechnicalTeamManager.Controllers
{
    [FilterConfig._AuthenticationFilter]
    public class DashboardController : Controller
    {
        dashTech_crm_Entities db
             = new dashTech_crm_Entities();
        // GET: TechnicalTeamManager/Dashboard
        public ActionResult Index()
        {
            return View();
        }

        #region Json Results
        #region Get Candidates Result
        public JsonResult GetCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            directDb.query = string.Format(@"
select CandidateMaster.*, UserAccountDetails.*,CandidateMaster.Date as EnrolledDate, SalesServiceMaster.*, LocationMaster.*, TechnologyMaster.*
from CandidateMaster
inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId
inner join SalesServiceMaster on SalesServiceMaster.ServiceId = CandidateMaster.RefServiceId
inner join TechnologyMaster on TechId = TechnologyId
where CandidateMaster.Date < '{0}'
", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNewCandidateList()
        {
            UserObject user = Session["userInfo"] as UserObject;
            dbConnectionModel directDb = new dbConnectionModel();
            directDb.query = string.Format(@"
select CandidateMaster.*, UserAccountDetails.*,CandidateMaster.Date as EnrolledDate, SalesServiceMaster.*, LocationMaster.*, TechnologyMaster.*
from CandidateMaster
inner join UserAccountDetails on UserAccountDetails.UserId = CandidateMaster.RefSalesAssociate
inner join LocationMaster on LocationMaster.LocationId = UserAccountDetails.RefLocationId
inner join SalesServiceMaster on SalesServiceMaster.ServiceId = CandidateMaster.RefServiceId
inner join TechnologyMaster on TechId = TechnologyId
where CandidateMaster.Date = '{0}'
", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            var data = directDb.GetDictionary();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Followup save
        [HttpPost]
        public ActionResult SaveFollowup(string remarks, int candidateId, string status)
        {
            try
            {
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
            }
            catch (Exception ex)
            {


            }
            return RedirectToAction("Index");
        }
        #endregion
        #endregion
    }
}