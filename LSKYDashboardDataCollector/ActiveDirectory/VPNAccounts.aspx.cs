using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace LSKYDashboardDataCollector.ActiveDirectory
{
    public partial class VPNAccounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the members of the AD VPN group
            
            ActiveDirectoryRepository repository = new ActiveDirectoryRepository("lskysd.ca");
            List<ADUser> groupMembers = repository.GetAllDialInUsers();
            
            foreach (ADUser user in groupMembers)
            {
                Response.Write("<BR>" + user);
            }

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";

            Response.Write("{\n\"AllVPNUsers\": ");
            Response.Write("[");
            for (int x = 0; x < groupMembers.Count; x++)
            {
                Response.Write(ADUserJSON(groupMembers[x]));
                if (x + 1 < groupMembers.Count)
                {
                    Response.Write(", ");
                }
            }
            Response.Write("]\n");
            Response.Write("}\n");
            
            // List all users
            // List enabled users
            // List disabled users


            Response.End();
        }

        private string ADUserJSON(ADUser user)
        {
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append("{ ");

            returnMe.Append("\"givenName\": \"" + user.GivenName + "\"");
            returnMe.Append(", \n");
            returnMe.Append("\"sn\": \"" + user.SN + "\"");
            returnMe.Append(", \n");
            returnMe.Append("\"sAMAccountName\": \"" + user.sAMAccountName + "\"");
            returnMe.Append(", \n");
            returnMe.Append("\"description\": \"" + user.description + "\"");
            returnMe.Append(", \n");
            returnMe.Append("\"comment\": \"" + user.comment + "\"");
            returnMe.Append(", \n");
            returnMe.Append("\"Enabled\": \"" + user.IsEnabled.ToString().ToUpper() + "\"");
            returnMe.Append(" \n");

            returnMe.Append("} ");

            return returnMe.ToString();
        }
    }
}