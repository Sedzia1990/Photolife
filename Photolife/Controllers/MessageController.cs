using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Photolife.Models;
using System.Web.Security;
using System.Threading;

namespace Photolife.Controllers
{
    [CustomAuthorize(Roles = "Administrator, User")]
    public class MessageController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        //
        // GET: /Message/

        public ViewResult Index()
        {
            return View();
        }
        
        public ViewResult InBox()
        {
            return View(db.Message.Where(o => o.Odbiorca == User.Identity.Name));
        }

        public ViewResult OutBox()
        {
            return View(db.Message.Where(o => o.Nadawca == User.Identity.Name));
        }

        //
        // GET: /Message/Details/5

        public ViewResult Details(int id)
        {
            Message message = db.Message.Find(id);
            return View(message);
        }

        //
        // GET: /Message/Create

        public ActionResult Create()
        {
            var users = Membership.GetAllUsers();
            List<SelectListItem> usernames = new List<SelectListItem>();
            
            foreach (MembershipUser item in users)
            {
                usernames.Add(new SelectListItem
                {
                    Text = item.UserName,
                    Value = item.UserName
                });
            }
            ViewBag.Odbiorca = usernames;

            return View();
        }

        //
        // POST: /Message/Create

        [HttpPost]
        public ActionResult Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.Nadawca = User.Identity.Name;
                db.Message.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(message);
        }

        //
        // GET: /Message/Edit/5

        public ActionResult Edit(int id)
        {
            Message message = db.Message.Find(id);
            return View(message);
        }

        //
        // POST: /Message/Edit/5

        [HttpPost]
        public ActionResult Edit(Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(message);
        }

        //
        // GET: /Message/Delete/5

        public ActionResult Delete(int id)
        {
            Message message = db.Message.Find(id);
            return View(message);
        }

        //
        // POST: /Message/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Message.Find(id);
            db.Message.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}