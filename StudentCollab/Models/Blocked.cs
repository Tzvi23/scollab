using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StudentCollab.Models
{
    public class Blocked
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        public int InsId { get; set; }

        public int DepId { get; set; }

        public int YearId { get; set; }

        public DateTime Bdate { get; set; }
    }
}