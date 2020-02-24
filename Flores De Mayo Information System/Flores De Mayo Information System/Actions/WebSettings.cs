using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flores_De_Mayo_Information_System.Actions
{
    public class WebSettings
    {

        public static int getCurrentDate()
        {
            return new DBEntities().WebSettings.Select(m => m.DSetId).FirstOrDefault();

        }

        public static bool IsApplicationOpen()
        {
            return new DBEntities().WebSettings.Select(m => m.ApplicationOpen).FirstOrDefault();
        }

    }
}