using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Security;
using System.Web.Helpers;
using Photolife.Models;

namespace Photolife.Models
{
    public class SampleData : DropCreateDatabaseIfModelChanges<PhotolifeEntities>
    {
        public void CreateAccount(string username, string password, string email, string role, string pesel)
        {
            MembershipCreateStatus createStatus;

            Membership.CreateUser(username, password, email, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                Roles.AddUserToRole(username, role);
            }

            try
            {
                WebMail.SmtpServer = "poczta.o2.pl";
                WebMail.UserName = "o-0@o2.pl";
                WebMail.Password = "123456";
                WebMail.Send(
                        email,
                        "Rejestracja w systemie ewidencji pacjentów SĘP",
                        "Witaj!<br /><br />" +
                        "Właśnie założono ci konto w systemie ewidencji pacjentów SĘP.<br /><br />" +
                        "Login: " + pesel + "<br />" +
                        "Hasło: " + password + "<br /><br />" +
                        "Po zalogowaniu się w systemie możesz zmienić swoje hasło.<br /><br />" +
                        "http://localhost:13215/",
                        "o-0@o2.pl"
                    );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
        }

        protected override void Seed(PhotolifeEntities db)
        {
            /*  var users = Membership.GetAllUsers();
              foreach (MembershipUser user in users)
                  Membership.DeleteUser(user.UserName, true);
             * */
            List<UserSeedData> uss = new List<UserSeedData>();
            for (int i = 0; i < 10; ++i)
                uss.Add(new UserSeedData { Login = "test" + i, Email = "test" + i + "@gmail.com", FirstName = "imie" + i, LastName = "nazwisko" + i });
            bool ok = true;
            foreach (UserSeedData us in uss)
            {
                us.Email = us.Email.ToLower();
                us.Login = us.Login.ToLower();

                ok = true;

                if (Membership.FindUsersByEmail(us.Email).Count != 0)
                    ok = false;
                if (Membership.FindUsersByName(us.Login).Count != 0)
                    ok = false;

                if (ok == true)
                {
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