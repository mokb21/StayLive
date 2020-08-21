using System.Web;
using System.Web.Optimization;

namespace StayLive
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region scripts
            bundles.Add(new ScriptBundle("~/bundles/main-script").Include(
                    "~/assets/plugins/jquery/jquery.js",
                    "~/assets/plugins/jquery/Scripts/jquery.validate.min.js",
                    "~/assets/plugins/jquery/Scripts/jquery.validate.unobtrusive.min.js",
                    "~/assets/js/theme/modernizr-2.6.2.min.js",
                    "~/assets/plugins/FoolProof/mvcfoolproof.unobtrusive.min.js",
                    "~/assets/js/theme/sidebarmenu.js",
                    "~/assets/plugins/sticky-kit-master/sticky-kit.min.js",
                    "~/assets/plugins/bootstrap/js/popper.min.js",
                    "~/assets/plugins/bootstrap/js/bootstrap.min.js",
                    "~/assets/plugins/sidebar-nav/sidebar-nav.min.js",
                    "~/assets/plugins/toast-master/js/jquery.toast.js",
                    "~/assets/js/theme/jquery.slimscroll.js",
                    "~/assets/js/theme/waves.js",
                    "~/assets/js/theme/main.js",
                    "~/assets/plugins/bootstrap-datepicker/bootstrap-datepicker.min.js",
                    "~/assets/plugins/moment/moment.min.js",
                    "~/assets/plugins/mask/jquery.mask.min.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/second-script").Include(
                "~/assets/js/custom/InfoMessage.js",
                "~/assets/js/custom/general.js",
                "~/assets/plugins/switchery/switchery.min.js",
                "~/assets/plugins/datatables/DataTables-1.10.16/js/jquery.dataTables.min.js",
                "~/assets/plugins/datatables/Buttons/js/dataTables.buttons.min.js",
                "~/assets/plugins/datatables/Select-1.2.6/js/dataTables.select.min.js",
                "~/assets/plugins/datatables/datatables-plugins.js",
                "~/assets/plugins/jstree/jstree.min.js",
                "~/assets/plugins/sweetalert/sweetalert.min.js",
                "~/assets/plugins/dropify/js/dropify.js",
                "~/assets/plugins/qtip/jquery.qtip.min.js",
                "~/assets/js/custom/custom.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/chart-script").Include(
                "~/assets/plugins/ChartJs/Chart.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/users-helpers").Include(
                "~/areas/Users/Content/js/users-helper.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/companies-helpers").Include(
                "~/areas/Companies/Content/js/company-helper.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/chat-helpers").Include(
               "~/assets/js/custom/chat.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/tickets-helpers").Include(
               "~/areas/Tickets/Content/js/tickets-helper.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/levels-helpers").Include(
               "~/areas/Levels/Content/js/levels-helper.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/home-helpers").Include(
               "~/Content/js/home-helper.js"
               ));
            #endregion

            #region styles
            bundles.Add(new StyleBundle("~/Content/main-style").Include(
                    "~/assets/plugins/bootstrap/css/bootstrap.min.css",
                    "~/assets/plugins/toast-master/css/jquery.toast.css",
                    "~/assets/plugins/sidebar-nav/sidebar-nav.min.css",
                    "~/assets/plugins/bootstrap-datepicker/bootstrap-datepicker.min.css",
                    "~/assets/css/theme/animate.css",
                    "~/assets/css/theme/style.css"));

            bundles.Add(new StyleBundle("~/Content/second-style").Include(
                "~/assets/plugins/switchery/switchery.min.css",
                "~/assets/plugins/datatables/Buttons/css/buttons.dataTables.min.css",
                "~/assets/plugins/datatables/Select-1.2.6/css/select.dataTables.min.css",
                "~/assets/plugins/jstree/themes/default/style.min.css",
                "~/assets/plugins/sweetalert/sweetalert.css",
                "~/assets/plugins/dropify/css/dropify.css",
                "~/assets/plugins/qtip/jquery.qtip.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/light-style").Include(
                    "~/assets/css/theme/colors/blue.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/custom-style").Include(
                    "~/assets/css/custom/custom.css"));

            bundles.Add(new StyleBundle("~/Content/tickets-style").Include(
                    "~/areas/Tickets/Content/css/tickets-helper.css"));
            #endregion
        }
    }
}