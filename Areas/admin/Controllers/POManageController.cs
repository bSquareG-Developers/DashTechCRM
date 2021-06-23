using DashTechCRM.Models;
using DashTechCRM.Models.User;
using System;
using System.Web.Mvc;

namespace DashTechCRM.Areas.admin.Controllers
{
    public class POManageController : Controller
    {
        dashTech_crm_Entities db = new dashTech_crm_Entities();
        // GET: admin/POManage
        public ActionResult Index()
        {
            return View();
        }

        #region Json Result
        public JsonResult getIndex(string s = null)
        {
            UserObject user = Session["userInfo"] as UserObject;

            UserAccountDetail current = db.UserAccountDetails.Find(user.UserId);

            dbConnectionModel directDB = new dbConnectionModel();
            if (s != null)
            {
                s = string.Format(" PODate < '{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }
            else
            {
                s = string.Format(" PODate = '{0}'", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }
            directDB.query = @"SELECT * FROM PODetails INNER JOIN UserAccountDetails ON UserId = PODETAILS.REFSALEID WHERE" + s + " and (ConfirmPO <> 'Deleted' or ConfirmPO is null)";
            var data = directDB.GetDictionary();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
