using System.Web.Optimization;

namespace DashTechCRM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                "~/Scripts/common.js"));

            //bundles.Add(new ScriptBundle("~/bundles/dx.all").Include(
            //    "~/Scripts/DevExpressJS/dx.all.js",
            //    "~/Scripts/DevExpressJS/jszip.js"));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/dxcss").Include(
            //    "~/Content/DevExpressCSS/dx.light.css",
            //    "~/Content/DevExpressCSS/dx.common.css"));


        }
    }
}
