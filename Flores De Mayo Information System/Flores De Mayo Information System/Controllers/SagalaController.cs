using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System.Controllers
{
    public class SagalaController : Controller
    {

        private DBEntities ent = new DBEntities();

        [HttpGet]
        [Authorize]
        public ActionResult PairSagala()
        {
            int datId = WebSettings.getCurrentDate();

            List<int> takenTitlesId = ent.SagalaPartners.Where(m => m.DSetId == datId).Select(m => m.SagalaTitle.STitId).ToList();
            TempData["sagalaTitles"] = ent.SagalaTitles.Where(m => !takenTitlesId.Contains(m.STitId)).ToList();

            // so that sagala applications that will only be displayed are those that are not yet paired 
            List<int> femaleSagala = ent.SagalaPartners.Where(m => m.DSetId == datId).Select(m => m.AppIdFirst).ToList();
            List<int?> maleSagala = ent.SagalaPartners.Where(m => m.DSetId == datId).Select(m => m.AppIdSec).ToList();

            return View(ent.Applications.Where(m => !maleSagala.Contains(m.AppId) && !femaleSagala.Contains(m.AppId) && m.DSetIdApplied == datId && m.ApplicationType.Name.ToLower() == "sagala" && m.ApplicationStatusType.Name.ToLower() == "approved"));
        }

        [HttpPost]
        [Authorize]
        public ActionResult PairSagala(string maleId, string femaleId, string titleId)
        {
            if ((maleId == "-1" && femaleId == "-1") || titleId == null )
            {
                return RedirectToAction("PairSagala");
            }

            int mId = int.Parse(maleId);
            int fId = int.Parse(femaleId);

            Application firstApp = ent.Applications.Where(m => m.AppId == fId).FirstOrDefault();
            Application secApp = ent.Applications.Where(m => m.AppId == mId).FirstOrDefault();

            SagalaPartner paired = new SagalaPartner()
            {
                AppIdFirst = firstApp.AppId,
                STitId = int.Parse(titleId),
                DSetId = WebSettings.getCurrentDate()
            };

            if(maleId != "-1") { paired.AppIdSec = secApp.AppId;  }

            ent.SagalaPartners.Add(paired);
            ent.SaveChanges();

            return RedirectToAction("PairSagala");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ListSagalas()
        {
            int datId = WebSettings.getCurrentDate();
            return View( ent.SagalaPartners.Where(m=>m.DSetId == datId).OrderBy(m=>m.SagalaTitle.STitId).ToList() );
        }

        [HttpGet] //for web content
        public ActionResult DisplaySagalas()
        {
            int datId = WebSettings.getCurrentDate();
            return View(ent.SagalaPartners.Where(m => m.DSetId == datId).OrderBy(m => m.SagalaTitle.STitId).ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult DisplaySagalaTitles()
        {
            return View( ent.SagalaTitles.ToList() );
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddSagalaTitle()
        {
            return PartialView("_AddSagalaTitle");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddSagalaTitle(SagalaTitle title)
        {
            SagalaTitle record = ent.SagalaTitles.Where(m => m.Name.ToLower() == title.Name.ToLower()).FirstOrDefault();

            if (record == null)
            {
                ent.SagalaTitles.Add(title);
                ent.SaveChanges();
            }
            else
            {
                //there is already a title
                TempData["errorMsg"] = "The title " + title.Name + " is already registered.";
            }

            return RedirectToAction("DisplaySagalaTitles");
        }

        [Authorize]
        public ActionResult DeleteTitle(int? titleId)
        {
            if (titleId == null)
            {
                return RedirectToAction("DisplaySagalaTitles");
            }

            SagalaTitle sagalaTitle = ent.SagalaTitles.Where(m => m.STitId == (int)titleId).FirstOrDefault();
            ent.SagalaTitles.Remove(sagalaTitle);
            ent.SaveChanges();

            return RedirectToAction("DisplaySagalaTitles");
        }

    }
}