using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

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


        public static IEnumerable<DayOfWeek> GetWeekdays()
        {
            return new List<DayOfWeek>()
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            };
        }

        public static IEnumerable<DayOfWeek>  GetWeekendDays()
        {
            return new List<DayOfWeek>()
            {
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
        }

        public static IEnumerable<DateTime> GetDaysOfMonth(int year, int month)
        {
            List<DateTime> returnMe = new List<DateTime>();
            for (int dayNum = 1; dayNum <= DateTime.DaysInMonth(year, month); dayNum++)
            {
                returnMe.Add(new DateTime(year, month, dayNum));
            }
            return returnMe;
        }

      
        public static DateTime GetDayOfMonth_Backwards(int year, int month, int iteration, DayOfWeek dayOfWeek)
        {
            return GetDayOfMonth_Backwards(year, month, iteration, new List<DayOfWeek>() { dayOfWeek });
        }

        public static DateTime GetDayOfMonth(int year, int month, int iteration, DayOfWeek dayOfWeek)
        {
            return GetDayOfMonth(year, month, iteration, new List<DayOfWeek>() { dayOfWeek });
        }

        public static DateTime GetDayOfMonth_Backwards(int year, int month, int iteration, IEnumerable<DayOfWeek> daysOfWeek)
        {
            if (iteration == 999)
            {
                //return GetLastDayOfMonth(year, month, dayOfWeek);
            }

            DateTime returnMe = new DateTime(year, month, 1);
            int foundIteration = 0;

            foreach (DateTime day in GetDaysOfMonth(year, month).Reverse())
            {
                if (daysOfWeek.Contains(day.DayOfWeek))
                {
                    foundIteration++;
                    if (foundIteration == iteration)
                    {
                        returnMe = day;
                        break;
                    }
                }
            }

            return returnMe;
        }

        public static DateTime GetDayOfMonth(int year, int month, int iteration, IEnumerable<DayOfWeek> daysOfWeek)
        {
            if (iteration == 999)
            {
                return GetDayOfMonth_Backwards(year, month, iteration, daysOfWeek);
            }

            DateTime returnMe = new DateTime(year, month, 1);
            int foundIteration = 0;

            foreach (DateTime day in GetDaysOfMonth(year, month))
            {
                if (daysOfWeek.Contains(day.DayOfWeek))
                {
                    foundIteration++;
                    if (foundIteration == iteration)
                    {
                        returnMe = day;
                        break;
                    }
                }
            }

            return returnMe;
        }

        public static string SanitizeForJSON(string inputString)
        {
            List<char> allowedCharacters = new List<char>()
            {
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8','9','0','-','=','_','+','\'','"','/','?','!','@','#','$','%','^','&','*','(',')','.',',',';',':','\\','<','>',' '
            };
            
            StringBuilder returnMe = new StringBuilder();
            foreach (char c in inputString)
            {
                if (allowedCharacters.Contains(c))
                {
                    returnMe.Append(c);
                }
            }

            return returnMe.ToString().Replace("\"", "\\\"").Replace("<div></div>", string.Empty).Replace("/", "\\/");
        }

        public static string GetMonthName(int monthNum)
        {
            string returnMe = "Unknown";
            switch (monthNum)
            {
                case 1:
                    returnMe = "January";
                    break;
                case 2:
                    returnMe = "February";
                    break;
                case 3:
                    returnMe = "March";
                    break;
                case 4:
                    returnMe = "April";
                    break;
                case 5:
                    returnMe = "May";
                    break;
                case 6:
                    returnMe = "June";
                    break;
                case 7:
                    returnMe = "July";
                    break;
                case 8:
                    returnMe = "August";
                    break;
                case 9:
                    returnMe = "September";
                    break;
                case 10:
                    returnMe = "October";
                    break;
                case 11:
                    returnMe = "November";
                    break;
                case 12:
                    returnMe = "December";
                    break;
            }
            return returnMe;
        }

    }
}