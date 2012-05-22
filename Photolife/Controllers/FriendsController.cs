using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Photolife.Models;
using System.Web.Security;

namespace PZ.Controllers
{ 
    public class FriendsController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        //
        // GET: /Friends/

        public ViewResult Index()
        {
            var friendships = db.Friendships.Where(o => o.User == User.Identity.Name);
            List<string> friends = new List<string>();
            foreach (var invitation in friendships)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() > 0)
                    friends.Add(invitation.User);
            }
            ViewBag.Friends = friends;

            var invitations = db.Friendships.Where(o => o.UserFriend == User.Identity.Name);
            List<string> invites = new List<string>();
            foreach (var invitation in invitations)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() > 0) ;
                else invites.Add(invitation.User);
            }
            ViewBag.Invites = invites;

            var urinvitations = db.Friendships.Where(o => o.User == User.Identity.Name);
            List<string> urinvites = new List<string>();
            foreach (var invitation in urinvitations)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() > 0) ;
                else urinvites.Add(invitation.User);
            }
            ViewBag.UrInvites = urinvites;
            return View();
        }

        
        //
        // GET: /Friends/Create

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
            ViewBag.UserFriend = usernames;
            return View();
        } 

        //
        // POST: /Friends/Create

        [HttpPost]
        public ActionResult Create(Friendship friendship)
        {
            if (ModelState.IsValid)
            {
                friendship.User = User.Identity.Name;
                db.Friendships.Add(friendship);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(friendship);
        }

        //public ActionResult Create(string name)
        //{
        //    if (Membership.FindUsersByName(name) == null) return View();
        //    Friendship friendship = new Friendship();
        //    friendship.User = User.Identity.Name;
        //    friendship.UserFriend = name;
        //    db.Friendships.Add(friendship);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");  
        //}

        //
        // GET: /Friends/Delete/5
 
        public ActionResult Delete(int id)
        {
            Friendship friendship = db.Friendships.Find(id);
            return View(friendship);
        }

        //
        // POST: /Friends/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Friendship friendship = db.Friendships.Find(id);
            db.Friendships.Remove(friendship);
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