using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System.Actions
{
    public class Uploader : Controller
    {

        private DBEntities ent = new DBEntities();

        public void SaveImage(HttpPostedFileBase[] images, Announcement anc)
        {
            string fileName, fileExtension, defaultPath = "";

            foreach (HttpPostedFileBase image in images)
            {

                fileName = Path.GetFileNameWithoutExtension(image.FileName);
                fileExtension = Path.GetExtension(image.FileName);

                defaultPath = "/Content/Images/";

                if (IsExisting(defaultPath, fileName, fileExtension))
                {
                    fileName += new Random().Next(101);
                }

                // variable imgPath would be stored, contains the actual location of the image.
                string imgPath = defaultPath + fileName;

                //fileName = Path.Combine(Server.MapPath((defaultPath)), fileName);
                string filePath = HostingEnvironment.MapPath(defaultPath + fileName + fileExtension);

                image.SaveAs(filePath);

                // inserts first to the storage to get the fileId
                FileStorage storage = new FileStorage()
                {
                    FileName = fileName,
                    Location = defaultPath + fileName + fileExtension
                };
                ent.FileStorages.Add(storage);
                ent.SaveChanges();

                AnnouncementAsset asset = new AnnouncementAsset()
                {
                    AncId = anc.AncId,
                    FileId = storage.FileId
                };
                ent.AnnouncementAssets.Add(asset);
                ent.SaveChanges();

            }

        }

        public bool IsExisting(string defPath, string fileName, string fileExtension)
        {
            var relativePath = defPath + fileName + fileExtension;
            string filePath = HostingEnvironment.MapPath(defPath + fileName + fileExtension);

            if (System.IO.File.Exists(filePath))
            {
                return true;
            }

            return false;
        }
    }
}