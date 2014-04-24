using LSKYDashboardDataCollector.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.CiscoWireless
{
    public partial class WirelessClients : System.Web.UI.Page
    {
        private static int LastAssociated = -1;
        private static int LastAuthenticated = -1;
        private static DateTime LastUpdated;
        private TimeSpan CacheLifetime = new TimeSpan(0, 10, 0);
        
        protected void Page_Load(object sender, EventArgs e)
        {
            int associatedClients = 0;
            int authenticatedClients = 0;
            DateTime lastChecked = DateTime.MinValue;
            bool UsedCache = false;

            if (
                (DateTime.Now.Subtract(LastUpdated) < CacheLifetime) &&
                (LastAssociated != -1) &&
                (LastAuthenticated != -1)
                )
            {
                // Use cache
                associatedClients = LastAssociated;
                authenticatedClients = LastAuthenticated;
                lastChecked = LastUpdated;
                UsedCache = true;
            }
            else
            {
                string FilePath = DDCConfiguration.GetCiscoWirelessLogPath();

                // Get the newest file from the share
                var directory = new DirectoryInfo(FilePath);
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

                // Parse the file
                StreamReader reader = File.OpenText(FilePath + @"\" + myFile);
                string line;
                int lineNum = 0;
                
                string lastCheckedString = string.Empty;

                string dateFormat = "MM/dd/yyyy HH:mm:ss \"GMT-06:00\"";

                while ((line = reader.ReadLine()) != null)
                {
                    lineNum++;

                    /* First two lines of this file are garbage */
                    if (!(lineNum <= 2))
                    {
                        string[] lineItems = line.Split(',');

                        if (lineItems.Length == 4)
                        {
                            lastCheckedString = lineItems[1];

                            DateTime parsedDate = DateTime.MinValue;

                            try
                            {
                                parsedDate = DateTime.ParseExact(lastCheckedString, dateFormat, null);
                            }
                            catch { }

                            if (parsedDate > lastChecked)
                            {
                                lastChecked = parsedDate;
                                associatedClients = int.Parse(lineItems[2]);
                                authenticatedClients = int.Parse(lineItems[3]);

                                // Refresh the cache
                                LastUpdated = DateTime.Now;
                                LastAssociated = associatedClients;
                                LastAuthenticated = authenticatedClients;
                            }
                        }
                    }
                }

            }

            /* Generate a JSON file */
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Wireless\": {\n");

            Response.Write("\"UsedCachedData\": \"" + UsedCache + "\",\n");
            Response.Write("\"LastChecked\": \"" + lastChecked.ToString() + "\",\n");
            Response.Write("\"Associated\": " + associatedClients + ",\n");
            Response.Write("\"Authenticated\": " + authenticatedClients + "\n");

            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();

        }
    }
}