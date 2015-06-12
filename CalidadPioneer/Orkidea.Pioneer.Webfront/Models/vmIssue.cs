using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class vmIssue : Issue
    {
        public List<Drill> lsUbicaion { get; set; }

        public string descripcionRig { get; set; }
        
        public string[] lsFuentes { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        public vmIssue()
        {

        }

        public vmIssue(bool getParameters)
        {
            NearMissParameterBiz nmPar = new NearMissParameterBiz();
            List<NearMissParameter> lsNMPar = nmPar.GetNearMissParameterList();

            lsFuentes = (lsNMPar.Where(x => x.parametro == "FuentesHallazgos").First()).valor.Split('|');

            DrillBiz drillBiz = new DrillBiz();
            lsUbicaion = new List<Drill>();
            lsUbicaion = drillBiz.GetDrillList();
        }

        public vmIssue(int idIssue)
        {
            IssueBiz issueBiz = new IssueBiz();

            Issue oIssue = issueBiz.GetIssuebyKey(new Issue() { id = idIssue });

            idTaladro = oIssue.idTaladro;
            fuente = oIssue.fuente;
            descripcion = oIssue.descripcion;
            nombreReportador = oIssue.nombreReportador;
            cargoReportador = oIssue.cargoReportador;
            empresaReportador = oIssue.empresaReportador;            
            fechaReporte = oIssue.fechaReporte;
            fotoAntes = oIssue.fotoAntes;
            fotoDespues = oIssue.fotoDespues;
            comentariosCierre = oIssue.comentariosCierre;
            fechaCierre = oIssue.fechaCierre;
            id = oIssue.id;

            NearMissParameterBiz nmPar = new NearMissParameterBiz();
            List<NearMissParameter> lsNMPar = nmPar.GetNearMissParameterList();

            lsFuentes = (lsNMPar.Where(x => x.parametro == "FuentesHallazgos").First()).valor.Split('|');

            DrillBiz drillBiz = new DrillBiz();
            lsUbicaion = new List<Drill>();
            lsUbicaion = drillBiz.GetDrillList();

            IssueDetail = issueBiz.getIssueDetailList(idIssue);
        }
    }
}