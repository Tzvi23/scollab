using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace StudentCollab.Models
{
    public class Like
    {
        [Key]
        [Column(Order = 0)]
        public int commentId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int threadId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int usrId { get; set; }

        public Like()
        {

        }

        public Like(Like l)
        {
            commentId = l.commentId;
            threadId = l.threadId;
            usrId = l.usrId;
        }

    }

}