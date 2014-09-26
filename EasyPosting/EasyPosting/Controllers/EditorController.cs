using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using EasyPosting.Models;
using CookComputing.XmlRpc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using kr.ac.kaist.swrc.jhannanum.comm;
using kr.ac.kaist.swrc.jhannanum.hannanum;
using System.Web.Script.Serialization;

namespace EasyPosting.Controllers
{
    class Record
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    [Authorize]
    [RequireHttps]
    public class EditorController : Controller
    {
        string result_sb = "";
        private DefaultConnection db = new DefaultConnection();

        // GET: /Editor/
        public ActionResult Index(int? Page_No, string Search_Data)
        {
            if (Page_No == null)
            {
                Page_No = 1;
            }

            int Size_Of_Page = 4;
            int No_Of_Page = (Page_No ?? 1);

            //var posts = from pst in db.EP_POST select pst; 
            //posts = posts.Where(pst => pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page));
            var posts = db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").OrderByDescending(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page);
            if (Search_Data != null)
                posts = db.EP_POST.ToList().Where(pst => pst.Type == "Post" && pst.Title.ToUpper().Contains(Search_Data.ToUpper()) && pst.UserID == User.Identity.GetUserId()).OrderByDescending(pst => pst.ID).ToPagedList(No_Of_Page, Size_Of_Page);

            //int userpost = db.EditorModel.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId()).Count();

            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();

            //return View(db.EP_POST.ToList().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").OrderBy(EP_POST => EP_POST.ID).ToPagedList(No_Of_Page, Size_Of_Page));
            return View(posts);
            /*EP_POST EP_POST = db.EditorModel.Find(User.Identity.GetUserId());
            if (EP_POST == null)
            {
                return View();
            }
            return View(EP_POST);*/
        }

        // GET: /Editor/Create
        public ActionResult Create()
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            return View();
        }

        // GET: /Editor/MarkdownCreate
        public ActionResult MarkdownCreate()
        {
            ViewBag.postnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Post").Count();
            ViewBag.attachnum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Attachment").Count();
            ViewBag.imagenum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Image").Count();
            ViewBag.videonum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Video").Count();
            ViewBag.linknum = db.EP_POST.ToArray().Where(EP_POST => EP_POST.UserID == User.Identity.GetUserId() && EP_POST.Type == "Link").Count();

            return View();
        }

