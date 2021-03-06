using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Photolife.Models;
using Microsoft.Web.Helpers;
using System.Web.Helpers;
using Facebook;
using System.Net;
using System.IO;
using System.Text;
using System.Data;

namespace Photolife.Controllers
{
    public class AccountController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        public ActionResult Index()
        {
            Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
            var UserData = db.UserDatas.First(o => o.MembershipUserID == userid);
            return View(UserData);
        }

        public ActionResult UserData(string user)
        {
            MembershipUser founduser = Membership.GetUser(user);
            if (founduser == null)
                return RedirectToAction("Index");
            ViewBag.user = founduser;
            var UserData = db.UserDatas.First(o => o.MembershipUserID == (Guid)founduser.ProviderUserKey);
            var userImage = db.Photos.Where(p => p.MembershipUserID == (Guid) founduser.ProviderUserKey).OrderByDescending(p => p.PhotoID) .FirstOrDefault();
            ViewBag.photo = userImage;
            ViewBag.UserDataName = founduser.UserName;
            return View(UserData);
        }
       

        [HttpPost]
        public ActionResult Search(string tofind)
        {
            var users = Membership.GetAllUsers();
            List<FoundUser> foundusers = new List<FoundUser>();

            FoundUser fu;
            foreach (MembershipUser user in users)
            {
                UserData ud = db.UserDatas.First(o=>o.MembershipUserID == (Guid)user.ProviderUserKey);
                if (user.UserName.Contains(tofind) ||
                    ud.FirstName.Contains(tofind) ||
                    ud.LastName.Contains(tofind))
                {
                    fu = new FoundUser();
                    fu.Login = user.UserName;
                    fu.FirstName = ud.FirstName;
                    fu.LastName = ud.LastName;
                    foundusers.Add(fu);
                }
            }
            return View(foundusers);
        }

        public ActionResult EditData()
        {
            Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
            var UserData = db.UserDatas.First(o => o.MembershipUserID == userid);
            return View(UserData);
        }

        [HttpPost]
        public ActionResult EditData(UserData model)
        {
            if (ModelState.IsValid)
            {
                Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                var UserData = db.UserDatas.First(o => o.MembershipUserID == userid);
                UserData.FirstName = model.FirstName;
                UserData.LastName = model.LastName;
                db.Entry(UserData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("EditData");
        }

        [HttpGet]
       
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string pass)
        {
            email = email.ToLower();
            pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pass, "SHA1");
            
          //  var result = from u in db.Users
            //             where u.Email == email && u.Pass == pass
              //           select u;

           // if (result.Count() > 0)
            //{
            //    FormsAuthentication.SetAuthCookie(email, false);

           //     return RedirectToAction("Index", "Home");
          //  }

            return View();
        }

        public ActionResult Facebook()
        {
            return new RedirectResult("https://graph.facebook.com/oauth/authorize?type=web_server&client_id=252734311486230&redirect_uri=http://localhost:1205/account/handshake/&scope=email%2Coffline_access%2Cuser_about_me");
        }

