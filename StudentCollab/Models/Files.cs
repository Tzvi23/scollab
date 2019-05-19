using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentCollab.Models;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace StudentCollab.Models
{
    public class Files
    {
        [Key]
        public int UploadNum { get; set; }
        public string UploaderName { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public Boolean Active { get; set; }
    }
}