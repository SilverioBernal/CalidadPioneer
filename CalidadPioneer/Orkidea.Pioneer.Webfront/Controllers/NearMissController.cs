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
                if(!position.cierraNearMiss)
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
                        usuarioCrea = (lsUser.Where(x => x.id.Equals(item.idUsuarioAbre)).FirstOrDefault()).usuario
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
        public ActionResult Edit(int id)
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
                fechaApertura = oNearMiss.fechaApertura
            };

            return View(nearMiss);
        }

        // POST: NearMiss/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, NearMiss nearMiss)
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
                nearMiss.idUsuarioAnalisis = user;
                nearMiss.fechaAnalisis = DateTime.Now;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
