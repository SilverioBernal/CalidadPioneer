using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmNews
    {
        public int id { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "Required")]
        public System.DateTime fecha { get; set; }

        [Display(Name = "Titulo")]
        [Required(ErrorMessage = "Required")]
        public string titulo { get; set; }

        [Display(Name = "Contenido")]
        [Required(ErrorMessage = "Required")]
        public string contenido { get; set; }
    }
}
