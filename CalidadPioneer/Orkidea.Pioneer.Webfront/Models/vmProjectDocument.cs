using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmProjectDocument
    {
        public int id { get; set; }

        [Display(Name = "Id Proyecto")]
        [Required(ErrorMessage = "Required")]
        public int idProyecto { get; set; }

        [Display(Name = "Tipo de documento")]
        [Required(ErrorMessage = "Required")]
        public int idTipoDocumento { get; set; }

        [Display(Name = "Nombre del documento")]
        [Required(ErrorMessage = "Required")]
        public string nombre { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        
        [Display(Name = "Tipo de documento")]
        public string desTipo { get; set; }

        [Display(Name = "Proyecto")]
        public string desProyecto { get; set; }

        [Display(Name = "Relacionado")]
        public bool relacionado { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        public List<Project> lstProject { get; set; }

        public List<DocumentType> lstDocType { get; set; }

        public vmProjectDocument()
        {
            lstDocType = new List<DocumentType>();
            lstProject = new List<Project>();
        }
    }
}