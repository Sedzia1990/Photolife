using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Photolife.Models;
using System.Web.Security;


namespace Photolife.Controllers
{
    [CustomAuthorize(Roles = "Administrator, User")]
    public class PhotoController : Controller
    {
        private PhotolifeEntities db = new PhotolifeEntities();

        public ViewResult List(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            Guid guid = (Guid)user.ProviderUserKey;
            ViewBag.user = user;
            return View(db.Photos.Where(p => p.MembershipUserID == guid));
        }

        //
        // GET: /Photo/

        public ViewResult Index()
        {
            Guid guid = (Guid)Membership.GetUser().ProviderUserKey;
            return View(db.Photos.Where(p => p.MembershipUserID == guid));
        }

        //
        // GET: /Photo/Details/5

        public ViewResult Details(int id)
        {
            Photo photo = db.Photos.Find(id);
            MembershipUser user = Membership.GetUser(photo.MembershipUserID);
            ViewBag.user = user;
            return View(photo);
        }

        //
        // GET: /Photo/Create

        public ActionResult Create()
        {
            ViewBag.error = false;
            return View();
        }

        //
        // POST: /Photo/Create

        [HttpPost]
        public JsonResult Create(HttpPostedFileWrapper imageFile)
        {
            JsonResult json = new JsonResult();

            if (imageFile == null || imageFile.ContentLength == 0)
            {

                json.Data = new { status = "no_file" };
                return json;
            }

            var fileName = String.Format("{0}", Guid.NewGuid().ToString());
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Content/UserImages")), fileName);

            imageFile.SaveAs(imagePath + ".jpg");

            // resize
            var oryginalImage = Image.FromFile(imagePath + ".jpg");
            var Image800 = ResizeImage(oryginalImage, 600, 800);
            var image200 = ResizeImage(oryginalImage, 200, 200);
            var image450width = ResizeImage(oryginalImage, 450, -1);
            // save to 
            Image800.Save(imagePath + "_800.jpg");
            image200.Save(imagePath + "_200.jpg");
            image450width.Save(imagePath + "_450width.jpg");

            // save to db
            var photo = new Photo();
            photo.prefix = fileName;
            photo.MembershipUserID = (Guid)Membership.GetUser().ProviderUserKey;
            db.Photos.Add(photo);
            db.SaveChanges();

            json.Data = new { status = "ok", redirect = Url.Action("Details", "Photo", new { id = photo.PhotoID.ToString() }) };
            return json;
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {

            Bitmap originalImage = new Bitmap(image);
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;


            if (maxHeight == -1)
            {
                newWidth = maxWidth;
                newHeight = (int)((double)originalImage.Height * (double)((double)newWidth / (double)originalImage.Width));
            }
            else if (maxHeight == -1)
            {
                newHeight = maxHeight;
                newWidth = (int)((double)originalImage.Width * (double)((double)newHeight / (double)originalImage.Height));
            }
            else
            {
                double aspectRatio = (double)originalImage.Width / (double)originalImage.Height;
                if (aspectRatio <= 1 && originalImage.Width > maxWidth)
                {
                    newWidth = maxWidth;
                    newHeight = (int)Math.Round(newWidth / aspectRatio);
                }
                else if (aspectRatio > 1 && originalImage.Height > maxHeight)
                {
                    newHeight = maxHeight;
                    newWidth = (int)Math.Round(newHeight * aspectRatio);
                }
            }

            Bitmap newImage = new Bitmap(originalImage, newWidth, newHeight);

            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImage(originalImage, 0, 0, newImage.Width, newImage.Height);

            originalImage.Dispose();

            return newImage;
        }



        //
        // POST: /Photo/Edit/5

        [HttpPost]
        public JsonResult EditDescription(int id, String description)
        {
            Photo photo = db.Photos.Find(id);
            photo.description = description;
            db.Entry(photo).State = EntityState.Modified;
            db.SaveChanges();
            JsonResult result = new JsonResult();
            result.Data = new
            {
                status = "ok",
                description = description
            };
            return result;

            //return View(photo);
        }


        //
        // GET: /Photo/Edit/5

        public ActionResult Edit(int id)
        {
            Photo photo = db.Photos.Find(id);
            return View(photo);
        }

        //
        // POST: /Photo/Edit/5

        [HttpPost]
        public ActionResult Edit(Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        //
        // GET: /Photo/Delete/5

        public ActionResult Delete(int id)
        {
            Photo photo = db.Photos.Find(id);
            return View(photo);
        }

        //
        // POST: /Photo/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}