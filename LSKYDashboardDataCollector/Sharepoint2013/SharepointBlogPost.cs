using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector
{
    public class SharepointBlogPost
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Categories { get; set; }
        public DateTime PublishDate { get; set; }

        public string PublishDateString
        {
            set
            {
                this.PublishDate = DateTime.Parse(value);
            }
        }
    }
}