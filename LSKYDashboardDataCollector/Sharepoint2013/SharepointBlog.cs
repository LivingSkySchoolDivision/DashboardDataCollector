using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class SharepointBlog
    {
        public string Title { get; set; }
        public string ID { get; set; }

        private string _updatedString;
        
        public List<SharepointBlogPost> BlogPosts { get; set; }

        public string RawData { get; set; }

        public SharepointBlog(string username, string password, string url)
        {
            StreamReader readStream = new StreamReader(SharepointResponseRetriever.GetSharepointResponse(username, password, url), Encoding.UTF8);

            this.BlogPosts = new List<SharepointBlogPost>();
            
            XDocument xmlDocument = XDocument.Load(readStream);

            RawData = xmlDocument.ToString();

            foreach (XElement rootElement in xmlDocument.Elements())
            {
                foreach (XElement element in rootElement.Elements())
                {
                    if (element.Name.LocalName.ToLower() == "title")
                    {
                        this.Title = element.Value;
                    }

                    if (element.Name.LocalName.ToLower() == "id")
                    {
                        this.ID = element.Value;
                    } 
                    
                    if (element.Name.LocalName.ToLower() == "updated")
                    {
                        this._updatedString = element.Value;
                    }

                    if (element.Name.LocalName.ToLower() == "entry")
                    {
                        // Parse out this entry somehow
                        // go through the sub elements of this element

                        string potentialEntryTitle = string.Empty;
                        string potentialEntryContent = string.Empty;
                        string potentialEntryUpdateDateString = string.Empty;

                        foreach (XElement entryElement in element.Elements())
                        {
                            if (entryElement.Name.LocalName.ToLower() == "title")
                            {
                                potentialEntryTitle = CommonFunctions.SanitizeForJSON(entryElement.Value);
                            }

                            if (entryElement.Name.LocalName.ToLower() == "updated")
                            {
                                potentialEntryUpdateDateString = entryElement.Value;
                            }

                            if (entryElement.Name.LocalName.ToLower() == "content")
                            {
                                foreach (XElement contentElement in entryElement.Elements())
                                {
                                    if (contentElement.Name.LocalName.ToLower() == "properties")
                                    {
                                        foreach (XElement propertiesElement in contentElement.Elements())
                                        {
                                            if (propertiesElement.Name.LocalName.ToLower() == "body")
                                            {
                                                potentialEntryContent = CommonFunctions.SanitizeForJSON(propertiesElement.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (
                            !string.IsNullOrEmpty(potentialEntryTitle)
                            )
                        {
                            this.BlogPosts.Add(new SharepointBlogPost()
                            {
                                Title = potentialEntryTitle,
                                Content = potentialEntryContent,
                                PublishDateString = potentialEntryUpdateDateString
                            }); 
                        }
                    }
                }
            }

            this.BlogPosts = this.BlogPosts.OrderByDescending(p => p.PublishDate).ToList();
        }
    }
    
}