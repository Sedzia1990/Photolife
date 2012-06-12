using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Photolife.Models;
using System.Data.Entity;


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
            Database.Delete("ApplicationServices");

            Database.SetInitializer<PhotolifeEntities>(null);
            MySeed(new PhotolifeEntities());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public void MySeed(PhotolifeEntities db)
        {
            var users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
                Membership.DeleteUser(user.UserName, true);

            var photos = db.Photos;

            foreach (Photo photo in photos)
                db.Photos.Remove(photo);

            db.SaveChanges();

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

                db.SaveChanges();
                UserData ud = new UserData();
                ud.MembershipUserID = (Guid)user.ProviderUserKey;
                ud.FirstName = us.FirstName;
                ud.LastName = us.LastName;
                
                db.UserDatas.Add(ud);
                db.SaveChanges();
                for (int j = 0; j < 10; j++)
                {
                    Photo photo = new Photo();
                    photo.prefix = "example";
                    photo.description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed dolor arcu, mollis vel commodo eget, rutrum ut felis. Nullam ultrices erat est, quis euismod lacus. Nunc vulputate, ligula sed feugiat laoreet, nunc nisl faucibus magna, vel commodo eros mauris vitae mi. Morbi at ligula magna, vel porttitor lectus. Pellentesque ultrices nibh ac sem pellentesque sit amet iaculis odio feugiat. Donec id purus nisl. Morbi massa sapien, pulvinar vitae dignissim vitae, ornare et massa. Morbi at diam tortor. Ut ultricies viverra purus, eu cursus sapien semper sed. \n\nAliquam pharetra, tellus at egestas fringilla, leo dui feugiat massa, vitae iaculis ante felis eget leo. Duis non risus dolor. Etiam vitae ante est. Nulla pulvinar, magna non bibendum mattis, nibh ligula porta massa, sit amet rutrum leo ligula et mi. Nulla facilisi. Morbi dapibus, nibh id placerat dapibus, mi lacus tincidunt ante, ut lobortis nisi sem euismod magna. Vestibulum ullamcorper tincidunt venenatis. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. In suscipit, nunc tristique tempor rutrum, est augue pulvinar turpis, at tincidunt turpis turpis eget velit. Aliquam erat volutpat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. In hendrerit porta mollis. ";
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