using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class FileManager
    {
        [Required]
        public HttpPostedFileBase FileToUpload { get; set; }
    }
}