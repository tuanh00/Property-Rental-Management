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
            if (ModelState.IsValid)
            {
                //Hash the password 
                owner.password = ComputeSha256Hash(owner.password);

                db.owners.Add(owner);
                db.SaveChanges();
                return RedirectToAction("Index", "OwnerAccess");
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ownerId,name,email,password,phoneNumber")] owner owner)
        {
            if (Session["owner"] == null || (int)Session["owner"] != owner.ownerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            if (ModelState.IsValid)
            {
                // Hash the password if it's changed
                owner.password = ComputeSha256Hash(owner.password);
                db.Entry(owner).State = EntityState.Modified;
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
            owner owner = db.owners.Find(id);
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
