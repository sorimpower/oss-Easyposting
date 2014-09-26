using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using EasyPosting.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class BlogAPIController : Controller
    {
        private DefaultConnection db = new DefaultConnection();

        // GET: BlogAPI/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: BlogAPI/List
        public ActionResult List()
        {
            return View(db.EP_META.ToList().Where(EP_META => EP_META.UserID == User.Identity.GetUserId()));
        }

        // GET: BlogAPI/Details/5
        public ActionResult Details(int? id)
        {
            EP_METAS eP_METAS = db.EP_META.Find(id);
            if (id == null | eP_METAS.UserID != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
           
            if (eP_METAS == null)
            {
                return HttpNotFound();
            }
            return View(eP_METAS);
        }


        // GET: MetaWeblog/Create
        public ActionResult Create(string SERVICE)
        {
            if (SERVICE == "Tistory" | SERVICE == "Naver" | SERVICE == "WordPress" | SERVICE == "Blogger")
            {
                ViewBag.BlogService = SERVICE;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }

        // POST: MetaWeblog/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598 을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,publish,publish_SITE,Publish_ID,Publish_PW,Publish_BLOGID,Publish_BLOGKEY")] EP_METAS eP_METAS, string SERVICE)
        {
            if (ModelState.IsValid)
            {
                if(SERVICE == "Tistory")
                {
                    try
                    {
                        string[] strarray = eP_METAS.publish_SITE.Split('/');
                        MetaWeblog api = new MetaWeblog("http://" + strarray[2] + "/api");
                        Post[] check_login = new Post[1];
                        check_login = api.getRecentPosts(eP_METAS.Publish_BLOGID, eP_METAS.Publish_ID, eP_METAS.Publish_PW, 1);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "사용자 이름 또는 암호가 잘못되었습니다.");
                        return View();
                    }
                }
                if(SERVICE == "Naver")
                {
                    try
                    {
                        string[] strarray = eP_METAS.publish_SITE.Split('/');
                        MetaWeblog api = new MetaWeblog("https://api.blog.naver.com/xmlrpc");
                        Post[] check_login = new Post[1];
                        check_login = api.getRecentPosts(eP_METAS.Publish_BLOGID, eP_METAS.Publish_ID, eP_METAS.Publish_BLOGKEY, 1);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "사용자 이름 또는 암호가 잘못되었습니다.");
                        return View();
                    }
                }
                if (SERVICE == "WordPress")
                {
                    try
                    {
                        string[] strarray = eP_METAS.publish_SITE.Split('/');
                        MetaWeblog api = new MetaWeblog(eP_METAS.publish_SITE + "/xmlrpc.php");
                        Post[] check_login = new Post[1];
                        check_login = api.getRecentPosts("", eP_METAS.Publish_ID, eP_METAS.Publish_PW, 1);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "사용자 이름 또는 암호가 잘못되었습니다.");
                        //return View();
                        return RedirectToAction("Create", "BlogAPI", new { SERVICE = "WordPress" });
                    }
                }
                //if (SERVICE == "Blogger")
                //{
                //    try 
                //    {
                //        string[] strarray = eP_METAS.publish_SITE.Split('/');
                //        MetaWeblog api = new MetaWeblog("http://www.blogger.com/feeds/" + eP_METAS.ID +"/posts/default");
                        
                //        Post[] check_login = new Post[1];
                //        check_login = api.getRecentPosts("", eP_METAS.Publish_ID, eP_METAS.Publish_PW, 1);
                //    }
                //    catch
                //    {
                //        ModelState.AddModelError("", "사용자 이름 또는 암호가 잘못되었습니다.");
                //        return View();

                //    }
                //}
                eP_METAS.publish = SERVICE;
                eP_METAS.UserID = User.Identity.GetUserId();
                //eP_METAS.publish = ViewData["Metaservice"].ToString;
                db.EP_META.Add(eP_METAS);
                db.SaveChanges();
                return RedirectToAction("Manage","Account");
            }

            return View(eP_METAS);
        }

        // GET: MetaWeblog/Edit/5
        public ActionResult Edit(int? id)
        {
            EP_METAS eP_METAS = db.EP_META.Find(id);
            if (id == null | eP_METAS.UserID != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            if (eP_METAS == null)
            {
                return HttpNotFound();
            }
            return View(eP_METAS);
        }

        // POST: MetaWeblog/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,publish,publish_SITE,Publish_ID,Publish_PW,Publish_BLOGID,Publish_BLOGKEY")] EP_METAS eP_METAS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eP_METAS).State = EntityState.Modified;
                db.SaveChanges();
               
                return RedirectToAction("Index");
            }
            return View(eP_METAS);
        }

        // GET: MetaWeblog/Delete/5
        public ActionResult Delete(int? id)
        {
            EP_METAS eP_METAS = db.EP_META.Find(id);
            if (id == null | eP_METAS.UserID != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (eP_METAS == null)
            {
                return HttpNotFound();
            }
            return View(eP_METAS);
        }

        // POST: MetaWeblog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EP_METAS eP_METAS = db.EP_META.Find(id);
            db.EP_META.Remove(eP_METAS);
            db.SaveChanges();
            return RedirectToAction("Manage","Account");
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
}
