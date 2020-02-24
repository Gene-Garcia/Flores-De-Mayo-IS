using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Flores_De_Mayo_Information_System.Controllers
{
    public class WebContentController : Controller
    {
        private DBEntities ent = new DBEntities();

        // GET: WebContent
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisplayAnnouncements()
        {
            return View(ent.Announcements.OrderBy(m => m.DatePosted).Where(m=> m.IsArchived == false).ToList());
        }

        // Start modification methods of admin

        [HttpGet]
        [Authorize]
        public ActionResult Announcements()
        {
            return View( ent.Announcements.OrderBy(m => m.DatePosted).ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult PostAnnouncement()
        {

            return PartialView("_PostAnnouncement");
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostAnnouncement(Announcement anc, HttpPostedFileBase[] images)
        {
            anc.DatePosted = DateTime.Now;
            anc.UserId = User.Identity.GetUserId();

            // add announcement first
            ent.Announcements.Add(anc);
            ent.SaveChanges();

            // call method to upload
            if (images.Count() > 0)
            {
                new Uploader().SaveImage(images, anc);
            }

            return RedirectToAction("Announcements");
        }

        [Authorize]
        public ActionResult EditAnnouncement(int? ancId)
        {
            if (ancId == null)
            {
                return RedirectToAction("Announcements");
            }

            return PartialView("_EditAnnouncement", ent.Announcements.Where(m => m.AncId == ancId).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAnnouncement(Announcement anc,HttpPostedFileBase[] images)
        {
            anc.DatePosted = DateTime.Now;
            ent.Entry(anc).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();

            // call method to upload
            if (images.Count() > 0)
            {
                new Uploader().SaveImage(images, anc);
            }

            return RedirectToAction("Announcements");
        }

        [Authorize]
        public ActionResult ArchiveAnnouncement(int? ancId, int tick = 1)
        {
            // 1 means to archive the announcement
            // 2 means to re-archive
            if (ancId != null)
            {
                Announcement anc = ent.Announcements.Where(m => m.AncId == ancId).FirstOrDefault();
                anc.IsArchived = tick == 1 ? true : false;

                ent.Entry(anc).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();
            }

            return RedirectToAction("Announcements");
        }

        [Authorize]
        public ActionResult PermanentlyDeleteAnnouncement(int? ancId)
        {
            if(ancId == null)
            {
                return RedirectToAction("Announcements");
            }

            Announcement anc = ent.Announcements.Where(m => m.AncId == (int)ancId).FirstOrDefault();

            //call DeleteImage, to remove connections from assets and file storage

            List<AnnouncementAsset> assets = new List<AnnouncementAsset>(anc.AnnouncementAssets);

            foreach (var asset in assets)
            {
                DeleteImage(asset.FileId);
            }

            //delete from announcements
            ent.Announcements.Remove(anc);
            ent.SaveChanges();

            return RedirectToAction("Announcements");
        }

        [Authorize]
        public ActionResult DeleteImage(int? fileId)
        {
            if (fileId == null)
            {
                return RedirectToAction("Announcement");
            }
            
            //remove from assets
            ent.AnnouncementAssets.Remove(ent.AnnouncementAssets.Where(m => m.FileId == fileId).FirstOrDefault());
            ent.SaveChanges();

            //remove from file storage
            ent.FileStorages.Remove(ent.FileStorages.Where(m => m.FileId == fileId).FirstOrDefault());
            ent.SaveChanges();

            return RedirectToAction("Announcements");
        }

        // end   modification methods of admin
    }
}