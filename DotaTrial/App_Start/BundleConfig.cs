using System.Web;
using System.Web.Optimization;

namespace DotaTrial
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/AdminTemplate/plugins/css").Include(
                        "~/AdminTemplate/plugins/node-waves/waves.css",
                         "~/AdminTemplate/plugins/animate-css/animate.css"));

            bundles.Add(new ScriptBundle("~/AdminTemplate/plugins/js").Include(
                       "~/AdminTemplate/plugins/node-waves/waves.js",
                       "~/AdminTemplate/plugins/jquery-slimscroll/jquery.slimscroll.js"));

            bundles.Add(new StyleBundle("~/AdminTemplate/css").Include(
                       "~/AdminTemplate/css/style.css",
                       "~/AdminTemplate/css/themes/all-themes.css"));

            bundles.Add(new ScriptBundle("~/AdminTemplate/js").Include(
                       "~/AdminTemplate/js/admin.js",
                       "~/AdminTemplate/js/demo.js"));
        }
    }
}
