using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Flores_De_Mayo_Information_System.Models.Application
{
    public class CreateApplicationViewModel
    {

        [Required]
        [Display(Name = "Application For")]
        public string ApplicationType { get; set; } // contains the type of application, sagala or officer

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


    }
}