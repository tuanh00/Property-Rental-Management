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
            if (Session["manager"] != null)
            {
                int managerId = Convert.ToInt32(Session["manager"]);
                var messageOwners = db.messageOwners
                    .Include(m => m.manager)
                    .Include(m => m.owner)
                    .Where(m => m.managerId == managerId);
                return View(messageOwners.ToList());
            }

            if (Session["owner"] != null)
            {
                int ownerId = Convert.ToInt32(Session["owner"]);
                var messageOwners = db.messageOwners
                    .Include(m => m.manager)
                    .Include(m => m.owner)
                    .Where(m => m.ownerId == ownerId);
                return View(messageOwners.ToList());
            }

            return RedirectToAction("Index", "Home");
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
            if (Session["owner"] != null)
            {
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
                return View();
            }

            return RedirectToAction("Index", "Home"); // Redirect if not an owner
        }

        // POST: MessageOwner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageId,ownerId,managerId,message")] messageOwner messageOwner)
        {
            if (Session["owner"] != null)
            {
                // Automatically assign the ownerId from the session
                messageOwner.ownerId = Convert.ToInt32(Session["owner"]);

                if (ModelState.IsValid)
                {
                    db.messageOwners.Add(messageOwner);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", messageOwner.managerId);
                return View(messageOwner);
            }

            return RedirectToAction("Index", "Home"); // Redirect if not an owner
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
        public ActionResult Edit([Bind(Include = "messageId,managerId,ownerId,message,responseMessage")] messageOwner messageOwner)
        {
            try
            {
                if (messageOwner == null)
                {
                    throw new ArgumentNullException(nameof(messageOwner), "messageOwner object is null.");
                }

                // Retrieve the existing message from the database
                var existingMessage = db.messageOwners.Find(messageOwner.messageId);
                if (existingMessage == null)
                {
                    ModelState.AddModelError("", "The messageOwner record was not found.");
                    return View(messageOwner);
                }

                // Check the session to determine the role
                if (Session["owner"] != null)
                {
                    // Owner updating their message
                    existingMessage.message = messageOwner.message; // Update the message
                }
                else if (Session["manager"] != null)
                {
                    // Manager updating their response
                    existingMessage.responseMessage = messageOwner.responseMessage; // Update responseMessage
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
