using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class Thread
    {
        [Key]
        public int ThreadId { get; set; }
        public string ThreadName { get; set; }
        public int SyearId { get; set; }
    }
}