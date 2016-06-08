using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using LSKYDashboardDataCollector.Proxy;
using Microsoft.SharePoint.Client;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class Sharepoint2013BlogParser
    {
        public static List<SharepointBlogPost> GetBlogByName(string username, string password, string siteBaseURL, string listName)
        {
            ClientContext sharepointClientContext = new ClientContext(siteBaseURL);
            sharepointClientContext.Credentials = new NetworkCredential(username, password);
            List sharepointList = sharepointClientContext.Web.Lists.GetByTitle(listName);

            return ParseSharepointList(sharepointClientContext, sharepointList);
        }

        public static List<SharepointBlogPost> GetBlogByGUID(string username, string password, string siteBaseURL,
            string guid)
        {
            ClientContext sharepointClientContext = new ClientContext(siteBaseURL);
            sharepointClientContext.Credentials = new NetworkCredential(username, password);
            List sharepointList = sharepointClientContext.Web.Lists.GetById(new Guid(guid));

            return ParseSharepointList(sharepointClientContext, sharepointList);
        }

        private static List<SharepointBlogPost> ParseSharepointList(ClientContext sharepointContext, List sharepointList)
        {
            List<SharepointBlogPost> returnMe = new List<SharepointBlogPost>();

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
                        if (!string.IsNullOrEmpty((string)item["Description"]))
                        {
                            // The description will almost always contain HTML, which we can't include in the JSON file or it won't validate
                            // For now, we dont need the description
                            // In the future perhaps we can strip out the HTML tags somehow

                            description = Helpers.SanitizeForJSON(item["Description"].ToString());
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
                            newCalendarEvent.Title = newCalendarEvent.Title.Remove(0, 8).Trim();
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
            #region Deal with recurring events
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

                                int dayFrequency = 1;

                                Regex dayFrequencyRegex = new Regex(@"dayFrequency=\""(\d+)\""");
                                Match dayFrequencyMatch = dayFrequencyRegex.Match(segment);
                                if (dayFrequencyMatch.Success)
                                {
                                    dayFrequency = Parsers.ParseInt(dayFrequencyMatch.Value.Substring(14, dayFrequencyMatch.Value.Length - (14 + 1)));
                                    if (dayFrequency < 1)
                                    {
                                        dayFrequency = 1;
                                    }
                                }

                                // Get the original start date, and accellerate it to a date closer to now
                                DateTime startDate = ev.EventStart;
                                if (startDate <= DateTime.Now)
                                {
                                    while (startDate <= DateTime.Now.AddDays(-7))
                                    {
                                        startDate = startDate.AddDays(dayFrequency);
                                    }
                                }


                                // Get the current week (Sunday), then subtract 1 week
                                //DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)).AddDays(-7);

                                for (int dayCounter = 0; dayCounter <= 30; dayCounter += dayFrequency)
                                {
                                    DateTime newEventStartDate = startDate.AddDays(dayCounter);
                                    //DateTime newEventEndDate = newEventStartDate.Add(ev.Duration);
                                    returnMe.Add(ev.CloneWithNewDates(newEventStartDate, newEventStartDate));
                                }

                            }


                            // Weekly
                            if (segment.StartsWith("<weekly"))
                            {
                                // For weekly events we want to create phantom events 4 weeks back, and 8 weeks ahead (12 weeks total)

                                // We need to factor in the weekFrequency="1", to handle events that might be every second week or something
                                int weekFrequency = 1;

                                Regex weekFrequencyRegex = new Regex(@"weekFrequency=\""(\d+)\""");
                                Match weeklyFrequencyMatch = weekFrequencyRegex.Match(segment);
                                if (weeklyFrequencyMatch.Success)
                                {
                                    weekFrequency = Parsers.ParseInt(weeklyFrequencyMatch.Value.Substring(15, weeklyFrequencyMatch.Value.Length - (15 + 1)));
                                    if (weekFrequency < 1)
                                    {
                                        weekFrequency = 1;
                                    }
                                }

                                // Get the original start date, and accellerate it to a date closer to now
                                DateTime startDate = ev.EventStart;
                                if (startDate <= DateTime.Now)
                                {
                                    while (startDate <= DateTime.Now.AddDays(-28))
                                    {
                                        startDate = startDate.AddDays(weekFrequency * 7);
                                    }
                                }

                                // Get the current week (Sunday), then subtract 4 weeks from it for a start date
                                //DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)).AddDays(-28);

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

                                // Get the sunday of the week of the start date of the event
                                DateTime startingWeekSunday = startDate.AddDays(-1 * (int)startDate.DayOfWeek);

                                for (int weekCounter = 0; weekCounter < 12; weekCounter += weekFrequency)
                                {
                                    foreach (int dayNum in eventDayNumbers)
                                    {
                                        DateTime newEventStartDate = startingWeekSunday.AddDays(dayNum).AddDays(7 * weekCounter);
                                        returnMe.Add(ev.CloneWithNewDates(newEventStartDate, newEventStartDate));
                                    }
                                }
                            }


                            // Monthly, given day numbers (Every 2nd of the month, for example)
                            if (segment.StartsWith("<monthly "))
                            {
                                // If the user sets it to every ___ day of every __ month, it looks like this:
                                //  <monthly monthFrequency="2" day="6" />

                                int monthFrequency = 1;
                                Regex monthFrequencyRegex = new Regex(@"monthFrequency=\""(\d+)\""");
                                Match monthFrequencyMatch = monthFrequencyRegex.Match(segment);
                                if (monthFrequencyMatch.Success)
                                {
                                    monthFrequency = Parsers.ParseInt(monthFrequencyMatch.Value.Substring(16, monthFrequencyMatch.Value.Length - 17));
                                    if (monthFrequency < 1)
                                    {
                                        monthFrequency = 1;
                                    }
                                }

                                // We ignore the actual day of the start date, in favor of this value (this is what Sharepoint does anyway)
                                int dayNumber = 0;
                                Regex dayNumberRegex = new Regex(@"day=\""(\d+)\""");
                                Match dayNumberMatch = dayNumberRegex.Match(segment);
                                if (dayNumberMatch.Success)
                                {
                                    dayNumber = Parsers.ParseInt(dayNumberMatch.Value.Substring(5, dayNumberMatch.Value.Length - (5 + 1)));
                                    if (dayNumber < 1)
                                    {
                                        dayNumber = 1;
                                    }
                                }

                                // Rebuild the event start date based on the recurrence information
                                ev.EventStart = new DateTime(ev.EventStart.Year, ev.EventStart.Month, dayNumber, ev.EventStart.Hour, ev.EventStart.Minute, ev.EventStart.Second);

                                // Find the original month and acellerate to a time closer to now, so we don't incorrectly assume that this month includes the event (if the month frequency causes it to skip the current month)
                                DateTime startDate = ev.EventStart;
                                if (startDate <= DateTime.Now)
                                {
                                    while (startDate <= DateTime.Now.AddDays(-1 * (31 * (monthFrequency + 1))))
                                    {
                                        startDate = startDate.AddMonths(monthFrequency);
                                    }
                                }

                                for (int monthCounter = 0; monthCounter < 12; monthCounter += monthFrequency)
                                {
                                    DateTime newEventStartDate = startDate.AddMonths(monthFrequency * monthCounter);
                                    returnMe.Add(ev.CloneWithNewDates(newEventStartDate, newEventStartDate));
                                }
                            }


                            // Monthly by Day (Every third wednesday, for example)
                            if (segment.StartsWith("<monthlyByDay "))
                            {
                                // weekdayOfMonth="first"
                                // First|Second|Third|Fourth|Last 
                                // Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday|Weekday|Weekend Day, etc


                                // Sharepoint only lets monthly events happen on a single day per event so we dont need to deal with multiple days

                                int monthFrequency = 1;
                                Regex monthFrequencyRegex = new Regex(@"monthFrequency=\""(\d+)\""");
                                Match monthFrequencyMatch = monthFrequencyRegex.Match(segment);
                                if (monthFrequencyMatch.Success)
                                {
                                    monthFrequency = Parsers.ParseInt(monthFrequencyMatch.Value.Substring(16, monthFrequencyMatch.Value.Length - 17));
                                    if (monthFrequency < 1)
                                    {
                                        monthFrequency = 1;
                                    }
                                }

                                // Get the weekday of month string
                                string weekdayOfMonthString = string.Empty;
                                Regex weekdayOfMonthRegex = new Regex(@"weekdayOfMonth=\""(.+?)\""");
                                Match weekdayOfMonthMatch = weekdayOfMonthRegex.Match(segment);
                                if (weekdayOfMonthMatch.Success)
                                {
                                    weekdayOfMonthString = weekdayOfMonthMatch.Value.Substring(16, weekdayOfMonthMatch.Value.Length - 17);
                                }


                                // Make a list of years and months that we care about, because the contextual days will differ year to year


                                // If we're missing the weekday of the month string, we can't continue
                                if (!string.IsNullOrEmpty(weekdayOfMonthString))
                                {
                                    int goalIteration = 0;
                                    switch (weekdayOfMonthString.ToLower())
                                    {
                                        case "first":
                                            goalIteration = 1;
                                            break;
                                        case "second":
                                            goalIteration = 2;
                                            break;
                                        case "third":
                                            goalIteration = 3;
                                            break;
                                        case "fourth":
                                            goalIteration = 4;
                                            break;
                                        case "last":
                                            goalIteration = 999;
                                            break;
                                    }

                                    DayOfWeek goalDayOfWeek = DayOfWeek.Saturday;

                                    if (segment.Contains("su=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Sunday;
                                    }

                                    if (segment.Contains("mo=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Monday;
                                    }

                                    if (segment.Contains("tu=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Tuesday;
                                    }

                                    if (segment.Contains("we=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Wednesday;
                                    }

                                    if (segment.Contains("th=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Thursday;
                                    }

                                    if (segment.Contains("fr=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Friday;
                                    }

                                    if (segment.Contains("sa=\"TRUE\""))
                                    {
                                        goalDayOfWeek = DayOfWeek.Saturday;
                                    }

                                    // Get the original start date, and accellerate it to a date closer to now
                                    DateTime startDate = ev.EventStart;
                                    if (startDate <= DateTime.Now)
                                    {
                                        while (startDate <= DateTime.Now.AddDays(-1 * monthFrequency * 31))
                                        {
                                            startDate = startDate.AddMonths(monthFrequency);
                                        }
                                    }

                                    // Make a list of months, in the form of datetimes of the first of those months, for each phantom event we want to create
                                    // We'll need to go through all the below BS for eachof them seperately, because they will differ year to year

                                    List<DateTime> phantomEventMonths = new List<DateTime>();
                                    for (int monthCount = 0; monthCount <= 12; monthCount += monthFrequency)
                                    {
                                        phantomEventMonths.Add(new DateTime(startDate.Year, startDate.Month, 1).AddMonths(monthCount));
                                    }

                                    foreach (DateTime phantomEventMonth in phantomEventMonths)
                                    {
                                        DateTime newEventStartDate = phantomEventMonth;

                                        if (goalIteration == 999)
                                        {
                                            // We're finding the last day of the month instead of the first day
                                            if (segment.ToLower().Contains("weekend_day=\"true\""))
                                            {
                                                newEventStartDate = Helpers.GetDayOfMonth_Backwards(phantomEventMonth.Year, phantomEventMonth.Month, 1, Helpers.GetWeekendDays());
                                            }
                                            else if (segment.ToLower().Contains("weekday=\"true\""))
                                            {
                                                newEventStartDate = Helpers.GetDayOfMonth_Backwards(phantomEventMonth.Year, phantomEventMonth.Month, 1, Helpers.GetWeekdays());
                                            }
                                            else if (segment.ToLower().Contains("day=\"true\""))
                                            {
                                                // Just the last day of the month
                                                newEventStartDate = new DateTime(phantomEventMonth.Year, phantomEventMonth.Month, DateTime.DaysInMonth(phantomEventMonth.Year, phantomEventMonth.Month));
                                            }
                                            else
                                            {
                                                // Find the day specifically
                                                newEventStartDate = Helpers.GetDayOfMonth_Backwards(phantomEventMonth.Year, phantomEventMonth.Month, 1, goalDayOfWeek);
                                            }
                                        }
                                        else
                                        {
                                            if (segment.ToLower().Contains("weekend_day=\"true\""))
                                            {
                                                newEventStartDate = Helpers.GetDayOfMonth(phantomEventMonth.Year, phantomEventMonth.Month, goalIteration, Helpers.GetWeekendDays());
                                            }
                                            else if (segment.ToLower().Contains("weekday=\"true\""))
                                            {
                                                newEventStartDate = Helpers.GetDayOfMonth(phantomEventMonth.Year, phantomEventMonth.Month, goalIteration, Helpers.GetWeekdays());
                                            }
                                            else if (segment.ToLower().Contains("day=\"true\""))
                                            {
                                                newEventStartDate = new DateTime(phantomEventMonth.Year, phantomEventMonth.Month, goalIteration);
                                            }
                                            else
                                            {
                                                // Find the day specifically
                                                newEventStartDate = Helpers.GetDayOfMonth(phantomEventMonth.Year, phantomEventMonth.Month, goalIteration, goalDayOfWeek);
                                            }
                                        }

                                        returnMe.Add(ev.CloneWithNewDates(newEventStartDate, newEventStartDate));
                                    }
                                }
                            }


                            // Yearly
                            if (segment.StartsWith("<yearly"))
                            {
                                // Don't support yearly events at the moment - I don't have the patience to deal with this right now, and theres little chance a yearly event will be in our calendars
                            }

                        }
                    }

                }
            }
            #endregion

            // Deal with deleted events 
            List<SharepointCalendarEvent> finalReturnedEventsList = new List<SharepointCalendarEvent>();
            foreach (SharepointCalendarEvent ev in returnMe)
            {
                bool foundDeletedEvent = false;
                foreach (SharepointCalendarEvent deletedEvent in deletedRecurringEvents)
                {
                    if ((
                        (ev.Title == deletedEvent.Title) &&
                        (ev.EventStart.Year == deletedEvent.EventStart.Year) &&
                        (ev.EventStart.Month == deletedEvent.EventStart.Month) &&
                        (ev.EventStart.Day == deletedEvent.EventStart.Day)
                        ))
                    {
                        foundDeletedEvent = true;
                    }
                }

                if (!foundDeletedEvent)
                {
                    finalReturnedEventsList.Add(ev);
                }
            }


            return finalReturnedEventsList.OrderBy(e => e.EventStart).ThenBy(e => e.EventEnd).ToList();
        } 

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