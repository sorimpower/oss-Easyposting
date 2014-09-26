using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace csharptest
{
    class HanrssParser : fileController
    {
        string categoryPath = "../../../category_blog/";

        string hanrssUrl = "http://www.hanrss.com/directory/index.qst?tag=";
        string tistoryUrl = "http://www.tistory.com/thankyou";
        string naverUrl = "http://section.blog.naver.com/sub/PowerBlogList.nhn?searchYear=2013&searchText=&isSearch=false&parentGroupNo=0&childGroupNo=0&currentPage=";

        // 한 rss 카테고리 별 블로그 주소 수집
        public void search_category()
        {
            int loof = 0;
            string urlAddress = hanrssUrl;
            string[] category = { "컴퓨터", "일상", "경제", "뉴스", "정치", "여행", "사진", "만화", "경영", "영화", "음악", 
                                    "게임", "쇼핑", "음식", "스포츠", "문화", "도서", "건강", "연예", "디자인", "어학", "사회",
                                    "교육", "육아", "자동차", "과학", "미술", "기업소식", "팟캐스트", "유머", "취업", "동영상", "문학"};

            for (int i = 0; i < category.Length; i++)
            {
                int j = 0;

                Console.WriteLine(category[i]);
                do
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress + category[i] + "&pno=" + j);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        if (response.CharacterSet == null)
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        string data = readStream.ReadToEnd();

                        loof = search_blogsite(data, category[i] + j);

                        response.Close();
                        readStream.Close();
                    }
                    j++;
                } while (loof == 1);
            }
        }

        // 카테고리 내부 블로그 주소 탐색 및 저장
        public int search_blogsite(string data, string fileName)
        {
            string result = null;
            int idx = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 't' && data[i + 1] == 'o' && data[i + 2] == 'p')
                {
                    if (data[i + 4] == '3' && data[i + 5] == 'p' && data[i + 6] == 'x')
                    {
                        if (data[i + 15] == 'h')
                        {
                            i += 21;
                            while (data[i] != '>')
                            {
                                i++;
                                idx++;
                            }
                            string str = data.Substring(i - idx, idx - 1);
                            idx = 0;
                            result += str;
                            result += '\n';
                        }
                    }
                }
            }
            if (result != null)
            {
                SaveContents(categoryPath, fileName + ".txt", result);
                return 1;
            }
            else return 0;
        }

        public void search_blog()
        {
            int loof = 0;
            string urlAddress = naverUrl;
            for (int i = 0; i < 25; i++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress + i);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    string data = readStream.ReadToEnd();

                    //loof = search_blog_tistory(data, "tistory");
                    loof = search_blog_url(data, "naver"+i);

                    response.Close();
                    readStream.Close();
                }
            }
        }
        // 카테고리 내부 블로그 주소 탐색 및 저장
        public int search_blog_url(string data, string fileName)
        {
            string result = null;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 'i' && data[i + 1] == 'm' && data[i + 2] == 'g')
                {
                    if (data[i + 3] == 'b' && data[i + 4] == 'o' && data[i + 5] == 'x')
                    {
                        if (data[i + 11] == 'h')
                        {
                            i += 11;
                            i += 6;
                            string str = "";
                            while (data[i] != '"')
                            {
                                str += data[i];
                                i++;
                            }
                            result += str;
                            result += '\n';
                        }
                    }
                }
            }
            if (result != null)
            {
                SaveContents(categoryPath, fileName + ".txt", result);
                return 1;
            }
            else return 0;
        }

        // 카테고리 별로 나뉜 텍스트 파일 합치기
        public void add_all_textfiles(string fileName)
        {
            string text = "";
            if (System.IO.Directory.Exists(categoryPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(categoryPath);
                foreach (var item in di.GetFiles())
                {
                    text += System.IO.File.ReadAllText(categoryPath + item.Name);
                }
            }

            SaveContents(categoryPath, fileName, text);
        }



    }
}
