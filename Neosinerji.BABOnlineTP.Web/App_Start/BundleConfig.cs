using System.Web;
using System.Web.Optimization;

namespace Neosinerji.BABOnlineTP.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/js/jquery.js",
                        "~/Content/js/jquery.blockUI.js"));
            // "~/Content/js/neosinerji.babonlinetp.common.js"

            //bundles.Add(new ScriptBundle("~/bundles/effects").Include(
            //            "~/Content/js/jquery-ui.custom.js"));

            //bundles.Add(new ScriptBundle("~/bundles/switch").Include(
            //                            "~/Content/js/bootstrapSwitch.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //                            "~/Content/js/jquery.validate.js",
            //                            "~/Content/js/jquery.validate.bootstrap.js",
            //                            "~/Content/js/jquery.validate.unobtrusive.js",
            //                            "~/Content/js/jquery.validate.custom.js"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //                            "~/Content/js/bootstrap.js",
            //                            "~/Content/js/bootstrap-datepicker.js",
            //                            "~/Content/js/bootstrap-datepicker.tr.js"));

            //bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
            //                            "~/Content/js/jquery.dataTables.js",
            //                            "~/Content/js/TableTools.js",
            //                            "~/Content/js/jquery.dataTables.bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                                        "~/Content/css/bootstrap.css",
                                        "~/Content/css/theme.css",
                                        "~/Content/css/TableTools.css"));

            bundles.Add(new StyleBundle("~/Content/switch").Include(
                                        "~/Content/css/bootstrapSwitch.css"));

            //bundles.Add(new StyleBundle("~/Content/wysihtml").Include(
            //                            "~/Content/css/bootstrap-wysihtml5-0.0.2.css"));

            //bundles.Add(new ScriptBundle("~/bundles/wysihtml").Include(
            //                            "~/Content/js/wysihtml5-0.3.0.js",
            //                            "~/Content/js/bootstrap-wysihtml5-0.0.2.js"));

            //bundles.Add(new ScriptBundle("~/bundles/smarttab").Include(
            //                             "~/Content/js/jquery.smartTab.js"));


            /*Home Page*/
            bundles.Add(new StyleBundle("~/bundles/home").Include(
                                        "~/Content/css/animate.css",
                                        "~/Content/css/theme_home.css",
                                        "~/Content/css/bootstrap.css"));

            bundles.Add(new ScriptBundle("~/bundles/homejs").Include(
                                         "~/Content/js/theme_home.js",
                                         "~/Content/js/bootstrap.js"));

            //File Upload
            //bundles.Add(new ScriptBundle("~/bundles/fileupload_js/").Include(
            //                                "~/Content/js/bootstrap-fileupload.js"));

            //bundles.Add(new StyleBundle("~/bundles/fileupload_css").Include(
            //                                "~/Content/css/bootstrap-fileupload.css"));

            //bundles.Add(new ScriptBundle("~/bundles/pwstrength.js").Include(
            //                             "~/Content/js/pwstrength.js"));

            //bundles.Add(new ScriptBundle("~/bundles/phone_js/").Include(
            //                             "~/Content/js/bootstrap-formhelpers-phone.js"));

            //bundles.Add(new ScriptBundle("~/bundles/Chart_js/").Include(
            //                             "~/Content/js/exporting.js",
            //                             "~/Content/js/highcharts.js"));

            //bundles.Add(new ScriptBundle("~/bundles/musteriekle").Include(
            //                             "~/Content/js/bootstrap-formhelpers-phone.js",
            //                             "~/Content/js/blockui.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
            //                             "~/Content/js/bootstrap-multiselect.js"));






            // ======== Bu Bölümde CSS Dosyaları ekleniyor. ======== //

            // ==== Her sayfada olması zorunlu css dosyaları bu bölümde ekleniyor ==== //
            StyleBundle RequiredCss = new StyleBundle("~/bundles/requiredcss");
            RequiredCss.Include("~/Content/css/bootstrap.css",
                                "~/Content/css/bootstrap-responsive.css",
                                "~/Content/css/style-metro.css",
                                "~/Content/css/style-responsive.css",
                                "~/Content/css/style.css",
                                "~/Content/css/themes/ram_sigorta.css",
                                "~/Content/css/DT_bootstrap.css",
                                "~/Content/css/font-awesome.css",
                                "~/Content/css/jquery.ui.datepicker.css",
                                "~/Content/css/jquery-ui.css",
                                "~/Content/css/custom.css");

            StyleBundle WysiHtmlCss = new StyleBundle("~/Content/wysihtml");
            WysiHtmlCss.Include("~/Content/css/bootstrap-wysihtml5-0.0.2.css");

            bundles.Add(RequiredCss);
            bundles.Add(WysiHtmlCss);



            // ======== Bu Bölümde JS Dosyaları ekleniyor. ======== //

            // ==== Her sayfada olması zorunlu js dosyaları bu bölümde ekneniyor ==== //
            ScriptBundle RequiredJs = new ScriptBundle("~/bundles/requiredjs");
            RequiredJs.Include("~/Content/js/jquery.js", "~/Content/js/bootstrap.js");
            RequiredJs.Include("~/Content/js/jquery.validate.js", "~/Content/js/jquery.validate.bootstrap.js", "~/Content/js/jquery.validate.unobtrusive.js", "~/Content/js/jquery.validate.custom.js");
            RequiredJs.Include("~/Content/js/jquery.blockUI.js", "~/Content/js/breakpoints.js");
            RequiredJs.Include("~/Content/js/app.js", "~/Content/js/jquery.slimscroll.min.js", "~/Content/js/jquery.ui.datepicker.js", "~/Content/js/jquery.ui.datepicker-tr.min.js");
            RequiredJs.Include("~/Content/js/jquery.cookie.js", "~/Content/js/neosinerji.babonlinetp.common.js");

            ScriptBundle DataTableJs = new ScriptBundle("~/bundles/dataTable");
            DataTableJs.Include("~/Content/js/jquery.dataTables.js",
                                "~/Content/js/TableTools.js",
                                "~/Content/js/jquery.dataTables.bootstrap.js",
                                "~/Content/js/zeroClickboard.js");

            ScriptBundle DataTableJsen = new ScriptBundle("~/bundles/dataTableen");
            DataTableJsen.Include("~/Content/js/jquery.dataTables.js",
                                "~/Content/js/TableTools.en.js",
                                "~/Content/js/jquery.dataTables.bootstrap.en.js",
                                "~/Content/js/zeroClickboard.js");

            ScriptBundle MultiSelectJs = new ScriptBundle("~/bundles/multiselect");
            MultiSelectJs.Include("~/Content/js/bootstrap-multiselect.js");

            ScriptBundle PhoneInputJs = new ScriptBundle("~/bundles/phoneinputjs");
            PhoneInputJs.Include("~/Content/js/bootstrap-formhelpers-phone.js");

            ScriptBundle FileUploadJs = new ScriptBundle("~/bundles/fileuploadjs");
            FileUploadJs.Include("~/Content/js/bootstrap-fileupload.js");

            ScriptBundle WysiHtmlJs = new ScriptBundle("~/bundles/wysihtml");
            WysiHtmlJs.Include("~/Content/js/wysihtml5-0.3.0.js", "~/Content/js/bootstrap-wysihtml5-0.0.2.js");

            ScriptBundle ChartJs = new ScriptBundle("~/bundles/Chart_js/");
            ChartJs.Include("~/Content/js/exporting.js", "~/Content/js/highcharts.js");

            ScriptBundle PwStrengthJs = new ScriptBundle("~/bundles/pwstrength.js");
            PwStrengthJs.Include("~/Content/js/pwstrength.js");

            ScriptBundle SwitchJs = new ScriptBundle("~/bundles/switch");
            SwitchJs.Include("~/Content/js/bootstrapSwitch.js");

            bundles.Add(RequiredJs);
            bundles.Add(DataTableJs);
            bundles.Add(DataTableJsen);
            bundles.Add(PhoneInputJs);
            bundles.Add(DataTableJs);
            bundles.Add(MultiSelectJs);
            bundles.Add(FileUploadJs);
            bundles.Add(WysiHtmlJs);
            bundles.Add(ChartJs);
            bundles.Add(PwStrengthJs);
            bundles.Add(SwitchJs);
        }
    }
}