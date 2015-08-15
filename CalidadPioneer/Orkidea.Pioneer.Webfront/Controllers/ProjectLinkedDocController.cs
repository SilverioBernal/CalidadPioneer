using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Webfront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class ProjectLinkedDocController : Controller
    {
        ProjectDocumentBiz projectDocumentBiz = new ProjectDocumentBiz();

        public ActionResult Index(int id)
        {
            ProjectDocument oProjectDocument = projectDocumentBiz.GetProjectDocumentbyKey(new ProjectDocument() { id = id });
            List<ProjectDocument> lsProjectDocument = projectDocumentBiz.GetProjectDocumentList(oProjectDocument.idProyecto).Where(x => x.id != id).ToList();
            List<ProjectLinkedDoc> lsLinkedDocs = projectDocumentBiz.GetProjectLinkedDocList(id);

            List<vmProjectDocument> lsDocs = new List<vmProjectDocument>();

            foreach (ProjectDocument item in lsProjectDocument)
            {
                vmProjectDocument document = new vmProjectDocument()
                {
                    id = item.id,
                    descripcion = item.descripcion
                };

                if (lsLinkedDocs.Where(x => x.idHijo.Equals(item.id)).Count() != 0)
                    document.relacionado = true;

                lsDocs.Add(document);
            }

            return View(lsDocs);
        }

        public JsonResult LinkDocument(string id)
        {
            string[] datos = id.Split('|');
            string padre = datos[0];
            string hijo = datos[1];

            projectDocumentBiz.SaveProjectLinkedDocument(new ProjectLinkedDoc() { idPadre = int.Parse(padre), idHijo = int.Parse(hijo) });

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnLinkDocument(string id)
        {
            string[] datos = id.Split('|');
            string padre = datos[0];
            string hijo = datos[1];

            projectDocumentBiz.DeleteProjectLinkedDocument(new ProjectLinkedDoc() { idPadre = int.Parse(padre), idHijo = int.Parse(hijo) });

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }
    }
}
