using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flores_De_Mayo_Information_System.Actions;

namespace Flores_De_Mayo_Information_System.Controllers
{
    public class FeedbackController : Controller
    {

        private DBEntities ent = new DBEntities();

        // GET: Feedback
        public ActionResult Index()
        {
            return View();
        }

        //admin
        [HttpGet]
        [Authorize]
        public ActionResult DisplayFeedbacks()
        {
            //display in table if answered, answered means meron ng laman yung remarks
            return View(ent.Feedbacks.Where( m => m.SpamMessage == false).ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult SpamFeedbacks()
        {
            return View(ent.Feedbacks.Where(m => m.SpamMessage == true).ToList());
        }

        [Authorize]
        public ActionResult AnswerFeedback(int? fId)
        {
            if (fId == null)
            {
                return View();
            }
            return PartialView("_AnswerFeedback", ent.Feedbacks.Where(m=>m.FId == fId).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public ActionResult AnswerFeedback(Feedback feedback)
        {
            ent.Entry(feedback).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();
            return RedirectToAction("DisplayFeedbacks");
        }


        //users
        [HttpGet]
        public ActionResult WriteFeedback()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WriteFeedback(Feedback feedback)
        {

            feedback.DSetId = WebSettings.getCurrentDate();
            feedback.SpamMessage = false;
            ent.Feedbacks.Add(feedback);
            ent.SaveChanges();
            return RedirectToAction("DisplayAnnouncements", "WebContent");
        }

    }
}