using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmUser
    {
        public int id { get; set; }
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Required")]
        public string usuario { get; set; }
        public string clave { get; set; }
        public bool admin { get; set; }
    }
}