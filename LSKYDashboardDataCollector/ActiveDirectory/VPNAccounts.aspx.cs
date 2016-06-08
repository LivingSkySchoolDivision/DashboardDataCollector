using System;
using System.Collections.Generic;
using System.Linq;
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
            List<ADUser> groupMembers = repository.GetGroupMembers("Cisco VPN");
            
            foreach (ADUser user in groupMembers)
            {
                Response.Write("<BR>" + user);
            }

            // Get the AD objects so we can see if they are enabled or disabled
            
        }
    }
}