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
    public class ManagerAccessController : Controller
    {
        // GET: ManagerAccess
        public ActionResult Index()
        {
            return View();
        }

        // Manager Login
        [HttpPost]
        public ActionResult Index(manager log)
        {
            // Hash the entered password
            string hashedPassword = ComputeSha256Hash(log.password);

            using (var context = new DbPropertyRentalEntities())
            {
                var storedManager = context.managers.SingleOrDefault(x => x.email == log.email);

                if (storedManager != null)
                {
                    // Compare stored hash with hashed input
                    if (storedManager.password == hashedPassword)
                    {
                        Session["manager"] = storedManager.managerId;
                        Session["owner"] = null;
                        Session["tenant"] = null;
                        return RedirectToAction("Index", "Manager");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                return View();
            }
        }

        // Helper method to compute SHA256 hash
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
