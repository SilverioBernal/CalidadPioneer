using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmNearMiss:NearMiss
    {
        public string[] lsAnalisisAsuntosAmbientales { get; set; }
        public string[] lsAnalisisCalidad { get; set; }
        public string[] lsAnalisisCondicionesTrabajo { get; set; }
        public string[] lsAnalisisSalud { get; set; }
        public string[] lsAnalisisUsoEpp { get; set; }
        public string[] lsReportador { get; set; }
        public string[] lsTipoHallazgo { get; set; }
        public string[] lsClasificacion { get; set; }
        public List<Drill> lsUbicaion { get; set; }
        public string usuarioCrea { get; set; }
        public string descripcionRig { get; set; }
        public vmNearMiss()
        {
            lsUbicaion = new List<Drill>();
        }
        public vmNearMiss(bool getParameters)
        {
            NearMissParameterBiz nmPar = new NearMissParameterBiz();
            List<NearMissParameter> lsNMPar = nmPar.GetNearMissParameterList();

            lsAnalisisAsuntosAmbientales = (lsNMPar.Where(x => x.parametro == "AnalisisAsuntosAmbientales").First()).valor.Split('|');            
            lsAnalisisCalidad = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "AnalisisCalidad").First()).valor.Split('|');
            lsAnalisisCondicionesTrabajo = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "AnalisisCondicionesTrabajo").First()).valor.Split('|');
            lsAnalisisSalud = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "AnalisisSalud").First()).valor.Split('|');
            lsAnalisisUsoEpp = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "AnalisisUsoEpp").First()).valor.Split('|');
            lsReportador = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "Reportador").First()).valor.Split('|');
            lsTipoHallazgo = nmPar.GetNearMissParameterbyKey(lsNMPar.Where(x => x.parametro == "TipoHallazgo").First()).valor.Split('|');
            lsClasificacion = ("Alto|medio|Bajo").Split('|');
            DrillBiz drillBiz = new DrillBiz();
            lsUbicaion = new List<Drill>();
            lsUbicaion = drillBiz.GetDrillList();
        }
    }
}