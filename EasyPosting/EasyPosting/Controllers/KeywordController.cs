using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPosting.Models;
using kr.ac.kaist.swrc.jhannanum.comm;
using kr.ac.kaist.swrc.jhannanum.hannanum;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace EasyPosting.Controllers
{
    [Authorize]
    [RequireHttps]
    public class KeywordController : Controller
    {       
        private DefaultConnection db = new DefaultConnection();

        public void NHanNanum(string url, int idx, EP_KEYWORD EP_KEYWORD)
        {
            try
            {
                FileInfo _finfo = new FileInfo(url);
                if (_finfo.Exists == true)
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(url);
                    XmlElement root = xmldoc.DocumentElement;
                    XmlNodeList nodes = root.ChildNodes;
                    string document = "";
                    string link = "";
                    string title = "";
                    string category = "";
                    string tag = "";

                    foreach (XmlNode node in nodes)
                    {
                        switch (node.Name)
                        {
                            case "title":
                                title = node.InnerText;
                                break;
                            case "category":
                                category = node.InnerText;
                                break;
                            case "link":
                                link = node.InnerText;
                                break;
                            case "description":
                                document = node.InnerText;
                                break;
                            case "tag":
                                tag = node.InnerText;
                                break;
                        }
                    }
                    document = Regex.Replace(document, @"[<][a-z|A-Z|/](.|\n|\r)*?[>]", "");
                    document = document.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
                    document = document.Replace("&nbsp;", " ").Replace("&amp;", "").Replace("&quot;", "").Replace("&lt;", "").Replace("&gt;", "");

                    if (document == "")
                    {
                        return;
                    }

                    EP_KEYWORD.ArticleID = idx;
                    EP_KEYWORD.Count = -1;
                    EP_KEYWORD.Link = link;
                    EP_KEYWORD.Description = document;
                    EP_KEYWORD.Title = title;
                    //EP_KEYWORD.Keyword = sp_tag[i];

                    db.EP_KEYWORD.Add(EP_KEYWORD);
                    db.SaveChanges();

                    if (tag != "")
                    {
                        string[] sp_tag = tag.Split(',');
                        if (sp_tag.Length > 1)
                        {
                            for (int i = 0; i < sp_tag.Length; i++)
                            {
                                EP_KEYWORD.ArticleID = idx;
                                EP_KEYWORD.Count = 0;
                                //EP_KEYWORD.Link = link;
                                //EP_KEYWORD.Description = document;
                                //EP_KEYWORD.Title = title;
                                EP_KEYWORD.Keyword = sp_tag[i];

                                db.EP_KEYWORD.Add(EP_KEYWORD);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            EP_KEYWORD.ArticleID = idx;
                            EP_KEYWORD.Count = 0;
                            //EP_KEYWORD.Link = link;
                            //EP_KEYWORD.Description = document;
                            //EP_KEYWORD.Title = title;
                            EP_KEYWORD.Keyword = sp_tag[0];

                            db.EP_KEYWORD.Add(EP_KEYWORD);
                            db.SaveChanges();
                        }
                    }



                    //Workflow workflow = WorkflowFactory.getPredefinedWorkflow(WorkflowFactory.WORKFLOW_NOUN_EXTRACTOR);
                    string newFileName = "/";
                    string newFileLocation = HttpContext.Server.MapPath(newFileName);
                    Workflow workflow = new Workflow(newFileLocation);

                    workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.SentenceSegmentor.SentenceSegmentor(), null);
                    workflow.appendPlainTextProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PlainTextProcessor.InformalSentenceFilter.InformalSentenceFilter(), null);

                    workflow.setMorphAnalyzer(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.MorphAnalyzer.ChartMorphAnalyzer.ChartMorphAnalyzer(), "conf/plugin/MajorPlugin/MorphAnalyzer/ChartMorphAnalyzer.json");
                    workflow.appendMorphemeProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.MorphemeProcessor.UnknownMorphProcessor.UnknownProcessor(), null);

                    workflow.setPosTagger(new kr.ac.kaist.swrc.jhannanum.plugin.MajorPlugin.PosTagger.HmmPosTagger.HMMTagger(), "conf/plugin/MajorPlugin/PosTagger/HmmPosTagger.json");
                    workflow.appendPosProcessor(new kr.ac.kaist.swrc.jhannanum.plugin.SupplementPlugin.PosProcessor.NounExtractor.NounExtractor(), null);

                    workflow.activateWorkflow(true);

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
                                    records.Add(new Record() { Id = cnt, Value = morphemes[j] });
                                    cnt++;
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
                        {
                            EP_KEYWORD.ArticleID = idx;
                            EP_KEYWORD.Count = v.Count;
                            //EP_KEYWORD.Link = link;
                            //EP_KEYWORD.Description = document;
                            //EP_KEYWORD.Title = title;
                            EP_KEYWORD.Keyword = v.Value;

                            db.EP_KEYWORD.Add(EP_KEYWORD);
                            db.SaveChanges();
                        }
                    }                                      
                    workflow.close();
                }
                else if (_finfo.Exists == false){ }
            }
            catch (Exception) { }
        }



        [Authorize(Roles = "Administrator")]
        // GET: Keyword

        public ActionResult Index(int? val, [Bind(Include="ID, ArticleID, Count, Title, Link, Description, Keyword")] EP_KEYWORD EP_KEYWORD)
        {
            if (val == null)
            {
                val = 2;
            }
            if (val == 1)
            {
                for (int i = 0; i < 34524; i++)
                {
                    string url = "C:\\Users\\SangYun\\Desktop\\HanRss ing\\item_result\\" + i + ".xml";
                    NHanNanum(url, i, EP_KEYWORD);
                }                
            }

            return View();
        }
    }
}