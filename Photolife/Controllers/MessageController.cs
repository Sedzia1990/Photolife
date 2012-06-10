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
            Guid user = (Guid)Membership.GetUser().ProviderUserKey;
            return View(db.Message.Where(o => o.Odbiorca == user));
        }

        public ViewResult OutBox()
        {
            Guid user = (Guid)Membership.GetUser().ProviderUserKey;
            return View(db.Message.Where(o => o.Nadawca == user));
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
        public ActionResult Create(AddingMessage message)
        {
            if (ModelState.IsValid)
            {
                Guid user = (Guid)Membership.GetUser().ProviderUserKey;
                Message m = new Message();
                m.Nadawca = user;
                m.Odbiorca = (Guid)Membership.GetUser(message.Odbiorca).ProviderUserKey;
                m.Treść = message.Treść;
                m.Tytuł = message.Tytuł;
                db.Message.Add(m);
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