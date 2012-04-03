﻿using System;
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
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
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
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
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
            if (ModelState.IsValid && ReCaptcha.Validate(privateKey: "6LcNtc8SAAAAABTcliRjCCdZyFuMyjy4TmR2S0OZ"))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
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
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
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
                    && (user = Membership.GetUser(Membership.GetUserNameByEmail(model.Email))) != null)
                {
                    string password = user.ResetPassword();
                    Membership.UpdateUser(user);
                    try
                    {
                        WebMail.SmtpServer = "poczta.o2.pl";
                        WebMail.UserName = "Photolifenet@o2.pl";
                        WebMail.Password = "dupa123";
                        WebMail.Send(
                                model.Email,
                                "Reset hasła na Photolifenet",
                                "Witaj!<br /><br />" +
                                "Właśnie zresetowaliśmy ci hasło w Photolifenet.<br /><br />" +
                                "Login: " + model.Username + "<br />" +
                                "Hasło: " + password + "<br /><br />" +
                                "Po zalogowaniu się w systemie możesz zmienić swoje hasło.<br /><br />",
                                "Photolifenet@o2.pl"
                            );

                        return RedirectToAction("ResetPasswordSuccess");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Użytkownik o podanym adresie e-mail lub peselu nie istnieje.");
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
