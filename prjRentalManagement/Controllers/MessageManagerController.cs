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
    public class MessageManagerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: MessageManager
        public ActionResult Index()
        {
            var messageManagers = db.messageManagers.Include(m => m.manager).Include(m => m.tenant);
            return View(messageManagers.ToList());
        }

        // GET: MessageManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageManager messageManager = db.messageManagers.Find(id);
            if (messageManager == null)
            {
                return HttpNotFound();
            }
            return View(messageManager);
        }

        // GET: MessageManager/Create
        public ActionResult Create()
        {
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name");
            return View();
        }

        // POST: MessageManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageId,managerId,tenantId,message")] messageManager messageManager)
        {
            if (ModelState.IsValid)
            {
                db.messageManagers.Add(messageManager);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageManager.managerId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", messageManager.tenantId);
            return View(messageManager);
        }

        // GET: MessageManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageManager messageManager = db.messageManagers.Find(id);
            if (messageManager == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageManager.managerId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", messageManager.tenantId);
            return View(messageManager);
        }

        // POST: MessageManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "messageId,managerId,tenantId,message")] messageManager messageManager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(messageManager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageManager.managerId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", messageManager.tenantId);
            return View(messageManager);
        }

        // GET: MessageManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messageManager messageManager = db.messageManagers.Find(id);
            if (messageManager == null)
            {
                return HttpNotFound();
            }
            return View(messageManager);
        }

        // POST: MessageManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            messageManager messageManager = db.messageManagers.Find(id);
            db.messageManagers.Remove(messageManager);
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
