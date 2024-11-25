using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class MessageOwnerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: MessageOwner
        public ActionResult Index()
        {
            var messageOwners = db.messageOwners.Include(m => m.manager).Include(m => m.owner);
            return View(messageOwners.ToList());
        }

        // GET: MessageOwner/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageOwner messageOwner = db.messageOwners.Find(id);
            if (messageOwner == null)
            {
                return HttpNotFound();
            }
            return View(messageOwner);
        }

        // GET: MessageOwner/Create
        public ActionResult Create()
        {
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name");
            return View();
        }

        // POST: MessageOwner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageId,ownerId,managerId,message")] messageOwner messageOwner)
        {
            if (ModelState.IsValid)
            {
                db.messageOwners.Add(messageOwner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageOwner.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", messageOwner.ownerId);
            return View(messageOwner);
        }

        // GET: MessageOwner/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageOwner messageOwner = db.messageOwners.Find(id);
            if (messageOwner == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageOwner.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", messageOwner.ownerId);
            return View(messageOwner);
        }

        // POST: MessageOwner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "messageId,ownerId,managerId,message")] messageOwner messageOwner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(messageOwner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageOwner.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", messageOwner.ownerId);
            return View(messageOwner);
        }

        // GET: MessageOwner/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageOwner messageOwner = db.messageOwners.Find(id);
            if (messageOwner == null)
            {
                return HttpNotFound();
            }
            return View(messageOwner);
        }

        // POST: MessageOwner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            messageOwner messageOwner = db.messageOwners.Find(id);
            db.messageOwners.Remove(messageOwner);
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
