using LSKYDashboardDataCollector.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.iBoss
{
    public partial class TopBandwidthUsers : System.Web.UI.Page
    {
        // Matches certain usernames to more descriptive strings
        private string FormatUsername(string givenUsername)
        {
            if (givenUsername == "*10.177.199.253")
            {
                return "Cisco VPN user(s)";
            }

            if (givenUsername == "*10.177.196.208")
            {
                return "mail.lskysd.ca";
            }

            if (givenUsername == "*10.177.194.224")
            {
                return "System Center";
            }

            if (
                (givenUsername == "*10.177.196.166") ||
                (givenUsername == "*10.177.196.167") ||
                (givenUsername == "*10.177.196.168") ||
                (givenUsername == "*10.177.196.169") 
                )
            {
                return "Web0";
            }

            return givenUsername;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            // Get username and password from configuration
            string iBossURL = DDCConfiguration.GetiBossURL();
            string iBossUsername = DDCConfiguration.GetiBossUsername();
            string iBossPassword = DDCConfiguration.GetiBossPassword();

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{");

            try
            {
                using (iBossConnection iBoss = new iBossConnection(iBossURL, iBossUsername, iBossPassword))
                {
                    List<iBossBandwidthUser> BandwidthUsers = iBoss.GetBandwidthConsumers().OrderByDescending(c => c.PacketCount).ToList<iBossBandwidthUser>();
                    
                    Response.Write("\"Count\": " + BandwidthUsers.Count + ",");
                    Response.Write("\"TopBandwidthUsers\" : [");

                    for (int x = 0; x < BandwidthUsers.Count; x++)
                    {
                        iBossBandwidthUser user = BandwidthUsers[x];

                        Response.Write("{");
                        Response.Write("\"Name\" : \"" + FormatUsername(user.Username) + "\",");
                        Response.Write("\"Bytes\" : \"" + user.TotalBytes + "\",");
                        Response.Write("\"Packets\" : \"" + user.PacketCount + "\"");
                        Response.Write("}");
                        if (x < BandwidthUsers.Count - 1)
                        {
                            Response.Write(",");
                        }

                    }

                    Response.Write("]");

                }
            }
            catch (Exception ex)
            {
                Response.Write(CommonJSONParts.Error(ex.Message));
            }

            Response.Write("}");
            Response.End();
        }
    }
}