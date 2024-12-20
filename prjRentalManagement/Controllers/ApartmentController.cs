﻿using System;
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
        public ActionResult Index(string search)
        {
            // Ensure a session is active for tenants, managers, or owners
            if (Session["manager"] == null && Session["tenant"] == null && Session["owner"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<apartment> apartments = db.apartments
                .Include(a => a.building)
                .Include(a => a.tenant);

            if (Session["manager"] != null)
            {
                // Manager: Only view apartments in their assigned buildings
                int managerId = Convert.ToInt32(Session["manager"]);
                apartments = apartments.Where(a => a.building.managerId == managerId);
            }
            else if (Session["tenant"] != null)
            {
                // Tenant: View "Available" apartments and those assigned to them
                int tenantId = Convert.ToInt32(Session["tenant"]);
                apartments = apartments.Where(a => (a.status == "Available" || a.tenantId == tenantId));
            }
            else if (Session["owner"] != null)
            {
                int ownerId = Convert.ToInt32(Session["owner"]);

                // Filter apartments based on buildings owned by the owner
                apartments = apartments.Where(a => a.building.ownerId == ownerId);
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                apartments = apartments.Where(a =>
                    a.apartmentNo.ToString().Contains(search) ||
                    a.building.address.Contains(search) ||
                    a.building.city.Contains(search) ||
                    a.building.province.Contains(search) ||
                    a.building.postalCode.Contains(search) ||
                    a.nbRooms.ToString().Contains(search) ||
                    a.price.ToString().Contains(search) ||
                    a.status.Contains(search));
            }

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
                return RedirectToAction("Index", "Home");
            }

            int managerId = Convert.ToInt32(Session["manager"]);

            // Fetch buildings managed by the logged-in manager
            ViewBag.buildingId = new SelectList(
                db.buildings.Where(b => b.managerId == managerId),
                "buildingId",
                "address"
            );
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
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.apartments.Add(apartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int managerId = Convert.ToInt32(Session["manager"]);
            ViewBag.buildingId = new SelectList(
                db.buildings.Where(b => b.managerId == managerId),
                "buildingId",
                "address",
                apartment.buildingId
            );
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
            return View(apartment);
        }

        // GET: Apartment/Edit/5 -> only for manager
        public ActionResult Edit(int? id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
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

            int managerId = Convert.ToInt32(Session["manager"]);

            // Ensure that the manager can only edit apartments within their managed buildings
            if (apartment.building.managerId != managerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.buildingId = new SelectList(
                db.buildings.Where(b => b.managerId == managerId),
                "buildingId",
                "address",
                apartment.buildingId
            );
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
            if (apartment == null)
            {
                return HttpNotFound();
            }

            // Check for related data in dependent tables
            bool hasEvents = db.eventOwners.Any(e => e.apartmentId == id);

            // If any related data exists, return an error message to the user
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "This apartment cannot be deleted because there are related events in the system. Please delete related events first.";
                return RedirectToAction("Delete", new { id });
            }

            // Proceed to delete the apartment if no related records exist
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
