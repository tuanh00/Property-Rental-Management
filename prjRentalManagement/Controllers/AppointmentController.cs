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
    public class AppointmentController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Appointment -> both Managers and Tenants should only see their assigned appointments.
        public ActionResult Index()
        {
            if (Session["manager"] != null)
            {
                int managerId = int.Parse(Session["manager"].ToString());
                var appointments = db.appointments
                    .Include(a => a.manager)
                    .Include(a => a.tenant)
                    .Where(a => a.managerId == managerId);
                return View(appointments.ToList());
            }

            if (Session["tenant"] != null)
            {
                int tenantId = int.Parse(Session["tenant"].ToString());
                var appointments = db.appointments
                    .Include(a => a.manager)
                    .Include(a => a.tenant)
                    .Where(a => a.tenantId == tenantId);
                return View(appointments.ToList());
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Appointment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            appointment appointment = db.appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointment/Create -> for Managers, display a ddl for tenants. for tenants, display a ddl for managers
        public ActionResult Create() 
        {
            if (Session["manager"] != null)
            {
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name");
                return View();
            }

            if (Session["tenant"] != null)
            {
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: Appointment/Create -> for Tenants and Managers(optional)
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "appointmentId,managerId,tenantId,appointmentDate")] appointment appointment)
        {
            if (Session["manager"] != null)
            {
                appointment.managerId = int.Parse(Session["manager"].ToString());
            }

            if (Session["tenant"] != null)
            {
                appointment.tenantId = int.Parse(Session["tenant"].ToString());
            }

            if (ModelState.IsValid)
            {
                db.appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (Session["manager"] != null)
            {
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", appointment.tenantId);
            }

            if (Session["tenant"] != null)
            {
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", appointment.managerId);
            }

            return View(appointment);
        }

        // GET: Appointment/Edit/5 -> only allow managers to edit their appointments and tenants to edit theirs.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            appointment appointment = null;

            if (Session["manager"] != null)
            {
                int managerId = int.Parse(Session["manager"].ToString());
                appointment = db.appointments
                    .Where(a => a.managerId == managerId && a.appointmentId == id)
                    .FirstOrDefault();

                if (appointment == null)
                {
                    return HttpNotFound();
                }

                // Populate tenants for the dropdown
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", appointment.tenantId);
            }
            else if (Session["tenant"] != null)
            {
                int tenantId = int.Parse(Session["tenant"].ToString());
                appointment = db.appointments
                    .Where(a => a.tenantId == tenantId && a.appointmentId == id)
                    .FirstOrDefault();

                if (appointment == null)
                {
                    return HttpNotFound();
                }

                // Populate managers for the dropdown (if needed)
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", appointment.managerId);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View(appointment);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "appointmentId,managerId,tenantId,appointmentDate")] appointment appointment)
        {
            if (Session["manager"] != null)
            {
                appointment.managerId = int.Parse(Session["manager"].ToString());
            }

            if (Session["tenant"] != null)
            {
                appointment.tenantId = int.Parse(Session["tenant"].ToString());
            }

            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Repopulate ViewBag for invalid ModelState
            if (Session["manager"] != null)
            {
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", appointment.tenantId);
            }
            else if (Session["tenant"] != null)
            {
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", appointment.managerId);
            }

            return View(appointment);
        }

        // GET: Appointment/Delete/5 -> Only for Manager can delete appointments assigned to them
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session["manager"] != null)
            {
                int managerId = int.Parse(Session["manager"].ToString());
                appointment appointment = db.appointments
                    .Where(a => a.managerId == managerId && a.appointmentId == id)
                    .FirstOrDefault();

                if (appointment == null)
                {
                    return HttpNotFound();
                }

                return View(appointment);
            }

            if (Session["tenant"] != null)
            {
                int tenantId = int.Parse(Session["tenant"].ToString());
                appointment appointment = db.appointments
                    .Where(a => a.tenantId == tenantId && a.appointmentId == id)
                    .FirstOrDefault();

                if (appointment == null)
                {
                    return HttpNotFound();
                }

                return View(appointment);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            appointment appointment = db.appointments.Find(id);

            if (Session["manager"] != null && appointment.managerId != int.Parse(Session["manager"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (Session["tenant"] != null && appointment.tenantId != int.Parse(Session["tenant"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            db.appointments.Remove(appointment);
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
