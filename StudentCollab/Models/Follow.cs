using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace StudentCollab.Models
{
    public class Follow
    {
        [Key]
        public int id { get; set; }
        public int followOn { get; set; }
        public int follower { get; set; }

        public Follow()
        {

        }

        public Follow(Follow f)
        {
            id = f.id;
            followOn = f.followOn;
            follower = f.follower;
        }
    }
}