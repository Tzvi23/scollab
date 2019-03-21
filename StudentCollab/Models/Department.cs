using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int InstitutionId { get; set; }

        //public List<Syear> Syears { get; set; }
    }
}