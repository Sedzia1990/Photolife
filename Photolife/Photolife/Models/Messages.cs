using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace PZ.Models
{
    public class Messages
    {
        

        public string From { get; set; }

        
        public string To   { get; set; }


        public string Title { get; set; }


        public string Content { get; set; }
    }



}