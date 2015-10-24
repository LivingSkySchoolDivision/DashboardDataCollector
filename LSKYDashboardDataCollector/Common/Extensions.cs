using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LSKYDashboardDataCollector.Common
{
    public static class Extensions
    {
        public static string InEnglish(this TimeSpan timeSpan)
        {
            double streamDuration_Minutes = timeSpan.TotalMinutes;
            if (streamDuration_Minutes == 1)
            {
                return "1 minute";
            }
            else if (streamDuration_Minutes <= 120)
            {
                return Math.Round(streamDuration_Minutes, 0) + " minutes";
            }
            else
            {
                double streamDuration_Hours = timeSpan.TotalHours;
                if (streamDuration_Hours == 1)
                {
                    return "1 hour";
                }
                else
                {
                    if ((streamDuration_Hours % 1) == 0)
                    {

                        return Math.Round(streamDuration_Hours, 0) + " hours";
                    }
                    else
                    {

                        return Math.Round(streamDuration_Hours, 1) + " hours";
                    }
                }
            }

        }

        public static string ToCommaSeperatedListWithQuotes(this List<string> thisList)
        {
            if (thisList.Count == 0)
            {
                return string.Empty;
            }

            if (thisList.Count == 1)
            {
                return thisList.First();
            }

            StringBuilder returnMe = new StringBuilder();
            foreach (string thisString in thisList)
            {
                if (!string.IsNullOrEmpty(thisString))
                {
                    returnMe.Append("'" + thisString + "',");
                }
            }
            returnMe.Remove(returnMe.Length - 1, 1);

            return returnMe.ToString();
        }
    }
}