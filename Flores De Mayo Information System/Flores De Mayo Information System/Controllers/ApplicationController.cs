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
    public class ApplicationController : Controller
    {
        private DBEntities ent = new DBEntities();
        private Searcher srch = new Searcher();

        [HttpGet]
        public ActionResult CreateApplication()
        {

            if(!WebSettings.IsApplicationOpen())
            {
                TempData["errorMsg"] = "Sorry we are not yet accepting applications";
                return RedirectToAction("DisplayAnnouncements","WebContent");
            }

            TempData["application_type"] = ent.ApplicationTypes.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult CreateApplication(CreateApplicationViewModel app)
        {
            if (ModelState.IsValid)
            {
                int dateId = WebSettings.getCurrentDate();

                SinisianDatabase sinDb = srch.FindInSinisian(app.FirstName, app.MiddleName, app.LastName);

                string referenceNum = ReferenceFactory.GenerateReference(app.FirstName + app.MiddleName + app.LastName);

                if (sinDb == null)
                {
                    TempData["errorMsg"] = "Sorry we cannot find your identity.";
                    return RedirectToAction("CreateApplication", app);
                }

                int count = ent.Applications.Where(m => m.SinId == sinDb.SinId && m.DSetIdApplied == dateId).Count();

                if (count >= 1)
                {
                    TempData["errorMsg"] = "You already have an application. You can check your status from the \"Application Status\" in the navbar.";
                    return RedirectToAction("CreateApplication");
                }

                /**
                 * isa, if officer na siya hindi na siya pwede pang mag aaply pa para maging officer
                 */

                Application application = new Application()
                {
                    ATypeId = int.Parse(app.ApplicationType),
                    ASTypId = ent.ApplicationStatusTypes.Where(m => m.Name.ToLower() == "queue").Select(m => m.ASTypId).FirstOrDefault(),
                    DSetIdApplied = dateId,
                    RerenceNum = referenceNum,
                    SinId = sinDb.SinId,

                };

                ent.Applications.Add(application);
                ent.SaveChanges();

                Mailer.SendEmail(new IdentityMessage() { Subject = "Application Reference Number", Body = "<b>" + referenceNum + "</b> is your reference number to your application.<br><br>Please click this link to check application status.<br><br><br><br><b>Sincerely,</b><br>Flores De Mayo President.<br><br><br><br>If you have any concern please send a feedback to us at <a href=\"\">this link.</a>", Destination = app.Email });

            }
            else
            {

            }

            return RedirectToAction("CreateApplication");

        }

        [HttpGet]
        public ActionResult ViewApplicationStatus()
        {

            return View();

        }

        [HttpPost]
        public ActionResult ViewApplicationStatus(Application app)
        {
            if (app.RerenceNum != null)
            {

                return View(ent.Applications.Where(m => m.RerenceNum == app.RerenceNum).FirstOrDefault());

            }
            else
            {
                TempData["errorMsg"] = "Invalid reference number. Please try again.";
                return RedirectToAction("VieWApplicationStatus");
            }
        }

        [Authorize]
        public ActionResult DisplayApplications(string appType = "officer")
        {

            int datId = WebSettings.getCurrentDate();

            TempData["applicationTypes"] = ent.ApplicationTypes.ToList();
            TempData["selectedAppType"] = appType;

            //getting all sinIds that are registered in aspnet user so that they will not be showed as applicants because they are already registered
            List<int> registeredSinisianIds = ent.AspNetUsers.Select(m => (int)m.SinId).ToList();

            return View(ent.Applications.Where(m => m.DSetIdApplied == datId && m.ApplicationType.Name.ToLower() == appType && !registeredSinisianIds.Contains(m.SinId)).ToList());
        }
        
        /**
         * this method can be called by approved buttons even without the second paramater (2 is approved),
         * moreover, if another button that will modify the application status just needs to use the dropdown to get the id
         * 
         */
        [Authorize]
        public ActionResult EvaluateApplication(int? appId, int statId = 2)
        {
            int datId = WebSettings.getCurrentDate();

            if (appId == null)
            {
                return RedirectToAction("DisplayApplications");
            }

            Application app = ent.Applications.Where(m => m.AppId == (int)appId).FirstOrDefault();
            
            if(app == null)
            {
                return RedirectToAction("DisplayApplications");
            }

            app.ASTypId = statId;
            app.UserIdEvaluated = User.Identity.GetUserId();
            app.DSetIdEvaluated = datId;

            ent.Entry(app).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();

            return RedirectToAction("DisplayApplications");
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditApplication(int? appId)
        {
            Application app = ent.Applications.Where(m => m.AppId == appId).FirstOrDefault();
            TempData["statusTypes"] = ent.ApplicationStatusTypes.Where(m => m.ASTypId != app.ASTypId).ToList();
            return PartialView("_EditApplication", app);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditApplication(Application app)
        {
            int datId = WebSettings.getCurrentDate();

            app.UserIdEvaluated = User.Identity.GetUserId();
            app.DSetIdEvaluated = datId;

            ent.Entry(app).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("DisplayApplications", new { appId = app.AppId, statId = app.ASTypId });

                
        }

        [Authorize]
        public ActionResult DeleteApplication(int? appId)
        {
            int datId = WebSettings.getCurrentDate();

            if (appId == null)
            {
                return RedirectToAction("DisplayApplications");
            }

            Application app = ent.Applications.Where(m => m.AppId == appId && m.DSetIdApplied == datId).FirstOrDefault();
            ent.Applications.Remove(app);
            ent.SaveChanges();

            return RedirectToAction("DisplayApplications", new { appId = app.AppId, statId = app.ASTypId });
        }

        [Authorize]
        public ActionResult AddApplication(int? appType)
        {

            if (appType == null)
            {
                return RedirectToAction("DisplayApplications", new { appType = appType });
            }
            TempData["application_type"] = ent.ApplicationTypes.ToList();

            return PartialView("_AddApplication", new CreateApplicationViewModel() { ApplicationType = appType.ToString() });
        }

        [HttpGet]
        [Authorize]
        public ActionResult ModifyWebSettings()
        {
            return View(ent.WebSettings.FirstOrDefault());
        }

        [Authorize]
        public ActionResult ChangeYear(string newChangedYear)
        {
            if(newChangedYear == null)
            {
                return RedirectToAction("ModifyWebSettings");
            }

            DateSetting dSet = new DateSetting()
            {
                Year = DateTime.Parse(newChangedYear+"/1/1")
            };
            ent.DateSettings.Add(dSet);
            ent.SaveChanges();

            WebSetting web = ent.WebSettings.FirstOrDefault();

            web.DSetId = dSet.DSetId;
            ent.SaveChanges();
            

            return RedirectToAction("ModifyWebSettings");
        }

        [Authorize]
        public ActionResult OpenApplication(string applicationRadio)
        {
            if(applicationRadio == null)
            {
                return RedirectToAction("ModifyWebSettings");
            }

            bool isOpen = false;

            if (applicationRadio.ToLower() == "open") { isOpen = true; }
            else if (applicationRadio.ToLower() == "close") { isOpen = false; }

            WebSetting web = ent.WebSettings.FirstOrDefault();

            web.ApplicationOpen = isOpen;
            ent.SaveChanges();

            return RedirectToAction("ModifyWebSettings");
        }
    }
}