        [ActionName("handshake")]
        public ActionResult Handshake(string code)
        {
            //after authentication, Facebook will redirect to this controller action with a QueryString parameter called "code" (this is Facebook's Session key)

            //example uri: http://www.examplewebsite.com/facebook/handshake/?code=2.DQUGad7_kFVGqKTeGUqQTQ__.3600.1273809600-1756053625|dil1rmAUjgbViM_GQutw-PEgPIg.

            //this is your Facebook App ID
            string clientId = "252734311486230";

            //this is your Secret Key
            string clientSecret = "daa2835f96c1fd0c3b04c86504096714";

            //we have to request an access token from the following Uri
            string url = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

            //your redirect uri must be EXACTLY the same Uri that caused the initial authentication handshake
            string redirectUri = "http://localhost:1205/account/handshake/";

            //Create a webrequest to perform the request against the Uri
            WebRequest request = WebRequest.Create(string.Format(url, clientId, redirectUri, clientSecret, code));

            //read out the response as a utf-8 encoding and parse out the access_token
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(stream, encode);
            string accessToken = streamReader.ReadToEnd().Replace("access_token=", "");
            streamReader.Close();
            response.Close();

            //set the access token to some session variable so it can be used through out the session
            Session["FacebookAccessToken"] = accessToken;

            //
            var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me");

            string email = me.email;
            email = email.ToLower();
            string Login = "";
            foreach (char c in email)
                if (c == '@') break;
                else Login += c;
            
                if (Membership.FindUsersByEmail(email).Count != 0)
                {
                    if(Membership.GetUser(email) != null)
                        Login = email;
                    FormsAuthentication.SetAuthCookie(Login, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    RegisterModel model = new RegisterModel();

                    model.Password = model.ConfirmPassword = Membership.GeneratePassword(10, 5);
                    model.Email = email;

                    model.Login = Login;

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            WebMail.SmtpServer = "poczta.o2.pl";
                            WebMail.UserName = "pznet@o2.pl";
                            WebMail.Password = "dupa123";
                            WebMail.Send(
                                    model.Email,
                                    "Hasło do serwisu Photolife",
                                    "Witaj!<br /><br />" +
                                    "Właśnie stworzyliśmy ci konto na Photolifenet.<br /><br />" +
                                    "Login: " + model.Login + "<br />" +
                                    "Email: " + model.Email + "<br />" +
                                    "Hasło: " + model.Password + "<br /><br />" +
                                    "Po zalogowaniu się w systemie możesz zmienić swoje hasło.<br /><br />",
                                    "pznet@o2.pl"
                                );
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                        }

                        // big
                        string remoteImgPathBig = "https://graph.facebook.com/" + me.username + "/picture?type=large";
                        Uri remoteImgPathUriBig = new Uri(remoteImgPathBig);
                        string localPath = Path.Combine(Server.MapPath(Url.Content("~/Content/UserImages/")) + me.username + "big.jpg");
                        WebRequest focia = WebRequest.Create(string.Format(remoteImgPathBig, code));
                        WebResponse odpfocia = focia.GetResponse();
                        String oo = odpfocia.ResponseUri.AbsoluteUri;
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(oo, localPath);
                        // big

                        //50
                        string remoteImg50Path = "https://graph.facebook.com/" + me.username + "/picture?size=small";
                        string localPath50 = Path.Combine(Server.MapPath(Url.Content("~/Content/UserImages/")) + me.username + "50.jpg");

                        Uri remoteImg50PathUri = new Uri(remoteImg50Path);
                        WebRequest focia50 = WebRequest.Create(string.Format(remoteImg50Path, code));
                        WebResponse odpfocia50 = focia50.GetResponse();
                        String oo50 = odpfocia50.ResponseUri.AbsoluteUri;
                        WebClient webClient50 = new WebClient();
                        webClient.DownloadFile(oo50, localPath50);
                        //50
                       
                        MembershipCreateStatus createStatus;

                        MembershipUser newuser = Membership.CreateUser(model.Login, model.Password, model.Email, null, null, true, null, out createStatus);
                        
                        UserData ud = new UserData();
                        ud.MembershipUserID = (Guid)newuser.ProviderUserKey;
                        if((ud.FirstName = me.first_name) == null)
                            ud.FirstName = "";
                        if ((ud.LastName = me.last_name) == null)
                            ud.LastName = "";
                        db.UserDatas.Add(ud);
                        db.SaveChanges();

                        if (Roles.RoleExists("User") == false)
                            Roles.CreateRole("User");
                        if(Roles.IsUserInRole(newuser.UserName, "User") == false)
                            Roles.AddUserToRole(model.Login, "User");


                        // powiązanie fot z userem
                        //50
                        var entity50 = new PhotolifeEntities();
                        var photo50 = new Photo();
                        photo50.prefix = localPath50;
                        photo50.MembershipUserID = (Guid)newuser.ProviderUserKey;
                      //  photo50.MembershipUser = newuser;
                        entity50.Photos.Add(photo50);
                        // entity50.SaveChanges();
                        // photo50.SaveChanges();

                        //big
                        var entitybig = new PhotolifeEntities();
                        var photobig = new Photo();
                        photobig.prefix = localPath;
                        photobig.MembershipUserID = (Guid)newuser.ProviderUserKey;
                        //photobig.MembershipUser = newuser;
                        entitybig.Photos.Add(photobig);
                        // entitybig.SaveChanges();
                        // photobig.SaveChanges();

                        if (createStatus == MembershipCreateStatus.Success)
                        {
                            FormsAuthentication.SetAuthCookie(model.Login, false /* createPersistentCookie */);
                            return RedirectToAction("FacebookCreateSuccess");
                        }
                    }
                }
            

           // return View();



            //FormsAuthentication.SetAuthCookie(email, false);

            //return RedirectToAction("Index", "Home");

