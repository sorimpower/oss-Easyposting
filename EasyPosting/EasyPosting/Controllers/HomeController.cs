using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;
using System.Drawing;
using EasyPosting.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : Controller
    {
        private DefaultConnection db = new DefaultConnection();
        
        public ActionResult Index()
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.postednum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post" && EP_POST.Publish1 != null).Count();
            int attach = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            int image = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            int video = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            int link = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();
            ViewBag.store = attach + image + video + link;

            ViewBag.ra = getPhrase(100, 1);
            string u = Request.ServerVariables["HTTP_USER_AGENT"];
            

            return View();          
        }

        //랜덤 문자열 생성
        public string getPhrase(int high, int low)
        {
            Guid myGuid = Guid.NewGuid();  //GUID 인스턴스 초기화
            string iRnd = myGuid.ToString("N"); //하이픈 없는 GUID값 생성
            return iRnd.Substring(0, 8); // 7번째 까지 자르기
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }

    public static class HtmlHelperExtensions
    {
        public static IHtmlString GenerateRelayQrCode(this HtmlHelper html, string groupName, int height = 250, int width = 250, int margin = 0)
        {
            var qrValue =  groupName;
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
           
            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Gif);

                var img = new TagBuilder("img");
                img.MergeAttribute("alt", "your alt tag");
                img.Attributes.Add("src", String.Format("data:image/gif;base64,{0}",
                    Convert.ToBase64String(stream.ToArray())));

                return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
            }
        }
    }


}