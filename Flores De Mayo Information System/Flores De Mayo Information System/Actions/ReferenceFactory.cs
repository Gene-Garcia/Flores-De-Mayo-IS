using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flores_De_Mayo_Information_System.Actions
{
    public class ReferenceFactory
    {

        private static Random random = new Random();
        public static string GenerateReference(string name, int length = 14)
        {
            string chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789*&%$@#!" + name + DateTime.Now.ToString();
            return new string(Enumerable.Repeat(chars, 14)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}