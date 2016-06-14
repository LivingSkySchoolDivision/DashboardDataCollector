using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LSKYDashboardDataCollector.FileSystem
{
    public partial class PhotoGallery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get photo gallery name from querystring
            string galleryName = Request.QueryString["gallery"];

            List<string> photoFileNames = new List<string>();

            if (galleryName != null)
            {
                if (!string.IsNullOrEmpty(galleryName))
                {
                    PhotoDirectoryRepository repository = new PhotoDirectoryRepository(galleryName);
                    photoFileNames = repository.FileNames();
                }
            }
            
            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n");
            Response.Write("\"TotalPhotos\": " + photoFileNames.Count + ",\n");
            Response.Write("\"Photos\": [\n");
            
            for (int x = 0; x < photoFileNames.Count;x++)
            {
                string fileName = photoFileNames[x];
                Response.Write("\"" + fileName + "\"");
                if (x + 1 < photoFileNames.Count)
                {
                    Response.Write(",");
                }
                Response.Write("\n");
            }
            Response.Write("]\n");
            Response.Write("}\n");
            Response.End(); 

        }
    }
}