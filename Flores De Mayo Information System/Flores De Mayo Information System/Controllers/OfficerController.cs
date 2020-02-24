using Flores_De_Mayo_Information_System.Actions;
using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flores_De_Mayo_Information_System.Controllers
{
    public class OfficerController : Controller
    {
        private DBEntities ent = new DBEntities();

        [HttpGet]
        [Authorize]
        public ActionResult RegisterOfficer()
        {
            int datId = WebSettings.getCurrentDate();
            int approvedId = ent.ApplicationStatusTypes.Where(m=> m.Name.ToLower() == "approved").Select(m=>m.ASTypId).FirstOrDefault();
            int appTypeId = ent.ApplicationTypes.Where(m => m.Name.ToLower() == "officer").Select(m => m.ATypeId).FirstOrDefault();
            List<int> registeredSinisianIds = ent.AspNetUsers.Select(m => (int)m.SinId).ToList();

            return View( ent.Applications.Where( m=> !registeredSinisianIds.Contains(m.SinId) && m.DSetIdApplied == datId && m.ASTypId == approvedId && m.ATypeId == appTypeId ).ToList() );
        }

        [HttpGet]
        [Authorize]
        public ActionResult DisplayCreateOfficer()
        {

            List<string> maleId = ent.OfficerPartners.Select(m => m.UserIdMale).ToList();
            List<string> femaleId = ent.OfficerPartners.Select(m => m.UserIdFemale).ToList();

            return View(ent.AspNetUsers.Where( m => !maleId.Contains( m.Id ) && !femaleId.Contains(m.Id) ).ToList());
        }

        [HttpPost]
        [Authorize]
        public ActionResult DisplayCreateOfficer(string maleId, string femaleId)
        {
            if(maleId == null && femaleId == null)
            {
                return View();
            }

            OfficerPartner partner = new OfficerPartner()
            {
                UserIdMale = maleId,
                UserIdFemale = femaleId,
                IsPresident = false
            };

            ent.OfficerPartners.Add(partner);
            ent.SaveChanges();


            //instead of incrementing, the new OParId is used as the rank basis. It is asured that the new id will be the highest
            //higher the rank means the pair is new officers
            int? highestRank = partner.OParId;

            ent.OfficerHierarchies.Add(
                new OfficerHierarchy()
                {
                    OParId = partner.OParId,
                    Rank = (int)highestRank
                }
                );

            ent.SaveChanges();

            return RedirectToAction("DisplayCreateOfficer");

        }

        [HttpGet]
        [Authorize]
        public ActionResult PromotePresident()
        {
            List<int> pareIds = ent.OfficerHierarchies.Select(m => m.OParId).ToList();

            //will return only the officers that are in the hierarchies, meaning sila yung may partner
            return View( ent.OfficerPartners.Where(m=> pareIds.Contains(m.OParId)).ToList() );
        }

        [Authorize]
        public ActionResult Promote(int? oParId)
        {

            //get the old president and change to false and remove them from the hierarchy
            OfficerPartner presidents = ent.OfficerPartners.Where(m => m.IsPresident == true).FirstOrDefault();
            
            if(presidents != null)
            {
                presidents.IsPresident = false;
                ent.Entry(presidents).State = System.Data.Entity.EntityState.Modified;
                ent.SaveChanges();

                //if the president is done, meaning they are not officers anymore
                OfficerHierarchy hierarchy = ent.OfficerHierarchies.Where(m => m.OParId == presidents.OParId).FirstOrDefault();
                ent.OfficerHierarchies.Remove(hierarchy);
                ent.SaveChanges();
            }

            OfficerPartner newPresidents = ent.OfficerPartners.Where(m => m.OParId == oParId).FirstOrDefault();
            newPresidents.IsPresident = true;
            ent.Entry(newPresidents).State = System.Data.Entity.EntityState.Modified;
            ent.SaveChanges();

            return RedirectToAction("PromotePresident");
        }

        [HttpGet]
        public ActionResult DisplayOfficerChart()
        {
            return View(ent.OfficerHierarchies.OrderBy(m => m.Rank).ToList());
        }
    }
}