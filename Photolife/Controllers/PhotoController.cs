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

        //
        // GET: /Photo/

        public ViewResult Index()
        {
            return View(db.Photos.ToList());
        }

        //
        // GET: /Photo/Details/5

        public ViewResult Details(int id)
        {
            Photo photo = db.Photos.Find(id);
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
        public ActionResult Create(HttpPostedFileWrapper imageFile)
        {

            if (imageFile == null || imageFile.ContentLength == 0)
            {
                ViewBag.error = true;
                return View();
            }

            var fileName = String.Format("{0}", Guid.NewGuid().ToString());
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Content/UserImages")), fileName);

            imageFile.SaveAs(imagePath + ".jpg");

            // resize
            var oryginalImage = Image.FromFile(imagePath + ".jpg");
            var Image800 = ResizeImage(oryginalImage, 600, 800);
            var image200 = ResizeImage(oryginalImage, 200, 200);

            // save
            Image800.Save(imagePath + "_800.jpg");
            image200.Save(imagePath + "_200.jpg");

            // save to db
            var photo = new Photo();
            photo.prefix = fileName;
            photo.MembershipUserID = (Guid)Membership.GetUser().ProviderUserKey;
            photo.MembershipUser = Membership.GetUser();
            db.Photos.Add(photo);
            db.SaveChanges();



            return RedirectToAction("Details", new { id = photo.PhotoID });


            //if (ModelState.IsValid)
            //  {
            //       db.Photos.Add(photo);
            //       db.SaveChanges();
            //       return RedirectToAction("Index");  
            //   }

            //return View(photo);
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {

            Bitmap originalImage = new Bitmap(image);
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;
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