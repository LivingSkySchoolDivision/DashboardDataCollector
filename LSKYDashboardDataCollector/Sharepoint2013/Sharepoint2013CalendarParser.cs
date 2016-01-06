using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
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

            List<SharepointCalendarEvent> deletedRecurringEvents = new List<SharepointCalendarEvent>();
            List<SharepointCalendarEvent> recurringEvents_Unexpanded = new List<SharepointCalendarEvent>();
            
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            Microsoft.SharePoint.Client.ListItemCollection sharepointListItems = sharepointList.GetItems(query);
            
            sharepointContext.Load(sharepointListItems);

            sharepointContext.ExecuteQuery();

            foreach (ListItem item in sharepointListItems)
            {
                try
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

                    bool isTimeGMT = true; // Previously we would have had to check the times to see if they are sane, but I think when loading data this way, all times are UTC instead

                    // Times returned are all in UTC - adjust to local time
                    if (isTimeGMT)
                    {
                        eventStarts = eventStarts + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                        eventEnds = eventEnds + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    }

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

                    SharepointCalendarEvent newCalendarEvent = new SharepointCalendarEvent()
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
                    };


                    // Correct start and end dates if the event is recurring
                    if (recurring)
                    {
                        // Deal with recurring events differently - Their dates will be screwed up, so use the data from them to create "phantom" events that line up with the dates required

                        // Events that start with "Deleted" cancel out recurring events
                        if (item["Title"].ToString().StartsWith("Deleted:"))
                        {
                            deletedRecurringEvents.Add(newCalendarEvent);
                        }
                        else
                        {
                            recurringEvents_Unexpanded.Add(newCalendarEvent);
                        }
                    }
                    else
                    {
                        returnMe.Add(newCalendarEvent);
                    }
                } 
                catch { }
            }

            
            // Deal with recurring events
            foreach (SharepointCalendarEvent ev in recurringEvents_Unexpanded)
            {
                
                // Figure out when this even reocurrs

                // the field "RecurrenceData" can have data in two different formats - either a string that looks like this:
                //  Every 1 month(s) on the fourth Wednesday
                //  Every 1 week(s) on: Wednesday
                //  Every 1 month(s) on the fourth Wednesday
                // or XML data that looks like this:
                //  <recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><weekly we="TRUE" weekFrequency="1" /></repeat><repeatForever>FALSE</repeatForever></rule></recurrence>
                //   This apparently means every wednesday, every week
                //  <recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><monthlyByDay we="TRUE" weekdayOfMonth="first" monthFrequency="1" /></repeat><repeatForever>FALSE</repeatForever></rule></recurrence>
                //   This apparently means every first wednesday of every month
                //  <recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><monthlyByDay we="TRUE" weekdayOfMonth="second" monthFrequency="1" /></repeat><repeatForever>FALSE</repeatForever></rule></recurrence>
                //   This apparently means every second wednesday of every month

                // The "<firstDayOfWeek>su</firstDayOfWeek>" means that Sunday is considered the first day of the week

                // Time range for phantom events
                //  Daily events: 2 weeks back, 6 weeks ahead
                //  Weekly events: 4 weeks back, 8 weeks ahead
                //  Monthly events: 2 months back, 12 months ahead
                //  Yearly events: 1 year back, 5 years ahead

                // Repeating:
                // Could be:
                //  <repeatInstances>10</repeatInstances>
                //  <repeatForever>FALSE</repeatForever>
                //  <windowEnd>2007-05-31T22:00:00Z</windowEnd>

                // parse XML recurrence data
                if (ev.RecurrenceInfo.StartsWith("<recurrence"))
                {
                    // Extract the <repeat> section(s), because we don't really care about the rest
                    // Regex: <repeat>(.+?)<\/repeat>
                    Regex regex = new Regex(@"<repeat>(.+?)<\/repeat>");
                    MatchCollection matches = regex.Matches(ev.RecurrenceInfo);

                    // su="TRUE"
                    // mo="TRUE"
                    // tu="TRUE"
                    // we="TRUE"
                    // th="TRUE"
                    // fr="TRUE"
                    // sa="TRUE"

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            // Strip the <repeat> and </repeat> from the start and end of the segment
                            string segment = match.Value.Substring(8, match.Value.Length - (8 + 9));

                            // Daily
                            if (segment.StartsWith("<daily"))
                            {
                                // parse the "dayFrequency", then just multiply
                            }

                            // Weekly
                            if (segment.StartsWith("<weekly"))
                            {
                                // For weekly events we want to create phantom events 4 weeks back, and 8 weeks ahead (12 weeks total)
                             
                                // We need to factor in the weekFrequency="1", to handle events that might be every second week or something
                                int weekFrequency = 1;
                                
                                Regex weekFrequencyRegex = new Regex(@"monthFrequency=\""(\d+)\""");
                                Match weeklyFrequencyMatch = weekFrequencyRegex.Match(segment);
                                if (weeklyFrequencyMatch.Success)
                                {
                                    weekFrequency = Parsers.ParseInt(weeklyFrequencyMatch.Value.Substring(16, weeklyFrequencyMatch.Value.Length - 17));
                                    if (weekFrequency < 1)
                                    {
                                        weekFrequency = 1;
                                    }
                                }
                                
                                // Get the current week (Sunday), then subtract 4 weeks from it for a start date
                                DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)).AddDays(-28);

                                // Translate detected days into day numbers (Sunday = 0)
                                List<int> eventDayNumbers = new List<int>();

                                if (segment.Contains("su=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(0);
                                }

                                if (segment.Contains("mo=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(1);
                                }

                                if (segment.Contains("tu=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(2);
                                }

                                if (segment.Contains("we=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(3);
                                }

                                if (segment.Contains("th=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(4);
                                }

                                if (segment.Contains("fr=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(5);
                                }

                                if (segment.Contains("sa=\"TRUE\""))
                                {
                                    eventDayNumbers.Add(6);
                                }


                                for (int weekCounter = 0; weekCounter < 12; weekCounter += weekFrequency)
                                {
                                    foreach (int dayNum in eventDayNumbers)
                                    {
                                        DateTime newEventStartDate = startOfWeek.AddDays(dayNum).AddDays(7*weekCounter);
                                        //DateTime newEventEndDate = newEventStartDate.Add(ev.Duration);
                                        returnMe.Add(ev.CloneWithNewDates(newEventStartDate, newEventStartDate));
                                    }
                                }


                            }

                            // Monthly
                            if (segment.StartsWith("<monthlyByDay"))
                            {

                            }

                            // Yearly
                            if (segment.StartsWith("<yearly"))
                            {

                            }

                        }
                    }

                }
                else
                {
                    // These events might all be deleted events that just need to be cancelled out
                    // Parse english instead of XML
                }


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