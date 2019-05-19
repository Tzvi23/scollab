using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class commentCounter
    {

        [Key]
        public int id { get; set; }
        public int managerId { get; set; }
        public int messageCounter { get; set; }
        public DateTime date { get; set; }

    }
}