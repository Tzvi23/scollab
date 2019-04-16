using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Models
{
    public class Syear
    {
        [Key]
        public int SyearId { get; set; }
        public string SyearName { get; set; }
        public int DepartmentId { get; set; }

        public Syear()
        {

        }

        public Syear(Syear syr)
        {
            SyearId = syr.SyearId;
            SyearName = syr.SyearName;
            DepartmentId = syr.DepartmentId;
        }

    }

}