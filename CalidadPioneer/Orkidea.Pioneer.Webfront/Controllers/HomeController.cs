using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class HomeController : Controller
    {
        NewsPaperBiz newsPaperBiz = new NewsPaperBiz();

        const int recordsPerPage = 3;

        [Authorize]
        public ActionResult Index(int? id)
        {
            var page = id ?? 0;

            if (Request.IsAjaxRequest())
            {
                List<NewsPaper> listOfProducts = GetPaginatedNews(page);
                return PartialView("_NewsPaper", listOfProducts);
            }

            return View("Index", newsPaperBiz.GetNewsList().Take(recordsPerPage));            
        }

        [Authorize]
        public ActionResult Contact()
        {            
            return View();
        }

        [Authorize]
        public JsonResult SendSuggestion(string id)
        {
            string res = "";
            string[] message = id.Split('|');

            #region User identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;

            string user = "";

            if (context.IsAuthenticated)
            {

                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                user = ci.Name;
            }

            #endregion

            CommonBiz commonBiz = new CommonBiz();
            try
            {
                string rootPath = Server.MapPath("~");                
                commonBiz.sendContactMessage(message[1], user, string.Format("{0} a través del SGD de {1} - {2} ", message[0], user, message[1]), message[2], rootPath);
                res = "OK";
            }
            catch (Exception)
            {
                res = "fail";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        
        private List<NewsPaper> GetPaginatedNews(int page = 1)
        {
            var skipRecords = page * recordsPerPage;

            List<NewsPaper> listOfProducts = newsPaperBiz.GetNewsList().Skip(skipRecords).Take(recordsPerPage).ToList();

            return listOfProducts;
        }

        public JsonResult visitante()
        {
            string res = "";

            try
            {
                List<ActivityLog> lsAL = ActivityLogBiz.GetActivityLogList("Login");
                res = lsAL.Count().ToString();
            }
            catch (Exception)
            {
                res = "";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}