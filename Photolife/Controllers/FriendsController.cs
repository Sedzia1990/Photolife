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
    public class FriendsController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        //
        // GET: /Friends/

        public ViewResult Index()
        {
            Guid user = (Guid)Membership.GetUser().ProviderUserKey;
            var friendships = db.Friendships.Where(o => o.User == user);
            List<string> friends = new List<string>();
            foreach (var invitation in friendships)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() > 0)
                    friends.Add(Membership.GetUser(invitation.UserFriend).UserName);
            }
            ViewBag.Friends = friends;

            var invitations = db.Friendships.Where(o => o.UserFriend == user);
            List<string> invites = new List<string>();
            foreach (var invitation in invitations)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() == 0)
                    invites.Add(Membership.GetUser(invitation.User).UserName);
            }
            ViewBag.Invites = invites;

            var urinvitations = db.Friendships.Where(o => o.User == user);
            List<string> urinvites = new List<string>();
            foreach (var invitation in urinvitations)
            {
                if (db.Friendships.Where(
                    o => o.User == invitation.UserFriend
                    && o.UserFriend == invitation.User).Count() == 0)
                    urinvites.Add(Membership.GetUser(invitation.UserFriend).UserName);
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
            Guid user = (Guid)Membership.GetUser().ProviderUserKey;
            foreach (MembershipUser item in users)
            {
                if (db.Friendships.Where(o => o.User == user
                    && o.UserFriend == (Guid)item.ProviderUserKey).Count() == 0)
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
        public ActionResult Create(AddingFriend friend)
        {
            if (Membership.GetUser(friend.UserFriend) != null)
            {
                Friendship fs = new Friendship();
                fs.User = (Guid)Membership.GetUser().ProviderUserKey;
                fs.UserFriend = (Guid)Membership.GetUser(friend.UserFriend).ProviderUserKey;

                db.Friendships.Add(fs);
                db.SaveChanges();
            }
            return RedirectToAction("Index");


            return View();
        }

        public ActionResult Accept(string name)
        {
            if (Membership.GetUser(name) == null) return View();
            Friendship friendship = new Friendship();
            friendship.User = (Guid)Membership.GetUser().ProviderUserKey;
            friendship.UserFriend = (Guid)Membership.GetUser(name).ProviderUserKey;
            db.Friendships.Add(friendship);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Friends/Delete/5
 
        public ActionResult Delete(int id)
        {
            Friendship friendship = db.Friendships.Find(id);
            return View(friendship);
        }

        public ActionResult DeleteByName(string name)
        {
            Guid user = (Guid)Membership.GetUser().ProviderUserKey;
            Guid userfriend = (Guid)Membership.GetUser(name).ProviderUserKey;
            var friendships = db.Friendships.Where(
                o => o.User == user &&
                    o.UserFriend == userfriend);

            if (friendships.Count() > 0)
            {
                Friendship fs = friendships.First();
                db.Friendships.Remove(fs);
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
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