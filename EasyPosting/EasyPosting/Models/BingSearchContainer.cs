//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Notice: Use of the service proxies that accompany this notice is subject to
//            the terms and conditions of the license agreement located at
//            http://go.microsoft.com/fwlink/?LinkID=202740
//            If you do not agree to these terms you may not use this content.
namespace EasyPosting.Models{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Net;
    using System.IO;
    
    
    public partial class ExpandableSearchResult {
        
        private Guid _ID;
        
        private Int64? _WebTotal;
        
        private Int64? _WebOffset;
        
        private Int64? _ImageTotal;
        
        private Int64? _ImageOffset;
        
        private Int64? _VideoTotal;
        
        private Int64? _VideoOffset;
        
        private Int64? _NewsTotal;
        
        private Int64? _NewsOffset;
        
        private Int64? _SpellingSuggestionsTotal;
        
        private String _AlteredQuery;
        
        private String _AlterationOverrideQuery;
        
        private System.Collections.ObjectModel.Collection<WebResult> _Web;
        
        private System.Collections.ObjectModel.Collection<ImageResult> _Image;
        
        private System.Collections.ObjectModel.Collection<VideoResult> _Video;
        
        private System.Collections.ObjectModel.Collection<NewsResult> _News;
        
        private System.Collections.ObjectModel.Collection<RelatedSearchResult> _RelatedSearch;
        
        private System.Collections.ObjectModel.Collection<SpellResult> _SpellingSuggestions;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public Int64? WebTotal {
            get {
                return this._WebTotal;
            }
            set {
                this._WebTotal = value;
            }
        }
        
        public Int64? WebOffset {
            get {
                return this._WebOffset;
            }
            set {
                this._WebOffset = value;
            }
        }
        
        public Int64? ImageTotal {
            get {
                return this._ImageTotal;
            }
            set {
                this._ImageTotal = value;
            }
        }
        
        public Int64? ImageOffset {
            get {
                return this._ImageOffset;
            }
            set {
                this._ImageOffset = value;
            }
        }
        
        public Int64? VideoTotal {
            get {
                return this._VideoTotal;
            }
            set {
                this._VideoTotal = value;
            }
        }
        
        public Int64? VideoOffset {
            get {
                return this._VideoOffset;
            }
            set {
                this._VideoOffset = value;
            }
        }
        
        public Int64? NewsTotal {
            get {
                return this._NewsTotal;
            }
            set {
                this._NewsTotal = value;
            }
        }
        
        public Int64? NewsOffset {
            get {
                return this._NewsOffset;
            }
            set {
                this._NewsOffset = value;
            }
        }
        
        public Int64? SpellingSuggestionsTotal {
            get {
                return this._SpellingSuggestionsTotal;
            }
            set {
                this._SpellingSuggestionsTotal = value;
            }
        }
        
        public String AlteredQuery {
            get {
                return this._AlteredQuery;
            }
            set {
                this._AlteredQuery = value;
            }
        }
        
