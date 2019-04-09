using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class Content
    {

        [Key]
        public int contentId { get; set; }
        public string threadName { get; set; }
        public string threadContent { get; set; }
        public int threadId { get; set; }

    }
}