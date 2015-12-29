using System.Web;
using System.Web.Optimization;

namespace FSPOC_WebProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.contextMenu.js",
                        "~/Scripts/jquery-collision.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                      "~/Scripts/knockout-{version}.js",
                      "~/Scripts/BlockViewModel.js",
                      "~/Scripts/RuleListViewModel.js",
                      "~/Scripts/RuleDetailViewModel.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery.contextMenu.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/CommonLibraries").IncludeDirectory(
                      "~/Scripts/CommonLibraries", "*.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/OmniusScripts")
                .IncludeDirectory("~/Scripts/Tapestry", "*.js")
                .IncludeDirectory("~/Scripts/Overview", "*.js")
                .IncludeDirectory("~/Scripts/Portal", "*.js")
                .IncludeDirectory("~/Scripts/DatabaseDesigner", "*.js")
                .IncludeDirectory("~/Scripts/AppManager", "*.js")
                .IncludeDirectory("~/Scripts/Nexus", "*.js")
                .IncludeDirectory("~/Scripts/Hermes", "*.js")
                .IncludeDirectory("~/Scripts/Watchtower", "*.js")
                .IncludeDirectory("~/Scripts/Persona", "*.js")
                .Include("~/Scripts/PlatformUtils.js")
            );
        }
    }
}
