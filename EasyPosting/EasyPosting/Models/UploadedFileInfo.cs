using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPosting.Models
{
    public interface IPDFConverter
    {
        byte[] ConvertToPDF(string source, string commandLocation);
    }

    public class ViewDataUploadFileResult
    {
        private string _error;

        //for JQuery file upload
        /// <summary>
        /// Gets or sets the error. If neither null nor empty indicates something wrong happened
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string error
        {
            get { return _error; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _error = value;
                    deleteUrl = String.Empty;
                    thumbnailUrl = String.Empty;
                    url = String.Empty;
                }

            }
        }

        public string name { get; set; }
        /// <summary>
        /// Gets or sets the size of the file in bytes
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }


        /// <summary>
        /// Gets or sets the full path. for use from any controller and/or views. 
        /// Should be removed in the returned json to client
        /// </summary>
        /// <value>
        /// The full path.
        /// </value>
        public string FullPath { get; set; }
        public string SavedFileName { get; set; }
        //public ErrorType? ErrorType { get; set; }

        //for storing
        public string Title { get; set; }
    }
    public class UploadedFileInfo
    {
        public UploadedFileInfo(string fileName, string webPath, string slides)
        {
            this.fileName = fileName;
            this.webPath = webPath;
            this.slides = slides;
        }

        private string fileName = null;
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        private string webPath = null;
        public string WebPath
        {
            get
            {
                return this.webPath;
            }
            set
            {
                this.webPath = value;
            }
        }
        private string slides = null;
        public string Slides
        {
            get
            {
                return this.slides;
            }
            set
            {
                this.slides = value;
            }
        }
    }
}