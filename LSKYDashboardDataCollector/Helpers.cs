using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector
{
    public static class Helpers
    {
        public static IEnumerable<DateTime> GetEachDayBetween(DateTime dateFrom, DateTime dateTo)
        {
            DateTime from = dateFrom;
            DateTime to = dateTo;

            // Dates need to be in chronological order, so reverse them if necesary
            if (dateFrom > dateTo)
            {
                to = dateFrom;
                @from = dateTo;
            }

            List<DateTime> returnMe = new List<DateTime>();
            for (DateTime day = @from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                returnMe.Add(day);
            }
            return returnMe;
        }

        public static string TimeSince(DateTime thisTime)
        {
            TimeSpan duration = DateTime.Now.Subtract(thisTime);
            String returnMe = string.Empty;

            if (duration.TotalMinutes < 1)
            {
                int totalSeconds = (int)Math.Round(duration.TotalSeconds, 0);
                if (totalSeconds == 1)
                {
                    returnMe = totalSeconds + " second ago";
                }
                else
                {
                    returnMe = totalSeconds + " seconds ago";
                }
            }
            else if (duration.TotalHours < 1)
            {
                int totalMinutes = (int)Math.Round(duration.TotalMinutes, 0);

                if (totalMinutes == 1)
                {
                    returnMe = totalMinutes + " minute ago";
                }
                else
                {
                    returnMe = totalMinutes + " minutes ago";
                }
            }
            else if (duration.TotalDays < 1)
            {
                int numHours = (int)Math.Round(duration.TotalHours, 0);

                if (numHours == 1)
                {
                    returnMe = numHours + " hour ago";
                }
                else
                {
                    returnMe = numHours + " hours ago";
                }
            }
            else
            {
                int numDays = (int)Math.Round(duration.TotalDays, 0);

                if (numDays == 1)
                {
                    returnMe = numDays + " day ago";
                }
                else
                {
                    returnMe = numDays + " days ago";
                }
            }

            return returnMe;
        }
    }
}