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
    public class EventOwnerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: EventOwner
        public ActionResult Index()
        {
            if (Session["manager"] == null && Session["owner"] == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect unauthorized users
            }

            IEnumerable<eventOwner> eventOwners;

            // Manager can view all events they created
            if (Session["manager"] != null)
            {
                int managerId = Convert.ToInt32(Session["manager"]);
                eventOwners = db.eventOwners
                                .Include(e => e.apartment)
                                .Include(e => e.manager)
                                .Include(e => e.owner)
                                .Where(e => e.managerId == managerId);
            }
            // Owner can view only events assigned to them
            else
            {
                int ownerId = Convert.ToInt32(Session["owner"]);
                eventOwners = db.eventOwners
                                .Include(e => e.apartment)
                                .Include(e => e.manager)
                                .Include(e => e.owner)
                                .Where(e => e.ownerId == ownerId);
            }

            return View(eventOwners.ToList());
        }

        // GET: EventOwner/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventOwner = db.eventOwners
                               .Include(e => e.apartment)
                               .Include(e => e.manager)
                               .Include(e => e.owner)
                               .FirstOrDefault(e => e.eventId == id);

            if (eventOwner == null)
            {
                return HttpNotFound();
            }

            // Allow both manager and owner to view details
            if (Session["manager"] != null || (Session["owner"] != null && eventOwner.ownerId == Convert.ToInt32(Session["owner"])))
            {
                return View(eventOwner);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // GET: EventOwner/Create
        public ActionResult Create(int? ownerId)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home"); // Only managers can create events
            }


            // Populate Owner List
            ViewBag.OwnerList = new SelectList(db.owners, "ownerId", "name");

            // Populate Apartment List based on selected owner
            if (ownerId.HasValue)
            {
                /*
                 - db.apartments: represents the Apartments table in the database
                        Each record corresponding to an apartment, which has fields such as: aprtmentId, apartmentNo, buildingId.
                 - db.buildings: represents the Buildings table in the database
                        Each record corresponding to a building, which has fields  such as: buildingId, ownerId

                How to build query:
                1. Outer Query - Apartments - db.apartments.Where(...)
                    This filters the db.apartments table to include only the apartments that meet a specific condition. The condition is defined inside the Where clause.

                2: Inner Query - Buildings - db.buildings.Any(b => b.ownerId == ownerId && b.buildingId == a.buildingId)
                        Any: This is a LINQ method that checks if at least one building satisfies the condition inside the lambda expression. If true, the apartment is included in the result.

                3. Condition Inside Any - b.ownerId == ownerId && b.buildingId == a.buildingId
                    This checks two things for every building in the db.buildings table:
                        3.1.     b.ownerId == ownerId:
                            -> Ensures that the building belongs to the selected owner (ownerId).
                            -> The ownerId is passed as a query parameter from the view when the dropdown changes.
                        3.2.     b.buildingId == a.buildingId:
                            -> Ensures that the building (b.buildingId) is the same building to which the apartment (a.buildingId) belongs.
                 */
                ViewBag.ApartmentList = new SelectList(
                    db.apartments.Where(a => db.buildings.Any(b => b.ownerId == ownerId && b.buildingId == a.buildingId)),
                    "apartmentId",
                    "apartmentNo"
                );
            }
            else
            {
                //Empty Apartment List for Initial Load:
                ViewBag.ApartmentList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        // POST: EventOwner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "eventId,managerId,ownerId,apartmentId,eventDate,description,status")] eventOwner eventOwner)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home"); // Only managers can create events
            }

            if (ModelState.IsValid)
            {
                eventOwner.managerId = Convert.ToInt32(Session["manager"]);
                eventOwner.status = "Pending"; // Default status
                eventOwner.eventDate = DateTime.Now; // Automatically set to the current time

                db.eventOwners.Add(eventOwner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // Repopulate dropdowns if ModelState is invalid
            ViewBag.OwnerList = new SelectList(db.owners, "ownerId", "name", eventOwner.ownerId);
            ViewBag.ApartmentList = new SelectList(db.apartments, "apartmentId", "apartmentNo", eventOwner.apartmentId);

            return View(eventOwner);
        }

        // GET: EventOwner/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            eventOwner eventOwner = db.eventOwners.Find(id);
            if (eventOwner == null)
            {
                return HttpNotFound();
            }

            // Check if the user is an owner or a manager
            bool isOwner = Session["owner"] != null;
            bool isManager = Session["manager"] != null;

            if (!isOwner && !isManager)
            {
                return RedirectToAction("Index", "Home"); // Unauthorized access
            }

            // Populate dropdowns for managers
            if (isManager)
            {
                ViewBag.ManagerList = new SelectList(db.managers, "managerId", "name", eventOwner.managerId);
                ViewBag.OwnerList = new SelectList(db.owners, "ownerId", "name", eventOwner.ownerId);
                ViewBag.ApartmentList = new SelectList(db.apartments, "apartmentId", "apartmentNo", eventOwner.apartmentId);
            }

            // Populate dropdown for owners (only for status)
            if (isOwner)
            {
                ViewBag.StatusList = new SelectList(new[] { "Pending", "Resolved" }, eventOwner.status);
            }

            return View(eventOwner);
        }

        // POST: EventOwner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "eventId,managerId,ownerId,apartmentId,eventDate,description,status")] eventOwner eventOwner)
        {
            if (Session["owner"] == null && Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var existingEvent = db.eventOwners.Find(eventOwner.eventId);
            if (existingEvent == null)
            {
                return HttpNotFound();
            }

            if (Session["owner"] != null)
            {
                if (existingEvent.ownerId != Convert.ToInt32(Session["owner"]))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Owners can only update the status
                existingEvent.status = eventOwner.status;
            }
            else if (Session["manager"] != null)
            {
                if (existingEvent.managerId != Convert.ToInt32(Session["manager"]))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Managers can only update the description
                existingEvent.description = eventOwner.description;
            }

            // Save changes
            db.Entry(existingEvent).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: EventOwner/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home"); // Only managers can delete events
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventOwner = db.eventOwners.Find(id);
            if (eventOwner == null)
            {
                return HttpNotFound();
            }

            return View(eventOwner);
        }

        // POST: EventOwner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["manager"] == null)
            {
                return RedirectToAction("Index", "Home"); // Only managers can delete events
            }

            var eventOwner = db.eventOwners.Find(id);
            db.eventOwners.Remove(eventOwner);
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
