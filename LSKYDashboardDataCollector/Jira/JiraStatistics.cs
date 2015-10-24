using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Web;

namespace LSKYDashboardDataCollector.Jira
{
    public static class JiraStatistics
    {
        public static double TicketsPerDay(List<JiraIssue> theseIssues, DateTime dateFrom, DateTime dateTo)
        {
            int returnMe = 0;
            
            List<int> ticketsCountsByDay = new List<int>();
            foreach (DateTime date in Helpers.GetEachDayBetween(dateFrom, dateTo))
            {
                List<JiraIssue> ticketsToday = theseIssues.Where(i => i.DateCreated >= date && i.DateCreated <= date.AddDays(1)).ToList();
                int ticketCountToday = ticketsToday.Count;

                if (ticketCountToday > 0)
                {
                    ticketsCountsByDay.Add(ticketCountToday);
                }
                else
                {
                    if ((date.DayOfWeek != DayOfWeek.Saturday) && (date.DayOfWeek != DayOfWeek.Sunday))
                    {
                        // Add a zero for the day, but not if it's a weekend
                        ticketsCountsByDay.Add(0);
                    }
                }
            }


            // Average the list together
            return ticketsCountsByDay.Average();
        }


        [Obsolete("I don't think the math on this checks out")]
        private static double TicketsPerDay(List<JiraIssue> theseIssues)
        {
            int returnMe = 0;

            // Get the date range for the listed issues
            // Step through each day inbetween and record the tickets on that day
            // Don't count weekends

            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;
            foreach (JiraIssue issue in theseIssues)
            {
                if (issue.DateCreated < minDate)
                {
                    minDate = issue.DateCreated;
                }

                if (issue.DateCreated > maxDate)
                {
                    maxDate = issue.DateCreated;
                }
            }

            List<int> ticketsCountsByDay = new List<int>();
            foreach (DateTime date in Helpers.GetEachDayBetween(minDate, maxDate))
            {
                List<JiraIssue> ticketsToday = theseIssues.Where(i => i.DateCreated >= date && i.DateCreated <= date.AddDays(1)).ToList();
                int ticketCountToday = ticketsToday.Count;

                if (ticketCountToday > 0)
                {
                    ticketsCountsByDay.Add(ticketCountToday);
                }
                else
                {
                    if ((date.DayOfWeek != DayOfWeek.Saturday) && (date.DayOfWeek != DayOfWeek.Sunday))
                    {
                        // Add a zero for the day, but not if it's a weekend
                        ticketsCountsByDay.Add(0);
                    }
                }
            }


            // Average the list together
            return ticketsCountsByDay.Average();
        }
    }
}