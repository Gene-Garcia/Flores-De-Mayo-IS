using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Application;
using Flores_De_Mayo_Information_System.Models.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System.Controllers
{
    public class AlayanController : Controller
    {
        private DBEntities ent = new DBEntities();
        private Searcher srch = new Searcher();

        public ActionResult Index()
        {
            
            int datId = WebSettings.getCurrentDate();

            return View(ent.AlayanDates.ToList());
        }

        [HttpGet]
        public ActionResult CreateAlayan(int? date)
        {
            if (!WebSettings.IsApplicationOpen())
            {
                TempData["errorMsg"] = "Sorry we are not yet accepting applications";
                return RedirectToAction("DisplayAnnouncements", "WebContent");
            }
            int dateId = WebSettings.getCurrentDate();

            List<int> datIdAlayan = ent.Alayans.Where(m => m.DSetIdApplied == dateId).Select(m => m.ADatId).ToList();

            TempData["availableDates"] = ent.AlayanDates.Where(m => !datIdAlayan.Contains(m.ADatId) /* m.alayantype ==regular*/).ToList(); //available dates that are not taken yet
            TempData["alayanTypes"] = ent.AlayanTypes.ToList(); //ids 1 - 3 are tuklos, sabog and pangako, 4, 5, and 6 are names that should not be seen by the user

            if (date == null)
            {
                return View();
            }
            else
            {
                return View(new CreateAlayanViewModel() { AlayDate = (int)date });
            }

            return View();

        }

        [HttpPost]
        public ActionResult CreateAlayan(CreateAlayanViewModel alayanVM)
        {

            if (ModelState.IsValid)
            {

                SinisianDatabase sinDb = srch.FindInSinisian(alayanVM.FirstName, alayanVM.MiddleName, alayanVM.LastName);

                string referenceNum = ReferenceFactory.GenerateReference(alayanVM.FirstName + alayanVM.MiddleName + alayanVM.LastName);
                int dateId = WebSettings.getCurrentDate();

                if (sinDb == null)
                {
                    TempData["errorMsg"] = "Sorry we cannot find your identity.";
                    return RedirectToAction("CreateAlayan");
                }

                int count = ent.Alayans.Where(m => m.SinId == sinDb.SinId && m.DSetIdApplied == dateId).Count();

                if (count >= 1)
                {
                    TempData["errorMsg"] = "You already have an application. You can check your status from the \"Application Status\" in the navbar.";
                    return RedirectToAction("CreateAlayan");
                }

                Alayan alayan = new Alayan()
                {
                    ADatId = alayanVM.AlayDate,
                    ATypId = alayanVM.AlayType,
                    DSetIdApplied = dateId,
                    Reference = referenceNum,
                    SinId = sinDb.SinId
                };

                ent.Alayans.Add(alayan);
                ent.SaveChanges();

                Mailer.SendEmail(new IdentityMessage() { Subject = "Alayan Schedule Reference Number", Body = "<b>" + referenceNum + "</b> is your reference number to your alayan schedule.<br><br>Please click this link to check the summary.<br><br><br><br><b>Sincerely,</b><br>Flores De Mayo President.<br><br><br><br>If you have any concern please send a feedback to us at <a href=\"\">this link.</a>", Destination = alayanVM.Email });

                return RedirectToAction("CreateAlayan");
            }
            else
            {
                TempData["errorMsg"] = "Sorry, please try again.";
                return RedirectToAction("CreateAlayan");
            }

        }

        [HttpGet]
        public ActionResult AlayanSummary()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AlayanSummary(Alayan alay)
        {
            if (alay.Reference != null)
            {

                return View(ent.Alayans.Where(m => m.Reference == alay.Reference).FirstOrDefault());

            }
            else
            {
                TempData["errorMsg"] = "Invalid reference number. Please try again.";
                return RedirectToAction("AlayanSummary");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult DisplayAlays()
        {
            int datId = WebSettings.getCurrentDate();

            return View(ent.Alayans.Where(m => m.DSetIdApplied == datId).OrderBy(n => n.AlayanDate.ADatId).ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAlayan(int? alayId)
        {

            if (alayId == null)
            {
                return View("DisplayAlays");
            }

            int dateId = WebSettings.getCurrentDate();

            List<int> datIdAlayan = ent.Alayans.Where(m => m.DSetIdApplied == dateId).Select(m => m.ADatId).ToList();

            Alayan alayan = ent.Alayans.Where(m => m.AlaId == alayId).FirstOrDefault();

            TempData["availableDates"] = ent.AlayanDates.Where(m => !datIdAlayan.Contains(m.ADatId) /* m.alayantype ==regular*/).ToList(); //available dates that are not taken yet
            TempData["alayanTypes"] = ent.AlayanTypes.Where(m => m.ATypId != alayan.ATypId).ToList(); //ids 1 - 3 are tuklos, sabog and pangako, 4, 5, and 6 are names that should not be seen by the user
            return PartialView("_EditAlayan", alayan);

        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAlayan(Alayan alay)
        {
            //return Content(alay.ADatId.ToString() + " " + alay.ATypId.ToString());
            ent.Entry(alay).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("DisplayAlays");
        }

        [Authorize]
        public ActionResult DeleteAlayan(int? alayId)
        {
            if (alayId == null)
            {
                return RedirectToAction("DisplayAlays");
            }

            int datId = WebSettings.getCurrentDate();

            Alayan alayan = ent.Alayans.Where(m => m.DSetIdApplied == datId && m.AlaId == alayId).FirstOrDefault();

            ent.Alayans.Remove(alayan);
            ent.SaveChanges();

            return RedirectToAction("DisplayAlays");
        }
        
        [Authorize]
        public ActionResult AddAlayan()
        {
            int dateId = WebSettings.getCurrentDate();

            List<int> datIdAlayan = ent.Alayans.Where(m => m.DSetIdApplied == dateId).Select(m => m.ADatId).ToList();

            TempData["availableDates"] = ent.AlayanDates.Where(m => !datIdAlayan.Contains(m.ADatId) /* m.alayantype ==regular*/).ToList(); //available dates that are not taken yet
            TempData["alayanTypes"] = ent.AlayanTypes.ToList(); //ids 1 - 3 are tuklos, sabog and pangako, 4, 5, and 6 are names that should not be seen by the user

            return PartialView("_AddAlayan", new CreateAlayanViewModel());
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAlayanDateType()
        {
            return View(ent.AlayanDates.ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditSelectedDate(int? dateId)
        {
            TempData["dateType"] = ent.AlayanDateTypes.ToList();
            return PartialView("_EditSelectedDate", ent.AlayanDates.Where( m=> m.ADatId == dateId).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditSelectedDate(AlayanDate alayan)
        {
            ent.Entry(alayan).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("EditAlayanDateType");
        }
    }
}