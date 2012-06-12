using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Photolife.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne hasłoo")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Hasło musi mieć od {0} do {2} znaków długości.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Hasła się nie zgadzają.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeEmailModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne hasło")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Login lub Email")]
        public string EmailOrLogin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć od {0} do {2} znaków długości.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła się nie zgadzają.")]
        public string ConfirmPassword { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "To pole jest wymagane.")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "E-mail jest wymagany, aby przypomnieć hasło.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Ciąg musi mieć postać adresu e-mail.")]
        public string Email { get; set; }
    }
    public class FoundUser
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
