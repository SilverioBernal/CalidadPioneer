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
    public class ProcessLinkedDocController : Controller
    {
        ProcessDocumentBiz processDocumentBiz = new ProcessDocumentBiz();
        
        public ActionResult Index(int id)
        {
            ProcessDocument oProcessDocument = processDocumentBiz.GetProcessDocumentbyKey(new ProcessDocument() { id = id });
            List<ProcessDocument> lsProcessDocument = processDocumentBiz.GetProcessDocumentList(oProcessDocument.idProceso).Where(x => x.id != id).ToList();
            List<ProcessLinkedDoc> lsLinkedDocs = processDocumentBiz.GetProcessChildLinkedDocList(id);

            List<vmProcessDocument> lsDocs = new List<vmProcessDocument>();

            foreach (ProcessDocument item in lsProcessDocument)
            {
                vmProcessDocument document = new vmProcessDocument()
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

            processDocumentBiz.SaveProcessLinkedDocument(new ProcessLinkedDoc() { idPadre = int.Parse(padre), idHijo = int.Parse(hijo) });

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnLinkDocument(string id)
        {
            string[] datos = id.Split('|');
            string padre = datos[0];
            string hijo = datos[1];

            processDocumentBiz.DeleteProcessLinkedDocument(new ProcessLinkedDoc() { idPadre = int.Parse(padre), idHijo = int.Parse(hijo) });

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }
    }
}
