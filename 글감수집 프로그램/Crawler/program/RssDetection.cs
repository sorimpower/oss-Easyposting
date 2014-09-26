using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;

namespace csharptest
{
    class RssDetection : fileController
    {
        static string blogResultPath = "../../../blog_result/";
        static string xmlFileResultPath = "../../../xml_result/";
        static string itemResultPath = "../../../item_result/";

        static string resultAll = "all_site.txt";
        static string resultTail = "tail_site.txt";
        static string resultNonTail = "naver.txt";
        static string resultNaver = "naver_site.txt";
        static string resultEgloos = "egloos_site.txt";
        static string resultNonEgloos = "non_egloos_site.txt";
        static string resultXml = "result_rss_site.txt";
        static string result = "result.txt";
        string[] blogs;

        // 블로그 주소 파일 로드
        public void load_blogs(string path)
        {
            blogs = System.IO.File.ReadAllLines(path);
        }

        // 1차 판별
        public void save_endrss_blog()
        {
            string tailResult = "";
            string nonTailResult = "";

            load_blogs(blogResultPath + resultAll);

            foreach(var site in blogs)
            {
                if (site == "") continue;
                string tempsite = site + "/rss";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tempsite);
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(site);
                        tailResult += site + '\n';
                    }
                }
                catch (WebException e)
                {
                    nonTailResult += site + '\n';
                }
            }
            SaveContents(blogResultPath, resultTail, tailResult);
            SaveContents(blogResultPath, resultNonTail, nonTailResult);
        }

        // 1차 판별 후 이글루스 판별
        public void save_egloos_blog()
        {
            string egloosResult = "";
            string nonegloosResult = "";

            load_blogs(blogResultPath + resultTail);

            foreach (var site in blogs)
            {
                int idx = site.IndexOf("egloos");
                if (idx == -1) nonegloosResult += site + '\n';
                else
                {
                    egloosResult += "http://rss.egloos.com/blog/" + site.Substring(7, idx - 8) + '\n';
                }
            }
            SaveContents(blogResultPath, resultEgloos, egloosResult);
            SaveContents(blogResultPath, resultNonEgloos, nonegloosResult);
        }

        // 1차 판별 후 url에 rss 추가
        public void plus_tail_rss()
        {
            string tailResult = "";

            load_blogs(blogResultPath + resultNonEgloos);

            foreach (var site in blogs)
            {
                if (site == "") continue;
                tailResult += site;
                if (site[site.Length - 1] != '/') tailResult += '/';
                tailResult += "rss/" + '\n';
            }
            SaveContents(blogResultPath, resultTail, tailResult);
        }

        // 1차 판별 후 네이버 판별
        public void save_naver_blog()
        {
            string naverResult = "";

            load_blogs(blogResultPath + resultNonTail);

            foreach (var site in blogs)
            {
               int idx = site.IndexOf("blog.naver");
               naverResult += site.Substring(0, idx + 5) + "rss." + site.Substring(idx + 5, site.Length - idx - 5) + '\n';        
            }
            SaveContents(blogResultPath, resultNaver, naverResult);
        }

        // 최종 블로그 유효성 검사
        public void save_effective_blog()
        {
            string rssResult = "";

            for (int i = 0; i < 3; i++)
            {

                if (i == 0) load_blogs(blogResultPath + resultEgloos);
                if (i == 1) load_blogs(blogResultPath + resultNaver);
                if (i == 2) load_blogs(blogResultPath + resultTail);

                foreach (var site in blogs)
                {
                    if (site == "") continue;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);
                    
                    try
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            readStream = new StreamReader(receiveStream);
                            string data = readStream.ReadToEnd();
                            if (data == "") continue;
                            if (data.Substring(0, 5) == "<?xml")
                            {
                                rssResult += site + '\n';
                                Console.WriteLine(site);
                            }
                            response.Close();
                            readStream.Close();
                        }
                    }
                    catch (WebException e)
                    {
                    }
                }
            }
            SaveContents(blogResultPath, resultXml, rssResult);
        }

        // RSS블로그 string을 XML파일로 저장
        public void save_as_xml()
        {
            int idx = 0;
            load_blogs(blogResultPath + result);
            CreateDirectory(xmlFileResultPath);

            foreach (var site in blogs)
            {
                if (site == "") continue;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        readStream = new StreamReader(receiveStream);
                        string data = readStream.ReadToEnd();
                        if (data == "") continue;
                        Console.WriteLine(idx);
                        File.WriteAllText(xmlFileResultPath + idx +".xml", data);

                        response.Close();
                        readStream.Close();
                        idx++;
                    }
                }
                catch (WebException e)
                {
                }
            }

            load_blogs(blogResultPath + resultXml);
            CreateDirectory(xmlFileResultPath);

            foreach (var site in blogs)
            {
                if (site == "") continue;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        readStream = new StreamReader(receiveStream);
                        string data = readStream.ReadToEnd();
                        if (data == "") continue;
                        Console.WriteLine(idx);
                        File.WriteAllText(xmlFileResultPath + idx + ".xml", data);

                        response.Close();
                        readStream.Close();
                        idx++;
                    }
                }
                catch (WebException e)
                {
                }
            }
        }

        // xml파일 item단위로 분류
        public void parse_xmlfile()
        {
            int idx = 0;
            CreateDirectory(itemResultPath);

            if (System.IO.Directory.Exists(xmlFileResultPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(xmlFileResultPath);
                foreach (var item in di.GetFiles())
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        string tempPath = xmlFileResultPath + item;
                        xmlDoc.Load(tempPath);

                        XmlNodeList itemList = xmlDoc.GetElementsByTagName("item");

                        foreach (XmlNode node in itemList)
                        {
                            Console.WriteLine(item);
                            CreateXML(node, idx++);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void CreateXML(XmlNode node, int idx)
        {
            try
            {
                string title = node["title"].InnerText;
                string link = node["link"].InnerText;
                string description = node["description"].InnerText;
                string category = node["category"].InnerText;
                string tag = "";
                if (node["tag"] != null) tag = node["tag"].InnerText;

                // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.
                XmlTextWriter textWriter = new XmlTextWriter(itemResultPath + idx + ".xml", Encoding.UTF8);

                // 들여쓰기 설정
                textWriter.Formatting = Formatting.Indented;

                // 문서에 쓰기를 시작합니다.
                textWriter.WriteStartDocument();

                // 루트 설정
                textWriter.WriteStartElement("item");

                // 노드와 값 설정
                textWriter.WriteStartElement("title");
                textWriter.WriteString(title);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("link");
                textWriter.WriteString(link);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("description");
                textWriter.WriteString(description);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("category");
                textWriter.WriteString(category);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("tag");
                textWriter.WriteString(tag);
                textWriter.WriteEndElement();

                textWriter.WriteEndDocument();
                textWriter.Close();
            }
            catch (Exception e) { }
        } 
    }
}