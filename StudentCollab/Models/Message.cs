using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{

    public class Message
    {
        [Key]
        public System.Int32 id { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public DateTime date { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String senderName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String reciverName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String mag { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String subject { get; set; }

        public Message()
        {

        }

        public Message(Message m)
        {
            id = m.id;
            date = m.date;
            senderName = m.senderName;
            reciverName = m.reciverName;
            mag = m.mag;
            subject = m.subject;

        }

    }

}