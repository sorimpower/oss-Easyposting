using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EasyPosting.Models;
using CookComputing.XmlRpc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class ArchiveController : Controller
    {
        private DefaultConnection db = new DefaultConnection();

        // GET: /Archive/

        public ActionResult Index(int? Page_No, string Search_Data, [Bind(Include = "ID,GUID,UserID,Ref,Publish1,Publish1_time,Publish2,Publish2_time,Type,Mime_type,Title,Category,Slug,Date,Body,Body2")] EP_POSTS EP_POST)
        {

            if (Page_No == null)
            {
                Page_No = 1;
            }

            //string str = "테스트1 | 테스트2";
            //string splitStr = "|";
            //string[] data = str.Split(splitStr.ToCharArray(), StringSplitOptions.None); //텍스트 나누기
            //string rev = data.Aggregate((cur, next) => cur + "&" + next); //합치기


            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);

            var posts = db.EP_POST.ToList().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Post").OrderByDescending(e => e.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId() && pst.Type == "Post").OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            ViewBag.postnum = db.EP_POST.ToArray().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(e => e.UserID == User.Identity.GetUserId() && e.Type == "Link").Count();

            SaveCate(EP_POST);
            return View(posts);
        }

        // GET: /Editor/Edit/
        public ActionResult VersionEdit(int? id)
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
            return View(EP_POST);
        }

        // POST: /Editor/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult VersionEdit([Bind(Include = "ID,Title,Category,Slug,Date,Body,Body2,UserID")] EP_POSTS EP_POST, int? return_id)
        {
            if (ModelState.IsValid)
            {
                EP_POSTS ep = db.EP_POST.Find(return_id);
                string guidval = ep.GUID;
                EP_POST.GUID = guidval;
                EP_POST.Ref = guidval;
                EP_POST.UserID = User.Identity.GetUserId();
                EP_POST.Date = DateTime.Now;
                EP_POST.Type = "Post";

                db.EP_POST.Add(EP_POST);
                db.SaveChanges();

                return RedirectToAction("Index", "Archive");
            }
            return View(EP_POST);
        }


        public void SaveCate(EP_POSTS EP_POSTS)
        {

            IEnumerable<EP_METAS> metas = db.EP_META.ToList().Where(c => c.UserID == User.Identity.GetUserId());
            if (metas != null)
            {
                foreach (EP_METAS meta in metas)
                {
                    String Publish_BLOGID = meta.Publish_BLOGID;
                    String service = meta.publish;
                    String publish_SITE = meta.publish_SITE;
                    String Publish_ID = meta.Publish_ID;
                    String Publish_BLOGKEY = meta.Publish_BLOGKEY;
                    String Publish_PW = meta.Publish_PW;
                    if (meta.publish == "Tistory")
                    {
                        Category[] category;
                        List<string> categorylist = new List<string>();
                        try
                        {
                            MetaWeblog api = new MetaWeblog(publish_SITE + "/api");
                            //api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, true);
                            category = api.getCategories(Publish_BLOGID, Publish_ID, Publish_BLOGKEY);
                            foreach (Category cate in category)
                            {
                                categorylist.Add(cate.title);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                        string catstring = categorylist.ToArray().Aggregate((cur, next) => cur + "|" + next);

                        EP_POSTS.Title = publish_SITE;
                        EP_POSTS.GUID = Guid.NewGuid().ToString();
                        EP_POSTS.UserID = User.Identity.GetUserId();
                        EP_POSTS.Date = DateTime.Now;
                        EP_POSTS.Type = "Categories";
                        EP_POSTS.Mime_type = service;
                        EP_POSTS.Category = catstring;
                        db.EP_POST.Add(EP_POSTS);
                        db.SaveChanges();
                    }

                    if (meta.publish == "Naver")
                    {
                        Category[] category;
                        List<string> categorylist = new List<string>();
                        try
                        {
                            MetaWeblog api = new MetaWeblog("https://api.blog.naver.com/xmlrpc");
                            //api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, true);
                            category = api.getCategories(Publish_BLOGID, Publish_ID, Publish_BLOGKEY);
                            foreach (Category cate in category)
                            {
                                categorylist.Add(cate.title);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                        string catstring = categorylist.ToArray().Aggregate((cur, next) => cur + "|" + next);

                        EP_POSTS.Title = publish_SITE;
                        EP_POSTS.GUID = Guid.NewGuid().ToString();
                        EP_POSTS.UserID = User.Identity.GetUserId();
                        EP_POSTS.Date = DateTime.Now;
                        EP_POSTS.Type = "Categories";
                        EP_POSTS.Mime_type = service;
                        EP_POSTS.Category = catstring;
                        db.EP_POST.Add(EP_POSTS);
                        db.SaveChanges();
                    }
                    if (meta.publish == "WordPress")
                    {
                        Category[] category;
                        List<string> categorylist = new List<string>();
                        try
                        {
                            MetaWeblog api = new MetaWeblog(publish_SITE + "/xmlrpc.php");
                            //api.newPost("", Publish_ID, Publish_PW, post, true);
                            category = api.getCategories("1", Publish_ID, Publish_PW);
                            foreach (Category cate in category)
                            {
                                categorylist.Add(cate.title);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                        string catstring = categorylist.ToArray().Aggregate((cur, next) => cur + "|" + next);

                        EP_POSTS.Title = publish_SITE;
                        EP_POSTS.GUID = Guid.NewGuid().ToString();
                        EP_POSTS.UserID = User.Identity.GetUserId();
                        EP_POSTS.Date = DateTime.Now;
                        EP_POSTS.Type = "Categories";
                        EP_POSTS.Mime_type = service;
                        EP_POSTS.Category = catstring;
                        db.EP_POST.Add(EP_POSTS);
                        db.SaveChanges();

                    }
                }
            }
        }

        // GET: /Archive/Version
        public ActionResult Version(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_POSTS EP_POST = db.EP_POST.Find(id);
            string guidroot = EP_POST.GUID;
            if (EP_POST.UserID != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }
            if (EP_POST == null)
            {
                return HttpNotFound();
            }

            var child = db.EP_POST.Where(a => a.Ref == guidroot).OrderByDescending(a => a.ID).Take(10);

            ViewBag.child = child;
            return View();
        }


        // GET: /Archive/Posting/5
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
            // db.EP_META.ToList().Where(EP_META => EP_META.UserID == User.Identity.GetUserId());
            //List<SelectListItem> items = new List<SelectListItem>();
            var metablog = new List<Metaweb>();
            foreach (var result in db.EP_META.ToList().Where(EP_META => EP_META.UserID == User.Identity.GetUserId()))
            {
                string ids = User.Identity.GetUserId().ToString();
                string site = result.publish_SITE;
                //items.Add(new SelectListItem { Text = result.publish_SITE, Value = result.publish_SITE });
                var catedata = db.EP_POST.Where(a => a.UserID == ids && a.Type == "Categories" && a.Title == site).OrderByDescending(a => a.ID).First();
                string splitStr = "|";
                string cate_str = catedata.Category;
                string[] cates = new string[100];
                if (cate_str != null)
                {
                    cates = cate_str.Split(splitStr.ToCharArray(), StringSplitOptions.None); //텍스트 나누기                
                }
                else if (cate_str == null)
                {
                    cates[0] = "분류없음";
                }
                //List<string> cat = new List<string>();



                metablog.Add(new Metaweb()
                {
                    category = cates,
                    service = result.publish,
                    site = result.publish_SITE
                });
            }

            ViewBag.metas = metablog;
            //ViewBag.Categories
            return View(EP_POST);
        }

        // POST: /Archive/Posting/5
        //[HttpPost, ActionName("Posting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Posting(int? id, string[] BlogService, string[] category, int?[] size, int[] pub)
        {
            Post post = new Post();

            EP_POSTS EP_POST = db.EP_POST.Find(id);

            bool post_private = true;

            for (int i = 0; i < BlogService.Count(s => s != null); i++)
            {

                int reali = Convert.ToInt32(BlogService[i].Substring(0, 1));

                char sp = '|';
                string[] spstring = BlogService[i].Split(sp);
                string realsite = spstring[1];
                string realcate = category[reali];
                int? realsize = size[reali];

                if (pub[reali] == 0)
                {
                    post_private = false;
                }

                IEnumerable<EP_METAS> metas = db.EP_META.ToList().Where(c => c.publish_SITE == realsite && c.UserID == User.Identity.GetUserId());
                foreach (var meta in metas)
                {
                    String Publish_BLOGID = meta.Publish_BLOGID;
                    String service = meta.publish;
                    String publish_SITE = meta.publish_SITE;
                    String Publish_ID = meta.Publish_ID;
                    String Publish_BLOGKEY = meta.Publish_BLOGKEY;
                    String Publish_PW = meta.Publish_PW;
                    if (service == "Tistory")
                    {
                        try
                        {
                            MetaWeblog api = new MetaWeblog(publish_SITE + "/api");
                            //EP_POST.Body 545가 있다면
                            if (realsize == null)
                            {
                                post.description = EP_POST.Body;
                                post.title = EP_POST.Title;
                                post.mt_keywords = EP_POST.Slug;
                                post.categories = new string[] { realcate };
                                post.dateCreated = DateTime.UtcNow;
                                api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, post_private);
                            }
                            else if (realsize != null)
                            {
                                string des = EP_POST.Body.Replace(@"width='545'", @"width='" + realsize + @"'");
                                post.description = des;
                                post.title = EP_POST.Title;
                                post.mt_keywords = EP_POST.Slug;
                                post.categories = new string[] { realcate };
                                post.dateCreated = DateTime.UtcNow;
                                api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, post_private);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                    }

                    if (service == "Naver")
                    {
                        try
                        {
                            MetaWeblog api = new MetaWeblog("https://api.blog.naver.com/xmlrpc");
                            if (realsize == null)
                            {
                                string des = EP_POST.Body.Replace(@"width='545'", @"width='730'");
                                post.description = des;
                                post.title = EP_POST.Title;
                                post.tags = EP_POST.Slug;
                                post.dateCreated = DateTime.UtcNow;
                                post.categories = new string[] { realcate };
                                api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, post_private);
                            }
                            else if (realsize != null)
                            {
                                string des = EP_POST.Body.Replace(@"width='545'", @"width='" + realsize + @"'");
                                post.description = des;
                                post.title = EP_POST.Title;
                                post.mt_keywords = EP_POST.Slug;
                                post.categories = new string[] { realcate };
                                post.dateCreated = DateTime.UtcNow;
                                api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, post_private);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                    }
                    if (service == "WordPress")
                    {
                        try
                        {
                            MetaWeblog api = new MetaWeblog(publish_SITE + "/xmlrpc.php");
                            if (realsize != null)
                            {
                                string des = EP_POST.Body.Replace(@"width='545'", @"width='" + realsize + @"'");
                                post.description = des;
                                post.title = EP_POST.Title;
                                post.mt_keywords = EP_POST.Slug;
                                post.categories = new string[] { realcate };
                                post.dateCreated = DateTime.UtcNow;
                                api.newPost(Publish_BLOGID, Publish_ID, Publish_BLOGKEY, post, post_private);
                            }
                            else
                            {
                                post.description = EP_POST.Body;
                                post.title = EP_POST.Title;
                                post.mt_keywords = EP_POST.Slug;
                                post.dateCreated = DateTime.UtcNow;
                                post.categories = new string[] { realcate };
                                api.newPost("", Publish_ID, Publish_PW, post, post_private);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "실패했습니다.");
                        }
                    }

                    if (EP_POST.Publish1 != null)
                    {
                        EP_POST.Publish2 = EP_POST.Publish1;
                        EP_POST.Publish2_time = EP_POST.Publish1_time;
                        EP_POST.Publish2_site = EP_POST.Publish1_site;
                        EP_POST.Publish1 = service;
                        EP_POST.Publish1_time = DateTime.Now;
                        EP_POST.Publish1_site = publish_SITE;

                    }
                    else if (EP_POST.Publish1 == null)
                    {
                        EP_POST.Publish1 = service;
                        EP_POST.Publish1_time = DateTime.Now;
                        EP_POST.Publish1_site = publish_SITE;
                    }
                    db.Entry(EP_POST).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

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
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class Metaweb
    {
        public string site { get; set; }
        public string service { get; set; }
        public string[] category { get; set; }

    }
}
