//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Flores_De_Mayo_Information_System.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Feedback
    {
        public int FId { get; set; }


        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string Message { get; set; }
        public int DSetId { get; set; }
        public bool SpamMessage { get; set; }
    
        public virtual DateSetting DateSetting { get; set; }
    }
}
