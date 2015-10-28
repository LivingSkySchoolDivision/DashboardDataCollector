using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using LSKYDashboardDataCollector.Proxy;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class Sharepoint2013BlogParser
    {
        public List<SharepointBlogPost> ParseRSSFeed(string username, string password, string blogXMLURL)
        {
            // Example url: https://portal.lskysd.ca/department/ss/_vti_bin/ListData.svc/NewsAnnouncements

            List<SharepointBlogPost> returnablePosts = new List<SharepointBlogPost>();
            
            // Make a web request to the specified url
            StreamReader readStream = new StreamReader(SharepointResponseRetriever.GetSharepointResponse(username, password, blogXMLURL), Encoding.UTF8);

            XmlReader xmlReader = XmlReader.Create(readStream);

            // Parse RSS items into objects
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            foreach (SyndicationItem item in feed.Items)
            {
                
                // Figure out author
                string author = string.Empty;
                foreach (SyndicationPerson person in item.Authors)
                {
                    author = person.Email; // Sharepoint puts the person's name in the email field
                }

                
                // Don't just go by line, because things can be multi-line it looks like
                // Better to use regex on the entire blob

                // Why the fuck is this a null reference exception
                // string contentBlob = item.Summary.Text;
                //string contentBlob = item.Content.AttributeExtensions[new XmlQualifiedName("Body")].ToString()



                string contentBlob = string.Empty;
                foreach (XmlQualifiedName key in item.Content.AttributeExtensions.Keys)
                {
                    contentBlob += key.Name + ";";
                }


                /*
                //Regex pattern = new Regex(@"ImageDimension=(?<imageWidth>\d+)x(?<imageHeight>\d+);ThumbnailDimension=(?<thumbWidth>\d+)x(?<thumbHeight>\d+)");


                // Body
                // <d:Body\>(.+?)\<\/d:Body\>/s

                // Date
                // <d:Modified m:type="Edm.DateTime">


                Regex pattern = new Regex(@"<d:Body\>(?<blogContent>.+?)\<\/d:Body\>/s");
                Match match = pattern.Match(contentBlob);
                string rawBlogContent = match.Groups["blogContent"].Value;

                // Now that we've extracted it, it still needs to be cleaned up
                // Strip the DIV from the beginning and end

                string fixedBlogContent = rawBlogContent;
                */

                returnablePosts.Add(new SharepointBlogPost()
                {
                    Title = item.Title.Text,
                    Author = author,
                    Content = contentBlob,
                    PublishDate = DateTime.Now
                });
            }
            
            // Return parsed calendar events
            return returnablePosts.ToList();

        }
    }

}