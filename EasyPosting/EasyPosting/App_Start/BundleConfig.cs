using System.Web;
using System.Web.Optimization;

namespace EasyPosting
{
    public class BundleConfig
    {
        // 번들 작성에 대한 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=301862를 참조하십시오.
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js",
            "~/Scripts/jquery-ui.unobtrusive-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
            "~/Scripts/jquery.form.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            "~/Content/themes/base/jquery.ui.core.css",
            "~/Content/themes/base/jquery.ui.datepicker.css",
            "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/html5shiv").Include(
                              "~/Scripts/html5shiv.js"));
            //동적 문단 객체화
            bundles.Add(new ScriptBundle("~/bundles/Editor").Include(
                "~/Scripts/gridster.js",
                "~/Scripts/shortcut.js",
                "~/Scripts/context.js",
                "~/Scripts/farbtastic.js",
                "~/Scripts/bootstrap-dropdown.js",
                "~/Scripts/wysiwyg.js",
                "~/Scripts/Editor.js"
                ));
            bundles.Add(new StyleBundle("~/Content/Editorcss").Include(
                "~/Content/farbtastic.css",
                "~/Content/context.bootstrap.css",
                "~/Content/Editor.css"));
            //동적 문단 객체화 편집 페이지
            bundles.Add(new ScriptBundle("~/bundles/Editor_Edit").Include(
                "~/Scripts/gridster.js",
                "~/Scripts/shortcut.js",
                "~/Scripts/farbtastic.js",
                "~/Scripts/context.js",
                "~/Scripts/bootstrap-dropdown.js",
                "~/Scripts/wysiwyg.js",
                "~/Scripts/Editor_Edit.js"
                ));
            //동적 문단 객체화 버전 분기 편집 페이지
            bundles.Add(new ScriptBundle("~/bundles/Version_Edit").Include(
                "~/Scripts/gridster.js",
                "~/Scripts/shortcut.js",
                "~/Scripts/farbtastic.js",
                "~/Scripts/context.js",
                "~/Scripts/bootstrap-dropdown.js",
                "~/Scripts/wysiwyg.js",
                "~/Scripts/Version_Edit.js"
                ));
            //마크다운 에디터
            bundles.Add(new ScriptBundle("~/bundles/markdowneditor").Include(
                        "~/Scripts/marked.js",
                        "~/Scripts/markdown.js"));
            bundles.Add(new StyleBundle("~/Content/markdowneditor").Include(
                      "~/Content/markdown.css"));

            bundles.Add(new ScriptBundle("~/bundles/markdowneditor_Edit").Include(
                        "~/Scripts/marked.js",
                        "~/Scripts/markdown_Edit.js"));

            //이미지 에디터
            bundles.Add(new ScriptBundle("~/bundles/img_editor").Include(
                          "~/Scripts/imagescript.js"));
            //이미지 자르기
            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                        "~/Scripts/jquery.Jcrop.js"));
            //이미지 에디터 위지윅 편집기
            bundles.Add(new ScriptBundle("~/bundles/img_wysiwyg").Include(
                        "~/Scripts/wysiwyg_image.js",
                        "~/Scripts/farbtastic.js",
                        "~/Scripts/shortcut.js"));

            //이미지 에디터 관련 속성 추가
            bundles.Add(new StyleBundle("~/Content/imgcss").Include(
                      "~/Content/imgstylesheet.css"));
            bundles.Add(new StyleBundle("~/Content/jcropcss").Include(
                      "~/Content/jquery.Jcrop.css"));
            bundles.Add(new StyleBundle("~/Content/img_wysiwygcss").Include(
                      "~/Content/wysiwyg.css",
                      "~/Content/farbtastic.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/style.min.css"));

            // Modernizr의 개발 버전을 사용하여 개발하고 배우십시오. 그런 다음
            // 프로덕션할 준비가 되면 http://modernizr.com의 빌드 도구를 사용하여 필요한 테스트만 선택하십시오.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bootstrap.file-input.js 추가됨(이미지 에디터에서 사용)
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.file-input.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/fonts").Include(
                      "~/Content/font-awesome.min.css"));
            //Cover 관련 속성 추가
            bundles.Add(new StyleBundle("~/Content/cover").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/cover.css"));
            //스토리지
            bundles.Add(new ScriptBundle("~/bundles/storage").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/Storage.js"));

            //스토리지
            bundles.Add(new StyleBundle("~/Content/storage").Include(
                      "~/Content/Storage.css"));

            // 디버깅하려면 EnableOptimizations를 false로 설정합니다. 자세한 내용을 보려면
            // http://go.microsoft.com/fwlink/?LinkId=301862를 방문하십시오.
            BundleTable.EnableOptimizations = true;
        }
    }
}
