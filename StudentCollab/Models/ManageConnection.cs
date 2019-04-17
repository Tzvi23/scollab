using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StudentCollab.Models
{
    public class ManageConnection
    {
        [Key]
        public int id { get; set; }
        public int managerId { get; set; }
        public int sYear { get; set; }
    }
}