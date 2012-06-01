using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Photolife.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public string Nadawca { get; set; }


        public string Odbiorca { get; set; }

        public string Tytuł { get; set; }

        public string Treść { get; set; }

    }
}