using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LSKYDashboardDataCollector.Sharepoint2013
{
    public class SharepointCalendarEvent
    {
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
        public bool Deleted { get; set; }

        public string Author { get; set; }

        public string RecurrenceInfo { get; set; }
        public SharepointCalendarEvent() { }

        public override string ToString()
        {
            return "SharepointCalendarEvent { Starts: " + EventStart.ToShortDateString() + " " + EventStart.ToShortTimeString() + ", Ends: " + EventEnd.ToShortDateString() + " " + EventEnd.ToShortTimeString() + ", All Day: " + AllDay + ", Title: " + Title + ", Location: " + Location + " }";
        }

        public SharepointCalendarEvent Clone()
        {
            return this.CloneWithNewDates(this.EventStart, this.EventEnd);
        }

        public SharepointCalendarEvent CloneWithNewDates(DateTime startDate, DateTime endDate)
        {
            return new SharepointCalendarEvent()
            {
                EventStart = new DateTime(startDate.Year, startDate.Month, startDate.Day, this.EventStart.Hour, this.EventStart.Minute, this.EventStart.Second),
                EventEnd = new DateTime(endDate.Year, endDate.Month, endDate.Day, this.EventEnd.Hour, this.EventEnd.Minute, this.EventEnd.Second),
                AllDay = this.AllDay,
                Author = this.Author,
                Deleted = this.Deleted,
                Description = this.Description,
                Location = this.Location,
                RecurrenceInfo = this.RecurrenceInfo,
                Recurring = this.Recurring,
                Title = this.Title
                
            };
        }
        
       
    }
}