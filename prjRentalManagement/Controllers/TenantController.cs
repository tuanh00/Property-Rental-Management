using System;
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
    public class TenantController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Tenant
        public ActionResult Index()
        {
            if (Session["owner"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.tenants.ToList());
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
            if (ModelState.IsValid)
            {
                //Hash the password 
                tenant.password = ComputeSha256Hash(tenant.password);

                db.tenants.Add(tenant);
                db.SaveChanges();
                return RedirectToAction("Index", "TenantAccess");
            }

            return View(tenant);
        }

        // GET: Tenant/Edit/5
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
            if (Session["owner"] == null && Session["tenant"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["tenant"] != null && (int)Session["tenant"] != tenant.tenantId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (ModelState.IsValid)
            {
                // Hash the password if it's changed
                tenant.password = ComputeSha256Hash(tenant.password);
                db.Entry(tenant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tenant);
        }

        // GET: Tenant/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tenant tenant = db.tenants.Find(id);
            db.tenants.Remove(tenant);
            db.SaveChanges();
            //If an owner deletes a tenant account, redirect back to the tenant list
            if (Session["owner"] != null)
            {
                return RedirectToAction("Index");
            }
            //if tenant delete their own accont, clear the session and redirect to Home
            else
            {
                Session.Clear();
                return RedirectToAction("Index", "Home");
            }
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
