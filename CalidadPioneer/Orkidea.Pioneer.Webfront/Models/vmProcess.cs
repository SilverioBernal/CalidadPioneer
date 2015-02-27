using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmProcess
    {
        public int id { get; set; }
        [Display(Name = "Nombre del proceso")]
        [Required(ErrorMessage = "Required")]
        public string nombre { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Archivo de caracterización")]
        [Required(ErrorMessage = "Required")]
        public string archivoCaracterizacion { get; set; }

        public List<DocumentType> lstDocumentType { get; set; }

        public List<vmProject> lsProject { get; set; }
        public vmProcess()
        {
            lstDocumentType = new List<DocumentType>();

            lsProject = new List<vmProject>();
        }
    }
}