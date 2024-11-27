using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class TenantController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Tenant
        public ActionResult Index()
        {
            // Redirect to Home if no owner or tenant session is active
            if (Session["owner"] == null && Session["tenant"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // If owner is logged in, show all tenants
            if (Session["owner"] != null)
            {
                return View(db.tenants.ToList());
            }

            // If a tenant is logged in, show only their details
            if (Session["tenant"] != null)
            {
                int tenantId = Convert.ToInt32(Session["tenant"]);
                var tenantInfo = db.tenants.Where(t => t.tenantId == tenantId).ToList();
                return View(tenantInfo); // Pass only the tenant's info to the view
            }

            return RedirectToAction("Index", "Home"); // Fallback
        }

        // GET: Tenant/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["owner"] == null && Session["tenant"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            tenant tenant = db.tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            if (Session["tenant"] != null && (int)Session["tenant"] != tenant.tenantId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(tenant);
        }

        // GET: Tenant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tenant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tenantId,name,email,password,phoneNumber")] tenant tenant)
        {
            // Check if the email already exists in the database
            if (db.tenants.Any(t => t.email == tenant.email))
            {
                // Add a custom error message to the ModelState
                ModelState.AddModelError("email", "This email is already in use. Please use a different email.");
            }

            // Proceed only if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Hash the password
                tenant.password = ComputeSha256Hash(tenant.password);

                // Save the tenant to the database
                db.tenants.Add(tenant);
                db.SaveChanges();

                // Redirect to the Tenant Access page or any other appropriate action
                return RedirectToAction("Index", "TenantAccess");
            }

            // If validation fails, return to the Create view with the current data
            return View(tenant);
        }

        // GET: Tenant/Edit/5 -> wrap tenant info sent to the Edit view through ID
        public ActionResult Edit(int? id) 
        {
            if (Session["owner"] == null && Session["tenant"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            tenant tenant = db.tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }

            // Restrict tenants from editing other tenants
            if (Session["tenant"] != null && (int)Session["tenant"] != tenant.tenantId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(tenant);
        }

        // POST: Tenant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tenantId,name,email,password,phoneNumber")] tenant tenant)
        {
            // Step 1: Check if the user is authorized (either owner or the logged-in tenant)
            if (Session["owner"] == null && Session["tenant"] == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect unauthorized users to Home
            }

            // Step 2: Restrict tenants from editing other tenant accounts
            if (Session["tenant"] != null && (int)Session["tenant"] != tenant.tenantId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // Return 403 Forbidden for unauthorized access
            }

            // Step 3: Fetch the existing tenant record from the database
            var existingTenant = db.tenants.Find(tenant.tenantId);
            if (existingTenant == null)
            {
                return HttpNotFound(); // Return 404 Not Found if the tenant does not exist
            }

            // Step 4: Validate email uniqueness (excluding the current tenant's email)
            if (db.tenants.Any(t => t.email == tenant.email && t.tenantId != tenant.tenantId))
            {
                // Add an error message if the email is already in use by another tenant
                ModelState.AddModelError("email", "This email is already in use by another tenant. Please use a different email.");
            }

            // Step 5: Skip email validation if the email has not changed
            if (tenant.email == existingTenant.email)
            {
                ModelState.Remove("email"); // Remove validation error if the email hasn't changed
            }

            // Step 6: Retain the existing password if the new password field is left blank
            if (string.IsNullOrWhiteSpace(Request.Form["password"]))
            {
                ModelState.Remove("password"); // Remove password validation for blank passwords
                tenant.password = existingTenant.password; // Keep the existing password
            }
            else
            {
                // Hash the new password and set it
                tenant.password = ComputeSha256Hash(Request.Form["password"]);
            }

            // Step 7: Proceed with saving changes only if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Update tenant details with new values
                existingTenant.name = tenant.name;
                existingTenant.email = tenant.email;
                existingTenant.phoneNumber = tenant.phoneNumber;
                existingTenant.password = tenant.password;

                try
                {
                    // Step 8: Mark the entity as modified and save changes
                    db.Entry(existingTenant).State = EntityState.Modified;
                    db.SaveChanges(); // Persist changes to the database

                    // Step 9: Redirect back to the Index page after a successful save
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException)
                {
                    // Step 10: Add a user-friendly error message if something goes wrong during the save
                    ModelState.AddModelError("", "An error occurred while saving changes. Please try again.");
                }
            }

            // Step 11: If validation fails, return to the Edit view with the current data
            return View(tenant);
        }

        // GET: Tenant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tenant tenant = db.tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }

            // Tenants can only delete their own account
            if (Session["tenant"] != null && (int)Session["tenant"] != tenant.tenantId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(tenant); // Render a confirmation view for deletion
        }

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tenant tenant = db.tenants.Find(id);

            if (tenant == null)
            {
                return HttpNotFound();
            }

            // Owner can delete any tenant account
            if (Session["owner"] != null)
            {
                db.tenants.Remove(tenant);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect owner to tenant list
            }

            // Tenant can delete only their own account
            if (Session["tenant"] != null)
            {
                if ((int)Session["tenant"] == id)
                {
                    db.tenants.Remove(tenant);
                    db.SaveChanges();
                    Session.Clear(); // Clear tenant session
                    return RedirectToAction("Index", "Home"); // Redirect to homepage
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // Unauthorized access
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
