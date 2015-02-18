using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.General
{
    public partial class JSONClientInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Client\": {");
            Response.Write(" \"IP\" : \"" + Request.ServerVariables["REMOTE_ADDR"] + "\"");
            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();
        }
    }
}