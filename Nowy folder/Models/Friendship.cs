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

        public Guid User { get; set; }

        public Guid UserFriend { get; set; }
    }
    public class AddingFriend
    {
        public string UserFriend { get; set; }
    }
}