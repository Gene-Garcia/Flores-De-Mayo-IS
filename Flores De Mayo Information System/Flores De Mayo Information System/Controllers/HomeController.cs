using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] images)
        {
            SaveImage(images);
            return RedirectToAction("Index");
        }

        public void SaveImage(HttpPostedFileBase[] images)
        {

            string fileName, fileExtension, defaultPath = "";

            foreach (HttpPostedFileBase image in images)
            {

                fileName = Path.GetFileNameWithoutExtension(image.FileName);
                fileExtension = Path.GetExtension(image.FileName);
                defaultPath = "~/Content/Images/";

                if (IsExisting(defaultPath, fileName, fileExtension))
                {
                    fileName += new Random().Next(101);
                }

                fileName += fileExtension;

                // variable imgPath would be stored, contains the actual location of the image.
                string imgPath = defaultPath + fileName;

                fileName = Path.Combine(Server.MapPath((defaultPath)), fileName);
                image.SaveAs(fileName);

            }

        }

        public bool IsExisting(string defPath, string fileName, string fileExtension)
        {
            var relativePath = defPath + "/" + fileName + fileExtension;
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            { 
                return true;
            }

            return false;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}