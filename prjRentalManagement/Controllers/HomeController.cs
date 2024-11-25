using prjRentalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjRentalManagement.Controllers
{
    public class HomeController : Controller
    {
        DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        public ActionResult Index(int? search)
        {
            //Clears all sessions when the Index action is called.
            Session["owner"] = null;
            Session["manager"] = null;
            Session["tenant"] = null;
            if(search.HasValue)
            {
                return View(db.apartments.Where(x => x.apartmentId == search).ToList());
            } 
            else
            {
                return View(db.apartments.ToList());
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}