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
        
        private List<NewsPaper> GetPaginatedNews(int page = 1)
        {
            var skipRecords = page * recordsPerPage;

            List<NewsPaper> listOfProducts = newsPaperBiz.GetNewsList().Skip(skipRecords).Take(recordsPerPage).ToList();

            return listOfProducts;
        }
    }
}