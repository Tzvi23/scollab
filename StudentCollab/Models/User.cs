using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StudentCollab.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int rank { get; set; }
        public string institution { get; set; }
        public int? year { get; set; }
        public Boolean? EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Boolean? active { get; set; }
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int Likes { get; set; }
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int studentUnionRank { get; set; }


        public User()
        {
       
        }

        public User(User usr)
        {
            this.UserName = usr.UserName;
            this.Password = usr.Password;
            this.rank = usr.rank;
            this.institution = usr.institution;
            this.year = usr.year;
            this.EmailConfirmed = usr.EmailConfirmed;
            this.FirstName = usr.FirstName;
            this.LastName = usr.LastName;
            this.active = usr.active;
            this.Likes = usr.Likes;
            this.studentUnionRank = usr.studentUnionRank;

        }

        public static explicit operator bool(User v)
        {
            throw new NotImplementedException();
        }
    }
}