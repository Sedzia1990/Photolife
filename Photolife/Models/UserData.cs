using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Photolife.Models
{
    public class UserData
    {
        [Key]
        public int UserDataId { get; set; }

      //  public int avatar_id { get; set;  }
        

        public Guid MembershipUserID { get; set; }

        [Display(Name="Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        //public string ProfilePhotoLink { get; set; }
    }
}