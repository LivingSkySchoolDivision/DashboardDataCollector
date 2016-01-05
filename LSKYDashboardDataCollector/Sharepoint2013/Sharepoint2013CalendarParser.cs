using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.AccessControl;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using LSKYDashboardDataCollector.Common;
using Microsoft.SharePoint.Client;
using ListItemCollection = System.Web.UI.WebControls.ListItemCollection;
using TimeZone = System.TimeZone;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class Sharepoint2013CalendarParser
    {
        private static string EventSignature(string title, DateTime startDate)
        {
            return title + startDate.ToString();
        }

        private static string EventSignature(SharepointCalendarEvent ev)
        {
            return EventSignature(ev.Title, ev.EventStart);
        }

        
        private static List<SharepointCalendarEvent> ParseSharepointList(ClientContext sharepointContext, List sharepointList)
        {
            List<SharepointCalendarEvent> returnMe = new List<SharepointCalendarEvent>();
            
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            Microsoft.SharePoint.Client.ListItemCollection sharepointListItems = sharepointList.GetItems(query);

            sharepointContext.Load(sharepointListItems);

            sharepointContext.ExecuteQuery();

            foreach (ListItem item in sharepointListItems)
            {
                //try
                {
                    // We need to deal with "Deleted" events
                    // These are events that cancel out existing events, if the events are recurring

                    // Deal with recurring events, somehow
                    
                    string title = string.Empty;
                    if (item.FieldValues.ContainsKey("Title"))
                    {
                        if (!string.IsNullOrEmpty((string)item["Title"]))
                        {
                            title = item["Title"].ToString();
                        }
                    };
                    
                    string location = string.Empty;
                    
                    string description = string.Empty;
                    if (item.FieldValues.ContainsKey("Description"))
                    {
                        if (!string.IsNullOrEmpty((string) item["Description"]))
                        {
                            // The description will almost always contain HTML, which we can't include in the JSON file or it won't validate
                            // For now, we dont need the description
                            // In the future perhaps we can strip out the HTML tags somehow

                            //description = item["Description"].ToString();
                        }
                    }

                    string author = string.Empty; // Haven't figured out how to get this yet

                    DateTime eventStarts = DateTime.MinValue;
                    if (item.FieldValues.ContainsKey("EventDate"))
                    {
                        eventStarts = Parsers.ParseDate(item["EventDate"].ToString());
                    }

                    DateTime eventEnds = DateTime.MinValue;
                    if (item.FieldValues.ContainsKey("EndDate"))
                    {
                       eventEnds = Parsers.ParseDate(item["EndDate"].ToString());
                    };


                    bool allDay = false;
                    if (item.FieldValues.ContainsKey("fAllDayEvent"))
                    {
                        allDay = Parsers.ParseBool(item["fAllDayEvent"].ToString());
                    }

                    bool recurring = false;
                    if (item.FieldValues.ContainsKey("fRecurrence"))
                    {
                        recurring = Parsers.ParseBool(item["fRecurrence"].ToString());
                    }
                    
                    
                    string recurrenceData = string.Empty;
                    if (item.FieldValues.ContainsKey("RecurrenceData"))
                    {
                        if (!string.IsNullOrEmpty((string)item["RecurrenceData"]))
                        {
                            recurrenceData = item["RecurrenceData"].ToString();
                        }
                    };
                     
                    //*/

                    // Correct start and end dates if the event is recurring
                    if (recurring)
                    {
                        // Deal with recurring events differently - Their dates will be screwed up, so use the data from them to create "phantom" events that line up with the dates required

                        
                    }
                    else
                    {
                        //throw new Exception("got this far");
                        // Non recurring events can go straight to the list
                        
                        returnMe.Add(new SharepointCalendarEvent()
                        {
                            Title = title,
                            Location = location,
                            Description = description,
                            Author = author,
                            AllDay = allDay,
                            Recurring = recurring,
                            EventStart = eventStarts,
                            EventEnd = eventEnds,
                            RecurrenceInfo = recurrenceData
                        });
                    }
                } 
                //catch { }
            }

            return returnMe.OrderBy(e => e.EventStart).ThenBy(e => e.EventEnd).ToList();
        }

        public static List<SharepointCalendarEvent> GetCalendarByName(string username, string password, string siteBaseURL, string listName)
        {
            ClientContext sharepointClientContext = new ClientContext(siteBaseURL);
            sharepointClientContext.Credentials = new NetworkCredential(username, password);
            List sharepointList = sharepointClientContext.Web.Lists.GetByTitle(listName);

            return ParseSharepointList(sharepointClientContext, sharepointList);
        }

        public static List<SharepointCalendarEvent> GetCalendarByGUID(string username, string password, string siteBaseURL, string listGUID)
        {
            ClientContext sharepointClientContext = new ClientContext(siteBaseURL);
            sharepointClientContext.Credentials = new NetworkCredential(username, password);
            List sharepointList = sharepointClientContext.Web.Lists.GetById(new Guid(listGUID));

            return ParseSharepointList(sharepointClientContext, sharepointList);
        }
    }
}