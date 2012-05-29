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
using Newtonsoft.Json.Linq;
using Facebook.Web;
using System.Net;

namespace Photolife.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FacebookLogin(string token)
        {
            WebClient client = new WebClient();
            string JsonResult = client.DownloadString(string.Concat(
                   "https://graph.facebook.com/me?access_token=", token));
            // Json.Net is really helpful if you have to deal
            // with Json from .Net http://json.codeplex.com/
            JObject jsonUserInfo = JObject.Parse(JsonResult);
            // you can get more user's info here. Please refer to:
            //     http://developers.facebook.com/docs/reference/api/user/
            string username = jsonUserInfo.Value<string>("username");
            string email = jsonUserInfo.Value<string>("email");
            string locale = jsonUserInfo.Value<string>("locale");
            int facebook_userID = jsonUserInfo.Value<int>("id");

            // store user's information here...
            FormsAuthentication.SetAuthCookie(username, true);
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/LogOn

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
