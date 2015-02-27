using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmProcessDocument
    {
        public int id { get; set; }

        [Display(Name = "Proceso")]
        [Required(ErrorMessage = "Required")]
        public int idProceso { get; set; }
        
        [Display(Name = "Tipo de documento")]
        [Required(ErrorMessage = "Required")]        
        public int idTipoDocumento { get; set; }
        
        [Display(Name = "Nombre del documento")]
        [Required(ErrorMessage = "Required")]
        public string nombre { get; set; }
        
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        
        [Display(Name = "Proceso")]
        public string desProceso { get; set; }
        
        [Display(Name = "Tipo de documento")]
        public string desTipo { get; set; }
        
        [Display(Name = "Relacionado")]
        public bool relacionado { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }
        
        public List<Process> lstProcess { get; set; }
        
        public List<DocumentType> lstDocType { get; set; }
        
        public vmProcessDocument()
        {
            lstDocType = new List<DocumentType>();
            lstProcess = new List<Process>();
        }
    }
}