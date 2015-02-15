using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class UserController : Controller
    {
        UserBiz userBiz = new UserBiz();

        //
        // GET: /User/
        [Authorize]
        public ActionResult Index()
        {
            List<User> lstUser = userBiz.GetUserList();

            return View(lstUser);
        }

        //
        // GET: /User/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /User/Create
        [Authorize]
        public ActionResult Create()
        {


            return View();
        }

        //
        // POST: /User/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                Cryptography oCrypto = new Cryptography();
                //Mailing mailing = new Mailing();

                //User oUser = userBiz.GetUserbyName(new User() { usuario = user.usuario });

                //if (oUser == null)
                //{
                //    string clave = RandomPasswordGenerator.Generate();
                //    user.clave = oCrypto.Encrypt(clave);
                //    userBiz.SaveUser(user);

                //    List<MailAddress> to = new List<MailAddress>();
                //    to.Add(new MailAddress(user.usuario, user.usuario));
                //    List<MailAddress> cc = new List<MailAddress>();
                //    List<MailAddress> bcc = new List<MailAddress>();

                //    string body = "<p>Apreciado usuario, <p><p>Se ha creado una cuenta para acceder al portal del Sistema de Gestión de Calidad. Su usuario es <b>" + user.usuario + "</b> y su clave es: <br /><br /> <b>" + clave + "</b></p><br /><br /><p>Sistema de Gestion de Calidad</p>";


                //    mailing.SendMail(to, cc, bcc, "Creacion de usuario", body);
                //}
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /User/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            //User oUser = userBiz.GetUserbyKey(new User() { id = id });
            //vmUser user = new vmUser() { admin = oUser.admin, clave = oUser.clave, id = oUser.id, usuario = oUser.usuario };
            //return View(user);
            return View();
        }

        //
        // POST: /User/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            try
            {
                user.id = id;
                userBiz.SaveUser(user);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /User/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            User oUser = userBiz.GetUserbyKey(new User() { id = id });
            userBiz.DeleteUser(oUser);

            return RedirectToAction("Index");
        }

        //
        // POST: /User/Delete/5
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
