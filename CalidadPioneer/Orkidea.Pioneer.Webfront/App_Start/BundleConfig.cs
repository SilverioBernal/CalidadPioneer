using System.Web;
using System.Web.Optimization;

namespace Orkidea.Pioneer.Webfront
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/locales/bootstrap-datepicker.es.min.js",
                      "~/Scripts/respond.js"));



            //bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
            //    "~/DataTables/jquery.dataTables.js",
            //    "~/DataTables/jquery.dataTables.sorting.js",
            //    "~/DataTables/DT_bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //                      "~/Content/bootstrap-datepicker.css",

            //bundles.Add(new StyleBundle("~/Content/DataTables").Include(
            //    "~/Datatables/css/demo_page.css",
            //    "~/Datatables/css/demo_table.css",
            //    "~/Datatables/css/demo_table_jui.css",
            //    "~/DataTables/DT_bootstrap.css"
            //    ));
        }
    }
}
