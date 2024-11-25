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
    public class ApartmentController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Apartment
        public ActionResult Index()
        {
            var apartments = db.apartments.Include(a => a.building).Include(a => a.tenant);
            return View(apartments.ToList());
        }

        // GET: Apartment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            apartment apartment = db.apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Apartment/Create -> only for manager
        public ActionResult Create()
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address");
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name");
            return View();
        }

        // POST: Apartment/Create -> only for manager
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "apartmentId,apartmentNo,nbRooms,price,status,buildingId,tenantId")] apartment apartment)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.apartments.Add(apartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", apartment.buildingId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
            return View(apartment);
        }

        // GET: Apartment/Edit/5 -> only for manager
        public ActionResult Edit(int? id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            apartment apartment = db.apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", apartment.buildingId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
            return View(apartment);
        }

        // POST: Apartment/Edit/5 -> only for manager
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "apartmentId,apartmentNo,nbRooms,price,status,buildingId,tenantId")] apartment apartment)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.Entry(apartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", apartment.buildingId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
            return View(apartment);
        }

        // GET: Apartment/Delete/5 -> only for manager
        public ActionResult Delete(int? id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            apartment apartment = db.apartments.Find(id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            apartment apartment = db.apartments.Find(id);
            db.apartments.Remove(apartment);
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
