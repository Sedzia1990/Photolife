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

        public int UserDataId { get; set; } 
        public Guid MembershipUserID { get; set; }
        virtual public MembershipUser MembershipUser { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        //public string ProfilePhotoLink { get; set; }
    }
}