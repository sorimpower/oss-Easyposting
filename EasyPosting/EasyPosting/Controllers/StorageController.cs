using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using EasyPosting.Models;
using CookComputing.XmlRpc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Google.API.Search;

using System.Xml;

using ZXing;
using System.Text.RegularExpressions;



namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class StorageController : Controller
    {
        private DefaultConnection db = new DefaultConnection();

        public static object refTrue = true;
        public static object refFalse = false;
        public static object refMissing = Type.Missing;







        // GET: /Storage/Attachment
        public ActionResult Attachment(int? Page_No, string Search_Data)
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            if (Page_No == null)
            {
                Page_No = 1;
            }
            int Size_Of_Page = 8;
            int No_Of_Page = (Page_No ?? 1);
            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.Type == "Attachment" && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            return View(posts);
        }

        // GET: /Storage/Attachment
        public ActionResult AttachmentEmbed(int? Page_No, string Search_Data, string Filter_Value)
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            //if (Page_No == null)
            //{
            //    Page_No = 1;
            //}
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);
            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            return View(posts);
        }
                
        public ActionResult YoutubeEmbed()
        {
            return View();
        }
        
        // POST: /Storage/Search
        public ActionResult Search(int? Page_No, string Search_Data)
        {
            if (Page_No == null)
            {
                Page_No = 1;
            }

            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);

            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            return View(posts);
        }

        // GET: /Editor/Embed
        public ActionResult Embed(int? Page_No, string Search_Data)
        {
            if (Page_No == null)
            {
                Page_No = 1;
            }

            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);

            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            return View(posts);
        }


        // GET: /Storage/Image
        public ActionResult Image(int? Page_No, string Search_Data)
        {
            if (Page_No == null)
            {
                Page_No = 1;
            }
            int Size_Of_Page = 8;
            int No_Of_Page = (Page_No ?? 1);

            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();
            ViewBag.Search_Data = Search_Data;

            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.Type == "Image" && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            return View(posts);
        }        

        public ActionResult ImageEmbed(int? Page_No, string Search_Data)
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            if (Page_No == null)
            {
                Page_No = 1;
            }
            int Size_Of_Page = 6;
            int No_Of_Page = (Page_No ?? 1);
            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            return View(posts);
        }

        public ActionResult BingImageEmbed()
        {
            return View();
        }

        public ActionResult YoutubeVideoEmbed()
        {
            return View();
        }

        // GET: /Storage/Video
        public ActionResult Video(int? Page_No, string Search_Data)
        {
            if (Page_No == null)
            {
                Page_No = 1;
            }
            int Size_Of_Page = 8;
            int No_Of_Page = (Page_No ?? 1);

            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();
            ViewBag.Search_Data = Search_Data;

            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.Type == "Video" && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            return View(posts);
        }

        public ActionResult VideoEmbed(int? Page_No, string Search_Data)
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            if (Page_No == null)
            {
                Page_No = 1;
            }
            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);
            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId() && pst.Type == "Video").OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            return View(posts);
        }

        [HttpPost]
        public ActionResult ImageSearch(string searchText)
        {
            ViewBag.searchText = searchText;
            List<ImageResult> searchData = new List<ImageResult>();
            if (!string.IsNullOrEmpty(searchText))
            {
                // Helper 클래스 초기화(AppSettings에서 불러온 Bing API 주소 사용)
                var bingContainer = new EasyPosting.Models.BingSearchContainer(new Uri(ConfigurationManager.AppSettings["BingApiUrl"]));

                // Bing 계정 키를 AppSettings에서 불러옴
                var accountKey = ConfigurationManager.AppSettings["BingAccountKey"];

                // the next line configures the bingContainer to use your credentials.
                bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);

                //Setup Query with container 
                var webQuery = bingContainer.Image(string.Format(searchText), null, null, null, null, null, null);
                //var webQuery = bingContainer.Web(
                //    string.Format("site:www.microsoft.com {0}", searchText), null, null, null, null, null, null, null);

                var ImageResults = webQuery.Execute();


                //결과를 반복해서 받아와서 모델에 정의되어 있는 변수를 매핑해 뷰에서 보여줌
                foreach (var result in ImageResults)
                {
                    searchData.Add(new ImageResult()
                    {
                        Title = result.Title,
                        Thumbnail = result.Thumbnail,
                        MediaUrl = result.MediaUrl
                    });

                }
                return Json(new { success = true, bingData = searchData });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult VideoSearch(string searchText)
        {
            ViewBag.searchText = searchText;
            GvideoSearchClient imageClient = new GvideoSearchClient("epp.so");
            List<VideoResult> searchData = new List<VideoResult>();
            IList<IVideoResult> results = imageClient.Search(searchText, 32);
            if (!string.IsNullOrEmpty(searchText))
            {

                //결과를 반복해서 받아와서 모델에 정의되어 있는 변수를 매핑해 뷰에서 보여줌
                foreach (var result in results)
                {

                    searchData.Add(new VideoResult()
                    {
                        Title = result.Title,
                        Thumbnail = "https://img.youtube.com/vi/" + result.Url.Substring(result.Url.LastIndexOf('=') + 1) + "/0.jpg",
                        MediaUrl = result.Url,
                        DisplayUrl = result.Content
                        //RunTime = result.ViewCount,
                        //DisplayUrl = result.PlayUrl
                    });

                }
                return Json(new { success = true, youtubeData = searchData });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        // GET: /Storage/Link
        public ActionResult Link()
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            return View(db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link"));
        }


        // GET: /Storage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }
            var post_type = EP_POST.Type.ToString();
            if (EP_POST.Type == "Video")
            {
                ViewBag.Videoembed = "<iframe width='640' height='360' src='https://www.youtube.com/embed/" + EP_POST.Body.Substring(EP_POST.Body.LastIndexOf("=") + 1) + "'frameborder='0' allowfullscreen></iframe>";
            }
            ViewBag.posttype = post_type;

            return View(EP_POST);
        }

        // GET: /Storage/DetailsPDF/5

        public ActionResult DetailsPDF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }
            var post_type = EP_POST.Type.ToString();
            ViewBag.posttype = post_type;
            return View(EP_POST);
        }


        // GET: /Storage/Details/5
        public ActionResult DetailsEmbed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }
            var post_type = EP_POST.Type.ToString();
            if (EP_POST.Type == "Video")
            {
                ViewBag.Videoembed = "<iframe width='640' height='360' src='https://www.youtube.com/embed/" + EP_POST.Body.Substring(EP_POST.Body.LastIndexOf("=") + 1) + "'frameborder='0' allowfullscreen></iframe>";
            }
            ViewBag.posttype = post_type;

            return View(EP_POST);
        }

        public ActionResult MapEmbed(int? Page_No, string Search_Data)
        {
            return View();
        }
        //String url = "http://www.there.net/img.png";
        //String fileName = "d:/a.png";
        //if (!DownloadRemoteImageFile(url, fileName))
        //{
        //    MessageBox.Show("Download Failed: " + url);
        //}
        [HttpPost]
        public ActionResult Savetag(string Type, string Title, string MediaUrl, string Thumbnail)
        {
            string guidval = Guid.NewGuid().ToString();
            string imagefilename = null;
            try
            {
                if (Type == "Image")
                {
                    String url = MediaUrl;
                    var originalDirectory = new DirectoryInfo(string.Format("{0}User\\Posts", Server.MapPath(@"\")));
                    string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Image");
                    string mimetype = getMIME(url);
                    string extension = null;
                    if (mimetype.Contains("PNG") || mimetype.Contains("png") || mimetype.Contains("Png"))
                        extension = ".png";
                    if (mimetype.Contains("JPG") || mimetype.Contains("JPEG") || mimetype.Contains("jpg") || mimetype.Contains("jpeg"))
                        extension = ".jpg";
                    if (mimetype.Contains("gif") || mimetype.Contains("GIF"))
                        extension = ".gif";
                    DownloadRemoteImageFile(url, pathString + "/" + guidval + extension);
                    imagefilename = guidval + extension;
                }
                EP_POSTS EP_POST = new EP_POSTS();
                if (ModelState.IsValid)
                {
                    EP_POST.GUID = guidval;
                    EP_POST.UserID = User.Identity.GetUserId();
                    if (Type == "Video") EP_POST.Body = MediaUrl.Replace("http://", "https://");
                    if (Type == "Image") EP_POST.Body = imagefilename;
                    EP_POST.Title = Title;
                    EP_POST.Date = DateTime.Now;
                    EP_POST.Thumbnail = Thumbnail;
                    EP_POST.Type = Type;

                    db.EP_POST.Add(EP_POST);
                    db.SaveChanges();
                }

                return Json(new { success = true, url = "/User/Posts/Image/"+EP_POST.Body });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        public string getMIME(string url)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            string contentType = "";
            if (request != null)
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response != null)
                    contentType = response.ContentType;
            }
            return contentType;
        }
        private bool DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            bool bImage = response.ContentType.StartsWith("image",
                StringComparison.OrdinalIgnoreCase);
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                bImage)
            {
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = System.IO.File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        public PartialViewResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Uploads([Bind(Include = "ID,GUID,UserID,Ref,Publish1,Publish1_time,Publish2,Publish2_time,Type,Mime_type,Title,Category,Slug,Date,Body")] EP_POSTS EP_POST)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fType = "";
            string fMime = "";
            string dbName = "";
            var fPath = "";
            var postguid = Guid.NewGuid().ToString();

            //PPTX 변환을 위한 변수
            String paths = null;
            int Slide = 0;

            //파일 업로드 완료시 Json Response를 위한 변수
            var status = new List<ViewDataUploadFileResult>();

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                fName = file.FileName;
                fMime = MimeMapping.GetMimeMapping(file.FileName);
                fType = fName.Substring(fName.LastIndexOf('.'));
                dbName = postguid + fType;

                status.Add(new ViewDataUploadFileResult()
                {
                    name = fName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    //thumbnailUrl = System.IO.Path.Combine(new DirectoryInfo(string.Format("{0}User\\Posts", Server.MapPath(@"\"))).ToString(), "User", fileName),
                    //url = System.IO.Path.Combine(new DirectoryInfo(string.Format("{0}User\\Posts", Server.MapPath(@"\"))).ToString(), "User", fileName),
                    //deleteUrl = System.IO.Path.Combine(new DirectoryInfo(string.Format("{0}User\\Posts", Server.MapPath(@"\"))).ToString(), "User", fileName),
                    //deleteType = "DELETE"
                });



                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}User\\Posts", Server.MapPath(@"\")));

                    string pathString = "";

                    // 문서 파일의 MIME 타입
                    //.docx, application/vnd.openxmlformats-officedocument.wordprocessingml.document
                    //.pptx, application/vnd.openxmlformats-officedocument.presentationml.presentation
                    //.xlsx, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                    //.hwp, application/unknown 또는  application/octet-stream  또는 application/hwp 

                    if (fMime.Contains("image/"))
                        pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Image");
                    else if (fMime.Contains("video/"))
                        pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Video");
                    else
                        pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Attachment");

                    var fileName1 = postguid;
                    bool isExists = System.IO.Directory.Exists(pathString);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);
                    var path = string.Format("{0}\\{1}", pathString, dbName);

                    file.SaveAs(path);
                    String newpath = (String)path;
                    paths = newpath;

                    string rawFilePath = (string)pathString;
                    string webDirectory = System.IO.Path.Combine(originalDirectory.ToString(), "User", fileName);

                }


                //파일 업로드가 성공할 경우
                if (isSavedSuccessfully)
                {

                    EP_POST.GUID = postguid;
                    EP_POST.Title = fName;
                    EP_POST.Body = fPath;
                    EP_POST.Mime_type = fMime;
                    EP_POST.Slug = Convert.ToString(Slide);
                    EP_POST.UserID = User.Identity.GetUserId();
                    EP_POST.Date = DateTime.Now;
                    if (fMime.Contains("image/"))
                    {
                        EP_POST.Type = "Image";
                    }
                    else if (fMime.Contains("video/"))
                    {
                        EP_POST.Type = "Video";
                    }
                    else
                    {
                        EP_POST.Type = "Attachment";
                    }
                    EP_POST.Body = postguid + fType;
                    db.EP_POST.Add(EP_POST);
                    db.SaveChanges();

                    var viewresult = Json(new { files = status });
                    return viewresult;
                }
                else
                {
                    return Json(new { Message = "업로드에 실패했습니다." });
                }



            }
            return View();

        }

  
        public void RegexPPTXHTML(string paths, int Slide, string filename)
        {
            String ival = null;

            // 정규식 패턴 정의
            List<string> pts = new List<string>();
            pts.Add("<html xmlns.*[^>]*>");
            pts.Add("<head>(?:.|\n|\r)+?</head>");
            pts.Add("<body.*[^>]*<*[^>]*>");
            pts.Add("<p:shaperange.*./>");
            pts.Add("<[!].*.]>");
            pts.Add("<v:shape.*[^>]*>");
            pts.Add("<v:*[^>]*/>");
            pts.Add("<div id=SlideObj.*[^>]*>");
            pts.Add("<p:slide.*[^>]*>");
            pts.Add("<p:shaperange.*[^>]*/>");
            pts.Add("<o:lock.*[^>]*/>");
            pts.Add("<p:placeholder.*[^>]*/>");
            pts.Add("</v:shape>");
            pts.Add("<div v:shape.*>");
            pts.Add("<div class=O[0-9].*[^'>]*>");
            pts.Add("<*.v:formulas>");
            pts.Add("</p:slide>");
            pts.Add("<*.body>");
            pts.Add("<*.html>");
            pts.Add("<v:rect(?:.|\n|\r)+?</v:rect>");
            pts.Add("<p:animation.*/>");
            pts.Add("<*.oa:.*>");
            pts.Add("<*.oa:.*/>");
            pts.Add("<oa:.*./>");
            pts.Add("<oa:.*[^>]*./>");
            pts.Add("<oa:.*[^>]*>");
            pts.Add("lang=[A-Z].-.[A-Z]");
            pts.Add("lang=[A-Z].");
            pts.Add("<*.v:shapetype*.>");
            pts.Add("<*.v:handles*.>");
            pts.Add("<v:group(?:.|\n|\r)+?</v:group>");
            pts.Add("<v:group.*\">");
            List<string> pptxlist = new List<string>();



            try
            {
                string xmlpath = paths + ".files\\filelist.xml";
                XmlDocument xml = new XmlDocument();
                xml.Load(xmlpath);
                XmlNode fileinfo = xml.DocumentElement;

                foreach (XmlNode info in fileinfo)
                {
                    XmlNode href = info.Attributes.GetNamedItem("HRef");
                    string hrefinfo = href.Value.ToString();
                    if (Regex.IsMatch(hrefinfo, @"slide.[0-9]...htm"))
                        pptxlist.Add(hrefinfo);
                }
            }



            catch (ArgumentException)
            {



            }


            int i = 1;
            //html 파일을 다이나믹 에디터에 넣을 수 있는 포맷으로 변환
            //for (int i = 1; i < Slide; i++)
            foreach (string pptx in pptxlist)
            {
                ival = i.ToString("0000");
                //String pptx0 = System.IO.File.ReadAllText(paths + ".files\\" + "slide" + ival + ".htm", Encoding.Default);
                String pptx0 = System.IO.File.ReadAllText(paths + ".files\\" + pptx, Encoding.Default);
                var pptx1 = Encoding.GetEncoding("euc-kr");
                var utf8Encoding = Encoding.UTF8;

                string pptx2 = pptx1.GetString(pptx1.GetBytes(pptx0));
                foreach (string pt in pts)
                {
                    pptx2 = Regex.Replace(pptx2, @pt, "");
                }

                string pptx2_paths = @"src=""" + "https://" + Request.Url.Host + ":" + Request.Url.Port + "/User/Posts/Attachment/" + filename + ".files/";
                pptx2 = pptx2.Replace(@"src=""", pptx2_paths);
                System.IO.File.WriteAllText(paths + ".files\\" + "slides" + i + ".htm", pptx2, Encoding.UTF8);

                i++;
            }
        }

        // GET: /Storage/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EP_POSTS EP_POST = db.EP_POST.Find(id);
        //    if (EP_POST.UserID != User.Identity.GetUserId())
        //    {
        //        return HttpNotFound();
        //    }
        //    if (EP_POST == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(EP_POST);
        //}

        // POST: /Storage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }
            string type = EP_POST.Type;
            db.EP_POST.Remove(EP_POST);
            db.SaveChanges();
            if (type == "Image")
                return RedirectToAction("Image");
            if (type == "Attachment")
                return RedirectToAction("Attachment");
            if (type == "Video")
                return RedirectToAction("Video");
            else
                return RedirectToAction("Image");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private readonly IPDFConverter _pdfConverter;

        public ActionResult PDFConvert(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }
            ViewData.Model = EP_POST;
            string relativePath = "~/Views/Storage/DetailsPDF.cshtml";
            string content = "";
            var view = ViewEngines.Engines.FindView(ControllerContext, relativePath, null);

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();
            }
            byte[] content1 = Encoding.UTF8.GetBytes(content);
            string content2 = Encoding.UTF8.GetString(content1);

            string path = Server.MapPath("~/User/");
            byte[] pdfBuf = ConvertToPDF(content2, path);
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");

        }



        public byte[] ConvertToPDF(string source, string commandLocation)
        {
            string HtmlToPdfExePath = Server.MapPath("~/exe/") + "wkhtmltopdf.exe";
            Process p;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(commandLocation, HtmlToPdfExePath);
            psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);

            // run the conversion utility
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // note: that we tell wkhtmltopdf to be quiet and not run scripts
            //string args = "-q -n ";
            string args = "";

            args += "--enable-smart-shrinking ";
            args += "--orientation Portrait ";
            args += "--outline-depth 0 ";
            args += "--page-size A4 ";
            args += " - -";

            psi.Arguments = args;

            p = Process.Start(psi);

            try
            {
                using (StreamWriter stdin = p.StandardInput)
                {
                    stdin.AutoFlush = true;
                    stdin.Write(source);
                }

                //read output
                byte[] buffer = new byte[32768];
                byte[] file;
                using (var ms = new MemoryStream())
                {
                    while (true)
                    {
                        int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            break;
                        ms.Write(buffer, 0, read);
                    }
                    file = ms.ToArray();
                }

                p.StandardOutput.Close();
                // wait or exit
                p.WaitForExit(60000);

                // read the exit code, close process
                int returnCode = p.ExitCode;
                p.Close();

                if (returnCode == 0)
                    return file;
                // else
                // log.Error("Could not create PDF, returnCode:" + returnCode);
            }
            catch (Exception)
            {
                //  log.Error("Could not create PDF", ex);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
            return null;
        }

    }



}
