using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentCollab.Models;

namespace StudentCollab.Models
{
    public class Institution
    {
        [Key]
        public int InstitutionId { get; set; }
        public string InstName { get; set; }

        //public List<Department> Departments { get; set; }

    public Institution()
        {
            InstitutionId = 0;
            InstName = null;
        }

        public Institution(Institution inst)
        {
            this.InstitutionId = inst.InstitutionId;
            this.InstName = inst.InstName;
        }
    }
}