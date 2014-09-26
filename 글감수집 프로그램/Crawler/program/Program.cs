using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace csharptest
{
    class Program
    {
        static void Main(string[] args)
        {
            HanrssParser HanRss = new HanrssParser();
            RssDetection Detector = new RssDetection();
            //HanRss.search_category();                                 // HanRSS블로그 카테고리 별 수집
            //HanRss.add_all_textfiles("naver.txt");                     // 블로그 전체 묶음
            //Detector.save_endrss_blog();                              // 1차분류 (url 끝에 rss가 붙는 블로그)
            //Detector.save_egloos_blog();                              // 분류된 블로그에서 이글루스 따로 분류
            //Detector.plus_tail_rss();                                 // 이글루스를 제외한 나머지 rss붙임
            //Detector.save_naver_blog();                               // 미분류된 블로그에서 naver만 따로 분류
            //Detector.save_effective_blog();                           // 통합하여 유효성 판단 후 저장
            Detector.save_as_xml();                                     // 블로그 rss 주소 string을 xml파일로 저장
           // Detector.parse_xmlfile();                                   // xml 파일 item 단위로 분류

            //HanRss.search_blog();
        }
    }
}

