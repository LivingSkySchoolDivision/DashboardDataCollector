using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class Sharepoint2013BlogParser
    {
        public static List<SharepointBlogPost> ParseRSSFeed(string username, string password, string blogXMLURL)
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

                // Parse the blog post content
                /*
                 <entry m:etag="W/&quot;3&quot;">
                    <id>https://portal.lskysd.ca/department/ss/_vti_bin/ListData.svc/NewsAnnouncements(2)</id>
                    <title type="text">Student services dashboard</title>
                    <updated>2015-05-29T15:54:03-06:00</updated>
                    <author>
                      <name />
                    </author>
                    <link rel="edit" title="NewsAnnouncementsItem" href="NewsAnnouncements(2)" />
                    <link rel="http://schemas.microsoft.com/ado/2007/08/dataservices/related/CreatedBy" type="application/atom+xml;type=entry" title="CreatedBy" href="NewsAnnouncements(2)/CreatedBy" />
                    <link rel="http://schemas.microsoft.com/ado/2007/08/dataservices/related/ModifiedBy" type="application/atom+xml;type=entry" title="ModifiedBy" href="NewsAnnouncements(2)/ModifiedBy" />
                    <link rel="http://schemas.microsoft.com/ado/2007/08/dataservices/related/Attachments" type="application/atom+xml;type=feed" title="Attachments" href="NewsAnnouncements(2)/Attachments" />
                    <category term="Microsoft.SharePoint.DataService.NewsAnnouncementsItem" scheme="http://schemas.microsoft.com/ado/2007/08/dataservices/scheme" />
                    <content type="application/xml">
                      <m:properties>
                        <d:ContentTypeID>0x010400B4770AD9EBF21B418E62232282AC48D4</d:ContentTypeID>
                        <d:Title>Student services dashboard</d:Title>
                        <d:Body>&lt;div class="ExternalClassE3418EECA9A64323861A63B73B4FAB21"&gt;&lt;p&gt;This is an announcement list on sharepoint.&lt;/p&gt;&#xD;
                &lt;p&gt;The most recent announcement will automatically get displayed on the &amp;quot;dashboard&amp;quot; television in the Student Services department.&lt;/p&gt;&#xD;
                &lt;p&gt;If a post is really long, it will get cut off on the bottom, so you don't want to write paragraphs of stuff here.&lt;br /&gt;&lt;/p&gt;&lt;/div&gt;</d:Body>
                        <d:Expires m:type="Edm.DateTime" m:null="true" />
                        <d:Id m:type="Edm.Int32">2</d:Id>
                        <d:ContentType>Announcement</d:ContentType>
                        <d:Modified m:type="Edm.DateTime">2015-05-29T15:54:03</d:Modified>
                        <d:Created m:type="Edm.DateTime">2015-05-29T15:43:43</d:Created>
                        <d:CreatedById m:type="Edm.Int32">17</d:CreatedById>
                        <d:ModifiedById m:type="Edm.Int32">17</d:ModifiedById>
                        <d:Owshiddenversion m:type="Edm.Int32">3</d:Owshiddenversion>
                        <d:Version>1.0</d:Version>
                        <d:Path>/department/ss/Announcements</d:Path>
                      </m:properties>
                    </content>
                  </entry>
                 */

                /* 
                 * To get body it looks like you'll need to regex between <d:Body> and </d:Body>, then clean up the DIV that it's in
                 */

                // Don't just go by line, because things can be multi-line it looks like
                // Better to use regex on the entire blob

                string contentBlob = item.Summary.Text;




                returnablePosts.Add(new SharepointBlogPost()
                {
                    Title = item.Title.Text,
                    Author = author,
                    Content = item.Content,
                    PublishDate = ""
                });
                
            }
            
            // Return parsed calendar events
            return returnablePosts.OrderBy(ev => ev.PublishDate).ToList();

        }
    }

}