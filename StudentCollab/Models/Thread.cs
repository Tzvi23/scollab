using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class Thread
    {
        [Key]
        public int ThreadId { get; set; }
        public string ThreadName { get; set; }
        public int SyearId { get; set; }
        public string ThreadType { get; set; }
        public int? OwnerId { get; set; }
        public Boolean? Solved { get; set; }
        public Boolean? Locked { get; set; }
        public Boolean? Pinned { get; set; }

        public Thread(Thread t)
        {
            ThreadId = t.ThreadId;
            ThreadName = t.ThreadName;
            SyearId = t.SyearId;
            ThreadType = t.ThreadType;
            OwnerId = t.OwnerId;
            Solved = t.Solved;
            Locked = t.Locked;
            Pinned = t.Pinned;
        }

        public Thread()
        {

        }

    }

    //public enum tType
    //{
    //    Question,
    //    Publication,
    //    Request,
    //    Exam
    //}
}