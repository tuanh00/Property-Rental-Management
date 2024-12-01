using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace prjRentalManagement.Controllers
{
    public class ManagerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Manager -> Depends on Owner | Manager session
        public ActionResult Index(string searchQuery)
        { // Check if the user is an owner or manager
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // If the owner is logged in, allow filtering of managers by email
            if (Session["owner"] != null)
            {
                var managers = db.managers.AsQueryable();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    managers = managers.Where(m => m.email.Contains(searchQuery));
                }

                return View(managers.ToList());
            }

            // If a manager is logged in, show only their information
            if (Session["manager"] != null)
            {
                int managerId = Convert.ToInt32(Session["manager"]);
                var managerInfo = db.managers.Where(m => m.managerId == managerId).ToList();
                return View(managerInfo);
            }

            return RedirectToAction("Index", "Home"); // Fallback
        }

        // GET: Manager/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            manager manager = db.managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            // If manager is logged in, ensure they can only view their own details
            if (Session["manager"] != null && (int)Session["manager"] != manager.managerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View(manager);
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "managerId,name,email,password,phoneNumber")] manager manager)
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                //Hash the password 
                manager.password = ComputeSha256Hash(manager.password);

                db.managers.Add(manager);
                db.SaveChanges();
                return RedirectToAction("Index", "ManagerAccess");
            }

            return View(manager);
        }

        // GET: Manager/Edit/5 ->wrap manager info sent to the Edit view through ID
        public ActionResult Edit(int? id)
        {
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            manager manager = db.managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }

            return View(manager);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "managerId,name,email,password,phoneNumber")] manager manager)
        {
            // Step 1: Verify session validity for owner or manager
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Step 2: Restrict managers from editing other manager accounts
            if (Session["manager"] != null && (int)Session["manager"] != manager.managerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Step 3: Fetch the existing manager record from the database
            var existingManager = db.managers.Find(manager.managerId);
            if (existingManager == null)
            {
                return HttpNotFound(); // Return 404 Error page if the manager doesn't exist
            }

            // Step 4: Check if the email is already used by another manager
            if (db.managers.Any(m => m.email == manager.email && m.managerId != manager.managerId))
            {
                ModelState.AddModelError("email", "This email is already in use by another manager. Please use a different email.");
            }

            // Step 5: Update non-password fields (name, email, phoneNumber)
            existingManager.name = manager.name;
            existingManager.phoneNumber = manager.phoneNumber;

            // Step 6: Handle password updates or retention
            if (string.IsNullOrWhiteSpace(Request.Form["password"]))
            {
                // Retain the existing password if no new password is provided
                ModelState.Remove("password"); // Remove validation error for password
            }
            else
            {
                string newPassword = Request.Form["password"];
                existingManager.password = ComputeSha256Hash(newPassword);
            }

            // Step 7: Save changes to the database
            if (ModelState.IsValid)
            {
                // Update the email only if ModelState is valid
                existingManager.email = manager.email;

                db.Entry(existingManager).State = EntityState.Modified;
                db.SaveChanges();

                // Step 8: Redirect back to the Index page after a successful save
                return RedirectToAction("Index");
            }

            // Step 9: If validation fails, return to the Edit view with the current data
            return View(manager);
        }

        // GET: Manager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            manager manager = db.managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            // Managers can only delete their own account
            if (Session["manager"] != null)
            {
                if ((int)Session["manager"] != manager.managerId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // Unauthorized attempt
                }
            }

            // Owners can access any manager's delete view
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect if no session
            }

            return View(manager);
        }

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            manager manager = db.managers.Find(id);

           if (manager == null)
            {
                return HttpNotFound();
            }
            // Owner can delete any manager account
            if (Session["owner"] != null)
            {
                db.managers.Remove(manager);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect owner to manager list
            }
            // Manager can delete only their own account
            if (Session["manager"] != null)
            {
                if ((int)Session["manager"] == id)
                {
                    db.managers.Remove(manager);
                    db.SaveChanges();
                    Session["manager"] = null; // Clear session after self-deletion
                    return RedirectToAction("Login", "ManagerAccess"); // Redirect to login
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // Unauthorized attempt
                }
            }

            // If no valid session exists, redirect to home
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
