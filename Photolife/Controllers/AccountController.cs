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

namespace Photolife.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
       
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string pass)
        {
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
            

           
            
            RegisterModel model = new RegisterModel();
            
            
                model.Email = email;
                model.Password = "test123";
                model.ConfirmPassword = "test123";

                if (Membership.FindUsersByEmail(model.Email).Count == 1){

                    FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }

                else if (ModelState.IsValid)
                {
                    MembershipCreateStatus createStatus;
                    
                    Membership.CreateUser(model.Email, model.Password, model.Email, null, null, true, null, out createStatus);
                    Roles.AddUserToRole(model.Email, "User");

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
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
                if (Membership.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
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
                    ModelState.AddModelError("", "Email lub hasło jest niepoprawne.");
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
            if (!ReCaptcha.Validate(privateKey: "6LcNtc8SAAAAABTcliRjCCdZyFuMyjy4TmR2S0OZ"))
            { ModelState.AddModelError("", "Błędnie przepisany kod captcha"); ok = false; }
            if (Membership.FindUsersByEmail(model.Email).Count != 0)
            { ModelState.AddModelError("", "Taki użytkownik już istnieje"); ok = false; }
            if (ok == true)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Email, model.Password, model.Email, null, null, true, null, out createStatus);
                Roles.AddUserToRole(model.Email, "User");

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

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
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
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

        public ActionResult ChangePasswordSuccess()
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
                MembershipUser user;
                if(Membership.FindUsersByEmail(model.Email).Count == 1
                    && (user = Membership.GetUser(Membership.GetUserNameByEmail(model.Email))) != null
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
                    ModelState.AddModelError("", "Nie istnieje użytkownik z takim e-mailem.");
                }
            }

            return View(model);
        }

        public ActionResult ResetPasswordSuccess()
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
