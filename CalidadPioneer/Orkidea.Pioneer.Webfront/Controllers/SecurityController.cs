using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using Orkidea.Pioneer.Webfront.Models;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class SecurityController : Controller
    {
        //
        // GET: /Security/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            Cryptography oCrypto = new Cryptography();
            if (!String.IsNullOrEmpty(model.UserName) && !String.IsNullOrEmpty(model.Password))
            {
                UserBiz userBiz = new UserBiz();
                User userTarget = userBiz.GetUserbyName(new User() { usuario = model.UserName });

                if (userTarget == null)
                    return View(model);

                string contraseñaDesencriptada = oCrypto.Decrypt(userTarget.clave);

                if (model.Password.Equals(contraseñaDesencriptada))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);

                    int id = userTarget.id;
                    int isAdmin = userTarget.admin ? 1 : 0;

                    string userData = id.ToString().Trim() + "|" + isAdmin.ToString().Trim();
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    HttpContext.Response.Cookies.Add(faCookie);

                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        CustomIdentity identity = new CustomIdentity(authTicket.Name, userData);
                        GenericPrincipal newUser = new GenericPrincipal(identity, new string[] { });
                        HttpContext.User = newUser;
                    }
                    
                    ActivityLogBiz.SaveActivityLog(new ActivityLog() { idUsuario = id, accion = "Login", fecha = DateTime.Now });

                    return RedirectToLocal(returnUrl);
                }
            }

            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //return RedirectToAction("Index", "Home");
                return RedirectToAction
                ("Login");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            UserBiz userBiz = new UserBiz();

            User user = userBiz.GetUserbyName(new User() { usuario = HttpContext.User.Identity.Name });
            vmUser oUser = new vmUser() { id = user.id, usuario = user.usuario, admin = user.admin};
            user.clave = "";
            return View(oUser);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(User userTarget)
        {
            Cryptography oCrypto = new Cryptography();
            string rootPath = Server.MapPath("~"); 
            UserBiz userBiz = new UserBiz();

            User user = userBiz.GetUserbyName(new User() { usuario = HttpContext.User.Identity.Name });

            user.clave = oCrypto.Encrypt(userTarget.clave);

            userBiz.SaveUser(user, rootPath);

            return RedirectToAction("Index", "Home");
        }
    }
}
