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
    public class ProjectDocumentController : Controller
    {
        ProjectDocumentBiz projectDocumentBiz = new ProjectDocumentBiz();

        //
        // GET: /ProcessDocument/
        [Authorize]
        public ActionResult Index()
        {
            ProjectBiz projectBiz = new ProjectBiz();
            DocumentTypeBiz docTypeBiz = new DocumentTypeBiz();

            List<Project> projectList = projectBiz.GetProjectList();
            List<DocumentType> documentTypeList = docTypeBiz.GetDocumentTypeList();


            List<ProjectDocument> lstProjectDocument = projectDocumentBiz.GetProjectDocumentList().OrderBy(x => x.idProyecto).ToList();

            List<vmProjectDocument> lstProjectDocumentRes = new List<vmProjectDocument>();

            foreach (ProjectDocument item in lstProjectDocument)
            {
                vmProjectDocument projectDoc = new vmProjectDocument()
                {
                    id = item.id,
                    descripcion = item.descripcion,
                    idProyecto = item.idProyecto,
                    idTipoDocumento = item.idTipoDocumento,
                    nombre = item.nombre,

                };

                projectDoc.desProyecto = projectList.Where(x => x.id.Equals(item.idProyecto)).FirstOrDefault().nombre;//processBiz.GetProcessbyKey(new Process() { id = item.idProceso }).nombre;
                projectDoc.desTipo = documentTypeList.Where(x => x.id.Equals(item.idTipoDocumento)).FirstOrDefault().nombre;//docTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = item.idTipoDocumento }).nombre;

                lstProjectDocumentRes.Add(projectDoc);
            }
            return View(lstProjectDocumentRes);
        }

        [Authorize]
        public ActionResult IndexByProject(int idProyecto, int idTipoDocumento)
        {
            ProjectDocument projectDocument = new ProjectDocument() { idProyecto = idProyecto, idTipoDocumento = idTipoDocumento };

            List<ProjectDocument> lstProjectDocument =
                projectDocumentBiz.GetProjectDocumentList(projectDocument).OrderBy(x => x.nombre).ToList();

            return View(lstProjectDocument);
        }

        //
        // GET: /ProcessDocument/Create
        [Authorize]
        public ActionResult Create()
        {
            ProjectBiz projectBiz = new ProjectBiz();
            DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();

            vmProjectDocument projectDocument = new vmProjectDocument();
            projectDocument.lstProject = projectBiz.GetProjectList().OrderBy(x => x.nombre).ToList();
            projectDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

            return View(projectDocument);
        }

        //
        // POST: /ProcessDocument/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(vmProjectDocument oProjectDocument, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                string fileExtension = Path.GetExtension(oProjectDocument.File.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;

                AzureStorageHelper.uploadFile(oProjectDocument.File.InputStream, fileName, "uploadedFiles");

                projectDocumentBiz.SaveProjectDocument(new ProjectDocument()
                {
                    idTipoDocumento = oProjectDocument.idTipoDocumento,
                    idProyecto = oProjectDocument.idProyecto,
                    nombre = fileName,
                    descripcion = string.IsNullOrEmpty(oProjectDocument.descripcion) ? "" : oProjectDocument.descripcion
                });
                ViewBag.menu = "medios";


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message + ex.StackTrace + ex.InnerException.Message;
                ProjectBiz projectBiz = new ProjectBiz();
                DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();
                vmProjectDocument projectDocument = new vmProjectDocument()
                {                    
                    idProyecto = oProjectDocument.idProyecto,
                    idTipoDocumento = oProjectDocument.idTipoDocumento,
                    descripcion = string.IsNullOrEmpty(oProjectDocument.descripcion) ? "" : oProjectDocument.descripcion
                };
                projectDocument.lstProject = projectBiz.GetProjectList().OrderBy(x => x.nombre).ToList();
                projectDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();
                return View(projectDocument);
            }
        }

        //
        // GET: /ProcessDocument/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            ProjectBiz projectBiz = new ProjectBiz();
            DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();
            ProjectDocument oProjectDocument = projectDocumentBiz.GetProjectDocumentbyKey(new ProjectDocument() { id = id });
            Project project = projectBiz.GetProjectbyKey(new Project() { id = oProjectDocument.idProyecto });
            DocumentType documentType = documentTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = oProjectDocument.idTipoDocumento });

            vmProjectDocument projectDocument = new vmProjectDocument()
            {
                id = id,
                descripcion = oProjectDocument.descripcion,
                desProyecto = project.nombre,
                idProyecto = oProjectDocument.idProyecto,
                idTipoDocumento = oProjectDocument.idTipoDocumento,
                desTipo = documentType.nombre,
                nombre = oProjectDocument.nombre,
            };

            projectDocument.lstProject = projectBiz.GetProjectList().OrderBy(x => x.nombre).ToList();
            projectDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

            ViewBag.idPadre = id;

            return View(projectDocument);
        }

        //
        // POST: /ProcessDocument/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, ProjectDocument oProjectDocument, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(oProjectDocument.descripcion))
                    oProjectDocument.descripcion = "";

                oProjectDocument.id = id;
                projectDocumentBiz.SaveProjectDocument(oProjectDocument);

                return RedirectToAction("Index");
            }
            catch
            {
                ProcessBiz processBiz = new ProcessBiz();
                DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();
                ProjectBiz projectBiz = new ProjectBiz();
                vmProjectDocument projectDocument = new vmProjectDocument()
                {
                    id = id,
                    descripcion = string.IsNullOrEmpty(oProjectDocument.descripcion) ? "" : oProjectDocument.descripcion,                   
                    idProyecto= oProjectDocument.idProyecto,
                    idTipoDocumento = oProjectDocument.idTipoDocumento,
                    nombre = oProjectDocument.nombre
                };

                projectDocument.lstProject = projectBiz.GetProjectList().OrderBy(x => x.nombre).ToList();
                projectDocument.lstDocType = documentTypeBiz.GetDocumentTypeList().OrderBy(x => x.nombre).ToList();

                return View(projectDocument);
            }
        }

        //
        // GET: /ProcessDocument/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            projectDocumentBiz.DeleteProjectDocument(new ProjectDocument() { id = id });
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

        public ActionResult ShowRelations(int id)
        {
            ProjectDocument oProjectDocument = projectDocumentBiz.GetProjectDocumentbyKey(new ProjectDocument() { id = id });
            List<ProjectDocument> lsProjectDocument = projectDocumentBiz.GetProjectDocumentList(oProjectDocument.idProyecto).Where(x => x.id != id).ToList();
            List<ProjectLinkedDoc> lsLinkedDocs = projectDocumentBiz.GetProjectLinkedDocList(id);

            List<ProjectDocument> lsDocs = new List<ProjectDocument>();

            foreach (ProjectLinkedDoc item in lsLinkedDocs)
            {
                ProjectDocument document = new ProjectDocument()
                {
                    id = item.idHijo,
                    descripcion = lsProjectDocument.Where(x => x.id.Equals(item.idHijo)).Select(x => x.descripcion).FirstOrDefault(),
                    nombre = lsProjectDocument.Where(x => x.id.Equals(item.idHijo)).Select(x => x.nombre).FirstOrDefault()
                };

                lsDocs.Add(document);
            }

            return View(lsDocs);
        }
    }
}
