using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using LSKYDashboardDataCollector.Common;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class Sharepoint2013CalendarParser
    {
        public static List<CalendarEvent> ParseRSSFeed(string username, string password, string siteBaseURL, string listGUID)
        {
            // Example url: https://portal.lskysd.ca/officecalendars/_layouts/15/listfeed.aspx?List={D492D463-8204-460C-99A2-81F7646FB65E}

            List<CalendarEvent> returnedEvents = new List<CalendarEvent>();

            // Build the URL
            string RSSURL = siteBaseURL + "/_layouts/15/listfeed.aspx?List=" + listGUID;
            
            // Make a web request to the specified url
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RSSURL);
            request.Credentials = new System.Net.NetworkCredential(username, password);

            //byte[] authBytes = Encoding.UTF8.GetBytes((username + ":" + password).ToCharArray());
            //request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);

            request.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            request.UserAgent = "Sharepoint Dashboard Connector, written by Mark Strendin, Living Sky School Division";
            WebResponse webresponse = request.GetResponse();
            Stream receiveStream = webresponse.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            XmlReader xmlReader = XmlReader.Create(readStream);

            // Parse RSS items into objects
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            foreach (SyndicationItem item in feed.Items)
            {
                // Filter out deleted events
                if (!item.Title.Text.ToLower().StartsWith("deleted:"))
                {
                    // Figure out author
                    string author = string.Empty;
                    foreach (SyndicationPerson person in item.Authors)
                    {
                        author = person.Email; // Sharepoint puts the person's name in the email field
                    }

                    // Extract dates and times from the description field
/*
<![CDATA[<div><b>Start Time:</b> 9/3/2015 1:30 PM</div>
<div><b>End Time:</b> 9/3/2015 3:30 PM</div>
<div><b>Description:</b> <div></div></div>
<div><b>Created:</b> 9/3/2015 11:01 AM</div>
<div><b>Created By:</b> Christeena Fisher</div>
<div><b>Modified:</b> 9/3/2015 11:01 AM</div>
<div><b>Modified By:</b> Christeena Fisher</div>
<div><b>Title:</b> Brenda</div>
<div><b>Version:</b> 1.0</div>
]]>
 */
                    string descriptionBlob = item.Summary.Text;

                    DateTime startDate = DateTime.MinValue;
                    DateTime endDate = DateTime.MinValue;
                    string description = string.Empty;
                    string location = string.Empty;

                    // Split into seperate lines
                    string[] descriptions = descriptionBlob.Split('\n');
                    foreach (string descLine in descriptions)
                    {
                        if (descLine.ToLower().Contains("start time:"))
                        {
                            string startTimeRaw = descLine.Replace("<b>Start Time:</b> ", string.Empty).Replace("</div>", string.Empty).Replace("<div>", string.Empty).Trim();
                            DateTime.TryParse(startTimeRaw, out startDate);
                        }

                        if (descLine.ToLower().Contains("end time:"))
                        {
                            string endTimeRaw = descLine.Replace("<b>End Time:</b> ", string.Empty).Replace("</div>", string.Empty).Replace("<div>", string.Empty).Trim();
                            DateTime.TryParse(endTimeRaw, out endDate);
                        }

                        if (descLine.ToLower().Contains("description:"))
                        {
                            description = descLine.Replace("<b>Description:</b> ", string.Empty).Replace("</div>", string.Empty).Replace("<div>", string.Empty).Trim();
                        }

                        if (descLine.ToLower().Contains("location:"))
                        {
                            location = descLine.Replace("<b>Location:</b> ", string.Empty).Replace("</div>", string.Empty).Replace("<div>", string.Empty).Trim();
                        }
                    }

                    returnedEvents.Add(new CalendarEvent()
                    {
                        Title = item.Title.Text,
                        Description = description,
                        Author = author,
                        EventStart = startDate,
                        EventEnd = endDate,
                        Location = location
                    });   
                }
            }

            // Return parsed calendar events
            return returnedEvents;

        }
    }
}