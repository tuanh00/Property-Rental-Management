﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class OwnerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Owner
        public ActionResult Index()
        {
            return View(db.owners.ToList());
        }

        // GET: Owner/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "OwnerAccess");
            }
            if (id == null || (int)Session["owner"] != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            owner owner = db.owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // GET: Owner/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Owner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ownerId,name,email,password,phoneNumber")] owner owner)
        {
            // Check if the email already exists in the database
            if (db.owners.Any(o => o.email == owner.email))
            {
                // Add a custom error message to the ModelState
                ModelState.AddModelError("email", "This email is already in use. Please use a different email.");
            }

            // Proceed only if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Hash the password
                owner.password = ComputeSha256Hash(owner.password);

                // Save the owner to the database
                db.owners.Add(owner);
                db.SaveChanges();

                // Redirect to the Owner Access page or any other appropriate action
                return RedirectToAction("Index", "OwnerAccess");
            }

            // If validation fails, return to the Create view with the current data
            return View(owner);

        }

        // GET: Owner/Edit/5 -> Only the logged-in owner can edit/delete their account.
        public ActionResult Edit(int? id)
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "OwnerAccess");
            }
            if (id == null || (int)Session["owner"] != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            owner owner = db.owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // POST: Owner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Owner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ownerId,name,email,password,phoneNumber")] owner owner)
        {
            if (Session["owner"] == null || (int)Session["owner"] != owner.ownerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Fetch the existing owner record from the database
            var existingOwner = db.owners.Find(owner.ownerId);
            if (existingOwner == null)
            {
                return HttpNotFound();
            }

            // Check if the email is already used by another owner
            if (db.owners.Any(o => o.email == owner.email && o.ownerId != owner.ownerId))
            {
                ModelState.AddModelError("email", "This email is already in use by another owner. Please use a different email.");
            }

            // Update non-password fields (name, email, phoneNumber)
            existingOwner.name = owner.name;
            existingOwner.phoneNumber = owner.phoneNumber;

            // Check if a new password has been provided
            if (string.IsNullOrWhiteSpace(Request.Form["password"]))
            {
                // Retain the existing password if no new password is entered
                ModelState.Remove("password"); // Remove validation error for password
            }
            else
            {
                // Hash the new password and update
                string newPassword = Request.Form["password"];
                existingOwner.password = ComputeSha256Hash(newPassword);
            }

            // Save changes to the database
            if (ModelState.IsValid)
            {
                // Update the email only if ModelState is valid
                existingOwner.email = owner.email;

                db.Entry(existingOwner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(owner);
        }

        // GET: Owner/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "OwnerAccess");
            }
            if (id == null || (int)Session["owner"] != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            owner owner = db.owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // POST: Owner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Find the owner
            owner owner = db.owners.Find(id);
            if (owner == null)
            {
                return HttpNotFound();
            }

            // Check for related data in dependent tables
            bool hasBuildings = db.buildings.Any(b => b.ownerId == id);
            bool hasEvents = db.eventOwners.Any(e => e.ownerId == id);
            bool hasMessages = db.messageOwners.Any(m => m.ownerId == id);

            // If any related data exists, return an error message to the user
            if (hasBuildings || hasEvents || hasMessages)
            {
                TempData["ErrorMessage"] = "This owner cannot be deleted because there are related records in the system. Please delete related buildings, events, or messages first.";
                return RedirectToAction("Delete", new { id });
            }

            // Proceed to delete the owner if no related records exist
            db.owners.Remove(owner);
            db.SaveChanges();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Helper method to compute SHA256 hash
        private string ComputeSha256Hash(String rawData)
        {
            //Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
