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
    public class NearMissController : Controller
    {
        NearMissBiz nearMissBiz = new NearMissBiz();

        // GET: NearMiss
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
                if (!position.cierraNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            List<NearMiss> lsNearMiss = nearMissBiz.GetNearMissList(true);
            List<vmNearMiss> lsNM = new List<vmNearMiss>();

            foreach (NearMiss item in lsNearMiss)
            {
                lsNM.Add(new vmNearMiss()
                    {
                        id = item.id,
                        fechaApertura = item.fechaApertura,
                        usuarioCrea = (lsUser.Where(x => x.id.Equals(item.idUsuarioAbre)).FirstOrDefault()).usuario,
                        fechaAnalisis = item.fechaAnalisis,
                        fechaReporte = item.fechaReporte,
                        nombreReporteador = item.nombreReporteador == null?"Dato no disponible":item.nombreReporteador,
                        descripcion = item.descripcion.Length>50?item.descripcion.Substring(0,50) + "...":item.descripcion
                    });
            }


            return View(lsNM);
        }

        // GET: NearMiss/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult DashBoard()
        {
            return View();
        }

        // GET: NearMiss/Create
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
                if (!position.abreNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            vmNearMiss nearMiss = new vmNearMiss(true);

            return View(nearMiss);
        }

        // POST: NearMiss/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(NearMiss nearMiss)
        {
            try
            {
                // TODO: Add insert logic here                
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

                nearMiss.fechaApertura = DateTime.Now;
                nearMiss.idUsuarioAbre = user;
                int id = nearMissBiz.SaveNearMiss(nearMiss);
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return RedirectToAction("Details", new { id = -1 });
            }
        }

        // GET: NearMiss/Edit/5
        [Authorize]
        public ActionResult Analize(int id)
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
                if (!position.cierraNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            NearMiss oNearMiss = nearMissBiz.GetNearMissbyKey(new NearMiss() { id = id });

            vmNearMiss nearMiss = new vmNearMiss(true)
            {
                id = id,
                tipoHallazgo = oNearMiss.tipoHallazgo,
                actividadReporteador = oNearMiss.actividadReporteador,
                idUbicacion = oNearMiss.idUbicacion,
                descripcion = oNearMiss.descripcion,
                accionSugeida = oNearMiss.accionSugeida,
                usuarioCrea = (userBiz.GetUserbyKey(new User() { id = oNearMiss.idUsuarioAbre })).usuario,
                fechaApertura = oNearMiss.fechaApertura,
                fechaAnalisis = oNearMiss.fechaAnalisis,
                fechaReporte = oNearMiss.fechaReporte,
                nombreReporteador = oNearMiss.nombreReporteador== null?"Información no disponible":oNearMiss.nombreReporteador,
                empresaReporteador = oNearMiss.empresaReporteador == null ? "Información no disponible" : oNearMiss.empresaReporteador,
            };

            return View(nearMiss);
        }

        // POST: NearMiss/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Analize(int id, NearMiss nearMiss)
        {
            try
            {
                // TODO: Add update logic here
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

                nearMiss.id = id;
                NearMiss nm = nearMissBiz.GetNearMissbyKey(nearMiss);


                nm.idUsuarioAnalisis = user;
                nm.fechaAnalisis = DateTime.Now;
                nm.calidad = string.IsNullOrEmpty(nearMiss.calidad) ? "" : nearMiss.calidad;
                nm.salud = string.IsNullOrEmpty(nearMiss.salud) ? "" : nearMiss.salud;
                nm.asuntosAmbientales = string.IsNullOrEmpty(nearMiss.asuntosAmbientales) ? "" : nearMiss.asuntosAmbientales;
                nm.usoEpp = string.IsNullOrEmpty(nearMiss.usoEpp) ? "" : nearMiss.usoEpp;
                nm.condicionesTrabajo = string.IsNullOrEmpty(nearMiss.condicionesTrabajo) ? "" : nearMiss.condicionesTrabajo;
                nm.analisisHallazgo = string.IsNullOrEmpty(nearMiss.analisisHallazgo) ? "Bajo" : nearMiss.analisisHallazgo;
                nm.responsable = nearMiss.responsable;
                nearMissBiz.SaveNearMiss(nm);
                //return RedirectToAction("Details", new { id = id });
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
                if (!position.cierraNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
            NearMiss oNearMiss = nearMissBiz.GetNearMissbyKey(new NearMiss() { id = id });

            vmNearMiss nearMiss = new vmNearMiss(true)
            {
                id = id,
                tipoHallazgo = oNearMiss.tipoHallazgo,
                actividadReporteador = oNearMiss.actividadReporteador,
                idUbicacion = oNearMiss.idUbicacion,
                descripcion = oNearMiss.descripcion,
                accionSugeida = oNearMiss.accionSugeida,
                usuarioCrea = (userBiz.GetUserbyKey(new User() { id = oNearMiss.idUsuarioAbre })).usuario,
                fechaApertura = oNearMiss.fechaApertura,
                asuntosAmbientales = oNearMiss.asuntosAmbientales,
                analisisHallazgo = oNearMiss.analisisHallazgo,
                salud = oNearMiss.salud,
                calidad = oNearMiss.calidad,
                condicionesTrabajo = oNearMiss.condicionesTrabajo,
                usoEpp = oNearMiss.usoEpp,
                fechaReporte = oNearMiss.fechaReporte,
                nombreReporteador = oNearMiss.nombreReporteador == null ? "Información no disponible" : oNearMiss.nombreReporteador,
                empresaReporteador = oNearMiss.empresaReporteador == null ? "Información no disponible" : oNearMiss.empresaReporteador,
                responsable = oNearMiss.responsable == null ? "Información no disponible" : oNearMiss.responsable,                
            };

            return View(nearMiss);
        }

        // POST: NearMiss/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Close(int id, NearMiss nearMiss)
        {
            try
            {
                // TODO: Add update logic here
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
                    if (!position.cierraNearMiss)
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "Home");

                nearMiss.id = id;
                NearMiss nm = nearMissBiz.GetNearMissbyKey(nearMiss);

                nm.idUsuarioCierra = user;
                nm.fechaCierre = nearMiss.fechaCierre;
                nm.observacionesCierre = nearMiss.observacionesCierre;

                nearMissBiz.SaveNearMiss(nm);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult View(int id)
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
                if (!position.cierraNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
            NearMiss oNearMiss = nearMissBiz.GetNearMissbyKey(new NearMiss() { id = id });

            vmNearMiss nearMiss = new vmNearMiss(true)
            {
                id = id,
                tipoHallazgo = oNearMiss.tipoHallazgo,
                actividadReporteador = oNearMiss.actividadReporteador,
                idUbicacion = oNearMiss.idUbicacion,
                descripcion = oNearMiss.descripcion,
                accionSugeida = oNearMiss.accionSugeida,
                usuarioCrea = (userBiz.GetUserbyKey(new User() { id = oNearMiss.idUsuarioAbre })).usuario,
                fechaApertura = oNearMiss.fechaApertura,
                asuntosAmbientales = oNearMiss.asuntosAmbientales,
                analisisHallazgo = oNearMiss.analisisHallazgo,
                salud = oNearMiss.salud,
                calidad = oNearMiss.calidad,
                condicionesTrabajo = oNearMiss.condicionesTrabajo,
                usoEpp = oNearMiss.usoEpp,
                fechaCierre = oNearMiss.fechaCierre,
                observacionesCierre = oNearMiss.observacionesCierre,
                fechaReporte = oNearMiss.fechaReporte,
                nombreReporteador = oNearMiss.nombreReporteador == null ? "Información no disponible" : oNearMiss.nombreReporteador
            };

            return View(nearMiss);
        }

        public ActionResult Query()
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
                if (!position.cierraNearMiss)
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");

            vmIssueQuery issue = new vmIssueQuery() { abierto = true, cerrado = true, desde = DateTime.Now, hasta = DateTime.Now };
            return View(issue);
        }

        public ActionResult QueryData(bool abierto, bool cerrado, DateTime desde, DateTime hasta)
        {
            UserBiz userBiz = new UserBiz();
            List<User> lsUser = userBiz.GetUserList();
            DrillBiz drillBiz = new DrillBiz();

            List<NearMiss> lsNearMiss = GetNMQuery(abierto, cerrado, desde, hasta);
            List<vmNearMiss> lsNM = new List<vmNearMiss>();
            List<Drill> lsDrill = drillBiz.GetDrillList();

            foreach (NearMiss item in lsNearMiss)
            {
                string rig = "";
                if (lsDrill.Where(x => x.id.Equals(item.idUbicacion)).FirstOrDefault() != null)
                    rig = (lsDrill.Where(x => x.id.Equals(item.idUbicacion)).FirstOrDefault()).descripcion;

                lsNM.Add(new vmNearMiss()
                {
                    id = item.id,
                    fechaApertura = item.fechaApertura,
                    usuarioCrea = (lsUser.Where(x => x.id.Equals(item.idUsuarioAbre)).FirstOrDefault()).usuario,
                    fechaAnalisis = item.fechaAnalisis,
                    descripcionRig = rig,
                    fechaReporte = item.fechaReporte,
                    nombreReporteador = item.nombreReporteador ==null?"Información no disponible": item.nombreReporteador,
                    fechaCierre = item.fechaCierre,
                    descripcion = item.descripcion.Length > 50 ? item.descripcion.Substring(0, 50) + "..." : item.descripcion
                });
            }            

            ViewBag.abiertas = string.Format("true|{0}|{1}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"));
            ViewBag.cerradas = string.Format("false|{0}|{1}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"));

            ViewBag.calidad = string.Format("{0}|{1}|{2}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), "calidad");
            ViewBag.salud = string.Format("{0}|{1}|{2}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), "salud");
            ViewBag.asuntos = string.Format("{0}|{1}|{2}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), "asuntosAmbientales");
            ViewBag.condiciones = string.Format("{0}|{1}|{2}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), "condicionesTrabajo");
            ViewBag.epp = string.Format("{0}|{1}|{2}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), "usoEpp");

            return PartialView("QueryData", lsNM);
        }

        private List<NearMiss> GetNMQuery(bool abierto, bool cerrado, DateTime desde, DateTime hasta)
        {
            List<NearMiss> lsNM = new List<NearMiss>();
            if (abierto)
                lsNM.AddRange(nearMissBiz.GetNearMissList(true, desde, hasta));

            if (cerrado)
                lsNM.AddRange(nearMissBiz.GetNearMissList(false, desde, hasta));

            return lsNM;
        }

        //public JsonResult OpenCloseGraph(bool estado)
        //{
        //    var chartsdata = nearMissBiz.GetReporteGenerico(estado);

        //    return Json(chartsdata, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult OpenCloseGraph(string id)
        {
            string[] parameters = id.Split('|');
            bool estado = bool.Parse(parameters[0]);
            string desde = parameters[1];
            string hasta = parameters[2];
            
            var chartsdata = nearMissBiz.GetReporteGenerico(estado, desde, hasta);            

            return Json(chartsdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult categoryGraph(string id)
        {
            string[] parameters = id.Split('|');            
            string desde = parameters[0];
            string hasta = parameters[1];
            string categoria = parameters[2];

            var chartsdata = nearMissBiz.GetReporteGenerico(desde, hasta, categoria);

            return Json(chartsdata, JsonRequestBehavior.AllowGet);
        }
    }
}
