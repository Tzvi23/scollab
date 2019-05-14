using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StudentCollab.Models
{
    public class ManagerLog
    {
        [Key]
        public int id { get; set; }
        public int userID { get; set; }
        public string userLog { get; set; }
        public string logDate { get; set; }
    }
}