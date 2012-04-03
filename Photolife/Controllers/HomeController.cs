using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Photolife.Controllers
{
    public class HomeController : Controller
    {
        //Photolife.Models.PhotolifeEntities storeDB = new Photolife.Models.PhotolifeEntities();
        public ActionResult Index()
        {
            ViewBag.Message = "!PHOTOBLOG!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize(Roles="Administrator")]
        public string AdminPage()
        {
            return "Coś co powinien zobacyzć tylko i wyłącznie admin.";
        }
    }
}
