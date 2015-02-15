using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    public class DocumentTypeController : Controller
    {
        DocumentTypeBiz documentTypeBiz = new DocumentTypeBiz();

        //
        // GET: /DocumentType/
        [Authorize]
        public ActionResult Index()
        {
            List<DocumentType> lstDocType = documentTypeBiz.GetDocumentTypeList();
            return View(lstDocType);
        }

        //
        // GET: /DocumentType/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DocumentType/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DocumentType/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(DocumentType documentType)
        {
            try
            {
                documentTypeBiz.SaveDocumentType(documentType);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DocumentType/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            DocumentType documentType = documentTypeBiz.GetDocumentTypeByKey(new DocumentType() { id = id });
            return View(documentType);
        }

        //
        // POST: /DocumentType/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, DocumentType documentType)
        {
            try
            {
                documentType.id = id;
                documentTypeBiz.SaveDocumentType(documentType);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //
        // GET: /DocumentType/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                documentTypeBiz.DeleteDocumentType(new DocumentType() { id = id });

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }     
    }
}
