using System.Web.Optimization;

namespace ProjectWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                "~/Scripts/admin/vendor.min.js",
                "~/Scripts/admin/app.min.js",
                "~/Scripts/admin/jquery-jvectormap-1.2.2.min.js",
                "~/Scripts/admin/jquery-jvectormap-world-mill-en.js"));
            
            // Code removed for clarity.
            bundles.Add(new StyleBundle("~/Content/base").Include(
                "~/Content/admin/jquery-jvectormap-1.2.2.css",
                "~/Content/admin/app.min.css",
                "~/Content/admin/icons.min.css"));
        }
    }
}