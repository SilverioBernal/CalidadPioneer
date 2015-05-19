using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class DrillController : Controller
    {
        DrillBiz drillBiz = new DrillBiz();

        // GET: Drill
        [Authorize]
        public ActionResult Index()
        {
            return View(drillBiz.GetDrillList());
        }

        // GET: Drill/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Drill/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Drill drill)
        {
            try
            {
                // TODO: Add insert logic here
                drillBiz.SaveDrill(drill);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(drill);
            }
        }

        // GET: Drill/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View(drillBiz.GetDrillbyKey(new Drill(){id = id}));
        }

        // POST: Drill/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Drill drill)
        {
            try
            {
                // TODO: Add update logic here
                drill.id = id;
                drillBiz.SaveDrill(drill);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(id);
            }
        }

        // GET: Drill/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            drillBiz.DeleteDrill(new Drill() { id = id });
            return RedirectToAction("Index");
        }
    }
}