        // 버전 관리 ajax
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Version(string title, string tag, string description, string save_count, int id, string table)
        {
            try
            {
                int return_id = 0;
                string guidval = Guid.NewGuid().ToString();
                if (id != 0)
                {
                    EP_POSTS ep = db.EP_POST.Find(id);
                    string refguid = ep.GUID;
                    EP_POSTS EP_POST = new EP_POSTS();
                    EP_POST.GUID = guidval;
                    EP_POST.UserID = User.Identity.GetUserId();
                    EP_POST.Ref = refguid;
                    EP_POST.Body = table;
                    EP_POST.Body2 = description;
                    EP_POST.Title = title;
                    EP_POST.Date = DateTime.Now;
                    EP_POST.Slug = tag;
                    EP_POST.Type = "Version";

                    db.EP_POST.Add(EP_POST);
                    db.SaveChanges();
                }
                else if (id == 0)
                {
                    EP_POSTS EP_POST = new EP_POSTS();

                    EP_POST.GUID = guidval;
                    EP_POST.Ref = guidval;
                    EP_POST.UserID = User.Identity.GetUserId();
                    EP_POST.Body = table;
                    EP_POST.Body2 = description;
                    EP_POST.Title = title;
                    EP_POST.Date = DateTime.Now;
                    EP_POST.Slug = tag;
                    EP_POST.Type = "Version";

                    db.EP_POST.Add(EP_POST);
                    db.SaveChanges();
                    return_id = EP_POST.ID;
                }
                return Json(new { success = true, id = return_id });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        // 코드 하이라이트 ajax
        [HttpPost]
        public ActionResult Code(string lexer, string style, string code)
        {
            try
            {
                byte[] responsebytes;
                string divstyles = ("border:solid gray;border-width:.1em .1em .1em .8em;padding:.2em .6em;");
                string ch_code = "";
                for (int i = 1; i < code.Length; i += 2)
                {
                    ch_code += code[i];
                }
                string result = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("code", ch_code);
                    reqparm.Add("lexer", lexer);
                    reqparm.Add("style", style);
                    reqparm.Add("divstyles", divstyles);
                    responsebytes = wc.UploadValues("http://hilite.me/api", "POST", reqparm);
                    result = Encoding.UTF8.GetString(responsebytes);
                }

                return Json(new { success = true, result = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Code Ajax Error\nERRORINFO: " + ex.Message });
            }
        }

        // 키워드 입력 ajax
        [HttpPost]
        public ActionResult SaveText(string str_text)
        {
            try
            {
                List<EP_KEYWORD> keywords = NHanNanum(str_text);
                if (keywords.Count == 0)
                {
                    return Json(new { success = false });
                }
                else
                {
                    string[] title_arr = new string[keywords.Count];
                    string[] link_arr = new string[keywords.Count];
                    string[] des_arr = new string[keywords.Count];

                    for (int i = 0; i < keywords.Count; i++)
                    {
                        keywords[i].Description = keywords[i].Description.Replace("&#8203;", " ");
                        keywords[i].Description.Trim();
                        title_arr[i] = keywords[i].Title;
                        if (keywords[i].Description.Length > 20)
                        {
                            if (keywords[i].Description.Length < 50)
                                des_arr[i] = keywords[i].Link + "|" + keywords[i].Description.Substring(0, keywords[i].Description.Length) + "...";
                            else
                                des_arr[i] = keywords[i].Link + "|" + keywords[i].Description.Substring(0, 50) + "...";
                        }
                        else
                            des_arr[i] = keywords[i].Link + "|" + keywords[i].Description + "...";
                    }

                    return Json(new { success = true, title = title_arr, description = des_arr, result_sb = result_sb });
                }
            }
            catch (Exception)
            {

                return Json(new { success = "error" });
            }
        }

        // 키워드 직접입력 ajax
        [HttpPost]
        public ActionResult Direct_Search(string str_text)
        {
            try
            {
                List<EP_KEYWORD> keywords = Direct(str_text);
                if (keywords.Count == 0)
                {
                    return Json(new { success = false });
                }
                else
                {
                    string[] title_arr = new string[keywords.Count];
                    string[] link_arr = new string[keywords.Count];
                    string[] des_arr = new string[keywords.Count];

                    for (int i = 0; i < keywords.Count; i++)
                    {
                        keywords[i].Description = keywords[i].Description.Replace("&#8203;", " ");
                        keywords[i].Description.Trim();
                        title_arr[i] = keywords[i].Title;
                        if (keywords[i].Description.Length > 20)
                        {
                            if (keywords[i].Description.Length < 50)
                                des_arr[i] = keywords[i].Link + "|" + keywords[i].Description.Substring(0, keywords[i].Description.Length) + "...";
                            else
                                des_arr[i] = keywords[i].Link + "|" + keywords[i].Description.Substring(0, 50) + "...";
                        }
                        else
                            des_arr[i] = keywords[i].Link + "|" + keywords[i].Description + "...";
                    }

                    return Json(new { success = true, title = title_arr, description = des_arr });
                }
            }
            catch (Exception)
            {
                return Json(new { success = "error" });
            }
        }

        public List<EP_KEYWORD> Direct(string file)
        {
            try
            {
                string document = file;
                string sb = "";
                string[] cmp = document.Split(" ");
                if (cmp.Length == 1 && cmp[0].Length <= 5)
                {
                    sb += cmp[0] + "|";
                }
                else
                {
                    string newFileName = "/";
                    string newFileLocation = HttpContext.Server.MapPath(newFileName);
                    Workflow workflow = new Workflow(newFileLocation);

                    workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.SentenceSegmentor.SentenceSegmentor(), null);
                    workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.InformalSentenceFilter.InformalSentenceFilter(), null);

                    workflow.setMorphAnalyzer(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.MorphAnalyzer.ChartMorphAnalyzer.ChartMorphAnalyzer(), "conf/plugin/MajorPlugin/MorphAnalyzer/ChartMorphAnalyzer.json");
                    workflow.appendMorphemeProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.MorphemeProcessor.UnknownMorphProcessor.UnknownProcessor(), null);

                    workflow.setPosTagger(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.PosTagger.HmmPosTagger.HMMTagger(), "conf/plugin/MajorPlugin/PosTagger/HmmPosTagger.json");
                    workflow.appendPosProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PosProcessor.NounExtractor.NounExtractor(), null);
                    //workflow = WorkflowFactory.getPredefinedWorkflow(WorkflowFactory.WORKFLOW_NOUN_EXTRACTOR);

                    /* Activate the work flow in the thread mode */
                    workflow.activateWorkflow(true);

                    //string document = System.IO.File.ReadAllText(file);                                                          

                    /* Analysis using the work flow */
                    workflow.analyze(document);

                    //int cnt = 1;
                    LinkedList<Sentence> resultList = workflow.getResultOfDocument(new Sentence(0, 0, false));
                    foreach (Sentence s in resultList)
                    {
                        Eojeol[] eojeolArray = s.Eojeols;
                        for (int i = 0; i < eojeolArray.Length; i++)
                        {
                            if (eojeolArray[i].length > 0)
                            {
                                String[] morphemes = eojeolArray[i].Morphemes;
                                for (int j = 0; j < morphemes.Length; j++)
                                {
                                    //sb.Append(morphemes[j]);
                                    //records.Add(new Record() { Id = cnt, Value = morphemes[j] });
                                    //cnt++;
                                    //sb.Append("|");
                                    sb += morphemes[j] + "|";
                                }
                            }
                        }
                    }

                    //var query = from r in records
                    //            group r by r.Value into g
                    //            select new { Count = g.Count(), Value = g.Key };
                    //query = query.OrderByDescending(a => a.Count);

                    //foreach (var v in query)
                    //{
                    //    sb += v.Value + "|";
                    //}

                    workflow.close();

                }
                string[] array = sb.Split('|');

                //var keyword_list = db.EP_KEYWORD.ToList().Where(e => e.Keyword.Contains(insert)).OrderByDescending(e => e.Count);                        
                DD[] map = new DD[46870];
                for (int i = 0; i < 46870; i++)
                {
                    map[i].value = i;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (array.Length <= i)
                        break;
                    string insert = array[i];
                    if (insert != "")
                    {
                        var keyword_list = (from o in db.EP_KEYWORD
                                            //where o.Keyword.Contains(insert)
                                            where o.Keyword == insert
                                            select o.ArticleID).ToList();
                        foreach (int idx in keyword_list)
                        {
                            map[idx].key += 5 - i;
                        }
                    }
                }

                IComparer myComparer = new myReverserClass();
                Array.Sort(map, myComparer);
                Array.Reverse(map);

                var article_result = new List<EP_KEYWORD>(50);
                for (int i = 0; i < 50; i++)
                {
                    if (map[i].key == 0)
                        break;
                    else if (map[i].key != 0)
                    {
                        int article = map[i].value;
                        article_result.AddRange(from o in db.EP_KEYWORD
                                                where o.ArticleID == article && o.Count == -1
                                                select o);
                        //article_list.AddRange(article_result.ToList());
                    }
                }
                //키워드 추출 후 블로그 포스트 추출된 부분
                //return article_list;
                return article_result;
            }
            catch (Exception) { List<EP_KEYWORD> article_list = new List<EP_KEYWORD>(); return article_list; }
        }

        public List<EP_KEYWORD> NHanNanum(string file)
        {
            try
            {
                string newFileName = "/";
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                Workflow workflow = new Workflow(newFileLocation);

                workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.SentenceSegmentor.SentenceSegmentor(), null);
                workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.InformalSentenceFilter.InformalSentenceFilter(), null);

                workflow.setMorphAnalyzer(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.MorphAnalyzer.ChartMorphAnalyzer.ChartMorphAnalyzer(), "conf/plugin/MajorPlugin/MorphAnalyzer/ChartMorphAnalyzer.json");
                workflow.appendMorphemeProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.MorphemeProcessor.UnknownMorphProcessor.UnknownProcessor(), null);

                workflow.setPosTagger(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.PosTagger.HmmPosTagger.HMMTagger(), "conf/plugin/MajorPlugin/PosTagger/HmmPosTagger.json");
                workflow.appendPosProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PosProcessor.NounExtractor.NounExtractor(), null);
                //workflow = WorkflowFactory.getPredefinedWorkflow(WorkflowFactory.WORKFLOW_NOUN_EXTRACTOR);

                /* Activate the work flow in the thread mode */
                workflow.activateWorkflow(true);

                //string document = System.IO.File.ReadAllText(file);
                string document = file;
                document.Trim();
                
                var article_result = new List<EP_KEYWORD>(50);
                string[] docu_split = document.Split(" ");

                for (int i = 0; i < docu_split.Length; i++)
                {
                    if (docu_split[i].Length > 85)
                        return article_result;
                }


                /* Analysis using the work flow */
                workflow.analyze(document);

                int cnt = 1;
                List<Record> records = new List<Record>();
                LinkedList<Sentence> resultList = workflow.getResultOfDocument(new Sentence(0, 0, false));
                foreach (Sentence s in resultList)
                {
                    Eojeol[] eojeolArray = s.Eojeols;
                    for (int i = 0; i < eojeolArray.Length; i++)
                    {
                        if (eojeolArray[i].length > 0)
                        {
                            String[] morphemes = eojeolArray[i].Morphemes;
                            for (int j = 0; j < morphemes.Length; j++)
                            {
                                //sb.Append(morphemes[j]);
                                records.Add(new Record() { Id = cnt, Value = morphemes[j] });
                                cnt++;
                                //sb.Append(", ");
                            }
                        }
                    }
                }

                var query = from r in records
                            group r by r.Value into g
                            select new { Count = g.Count(), Value = g.Key };
                query = query.OrderByDescending(a => a.Count);

                foreach (var v in query)
                {
                    if (v.Count > 1 && v.Value[0].ToString() != @"\" && v.Value[0].ToString() != @"&" && v.Value.Length != 1)
                        result_sb += v.Value + "|";
                }

                workflow.close();

                string[] array = result_sb.Split('|');
                if (array.Length == 0)
                {
                    return article_result;
                }

                //var keyword_list = db.EP_KEYWORD.ToList().Where(e => e.Keyword.Contains(insert)).OrderByDescending(e => e.Count);                        
                DD[] map = new DD[46870];
                for (int i = 0; i < 46870; i++)
                {
                    map[i].value = i;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (array.Length - 1 <= i)
                        break;
                    string insert = array[i];
                    var keyword_list = (from o in db.EP_KEYWORD
                                        //where o.Keyword.Contains(insert)
                                        where o.Keyword == insert
                                        select o.ArticleID).ToList();
                    foreach (int idx in keyword_list)
                    {
                        map[idx].key += 5 - i;
                    }
                }

                IComparer myComparer = new myReverserClass();
                Array.Sort(map, myComparer);
                Array.Reverse(map);

                for (int i = 0; i < 50; i++)
                {
                    if (map[i].key == 0)
                        break;
                    else if (map[i].key != 0)
                    {
                        int article = map[i].value;
                        article_result.AddRange(from o in db.EP_KEYWORD
                                                where o.ArticleID == article && o.Count == -1
                                                select o);
                        //article_list.AddRange(article_result.ToList());
                    }
                }
                //키워드 추출 후 블로그 포스트 추출된 부분
                //return article_list;
                return article_result;
            }
            catch (Exception) { List<EP_KEYWORD> article_list = new List<EP_KEYWORD>(); return article_list; }
        }

        struct DD
        {
            public int key;
            public int value;
        }

        public class myReverserClass : IComparer
        {
            //Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object _x, Object _y)
            {
                DD x = (DD)_x;
                DD y = (DD)_y;

                return x.key.CompareTo(y.key);
            }
        }


        public void CreateDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Exists == false)
            {
                di.Create();
            }
        }

        public void SaveContents(string path, string name, string data)
        {
            CreateDirectory(path);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + name))
            {
                file.WriteLine(data);
            }
        }

        // POST: /Editor/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GUID,UserID,Ref,Publish1,Publish1_time,Publish2,Publish2_time,Type,Mime_type,Title,Category,Slug,Date,Body,Body2")] EP_POSTS EP_POST, int? return_id)
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


                //Body 부분 인코딩 문제 해결을 위한 부분
                //EP_POST.Body = HttpUtility.UrlEncode(EP_POST.Body);
                //byte[] bodytext = Encoding.GetEncoding("utf-8").GetBytes(EP_POST.Body);
                //EP_POST.Body = Encoding.GetEncoding("utf-8").GetString(bodytext);

                db.EP_POST.Add(EP_POST);
                db.SaveChanges();
                return RedirectToAction("Index", "Archive");
            }

            return View(EP_POST);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkdownCreate([Bind(Include = "ID,GUID,UserID,Ref,Publish1,Publish1_time,Publish2,Publish2_time,Type,Mime_type,Title,Category,Slug,Date,Body,Body2")] EP_POSTS EP_POST)
        {
            if (ModelState.IsValid)
            {
                EP_POST.GUID = Guid.NewGuid().ToString();
                EP_POST.UserID = User.Identity.GetUserId();
                EP_POST.Date = DateTime.Now;
                EP_POST.Type = "Post";
                EP_POST.Mime_type = "mk";

                //Body 부분 인코딩 문제 해결을 위한 부분
                //EP_POST.Body = HttpUtility.UrlEncode(EP_POST.Body);
                //byte[] bodytext = Encoding.GetEncoding("utf-8").GetBytes(EP_POST.Body);
                //EP_POST.Body = Encoding.GetEncoding("utf-8").GetString(bodytext);

                db.EP_POST.Add(EP_POST);
                db.SaveChanges();
                return RedirectToAction("Index", "Archive");
            }

            return View(EP_POST);
        }
        // GET: /Editor/Edit/
        public ActionResult Edit(int? id)
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
        public ActionResult Edit([Bind(Include = "ID,Title,Category,Slug,Date,Body,Body2,UserID")] EP_POSTS EP_POST)
        {
            if (ModelState.IsValid)
            {
                EP_POST.UserID = User.Identity.GetUserId();
                EP_POST.Date = DateTime.Now;
                EP_POST.Type = "Post";
                db.Entry(EP_POST).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Archive");
            }
            return View(EP_POST);
        }

        // GET: /MarkdownEditor/Edit/
        public ActionResult MarkdownEdit(int? id)
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

        // POST: /MarkdownEditor/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkdownEdit([Bind(Include = "ID,Title,Category,Slug,Date,Body,Body2,UserID")] EP_POSTS EP_POST)
        {
            if (ModelState.IsValid)
            {
                EP_POST.UserID = User.Identity.GetUserId();
                EP_POST.Date = DateTime.Now;
                EP_POST.Type = "Post";
                EP_POST.Mime_type = "mk";
                db.Entry(EP_POST).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Archive");
            }
            return View(EP_POST);
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


