using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmProject
    {
        public int id { get; set; }
        [Display(Name = "Nombre proyecto")]
        [Required(ErrorMessage = "Requerido")]
        public string nombre { get; set; }        

        [Display(Name = "Nombre proceso")]
        [Required(ErrorMessage = "Requerido")]
        public int idProceso { get; set; }

        public string nombreProceso { get; set; }

        public List<DocumentType> lstDocumentType { get; set; }

        public List<Process> lstProcess { get; set; }

        public vmProject()
        {
            lstDocumentType = new List<DocumentType>();
            lstProcess = new List<Process>();
        }
    }
}