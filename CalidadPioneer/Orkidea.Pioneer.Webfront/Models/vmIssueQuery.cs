using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmIssueQuery
    {
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public bool abierto { get; set; }
        public bool cerrado { get; set; }
    }
}