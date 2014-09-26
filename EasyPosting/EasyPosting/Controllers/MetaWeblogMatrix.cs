using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EasyPosting.Models;
using CookComputing.XmlRpc;


namespace EasyPosting.Controllers
{

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Enclosure
    {
        public int length;
        public string type;
        public string url;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Source
    {
        public string name;
        public string url;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct UserBlog
    {
        public string url;
        public string blogid;
        public string blogName;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct UserInfo
    {
        public string url;
        public string blogid;
        public string blogName;
        public string firstname;
        public string lastname;
        public string email;
        public string nickname;
    }


    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Post
    {
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public DateTime dateCreated;
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public string description;
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember(Description = "Required when posting.")]
        public string title;

        public string[] categories;
        public Enclosure enclosure;
        public string link;
        public string permalink;
        [XmlRpcMember(
           Description = "Not required when posting. Depending on server may "
           + "be either string or integer. "
           + "Use Convert.ToInt32(postid) to treat as integer or "
           + "Convert.ToString(postid) to treat as string")]
        public object postid;
        public Source source;
        public string userid;

        public object mt_allow_comments;
        public object mt_allow_pings;
        public object mt_convert_breaks;
        public string mt_text_more;
        public string mt_excerpt;
        public string mt_keywords;
        public string tags;
    }
    
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Category
    {
        public string description;
        public string title;
    }

    public struct CategoryInfo
    {
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
        public string categoryid;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MediaObjectUrl
    {
        public string url;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MediaObject
    {
        public string name;
        public string type;
        public byte[] bits;
    }

    public class MetaWeblog : XmlRpcClientProtocol
    {
        public MetaWeblog(String uri)
        {
            base.Url = uri;
        }
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public Post[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            return (Post[])this.Invoke("getRecentPosts", new object[] { blogid, username, password, numberOfPosts });
        }
        [XmlRpcMethod("metaWeblog.newPost")]
        public string newPost(string blogid, string username, string password, Post content, bool publish)
        {
            return (string)this.Invoke("newPost", new object[] { blogid, username, password, content, publish });
        }
        [XmlRpcMethod("metaWeblog.editPost")]
        public bool editPost(string postid, string username, string password, Post content, bool publish)
        {
            return (bool)this.Invoke("editPost", new object[] { postid, username, password, content, publish });
        }
        [XmlRpcMethod("blogger.deletePost")]
        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            return (bool)this.Invoke("deletePost", new object[] { appKey, postid, username, password, publish });
        }
        [XmlRpcMethod("blogger.getUsersBlogs")]
        public UserBlog[] getUsersBlogs(string appKey, string username, string password)
        {
            return (UserBlog[])this.Invoke("getUsersBlogs", new object[] { appKey, username, password });
        }
        [XmlRpcMethod("blogger.getUserInfo")]
        public UserInfo getUserInfo(string appKey, string username, string password)
        {
            return (UserInfo)this.Invoke("getUserInfo", new object[] { appKey, username, password });
        }
        [XmlRpcMethod("metaWeblog.getPost")]
        public Post getPost(string postid, string username, string password)
        {
            return (Post)this.Invoke("getPost", new object[] { postid, username, password });
        }
        [XmlRpcMethod("metaWeblog.getCategories")]
        public Category[] getCategories(string blogid, string username, string password)
        {
            return (Category[])this.Invoke("getCategories", new object[] { blogid, username, password });
        }
        [XmlRpcMethod("metaWeblog.newMediaObject", Description = "Add a media object to a post using the " + "metaWeblog API. Returns media url as a string.")]
        public MediaObjectUrl newMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            return (MediaObjectUrl)this.Invoke("newMediaObject", new object[] { blogid, username, password, mediaObject });
        }
    }
}