        public String AlterationOverrideQuery {
            get {
                return this._AlterationOverrideQuery;
            }
            set {
                this._AlterationOverrideQuery = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<WebResult> Web {
            get {
                return this._Web;
            }
            set {
                this._Web = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<ImageResult> Image {
            get {
                return this._Image;
            }
            set {
                this._Image = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<VideoResult> Video {
            get {
                return this._Video;
            }
            set {
                this._Video = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<NewsResult> News {
            get {
                return this._News;
            }
            set {
                this._News = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<RelatedSearchResult> RelatedSearch {
            get {
                return this._RelatedSearch;
            }
            set {
                this._RelatedSearch = value;
            }
        }
        
        public System.Collections.ObjectModel.Collection<SpellResult> SpellingSuggestions {
            get {
                return this._SpellingSuggestions;
            }
            set {
                this._SpellingSuggestions = value;
            }
        }
    }
    
    public partial class WebResult {
        
        private Guid _ID;
        
        private String _Title;
        
        private String _Description;
        
        private String _DisplayUrl;
        
        private String _Url;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public String Title {
            get {
                return this._Title;
            }
            set {
                this._Title = value;
            }
        }
        
        public String Description {
            get {
                return this._Description;
            }
            set {
                this._Description = value;
            }
        }
        
        public String DisplayUrl {
            get {
                return this._DisplayUrl;
            }
            set {
                this._DisplayUrl = value;
            }
        }
        
        public String Url {
            get {
                return this._Url;
            }
            set {
                this._Url = value;
            }
        }
    }
    
    public partial class ImageResult {
        
        private Guid _ID;
        
        private String _Title;
        
        private String _MediaUrl;
        
        private String _SourceUrl;
        
        private String _DisplayUrl;
        
        private Int32? _Width;
        
        private Int32? _Height;
        
        private Int64? _FileSize;
        
        private String _ContentType;
        
        private Thumbnail _Thumbnail;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public String Title {
            get {
                return this._Title;
            }
            set {
                this._Title = value;
            }
        }
        
        public String MediaUrl {
            get {
                return this._MediaUrl;
            }
            set {
                this._MediaUrl = value;
            }
        }
        
        public String SourceUrl {
            get {
                return this._SourceUrl;
            }
            set {
                this._SourceUrl = value;
            }
        }
        
        public String DisplayUrl {
            get {
                return this._DisplayUrl;
            }
            set {
                this._DisplayUrl = value;
            }
        }
        
        public Int32? Width {
            get {
                return this._Width;
            }
            set {
                this._Width = value;
            }
        }
        
        public Int32? Height {
            get {
                return this._Height;
            }
            set {
                this._Height = value;
            }
        }
        
        public Int64? FileSize {
            get {
                return this._FileSize;
            }
            set {
                this._FileSize = value;
            }
        }
        
        public String ContentType {
            get {
                return this._ContentType;
            }
            set {
                this._ContentType = value;
            }
        }
        
        public Thumbnail Thumbnail {
            get {
                return this._Thumbnail;
            }
            set {
                this._Thumbnail = value;
            }
        }
    }

    public partial class VideoResult
    {

        private Guid _ID;

        private String _Title;

        private String _MediaUrl;

        private String _DisplayUrl;

        private Int32? _RunTime;

        //private Thumbnail _Thumbnail;

        private String _Thumbnail;

        public Guid ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public String Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        public String MediaUrl
        {
            get
            {
                return this._MediaUrl;
            }
            set
            {
                this._MediaUrl = value;
            }
        }

        public String DisplayUrl
        {
            get
            {
                return this._DisplayUrl;
            }
            set
            {
                this._DisplayUrl = value;
            }
        }

        public Int32? RunTime
        {
            get
            {
                return this._RunTime;
            }
            set
            {
                this._RunTime = value;
            }
        }

        public String Thumbnail
        {
            get
            {
                return this._Thumbnail;
            }
            set
            {
                this._Thumbnail = value;
            }
        }

        //public Thumbnail Thumbnail
        //{
        //    get
        //    {
        //        return this._Thumbnail;
        //    }
        //    set
        //    {
        //        this._Thumbnail = value;
        //    }
        //}
    }
    
    public partial class NewsResult {
        
        private Guid _ID;
        
        private String _Title;
        
        private String _Url;
        
        private String _Source;
        
        private String _Description;
        
        private DateTime? _Date;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public String Title {
            get {
                return this._Title;
            }
            set {
                this._Title = value;
            }
        }
        
        public String Url {
            get {
                return this._Url;
            }
            set {
                this._Url = value;
            }
        }
        
        public String Source {
            get {
                return this._Source;
            }
            set {
                this._Source = value;
            }
        }
        
        public String Description {
            get {
                return this._Description;
            }
            set {
                this._Description = value;
            }
        }
        
        public DateTime? Date {
            get {
                return this._Date;
            }
            set {
                this._Date = value;
            }
        }
    }
    
    public partial class RelatedSearchResult {
        
        private Guid _ID;
        
        private String _Title;
        
        private String _BingUrl;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public String Title {
            get {
                return this._Title;
            }
            set {
                this._Title = value;
            }
        }
        
        public String BingUrl {
            get {
                return this._BingUrl;
            }
            set {
                this._BingUrl = value;
            }
        }
    }
    
    public partial class SpellResult {
        
        private Guid _ID;
        
        private String _Value;
        
        public Guid ID {
            get {
                return this._ID;
            }
            set {
                this._ID = value;
            }
        }
        
        public String Value {
            get {
                return this._Value;
            }
            set {
                this._Value = value;
            }
        }
    }
    
    public partial class Thumbnail {
        
        private String _MediaUrl;
        
        private String _ContentType;
        
        private Int32? _Width;
        
        private Int32? _Height;
        
        private Int64? _FileSize;
        
        public String MediaUrl {
            get {
                return this._MediaUrl;
            }
            set {
                this._MediaUrl = value;
            }
        }
        
        public String ContentType {
            get {
                return this._ContentType;
            }
            set {
                this._ContentType = value;
            }
        }
        
        public Int32? Width {
            get {
                return this._Width;
            }
            set {
                this._Width = value;
            }
        }
        
        public Int32? Height {
            get {
                return this._Height;
            }
            set {
                this._Height = value;
            }
        }
        
        public Int64? FileSize {
            get {
                return this._FileSize;
            }
            set {
                this._FileSize = value;
            }
        }
    }
    
    public partial class BingSearchContainer : System.Data.Services.Client.DataServiceContext {
        
        public BingSearchContainer(Uri serviceRoot) : 
                base(serviceRoot) {
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Sources">Bing search sources Sample Values : web+image+video+news+spell</param>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="WebSearchOptions">Specify options for a request to the Web SourceType. Valid values are: DisableHostCollapsing, DisableQueryAlterations. Sample Values : DisableQueryAlterations</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="WebFileType">File extensions to return Sample Values : XLS</param>
        /// <param name="ImageFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Size:Small, Size:Medium, Size:Large, Size:Width:[Width], Size:Height:[Height], Aspect:Square, Aspect:Wide, Aspect:Tall, Color:Color, Color:Monochrome, Style:Photo, Style:Graphics, Face:Face, Face:Portrait, Face:Other. Sample Values : Size:Small+Aspect:Square</param>
        /// <param name="VideoFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Duration:Short, Duration:Medium, Duration:Long, Aspect:Standard, Aspect:Widescreen, Resolution:Low, Resolution:Medium, Resolution:High. Sample Values : Duration:Short+Resolution:High</param>
        /// <param name="VideoSortBy">The sort order of results returned Sample Values : Date</param>
        /// <param name="NewsLocationOverride">Overrides Bing location detection. This parameter is only applicable in en-US market. Sample Values : US.WA</param>
        /// <param name="NewsCategory">The category of news for which to provide results Sample Values : rt_Business</param>
        /// <param name="NewsSortBy">The sort order of results returned Sample Values : Date</param>
        public DataServiceQuery<ExpandableSearchResult> Composite(String Sources, String Query, String Options, String WebSearchOptions, String Market, String Adult, Double? Latitude, Double? Longitude, String WebFileType, String ImageFilters, String VideoFilters, String VideoSortBy, String NewsLocationOverride, String NewsCategory, String NewsSortBy) {
            if ((Sources == null)) {
                throw new System.ArgumentNullException("Sources", "Sources value cannot be null");
            }
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<ExpandableSearchResult> query;
            query = base.CreateQuery<ExpandableSearchResult>("Composite");
            if ((Sources != null)) {
                query = query.AddQueryOption("Sources", string.Concat("\'", System.Uri.EscapeDataString(Sources), "\'"));
            }
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((WebSearchOptions != null)) {
                query = query.AddQueryOption("WebSearchOptions", string.Concat("\'", System.Uri.EscapeDataString(WebSearchOptions), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((WebFileType != null)) {
                query = query.AddQueryOption("WebFileType", string.Concat("\'", System.Uri.EscapeDataString(WebFileType), "\'"));
            }
            if ((ImageFilters != null)) {
                query = query.AddQueryOption("ImageFilters", string.Concat("\'", System.Uri.EscapeDataString(ImageFilters), "\'"));
            }
            if ((VideoFilters != null)) {
                query = query.AddQueryOption("VideoFilters", string.Concat("\'", System.Uri.EscapeDataString(VideoFilters), "\'"));
            }
            if ((VideoSortBy != null)) {
                query = query.AddQueryOption("VideoSortBy", string.Concat("\'", System.Uri.EscapeDataString(VideoSortBy), "\'"));
            }
            if ((NewsLocationOverride != null)) {
                query = query.AddQueryOption("NewsLocationOverride", string.Concat("\'", System.Uri.EscapeDataString(NewsLocationOverride), "\'"));
            }
            if ((NewsCategory != null)) {
                query = query.AddQueryOption("NewsCategory", string.Concat("\'", System.Uri.EscapeDataString(NewsCategory), "\'"));
            }
            if ((NewsSortBy != null)) {
                query = query.AddQueryOption("NewsSortBy", string.Concat("\'", System.Uri.EscapeDataString(NewsSortBy), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="WebSearchOptions">Specify options for a request to the Web SourceType. Valid values are: DisableHostCollapsing, DisableQueryAlterations. Sample Values : DisableQueryAlterations</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="WebFileType">File extensions to return Sample Values : XLS</param>
        public DataServiceQuery<WebResult> Web(String Query, String Options, String WebSearchOptions, String Market, String Adult, Double? Latitude, Double? Longitude, String WebFileType) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<WebResult> query;
            query = base.CreateQuery<WebResult>("Web");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((WebSearchOptions != null)) {
                query = query.AddQueryOption("WebSearchOptions", string.Concat("\'", System.Uri.EscapeDataString(WebSearchOptions), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((WebFileType != null)) {
                query = query.AddQueryOption("WebFileType", string.Concat("\'", System.Uri.EscapeDataString(WebFileType), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="ImageFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Size:Small, Size:Medium, Size:Large, Size:Width:[Width], Size:Height:[Height], Aspect:Square, Aspect:Wide, Aspect:Tall, Color:Color, Color:Monochrome, Style:Photo, Style:Graphics, Face:Face, Face:Portrait, Face:Other. Sample Values : Size:Small+Aspect:Square</param>
        public DataServiceQuery<ImageResult> Image(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, String ImageFilters) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<ImageResult> query;
            query = base.CreateQuery<ImageResult>("Image");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((ImageFilters != null)) {
                query = query.AddQueryOption("ImageFilters", string.Concat("\'", System.Uri.EscapeDataString(ImageFilters), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="VideoFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof. Valid values are: Duration:Short, Duration:Medium, Duration:Long, Aspect:Standard, Aspect:Widescreen, Resolution:Low, Resolution:Medium, Resolution:High. Sample Values : Duration:Short+Resolution:High</param>
        /// <param name="VideoSortBy">The sort order of results returned Sample Values : Date</param>
        public DataServiceQuery<VideoResult> Video(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, String VideoFilters, String VideoSortBy) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<VideoResult> query;
            query = base.CreateQuery<VideoResult>("Video");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((VideoFilters != null)) {
                query = query.AddQueryOption("VideoFilters", string.Concat("\'", System.Uri.EscapeDataString(VideoFilters), "\'"));
            }
            if ((VideoSortBy != null)) {
                query = query.AddQueryOption("VideoSortBy", string.Concat("\'", System.Uri.EscapeDataString(VideoSortBy), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        /// <param name="NewsLocationOverride">Overrides Bing location detection. This parameter is only applicable in en-US market. Sample Values : US.WA</param>
        /// <param name="NewsCategory">The category of news for which to provide results Sample Values : rt_Business</param>
        /// <param name="NewsSortBy">The sort order of results returned Sample Values : Date</param>
        public DataServiceQuery<NewsResult> News(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude, String NewsLocationOverride, String NewsCategory, String NewsSortBy) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<NewsResult> query;
            query = base.CreateQuery<NewsResult>("News");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            if ((NewsLocationOverride != null)) {
                query = query.AddQueryOption("NewsLocationOverride", string.Concat("\'", System.Uri.EscapeDataString(NewsLocationOverride), "\'"));
            }
            if ((NewsCategory != null)) {
                query = query.AddQueryOption("NewsCategory", string.Concat("\'", System.Uri.EscapeDataString(NewsCategory), "\'"));
            }
            if ((NewsSortBy != null)) {
                query = query.AddQueryOption("NewsSortBy", string.Concat("\'", System.Uri.EscapeDataString(NewsSortBy), "\'"));
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xbox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        public DataServiceQuery<RelatedSearchResult> RelatedSearch(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<RelatedSearchResult> query;
            query = base.CreateQuery<RelatedSearchResult>("RelatedSearch");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            return query;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Query">Bing search query Sample Values : xblox</param>
        /// <param name="Options">Specifies options for this request for all Sources. Valid values are: DisableLocationDetection, EnableHighlighting. Sample Values : EnableHighlighting</param>
        /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
        /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
        /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
        /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
        public DataServiceQuery<SpellResult> SpellingSuggestions(String Query, String Options, String Market, String Adult, Double? Latitude, Double? Longitude) {
            if ((Query == null)) {
                throw new System.ArgumentNullException("Query", "Query value cannot be null");
            }
            DataServiceQuery<SpellResult> query;
            query = base.CreateQuery<SpellResult>("SpellingSuggestions");
            if ((Query != null)) {
                query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
            }
            if ((Options != null)) {
                query = query.AddQueryOption("Options", string.Concat("\'", System.Uri.EscapeDataString(Options), "\'"));
            }
            if ((Market != null)) {
                query = query.AddQueryOption("Market", string.Concat("\'", System.Uri.EscapeDataString(Market), "\'"));
            }
            if ((Adult != null)) {
                query = query.AddQueryOption("Adult", string.Concat("\'", System.Uri.EscapeDataString(Adult), "\'"));
            }
            if (((Latitude != null) 
                        && (Latitude.HasValue == true))) {
                query = query.AddQueryOption("Latitude", Latitude.Value);
            }
            if (((Longitude != null) 
                        && (Longitude.HasValue == true))) {
                query = query.AddQueryOption("Longitude", Longitude.Value);
            }
            return query;
        }
    }
}
