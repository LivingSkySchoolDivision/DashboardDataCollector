using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public partial class Calendar : System.Web.UI.Page
    {
        private string CalendarEventJSON(CalendarEvent ce)
        {
            StringBuilder returnMe = new StringBuilder();

            // Create some friendly text to use in the dashboard
            string StartDateFriendly = ce.EventStart.DayOfWeek.ToString().Substring(0, 3) + ", " + ce.EventStart.ToString("MMM dd");

            double DaysUntil = ce.EventStart.Subtract(DateTime.Today).TotalDays;

            if ((DaysUntil > 0.0) && (DaysUntil < 1.0))
            {
                StartDateFriendly = "Today";
            }

            if ((DaysUntil > 1.0) && (DaysUntil < 2.0))
            {
                StartDateFriendly = "Tomorrow";
            }

            returnMe.Append("{ ");
            returnMe.Append("\"id\" : \"" + ce.ID + "\",");
            returnMe.Append("\"startdate\" : \"" + ce.EventStart.ToShortDateString() + "\",");
            returnMe.Append("\"startdatefriendly\" : \"" + StartDateFriendly + "\",");
            returnMe.Append("\"daysuntil\" : \"" + DaysUntil.ToString() + "\",");
            returnMe.Append("\"starttime\" : \"" + ce.EventStart.ToShortTimeString() + "\",");
            returnMe.Append("\"enddate\" : \"" + ce.EventEnd.ToShortDateString() + "\",");
            returnMe.Append("\"endtime\" : \"" + ce.EventEnd.ToShortTimeString() + "\",");
            returnMe.Append("\"totalhours\" : \"" + ce.Duration.Hours.ToString() + "\",");
            returnMe.Append("\"totaldays\" : \"" + ce.Duration.Days.ToString() + "\",");
            returnMe.Append("\"title\" : \"" + ce.Title + "\",");
            returnMe.Append("\"location\" : \"" + ce.Location + "\",");
            returnMe.Append("\"description\" : \"" + ce.Description + "\",");
            returnMe.Append("\"allday\" : \"" + ce.AllDay + "\"");
            returnMe.Append(" }");

            return returnMe.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string SharePointUsername = @"lskysd\dashboard";
            string SharePointPassword = "W3JoPAV14xY6D2cb6hZuSGYxgRULm";
            
            // Parse data from the querystring
            string baseURL = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["url"]))
            {
                baseURL = Request.QueryString["url"].ToString().Trim();
            }

            string guid = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["guid"]))
            {
                guid = Request.QueryString["guid"].ToString().Trim();
            }
            
            List<CalendarEvent> allEvents = new List<CalendarEvent>();
            if (!string.IsNullOrEmpty(baseURL) && !string.IsNullOrEmpty(guid))
            {
                try
                {
                    allEvents = Sharepoint2013CalendarParser.ParseRSSFeed(SharePointUsername, SharePointPassword, baseURL, guid);
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }

            // Sort into Today and Tomorrow lists
            List<CalendarEvent> eventsToday = new List<CalendarEvent>();
            List<CalendarEvent> eventsTomorrow = new List<CalendarEvent>();
            List<CalendarEvent> eventsRightNow = new List<CalendarEvent>();
            List<CalendarEvent> eventsUpcoming = new List<CalendarEvent>();

            foreach (CalendarEvent ce in allEvents)
            {
                // Today
                if ((ce.EventStart > DateTime.Today) && (ce.EventStart < DateTime.Today.AddDays(1)))
                {
                    eventsToday.Add(ce);
                }

                // Tomorrow
                if ((ce.EventStart > DateTime.Today.AddDays(1)) && (ce.EventStart < DateTime.Today.AddDays(2)))
                {
                    eventsTomorrow.Add(ce);
                }

                // Events happening now
                if ((ce.EventStart < DateTime.Now) && (ce.EventEnd > DateTime.Now))
                {
                    eventsRightNow.Add(ce);
                }

                // Events upcoming (because sharepoint likes to list events in the past for some reason?
                if (ce.EventStart > DateTime.Now)
                {
                    eventsUpcoming.Add(ce);
                }

            }
            
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";

            Response.Write("{\n");

            //Response.Write("\"XMLURL\": \"" + calendarXMLURL + "\",\n");

            Response.Write("\"allevents\": [\n");
            for (int x = 0; x < allEvents.Count; x++)
            {
                Response.Write(CalendarEventJSON(allEvents[x]));
                if (!(x + 1 >= allEvents.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("],\n");

            Response.Write("\"today\": [\n");
            for (int x = 0; x < eventsToday.Count; x++)
            {
                Response.Write(CalendarEventJSON(eventsToday[x]));
                if (!(x + 1 >= eventsToday.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("],\n");

            Response.Write("\"tomorrow\": [\n");
            for (int x = 0; x < eventsTomorrow.Count; x++)
            {
                Response.Write(CalendarEventJSON(eventsTomorrow[x]));
                if (!(x + 1 >= eventsTomorrow.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("],\n");

            Response.Write("\"rightnow\": [\n");
            for (int x = 0; x < eventsRightNow.Count; x++)
            {
                Response.Write(CalendarEventJSON(eventsRightNow[x]));
                if (!(x + 1 >= eventsRightNow.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("],\n");

            Response.Write("\"upcoming\": [\n");
            for (int x = 0; x < eventsUpcoming.Count; x++)
            {
                Response.Write(CalendarEventJSON(eventsUpcoming[x]));
                if (!(x + 1 >= eventsUpcoming.Count))
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("]\n");

            Response.Write("}\n");
            Response.End();  




        }
    }
}