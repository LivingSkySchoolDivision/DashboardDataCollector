using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class CalendarEvent
    {
        public string ID
        {
            get
            {
                return GetSHA256(this.Title + this.EventStart.ToShortDateString() + this.EventEnd.ToShortDateString());
            }
        }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public TimeSpan Duration
        {
            get
            {
                return EventEnd.Subtract(EventStart);
            }
        }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool AllDay { get; set; }
        public bool Recurring { get; set; }

        public string Author { get; set; }

        public CalendarEvent() { }

        public override string ToString()
        {
            return "CalendarEvent { Starts: " + EventStart.ToShortDateString() + " " + EventStart.ToShortTimeString() + ", Ends: " + EventEnd.ToShortDateString() + " " + EventEnd.ToShortTimeString() + ", All Day: " + AllDay + ", Title: " + Title + ", Location: " + Location + " }";
        }
        
        private static string GetSHA256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}