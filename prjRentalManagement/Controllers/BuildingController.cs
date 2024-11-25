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
    public class BuildingController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Building
        public ActionResult Index()
        {
            var buildings = db.buildings.Include(b => b.manager).Include(b => b.owner);
            return View(buildings.ToList());
        }

        // GET: Building/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            building building = db.buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            return View(building);
        }

        // GET: Building/Create -> only for manager
        public ActionResult Create()
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name");
            return View();
        }

        // POST: Building/Create -> only for manager
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "buildingId,address,city,province,postalCode,ownerId,managerId")] building building)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.buildings.Add(building);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", building.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", building.ownerId);
            return View(building);
        }

        // GET: Building/Edit/5 -> only for manager
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
            building building = db.buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", building.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", building.ownerId);
            return View(building);
        }

        // POST: Building/Edit/5 -> only for manager
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "buildingId,address,city,province,postalCode,ownerId,managerId")] building building)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                db.Entry(building).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.managers, "managerId", "name", building.managerId);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", building.ownerId);
            return View(building);
        }

        // GET: Building/Delete/5 -> only for manager
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
            building building = db.buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            return View(building);
        }

        // POST: Building/Delete/5 -> only for manager
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index");
            }
            building building = db.buildings.Find(id);
            db.buildings.Remove(building);
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
