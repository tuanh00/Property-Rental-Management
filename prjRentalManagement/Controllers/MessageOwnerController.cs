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
        public ActionResult Edit([Bind(Include = "messageId,managerId,tenantId,message,responseMessage")] messageManager messageManager)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate the existence of the record in the database
                    var existingMessage = db.messageManagers
                        .Include(m => m.tenant)
                        .Include(m => m.manager)
                        .FirstOrDefault(m => m.messageId == messageManager.messageId);

                    if (existingMessage == null)
                    {
                        ModelState.AddModelError("", "The messageManager record was not found.");
                        return View(messageManager);
                    }

                    // Validate foreign key relationships
                    var tenantExists = db.tenants.Any(t => t.tenantId == existingMessage.tenantId);
                    var managerExists = db.managers.Any(m => m.managerId == existingMessage.managerId);

                    if (!tenantExists)
                    {
                        ModelState.AddModelError("", "The associated tenant does not exist.");
                        return View(messageManager);
                    }

                    if (!managerExists)
                    {
                        ModelState.AddModelError("", "The associated manager does not exist.");
                        return View(messageManager);
                    }

                    // Update only the responseMessage field
                    existingMessage.responseMessage = messageManager.responseMessage;

                    // Save changes to the database
                    db.Entry(existingMessage).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {
                    ModelState.AddModelError("", "Validation failed. Please check the inputs.");
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    ModelState.AddModelError("", "Database error occurred. Please ensure all foreign keys are valid.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Rebind dropdowns in case of error
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageManager.managerId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", messageManager.tenantId);
            return View(messageManager);
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
