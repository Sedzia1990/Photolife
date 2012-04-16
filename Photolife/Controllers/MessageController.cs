using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Photolife.Models;
using System.Web.Security;

namespace Photolife.Controllers
{
    public class MessageController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        //
        // GET: /Message/

        public ViewResult Index()
        {
            return View(db.Message.ToList());
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
            ViewBag.Usernames = new List<SelectListItem>();
            
            foreach (MembershipUser item in users)
            {
                ViewBag.Usernames.Add(new SelectListItem
                {
                    Text = item.UserName,
                    Value = item.UserName
                });
            }

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