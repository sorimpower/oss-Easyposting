using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace EasyPosting.Models
{
    public class EP_POSTS
    {
        public int ID { get; set; }
        public string GUID { get; set; }                // 글의 고유 ID
        public string UserID { get; set; }              // 사용자의 고유 ID
        public string Ref { get; set; }                 // 관련 포스팅(버전관리) ; 0 -> 최초본, GUID -> 수정본
        public string Publish1{get; set;}               // METAWEBLOG로 PUBLISH 했을 경우 서비스명 
        public DateTime Publish1_time { get; set;}      // METAWEBLOG로 PUBLISH 했을 경우 그때의 시간 및 날짜
        public string Publish1_site { get; set; }
        public string Publish2 { get; set; }            // METAWEBLOG로 PUBLISH 했을 경우 서비스명
        public DateTime Publish2_time { get; set; }     // METAWEBLOG로 PUBLISH 했을 경우 그때의 시간 및 날짜
        public string Publish2_site { get; set; }
        public string Type { get; set; }                // 포스팅 타입
        public string Mime_type { get; set; }           // 포스팅 타입이 첨부파일일 경우 MIME TYPE 명기
        public int Isdeleted { get; set; }
        public string Thumbnail { get; set; }
        public string Share { get; set; }
        public string Sharekey { get; set; }
        public DateTime ShareTime { get; set; }

        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "제목")]
        public string Title { get; set; }
        //[Required]
        [DataType(DataType.Text)]
        [Display(Name = "카테고리")]
        public string Category { get; set; }
        
        [DataType(DataType.Text)]
        [Display(Name = "태그")]
        public string Slug { get; set; }
        
        //[Required]
        [Display(Name="작성 일자")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        
        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "내용")]
        public string Body { get; set; }

        public string Body2 { get; set; }


     }
    public class EP_METAS
    {

        public int ID { get; set; }
        public string UserID { get; set; }                  // 사용자의 고유 ID
        //[Required]
        [Display(Name = "블로그 서비스")]
        [DataType(DataType.Text)]
        public string publish { get; set; }                 // METAWEBLOG로 PUBLISH할 서비스명
        //[Required]
        [Display(Name = "블로그 사이트 주소")]
        [DataType(DataType.Text)]
        public string publish_SITE { get; set; }            // METAWEBLOG로 PUBLISH할 사이트
        //[Required]
        [Display(Name = "사용자 ID")]
        [DataType(DataType.Text)]
        public string Publish_ID { get; set; }              // METAWEBLOG로 PUBLISH할 ID
        //[Required]
        [Display(Name = "사용자 비밀번호")]
        [DataType(DataType.Password)]
        public string Publish_PW { get; set; }              // METAWEBLOG로 PUBLISH할 PW
        //[Required]
        [Display(Name = "BLOGID")]
        [DataType(DataType.Text)]
        public string Publish_BLOGID { get; set; }          // METAWEBLOG로 PUBLISH할 BLOGID
        //[Required]
        [Display(Name = "BLOGAPIKEY")]
        [DataType(DataType.Password)]
        public string Publish_BLOGKEY { get; set; }         // METAWEBLOG로 PUBLISH할 BLOGKEY
    }

    public class EP_KEYWORD
    {

        public int ID { get; set; }
        
        public int ArticleID { get; set; }                  // 글 ID

        public int Count { get; set; }                      // 키워드들의 가중치

        [Display(Name = "키워드")]
        [DataType(DataType.Text)]
        public string Keyword { get; set; }                 // 키워드

        [Display(Name = "제목")]
        [DataType(DataType.Text)]
        public string Title { get; set; }                  // 글 제목

        [Display(Name = "링크")]
        [DataType(DataType.Text)]
        public string Link { get; set; }                  // 원본 사이트 링크

        [Display(Name = "내용")]
        [DataType(DataType.Text)]
        public string Description { get; set; }           // 글 내용

    }

    public class DefaultConnection : DbContext
    {
        public DbSet<EP_POSTS> EP_POST { get; set; }
        public DbSet<EP_METAS> EP_META { get; set; }
        public DbSet<EP_KEYWORD> EP_KEYWORD { get; set; }
    }
}