            return Content(email);
        }

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string username = model.EmailOrLogin.ToLower();
                if (Membership.FindUsersByEmail(model.EmailOrLogin).Count > 0)
                {
                    username = Membership.GetUserNameByEmail(model.EmailOrLogin);
                }

                if (Membership.ValidateUser(username, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(username, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email/Login lub hasło jest niepoprawne.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            bool ok = true;
            if (!ModelState.IsValid)
            { ModelState.AddModelError("", "Złe dane"); ok = false; }
            {
                model.Email = model.Email.ToLower();
                model.Login = model.Login.ToLower();
                if (!ReCaptcha.Validate(privateKey: "6LcNtc8SAAAAABTcliRjCCdZyFuMyjy4TmR2S0OZ"))
                { ModelState.AddModelError("", "Błędnie przepisany kod captcha"); ok = false; }
                if (Membership.FindUsersByEmail(model.Email).Count != 0)
                { ModelState.AddModelError("", "Email w użyciu"); ok = false; }
                if (Membership.FindUsersByName(model.Login).Count != 0)
                { ModelState.AddModelError("", "Taki użytkownik już istnieje"); ok = false; }
                if (ok == true)
                {
                    // Attempt to register the user
                    MembershipCreateStatus createStatus;
                    MembershipUser user = Membership.CreateUser(model.Login, model.Password, model.Email, null, null, true, null, out createStatus);

                    if (Roles.RoleExists("User") == false)
                        Roles.CreateRole("User");
                    if (Roles.IsUserInRole(model.Login, "User") == false)
                        Roles.AddUserToRole(model.Login, "User");

                    UserData ud = new UserData();
                    ud.MembershipUserID = (Guid)user.ProviderUserKey;
                    ud.FirstName = "";
                    ud.LastName = "";
                    db.UserDatas.Add(ud);
                    db.SaveChanges();
                    

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [CustomAuthorize(Roles = "Administrator, User")]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [CustomAuthorize(Roles = "Administrator, User")]
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name.ToLower(), true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Aktualne lub nowe hasło jest niepoprawne");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        [CustomAuthorize(Roles = "Administrator, User")]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //
        // GET: /Account/ChangeEmail

        [CustomAuthorize(Roles = "Administrator, User")]
        [Authorize]
        public ActionResult ChangeEmail()
        {
            ViewBag.Email = Membership.GetUser().Email.ToLower();
            return View();
        }

        //
        // POST: /Account/ChangeEmail

        [CustomAuthorize(Roles = "Administrator, User")]
        [Authorize]
        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
            if (ModelState.IsValid)
            {
                model.Email = model.Email.ToLower();
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded = true;
                if (Membership.FindUsersByEmail(model.Email).Count != 0) changePasswordSucceeded = false;
                try
                {
                    MembershipUser currentUser = Membership.GetUser();
                    currentUser.Email = model.Email;
                    Membership.UpdateUser(currentUser);

                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded == true)
                {
                    return RedirectToAction("ChangeEmailSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Aktualne hasło lub email jest niepoprawny");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangeEmailSuccess

        [CustomAuthorize(Roles = "Administrator, User")]
        public ActionResult ChangeEmailSuccess()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Email = model.Email.ToLower();
                model.Login = model.Login.ToLower();
                MembershipUser user;
                if (Membership.FindUsersByEmail(model.Email).Count == 1 &&
                    (user = Membership.GetUser(Membership.GetUserNameByEmail(model.Email))) != null
                    && user.UserName == model.Login
                    && ReCaptcha.Validate(privateKey: "6LcNtc8SAAAAABTcliRjCCdZyFuMyjy4TmR2S0OZ"))
                {
                    string password = user.ResetPassword();
                    try
                    {
                        WebMail.SmtpServer = "poczta.o2.pl";
                        WebMail.UserName = "pznet@o2.pl";
                        WebMail.Password = "dupa123";
                        WebMail.Send(
                                model.Email,
                                "Reset hasła na Photolife",
                                "Witaj!<br /><br />" +
                                "Właśnie zresetowaliśmy ci hasło w Photolifenet.<br /><br />" +
                                "Email: " + model.Email + "<br />" +
                                "Hasło: " + password + "<br /><br />" +
                                "Po zalogowaniu się w systemie możesz zmienić swoje hasło.<br /><br />",
                                "pznet@o2.pl"
                            );
                        Membership.UpdateUser(user);
                        return RedirectToAction("ResetPasswordSuccess");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nie istnieje użytkownik z takim loginem i e-mailem.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Nie istnieje użytkownik z takim loginem i e-mailem.");
            }

            return View();
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        public ActionResult FacebookCreateSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Taki użytkownik już istnieje. Spróbuj ponownie.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Taki e-mail jest już w użyciu. Spróbuj ponownie.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Podane hasło jest niepoprawne. Spróbuj ponownie.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Podany e-mail jest niepoprawny. Spróbuj ponownie.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Odpowiedź jest niepoprawna. Spróbuj ponownie.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Pytanie jest niepoprawne. Spróbuj ponownie.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Podana nazwa użytkownika jest niepoprawna. Spróbuj ponownie.";

                case MembershipCreateStatus.ProviderError:
                    return "Wystąpił problem od strony usługodawcy. W razie dalszych problemów skontaktuj się z administratorem.";

                case MembershipCreateStatus.UserRejected:
                    return "Tworzenie użytkownika zostało przerwane. W razie dalszych problemów skontaktuj się z administratorem.";

                default:
                    return "Wystąpił nieznany błąd. W razie dalszych problemów skontaktuj się z administratorem.";
            }
        }
        #endregion
    }
}
