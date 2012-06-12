using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Photolife.Models;


namespace Photolife
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            MySeed(new PhotolifeEntities());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public void MySeed(PhotolifeEntities db)
        {
            var users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
                Membership.DeleteUser(user.UserName);

            List<UserSeedData> uss = new List<UserSeedData>();
            for (int i = 0; i < 10; ++i)
                uss.Add(new UserSeedData { Login = "test"+i, Email = "test"+i+"@gmail.com", FirstName = "imie"+i, LastName = "nazwisko"+i });
            foreach (UserSeedData us in uss)
            {
                us.Email = us.Email.ToLower();
                us.Login = us.Login.ToLower();


                // Attempt to register the user
                MembershipCreateStatus createStatus;
                MembershipUser user = Membership.CreateUser(us.Login, "qwe123", us.Email, null, null, true, null, out createStatus);

                if (Roles.RoleExists("User") == false)
                    Roles.CreateRole("User");
                if (Roles.IsUserInRole(us.Login, "User") == false)
                    Roles.AddUserToRole(us.Login, "User");

                UserData ud = new UserData();
                ud.MembershipUserID = (Guid)user.ProviderUserKey;
                ud.FirstName = us.FirstName;
                ud.LastName = us.LastName;
                db.UserDatas.Add(ud);
                db.SaveChanges();

                for (int j = 0; j < 50; j++)
                {
                    Photo photo = new Photo();
                    photo.prefix = "example";
                    photo.MembershipUserID = (Guid) user.ProviderUserKey;
                    db.Photos.Add(photo);
                    db.SaveChanges();
                }
            }

        }
    }

    public class UserSeedData
    {
        public string Login;
        public string Email;
        public string FirstName;
        public string LastName;
    }
}