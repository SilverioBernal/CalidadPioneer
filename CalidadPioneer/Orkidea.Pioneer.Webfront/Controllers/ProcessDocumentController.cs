using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
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
                    ruta = item.ruta
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
                processDocumentBiz.GetProcessDocumentListByProcessDocumentType(processDocument)
                .OrderBy(x => x.nombre).ToList();

            return View(lstProcessDocument);
        }

        //
        // GET: /ProcessDocument/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
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
        public ActionResult Create(ProcessDocument oProcessDocument, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(oProcessDocument.descripcion))
                    oProcessDocument.descripcion = "";

                if (files != null)
                {
                    FileTypeBiz fileTypeBiz = new FileTypeBiz();
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file.FileName != null)
                        {
                            string physicalPath = HttpContext.Server.MapPath("~") + "UploadedFiles" + "\\";
                            string fileName = Guid.NewGuid().ToString() + fileTypeBiz.GetFileTypebyKey(new FileType() { tipoMIME = file.ContentType }).extension;

                            using (Stream output = System.IO.File.OpenWrite(physicalPath + fileName))
                            using (Stream input = file.InputStream)
                            {
                                input.CopyTo(output);

                                oProcessDocument.ruta = fileName;
                            }
                        }
                    }
                    processDocumentBiz.SaveProcessDocument(oProcessDocument);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message + ex.StackTrace + ex.InnerException.Message;
                ProcessBiz processBiz = new ProcessBiz();
                DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();

                vmProcessDocument processDocument = new vmProcessDocument()
                {
                    descripcion = oProcessDocument.descripcion,
                    desProceso = oProcessDocument.descripcion,
                    idProceso = oProcessDocument.idProceso,
                    idTipoDocumento = oProcessDocument.idTipoDocumento,
                    nombre = oProcessDocument.nombre,
                    ruta = oProcessDocument.ruta
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
                ruta = oProcessDocument.ruta
            };

            processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
            processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

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

                if (files != null)
                {
                    FileTypeBiz fileTypeBiz = new FileTypeBiz();
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file.FileName != null)
                        {
                            string physicalPath = HttpContext.Server.MapPath("~") + "UploadedFiles" + "\\";
                            string fileName = Guid.NewGuid().ToString() + fileTypeBiz.GetFileTypebyKey(new FileType() { tipoMIME = file.ContentType }).extension;

                            using (Stream output = System.IO.File.OpenWrite(physicalPath + fileName))
                            using (Stream input = file.InputStream)
                            {
                                input.CopyTo(output);

                                oProcessDocument.ruta = fileName;
                            }
                        }
                    }
                    oProcessDocument.id = id;
                    processDocumentBiz.SaveProcessDocument(oProcessDocument);
                }

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
                    nombre = oProcessDocument.nombre,
                    ruta = oProcessDocument.ruta
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
                ruta = oProcessDocument.ruta
            };

            processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
            processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

            return View(processDocument);
        }

        //
        // POST: /ProcessDocument/Delete/5
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id, ProcessDocument oProcessDocument)
        {
            try
            {
                oProcessDocument.id = id;
                processDocumentBiz.DeleteProcessDocument(oProcessDocument);
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
                    nombre = oProcessDocument.nombre,
                    ruta = oProcessDocument.ruta
                };

                processDocument.lstProcess = processBiz.GetProcessList().OrderBy(x => x.nombre).ToList();
                processDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

                return View(processDocument);
            }
        }        
    }
}
