using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            if (Session["manager"] != null)
            {
                int managerId = Convert.ToInt32(Session["manager"]);
                var messageManagers = db.messageManagers
                    .Include(m => m.manager)
                    .Include(m => m.tenant)
                    .Where(m => m.managerId == managerId);
                return View(messageManagers.ToList());
            }

            if (Session["tenant"] != null)
            {
                int tenantId = Convert.ToInt32(Session["tenant"]);
                var messageManagers = db.messageManagers
                    .Include(m => m.manager)
                    .Include(m => m.tenant)
                    .Where(m => m.tenantId == tenantId);
                return View(messageManagers.ToList());
            }

            return RedirectToAction("Index", "Home");
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

        // GET: MessageManager/Create -> Only tenants can create messages

        public ActionResult Create()
        {
            if (Session["tenant"] != null)
            {
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
                return View();
            }

            return RedirectToAction("Index", "Home"); // Redirect if not a tenant
        }

        // POST: MessageManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageId,managerId,tenantId,message")] messageManager messageManager)
        {
            if (Session["tenant"] != null)
            {
                // Automatically assign the tenantId from the session
                messageManager.tenantId = Convert.ToInt32(Session["tenant"]);

                if (ModelState.IsValid)
                {
                    messageManager.responseMessage = null; // Manager hasn't responded yet
                    db.messageManagers.Add(messageManager);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageManager.managerId);
                return View(messageManager);
            }

            return RedirectToAction("Index", "Home"); // Redirect if not a tenant
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
        public ActionResult Edit([Bind(Include = "messageId,managerId,tenantId,message,responseMessage")] messageManager messageManager)
        {
            try
            {
                if (messageManager == null)
                {
                    throw new ArgumentNullException(nameof(messageManager), "messageManager object is null.");
                }

                // Retrieve the existing message from the database
                var existingMessage = db.messageManagers.Find(messageManager.messageId);
                if (existingMessage == null)
                {
                    ModelState.AddModelError("", "The messageManager record was not found.");
                    return View(messageManager);
                }

                // Check the session to determine the role
                if (Session["tenant"] != null)
                {
                    // Tenant updating their message
                    existingMessage.message = messageManager.message; // Update the message
                    existingMessage.responseMessage = null; // Reset responseMessage
                }
                else if (Session["manager"] != null)
                {
                    // Manager updating their response
                    existingMessage.responseMessage = messageManager.responseMessage; // Update responseMessage
                }
                else
                {
                    // Invalid session
                    return RedirectToAction("Index", "Home");
                }

                db.Entry(existingMessage).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the message. Please try again.");
            }

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
