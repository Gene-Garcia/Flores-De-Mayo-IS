using Flores_De_Mayo_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flores_De_Mayo_Information_System.Actions
{
    public class Searcher
    {
        private DBEntities ent = new DBEntities();
        public SinisianDatabase FindInSinisian(string fName, string mName, string lName)
        {

            SinisianDatabase sinisianDb = new SinisianDatabase();

            sinisianDb = ent.SinisianDatabases.Where(m => m.FirstName.ToLower() == fName && m.MiddleName.ToLower() == mName && m.LastName.ToLower() == lName).FirstOrDefault();

            return sinisianDb;
        }

    }
}