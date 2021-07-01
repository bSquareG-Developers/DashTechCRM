using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DashTechCRM.Models;

namespace DashTechCRM.Areas.User.Controllers
{
    public class TeamDetailsController : Controller
    {
        private dashTech_crm_Entities db = new dashTech_crm_Entities();

        // GET: User/TeamDetails
        public ActionResult Index()
        {
            var teamDetails = db.TeamDetails.Include(t => t.UserAccountDetail).Include(t => t.UserAccountDetail1).Include(t => t.UserAccountDetail2);
            return View(teamDetails.ToList());
        }

        // GET: User/TeamDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamDetail teamDetail = db.TeamDetails.Find(id);
            if (teamDetail == null)
            {
                return HttpNotFound();
            }
            return View(teamDetail);
        }

        // GET: User/TeamDetails/Create
        public ActionResult Create()
        {
            ViewBag.Member = new SelectList(db.UserAccountDetails, "UserId", "FullName");
            ViewBag.TeamLead = new SelectList(db.UserAccountDetails, "UserId", "FullName");
            ViewBag.TeamManager = new SelectList(db.UserAccountDetails, "UserId", "FullName");
            return View();
        }

        // POST: User/TeamDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeamId,Member,TeamLead,TeamManager,Department,IsEnabledTeam")] TeamDetail teamDetail)
        {
            if (ModelState.IsValid)
            {
                db.TeamDetails.Add(teamDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Member = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.Member);
            ViewBag.TeamLead = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamLead);
            ViewBag.TeamManager = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamManager);
            return View(teamDetail);
        }

        // GET: User/TeamDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamDetail teamDetail = db.TeamDetails.Find(id);
            if (teamDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.Member = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.Member);
            ViewBag.TeamLead = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamLead);
            ViewBag.TeamManager = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamManager);
            return View(teamDetail);
        }

        // POST: User/TeamDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeamId,Member,TeamLead,TeamManager,Department,IsEnabledTeam")] TeamDetail teamDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Member = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.Member);
            ViewBag.TeamLead = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamLead);
            ViewBag.TeamManager = new SelectList(db.UserAccountDetails, "UserId", "FullName", teamDetail.TeamManager);
            return View(teamDetail);
        }

        // GET: User/TeamDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamDetail teamDetail = db.TeamDetails.Find(id);
            if (teamDetail == null)
            {
                return HttpNotFound();
            }
            return View(teamDetail);
        }

        // POST: User/TeamDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeamDetail teamDetail = db.TeamDetails.Find(id);
            db.TeamDetails.Remove(teamDetail);
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
