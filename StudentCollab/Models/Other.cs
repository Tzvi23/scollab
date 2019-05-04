using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StudentCollab.Models
{
    public class Other
    {
        [Key]
        public int Id { get; set; }
        public string Val { get; set; }

        public Other() { }
        public Other(Other obj)
        {
            Id = obj.Id;
            Val = obj.Val;
        }
    }
}