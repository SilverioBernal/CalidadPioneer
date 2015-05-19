using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class PositionController : Controller
    {
        PositionBiz positionBiz = new PositionBiz();

        // GET: Position
        [Authorize]
        public ActionResult Index()
        {
            return View(positionBiz.GetPositionList());
        }

        // GET: Position/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Position/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Position position)
        {
            try
            {
                // TODO: Add insert logic here
                positionBiz.SavePosition(position);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Position/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View(positionBiz.GetPositionbyKey(new Position() { id = id}));
        }

        // POST: Position/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Position position)
        {
            try
            {
                // TODO: Add update logic here
                position.id = id;
                positionBiz.SavePosition(position);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(positionBiz.GetPositionbyKey(new Position() { id = id }));
            }
        }

        // GET: Position/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            UserBiz userBiz = new UserBiz();

            List<User> lsUser = userBiz.GetUserList();

            foreach (User item in lsUser)
            {
                if (item.idCargo.Equals(id))
                {
                    item.idCargo = null;
                    userBiz.SaveUser(item);
                }
            }

            positionBiz.DeletePosition(new Position() { id = id });
            return RedirectToAction("Index");
        }
    }
}
