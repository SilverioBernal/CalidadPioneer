using Orkidea.Pioneer.Business;
using Orkidea.Pioneer.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orkidea.Pioneer.Webfront.Controllers
{
    [Authorize]
    public class ParameterController : Controller
    {
        // GET: Parameter/Edit/5
        public ActionResult Edit()
        {
            ViewBag.estructura = ConfigurationManager.AppSettings["estructura"].ToString();
            ViewBag.direccionamiento = ConfigurationManager.AppSettings["direccionamiento"].ToString();
            ViewBag.normativas = ConfigurationManager.AppSettings["normativas"].ToString();
            return View();
        }

        [Authorize]
        public JsonResult GuardaParametros(string id)
        {
            string res = "";

            try
            {
                string[] parametros = id.Split('|');

                Configuration config = ConfigurationManager.OpenExeConfiguration("");
                config.AppSettings.Settings["estructura"].Value = parametros[0].Replace("_", ".");
                config.AppSettings.Settings["direccionamiento"].Value = parametros[1].Replace("_", ".");
                config.AppSettings.Settings["normativas"].Value = parametros[2].Replace("_", ".");
                config.Save(ConfigurationSaveMode.Modified);

                res = "Ok";
            }
            catch (Exception)
            {
                res = "Fail";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Details(string archivo)
        {
            string mimeType = "";
            int cuentaPuntos = 0;

            archivo = ConfigurationManager.AppSettings[archivo].ToString();

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
