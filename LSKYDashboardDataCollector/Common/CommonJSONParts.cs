using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LSKYDashboardDataCollector.Common
{
    public static class CommonJSONParts
    {
        public static string Error(string errorString)
        {
            StringBuilder ErrorString = new StringBuilder();

            ErrorString.Append("{");
            ErrorString.Append("\"Error\": ");
            ErrorString.Append("\"");
            ErrorString.Append(errorString);
            ErrorString.Append("\"");
            ErrorString.Append("}");

            return ErrorString.ToString();
        }
    }
}