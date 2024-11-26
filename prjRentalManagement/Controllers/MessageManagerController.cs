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
                // Initialize responseMessage to null when a new msg is created
                messageManager.responseMessage = null;

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
        public ActionResult Edit([Bind(Include = "messageId,managerId,tenantId,message,responseMessage")] messageManager messageManager)
        {
            try
            {
                // Debug: Log incoming data
                System.Diagnostics.Debug.WriteLine($"Incoming MessageManager: messageId={messageManager.messageId}, tenantId={messageManager.tenantId}, managerId={messageManager.managerId}");
                System.Diagnostics.Debug.WriteLine($"Incoming ResponseMessage: {messageManager.responseMessage}");

                // 1. Validate `messageManager` is not null
                if (messageManager == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error: messageManager object is null.");
                    throw new ArgumentNullException(nameof(messageManager), "messageManager object is null.");
                }

                // Retrieve the original entity from the database
                var existingMessage = db.messageManagers
                    .Include(m => m.tenant)
                    .Include(m => m.manager)
                    .FirstOrDefault(m => m.messageId == messageManager.messageId);

                // Debug: Check if the entity exists in the database
                if (existingMessage == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error: messageManager entity not found in database.");
                    ModelState.AddModelError("", "The messageManager record was not found.");
                    return View(messageManager);
                }

                // Debug: Log database data
                System.Diagnostics.Debug.WriteLine($"Database MessageManager: messageId={existingMessage.messageId}, tenantId={existingMessage.tenantId}, managerId={existingMessage.managerId}");
                System.Diagnostics.Debug.WriteLine($"Database Message: {existingMessage.message}, ResponseMessage: {existingMessage.responseMessage}");

                // 2. Check foreign key relationships
                var tenantExists = db.tenants.Any(t => t.tenantId == existingMessage.tenantId);
                var managerExists = db.managers.Any(m => m.managerId == existingMessage.managerId);

                // Debug: Log foreign key checks
                System.Diagnostics.Debug.WriteLine($"Tenant Exists: {tenantExists}");
                System.Diagnostics.Debug.WriteLine($"Manager Exists: {managerExists}");

                if (!tenantExists)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Tenant does not exist.");
                    ModelState.AddModelError("", "The associated tenant does not exist.");
                    return View(messageManager);
                }

                if (!managerExists)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Manager does not exist.");
                    ModelState.AddModelError("", "The associated manager does not exist.");
                    return View(messageManager);
                }

                // 3. Ensure only the `responseMessage` field is updated
                existingMessage.responseMessage = messageManager.responseMessage;

                // Update the entity in the database
                db.Entry(existingMessage).State = EntityState.Modified;
                db.SaveChanges();

                // Debug: Log success
                System.Diagnostics.Debug.WriteLine("MessageManager updated successfully.");
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                // Debug: Log detailed exception information
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}");
                var inner = ex.InnerException;
                while (inner != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {inner.Message}");
                    inner = inner.InnerException;
                }
                ModelState.AddModelError("", "An error occurred while updating the database. Please try again.");
            }
            catch (Exception ex)
            {
                // Debug: Log unexpected errors
                System.Diagnostics.Debug.WriteLine($"Unexpected Exception: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            // Reload dropdowns if any and return the view for correction
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
