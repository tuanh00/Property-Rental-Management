﻿using System;
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
    public class OwnerAccessController : Controller
    {
        //private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: OwnerAccess
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(owner log)
        {
            // Hash the entered password
            string hashedPassword = ComputeSha256Hash(log.password);

            using (var context = new DbPropertyRentalEntities())
            {
                bool isValid = context.owners.Any(x => x.email == log.email && x.password == hashedPassword);
                if (isValid)
                {
                    Session["owner"] = context.owners.SingleOrDefault(i => i.email == log.email).ownerId;
                    Session["manager"] = null;
                    Session["tenant"] = null;
                    return RedirectToAction("Index", "Owner");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                    return View();
                }
            }
        }
        // Helper method to compute SHA256 hash
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array
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
