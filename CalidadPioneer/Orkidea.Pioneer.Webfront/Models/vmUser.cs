using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
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
        [Required(ErrorMessage = "Requerido")]
        public string usuario { get; set; }

        [Display(Name = "Clave")]
        [Required(ErrorMessage = "Requerido")]
        public string clave { get; set; }

        public bool admin { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Requerido")]
        public int? idCargo { get; set; }

        public string desCargo { get; set; }

        public List<Position> lsPosition { get; set; }

        public vmUser()
        {
            //lsPosition = 
        }

        public vmUser(bool getPositions)
        {
            if (getPositions)
            {
                PositionBiz PositionBiz = new PositionBiz();

                lsPosition = PositionBiz.GetPositionList();
            }
        }
    }
}