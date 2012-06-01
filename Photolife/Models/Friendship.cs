using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Photolife.Models
{
    public class Friendship
    {
        public int FriendshipID { get; set; }

        public string User { get; set; }

        public string UserFriend { get; set; }
    }
}