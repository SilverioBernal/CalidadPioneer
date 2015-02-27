using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using Orkidea.Pioneer.Webfront.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class ProcessDocumentController : Controller
    {
        ProcessDocumentBiz processDocumentBiz = new ProcessDocumentBiz();

        //
        // GET: /ProcessDocument/
        [Authorize]
        public ActionResult Index()
        {
            ProcessBiz processBiz = new ProcessBiz();
            DocumentTypeBiz docTypeBiz = new DocumentTypeBiz();

            List<Process> processList = processBiz.GetProcessList();
            List<DocumentType> documentTypeList = docTypeBiz.GetDocumentTypeList();


            List<ProcessDocument> lstProcessDocument = processDocumentBiz.GetProcessDocumentList().OrderBy(x => x.idProceso).ToList();

            List<vmProcessDocument> lstProcessDocumentRes = new List<vmProcessDocument>();

            foreach (ProcessDocument item in lstProcessDocument)
            {
                vmProcessDocument processDoc = new vmProcessDocument()
                {
                    id = item.id,
                    descripcion = item.descripcion,
                    idProceso = item.idProceso,
                    idTipoDocumento = item.idTipoDocumento,
                    nombre = item.nombre,

                };

                processDoc.desProceso = processList.Where(x => x.id.Equals(item.idProceso)).FirstOrDefault().nombre;//processBiz.GetProcessbyKey(new Process() { id = item.idProceso }).nombre;
                processDoc.desTipo = documentTypeList.Where(x => x.id.Equals(item.idTipoDocumento)).FirstOrDefault().nombre;//docTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = item.idTipoDocumento }).nombre;

                lstProcessDocumentRes.Add(processDoc);
            }
            return View(lstProcessDocumentRes);
        }

        public ActionResult IndexByProcess(int idProceso, int idTipoDocumento)
        {
            ProcessDocument processDocument = new ProcessDocument() { idProceso = idProceso, idTipoDocumento = idTipoDocumento };

            List<ProcessDocument> lstProcessDocument =
                processDocumentBiz.GetProcessDocumentList(processDocument)
                .OrderBy(x => x.nombre).ToList();

            return View(lstProcessDocument);
        }

        //
        // GET: /ProcessDocument/Create
        [Authorize]
        public ActionResult Create()
        {
            ProcessBiz processBiz = new ProcessBiz();
            DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();

            vmProcessDocument processDocument = new vmProcessDocument();
            processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
            processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

            return View(processDocument);
        }

        //
        // POST: /ProcessDocument/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(vmProcessDocument oProcessDocument, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {                
                string fileExtension = Path.GetExtension(oProcessDocument.File.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;                

                AzureStorageHelper.uploadFile(oProcessDocument.File.InputStream, fileName, "uploadedFiles");

                processDocumentBiz.SaveProcessDocument(new ProcessDocument()
                {
                    idTipoDocumento = oProcessDocument.idTipoDocumento,
                    idProceso = oProcessDocument.idProceso,
                    nombre = fileName,
                    descripcion = string.IsNullOrEmpty(oProcessDocument.descripcion) ? "" : oProcessDocument.descripcion
                });
                ViewBag.menu = "medios";


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message + ex.StackTrace + ex.InnerException.Message;
                ProcessBiz processBiz = new ProcessBiz();
                DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();
                vmProcessDocument processDocument = new vmProcessDocument()
                {
                    desProceso = oProcessDocument.desProceso,
                    idProceso = oProcessDocument.idProceso,
                    idTipoDocumento = oProcessDocument.idTipoDocumento,
                    descripcion = string.IsNullOrEmpty(oProcessDocument.descripcion) ? "" : oProcessDocument.descripcion
                };
                processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
                processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();
                return View(processDocument);
            }
        }

        //
        // GET: /ProcessDocument/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            ProcessBiz processBiz = new ProcessBiz();
            DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();
            ProcessDocument oProcessDocument = processDocumentBiz.GetProcessDocumentbyKey(new ProcessDocument() { id = id });
            Process process = processBiz.GetProcessbyKey(new Process() { id = oProcessDocument.idProceso });
            DocumentType documentType = documentTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = oProcessDocument.idTipoDocumento });

            vmProcessDocument processDocument = new vmProcessDocument()
            {
                id = id,
                descripcion = oProcessDocument.descripcion,
                desProceso = process.nombre,
                idProceso = oProcessDocument.idProceso,
                idTipoDocumento = oProcessDocument.idTipoDocumento,
                desTipo = documentType.nombre,
                nombre = oProcessDocument.nombre,
            };

            processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
            processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

            ViewBag.idPadre = id;

            return View(processDocument);
        }

        //
        // POST: /ProcessDocument/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, ProcessDocument oProcessDocument, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(oProcessDocument.descripcion))
                    oProcessDocument.descripcion = "";

                oProcessDocument.id = id;
                processDocumentBiz.SaveProcessDocument(oProcessDocument);

                return RedirectToAction("Index");
            }
            catch
            {
                ProcessBiz processBiz = new ProcessBiz();
                DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();

                vmProcessDocument processDocument = new vmProcessDocument()
                {
                    id = id,
                    descripcion = oProcessDocument.descripcion,
                    desProceso = oProcessDocument.descripcion,
                    idProceso = oProcessDocument.idProceso,
                    idTipoDocumento = oProcessDocument.idTipoDocumento,
                    nombre = oProcessDocument.nombre
                };

                processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
                processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

                return View(processDocument);
            }
        }

        //
        // GET: /ProcessDocument/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            processDocumentBiz.DeleteProcessDocument(new ProcessDocument() { id = id });
            return RedirectToAction("Index");
        }

        public ActionResult Details(string archivo)
        {
            string mimeType = "";
            int cuentaPuntos = 0;
            string[] nombreArchivo = archivo.Split('.');
            cuentaPuntos = nombreArchivo.Length;
            mimeType = MimeTypeBiz.GetMimeType(nombreArchivo[cuentaPuntos - 1]).mimetype1;
            //dynamically generate a file
            System.IO.MemoryStream ms;
            ms = AzureStorageHelper.getFile(archivo, "uploadedFiles");
            // return the file
            return File(ms.ToArray(), mimeType, archivo);
        }

        public ActionResult ShowRelations(int id) {
            ProcessDocument oProcessDocument = processDocumentBiz.GetProcessDocumentbyKey(new ProcessDocument() { id = id });
            List<ProcessDocument> lsProcessDocument = processDocumentBiz.GetProcessDocumentList(oProcessDocument.idProceso).Where(x => x.id != id).ToList();
            List<ProcessLinkedDoc> lsLinkedDocs = processDocumentBiz.GetProcessLinkedDocList(id);

            List<ProcessDocument> lsDocs = new List<ProcessDocument>();

            foreach (ProcessLinkedDoc item in lsLinkedDocs)
            {
                ProcessDocument document = new ProcessDocument()
                {
                    id = item.idHijo,                  
                    descripcion = lsProcessDocument.Where(x => x.id.Equals(item.idHijo)).Select(x => x.descripcion).FirstOrDefault(),
                    nombre = lsProcessDocument.Where(x => x.id.Equals(item.idHijo)).Select(x => x.nombre).FirstOrDefault()
                };

                lsDocs.Add(document);
            }

            return View(lsDocs);
        }
    }
}
