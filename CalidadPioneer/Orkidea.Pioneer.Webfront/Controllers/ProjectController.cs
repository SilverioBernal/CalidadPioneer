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
    public class ProjectController : Controller
    {
        ProjectBiz projectBiz = new ProjectBiz();
        ProcessBiz processBiz = new ProcessBiz();
        ProcessDocumentBiz processDocumentBiz = new ProcessDocumentBiz();
        ProjectDocumentBiz projectDocumentBiz = new ProjectDocumentBiz();
        //
        // GET: /Project/
        [Authorize]
        public ActionResult Index(int id)
        {
            #region User identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;

            int user = 0;

            if (context.IsAuthenticated)
            {

                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                user = int.Parse(userRole[0]);
            }

            #endregion

            Process process = processBiz.GetProcessbyKey(new Process() { id = id });
            List<Project> lstProject = projectBiz.GetProjectList().Where(x => x.idProceso.Equals(id)).ToList();

            vmProcess oProcess = new vmProcess() { id = id, nombre = process.nombre, descripcion = process.descripcion, archivoCaracterizacion = process.archivoCaracterizacion };

            oProcess.lstDocumentType = processDocumentBiz.GetDocumentTypeByProcess(process);

            foreach (Project item in lstProject)
            {
                vmProject project = new vmProject() { id = item.id, nombre = item.nombre };
                project.lstDocumentType = projectDocumentBiz.GetProjectDocumentTypeListByProject(item);

                oProcess.lsProject.Add(project);
            }

            string accion = "Proceso " + process.nombre;

            ActivityLogBiz.SaveActivityLog(new ActivityLog() { idUsuario = user, accion = accion.Length>=49? accion.Substring(0, 49):accion, fecha = DateTime.Now });

            return View(oProcess);
        }

        [Authorize]
        public ActionResult List()
        {
            List<vmProject> projects = new List<vmProject>();
            List<Project> lstProject = projectBiz.GetProjectList();
            List<Process> Processes = processBiz.GetProcessList();
            foreach (Project item in lstProject)
            {
                vmProject project = new vmProject()
                {
                    id = item.id,
                    nombre = item.nombre,
                    idProceso = item.idProceso,
                    nombreProceso = Processes.Where(x => x.id.Equals(item.idProceso)).Select(x => x.nombre).FirstOrDefault()
                };

                projects.Add(project);
            }

            return View(projects);
        }

        //
        // GET: /Project/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        public ActionResult ProjectDocumentTypeList(int id)
        {


            List<DocumentType> pdList = projectDocumentBiz.GetProjectDocumentTypeListByProject(new Project() { id = id });
            ViewBag.idProject = id;
            return View(pdList);
        }

        //
        // GET: /Project/Create
        [Authorize]
        public ActionResult Create()
        {
            vmProject project = new vmProject();

            project.lstProcess.AddRange(processBiz.GetProcessList());
            //project.lstProcess.Add(new Process() { id =3, nombre="PERF" });
            //project.lstProcess.Add(new Process() { id = 7, nombre = "QHSE" });

            return View(project);
        }

        //
        // POST: /Project/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Project project)
        {
            try
            {
                projectBiz.SaveProject(project);

                return RedirectToAction("List");
            }
            catch
            {
                vmProject oProject = new vmProject() { nombre = project.nombre, idProceso = project.idProceso };
                oProject.lstProcess.AddRange(processBiz.GetProcessList());
                return View(oProject);
            }
        }

        //
        // GET: /Project/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Project project = projectBiz.GetProjectbyKey(new Project() { id = id });
            vmProject oProject = new vmProject() { nombre = project.nombre, idProceso = project.idProceso };
            oProject.lstProcess.AddRange(processBiz.GetProcessList());

            return View(oProject);
        }

        //
        // POST: /Project/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Project project)
        {
            try
            {
                project.id = id;
                projectBiz.SaveProject(project);

                return RedirectToAction("List");
            }
            catch
            {
                vmProject oProject = new vmProject() { nombre = project.nombre, idProceso = project.idProceso };
                oProject.lstProcess.AddRange(processBiz.GetProcessList());
                return View(oProject);
            }
        }

        //
        // GET: /Project/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            projectBiz.DeleteProject(new Project() { id = id });
            return RedirectToAction("List");
        }

        //
        // POST: /Project/Delete/5
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
