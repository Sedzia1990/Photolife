using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Photolife.Models
{
    public class PhotoVote
    {
        public int PhotoVoteID { get; set; }

        virtual public Photo Photo { get; set; }
        virtual public MembershipUser MembershipUser { get; set; }

        public int Rate { get; set; }
        public DateTime Date { get; set; }
    }
}