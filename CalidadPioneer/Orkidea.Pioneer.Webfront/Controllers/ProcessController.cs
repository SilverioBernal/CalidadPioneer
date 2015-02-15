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
    public class ProcessController : Controller
    {
        ProcessBiz processBiz = new ProcessBiz();
        //
        // GET: /Process/
        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }
        //[Authorize]
        public ActionResult List()
        {
            List<Process> lstProcess = processBiz.GetProcessList();
            return View(lstProcess);
        }

        //
        // GET: /Process/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            ProcessDocumentBiz processDocumentBiz = new ProcessDocumentBiz();
            Process process = processBiz.GetProcessbyKey(new Process() { id = id });
            vmProcess oProcess = new vmProcess() { id = id, nombre = process.nombre, descripcion = process.descripcion, archivoCaracterizacion = process.archivoCaracterizacion };

            oProcess.lstDocumentType = processDocumentBiz.GetDocumentTypeByProcess(process);

            return View(oProcess);
        }

        //
        // GET: /Process/Create
        [Authorize]
        public ActionResult Create()
        {
            vmProcess process = new vmProcess();
            return View(process);
        }

        //
        // POST: /Process/Create
        [HttpPost]
        public ActionResult Create(Process process, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(process.descripcion))
                    process.descripcion = "";

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

                                process.archivoCaracterizacion = fileName;
                            }
                        }
                    }
                }

                processBiz.SaveProcess(process);
                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Process/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Process process = processBiz.GetProcessbyKey(new Process() { id = id });

            vmProcess oProcess = new vmProcess() { id = process.id, archivoCaracterizacion = process.archivoCaracterizacion, descripcion = process.descripcion, nombre = process.nombre };
            ViewBag.idProcess = id;

            return View(oProcess);
        }

        //
        // POST: /Process/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Process process, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(process.descripcion))
                    process.descripcion = "";

                Process oProcess = processBiz.GetProcessbyKey(new Process() { id = id });

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

                                process.archivoCaracterizacion = fileName;
                            }
                        }
                    }
                }
                //if (Request.ContentLength > 0)
                //{
                //    FileTypeBiz fileTypeBiz = new FileTypeBiz();
                //    UploadedFile file = RetrieveFileFromRequest();

                //    if (!string.IsNullOrEmpty(file.Filename))                                                          
                //        process.archivoCaracterizacion += fileTypeBiz.GetFileTypebyKey(new FileType() { tipoMIME = file.ContentType }).extension;                    
                //    else
                //        process.archivoCaracterizacion = oProcess.archivoCaracterizacion;
                //}
                //else
                //    process.archivoCaracterizacion = oProcess.archivoCaracterizacion;

                process.id = id;
                processBiz.SaveProcess(process);
                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Process/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Process process = processBiz.GetProcessbyKey(new Process() { id = id });

            vmProcess oProcess = new vmProcess() { id = process.id, descripcion = process.descripcion, nombre = process.nombre };
            ViewBag.idProcess = id;

            return View(oProcess);
        }

        //
        // POST: /Process/Delete/5
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id, Process process)
        {

            {
                try
                {
                    process.id = id;
                    processBiz.DeleteProcess(process);
                    return RedirectToAction("List");
                }
                catch
                {
                    return View();
                }
            }
        }        
    }
}
