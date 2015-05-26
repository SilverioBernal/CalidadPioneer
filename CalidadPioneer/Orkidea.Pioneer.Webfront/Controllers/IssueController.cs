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
    public class IssueController : Controller
    {
        IssueBiz issueBiz = new IssueBiz();
        // GET: Issue
        [Authorize]
        public ActionResult Index()
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

            UserBiz userBiz = new UserBiz();
            User oUser = userBiz.GetUserbyKey(new User() { id = user });
            List<User> lsUser = userBiz.GetUserList();
            PositionBiz positionBiz = new PositionBiz();

            if (oUser.idCargo != null)
            {
                Position position = positionBiz.GetPositionbyKey(new Position() { id = (int)oUser.idCargo });
                if (!position.cierraHallazgo)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            return View(issueBiz.GetIssueList(true));
        }

        // GET: Issue/Create
        [Authorize]
        public ActionResult Create()
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

            UserBiz userBiz = new UserBiz();
            User oUser = userBiz.GetUserbyKey(new User() { id = user });
            PositionBiz positionBiz = new PositionBiz();

            if (oUser.idCargo != null)
            {
                Position position = positionBiz.GetPositionbyKey(new Position() { id = (int)oUser.idCargo });
                if (!position.abreHallazgo)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            vmIssue issue = new vmIssue(true);
            return View(issue);
        }

        // POST: Issue/Create
        [HttpPost]
        public ActionResult Create(vmIssue oIssue,  IEnumerable<HttpPostedFileBase> files)
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

            try
            {
                string fileExtension = Path.GetExtension(oIssue.File.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;

                AzureStorageHelper.uploadFile(oIssue.File.InputStream, fileName, "uploadedFiles");

                issueBiz.SaveIssue(new Issue()
                {
                    idTaladro = oIssue.idTaladro,
                    fuente = oIssue.fuente,
                    descripcion = oIssue.descripcion,
                    nombreReportador = oIssue.nombreReportador,
                    cargoReportador = oIssue.cargoReportador,
                    empresaReportador = oIssue.empresaReportador,
                    fechaCreacion = DateTime.Now,
                    fechaReporte = oIssue.fechaReporte,
                    fotoAntes = fileName,
                    idUsuarioCrea = user
                });


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Manage(int id)
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

            UserBiz userBiz = new UserBiz();
            User oUser = userBiz.GetUserbyKey(new User() { id = user });
            PositionBiz positionBiz = new PositionBiz();

            if (oUser.idCargo != null)
            {
                Position position = positionBiz.GetPositionbyKey(new Position() { id = (int)oUser.idCargo });
                if (!position.cierraHallazgo)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            vmIssue issue = new vmIssue(id);
            return View(issue);
        }

        [Authorize]
        public ActionResult ManageRecord()
        {           
            return View();
        }

        [HttpPost]
        public ActionResult ManageRecord(IssueDetail issueDetail)
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

            issueDetail.fechaRegistro = DateTime.Now;
            issueDetail.idUsuarioRegistra = user;
            
            issueBiz.SaveIssueDetail(issueDetail);
            return RedirectToAction("Manage", new { id = issueDetail.idHallazgo });
        }

        [Authorize]
        public ActionResult DeleteRecord(string id)
        {
            string[] ids = id.Split('|');
            int idIssue = int.Parse(ids[0]);
            int idIssueDetail = int.Parse(ids[1]);

            issueBiz.DeleteIssueDetail(idIssueDetail);
            return RedirectToAction("Manage", new { id = idIssue });
        }


        [Authorize]
        public ActionResult Close(int id)
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

            UserBiz userBiz = new UserBiz();
            User oUser = userBiz.GetUserbyKey(new User() { id = user });
            PositionBiz positionBiz = new PositionBiz();

            if (oUser.idCargo != null)
            {
                Position position = positionBiz.GetPositionbyKey(new Position() { id = (int)oUser.idCargo });
                if (!position.cierraHallazgo)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            vmIssue issue = new vmIssue(id);
            return View(issue);
        }

        [HttpPost]
        public ActionResult Close(int id, vmIssue oIssue)
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

            try
            {
                Issue issue = issueBiz.GetIssuebyKey(new Issue() { id = id });

                string fileExtension = Path.GetExtension(oIssue.File.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;

                AzureStorageHelper.uploadFile(oIssue.File.InputStream, fileName, "uploadedFiles");

                issue.fotoDespues = fileName;
                issue.comentariosCierre = oIssue.comentariosCierre;
                issue.fechaCierre = DateTime.Now;
                issue.idUsuarioCierre = user;

                issueBiz.SaveIssue(issue);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult GetPhoto(string archivo)
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
    }
}
