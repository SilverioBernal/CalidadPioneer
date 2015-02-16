using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmFileUpload
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public string nombre { get; set; }
    }
}