using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Photolife.Models
{
    public class Photo
    {
        public int PhotoID { get; set; }

        public Guid MembershipUserID { get; set; }

        public string description { get; set; }

        public string prefix { get; set; }


        [Display(AutoGenerateField = false)]
        public string FileName
        {
            get
            {
                return prefix + ".jpg";
            }
        }
        [Display(AutoGenerateField = false)]
        public string FilePath
        {
            get
            {
                return Path.Combine("~/Content/UserImages", FileName);
            }
        }

        [Display(AutoGenerateField = false)]
        public string FileName800
        {
            get
            {
                return prefix + "_800.jpg";
            }
        }

        [Display(AutoGenerateField = false)]
        public string FilePath800
        {
            get
            {
                return Path.Combine("~/Content/UserImages", FileName800);
            }
        }

        [Display(AutoGenerateField = false)]
        public string FileName200
        {
            get
            {

                return prefix + "_200.jpg";
            }
        }

       

        [Display(AutoGenerateField = false)]
        public string FilePath200
        {
            get
            {
                return Path.Combine("~/Content/UserImages", FileName200);
            }
        }

        [Display(AutoGenerateField = false)]
        public string FileName100width
        {
            get
            {

                return prefix + "_100width.jpg";
            }
        }

        [Display(AutoGenerateField = false)]
        public string FilePath100width
        {
            get
            {
                return Path.Combine("~/Content/UserImages", FileName100width);
            }
        }

        [Display(AutoGenerateField = false)]
        public string FileName450width
        {
            get
            {

                return prefix + "_450width.jpg";
            }
        }



        [Display(AutoGenerateField = false)]
        public string FilePath450width
        {
            get
            {
                return Path.Combine("~/Content/UserImages", FileName450width);
            }
        }

  

    }
}