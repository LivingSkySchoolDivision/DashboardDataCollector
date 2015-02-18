using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.General
{
    public partial class JSONTime : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"Year\": " + DateTime.Now.Year + ",\n");
            Response.Write("\"Month\": " + DateTime.Now.Month + ",\n");
            Response.Write("\"Day\": " + DateTime.Now.Day + ",\n");
            Response.Write("\"Hour\": " + DateTime.Now.Hour + ",\n");
            Response.Write("\"Minute\": " + DateTime.Now.Minute + ",\n");
            Response.Write("\"Second\": " + DateTime.Now.Second + ",\n");
            Response.Write("\"Millisecond\": " + DateTime.Now.Millisecond + "\n");
            Response.Write("}\n");
            Response.End();  
        }
    }
}