using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentCollab.Models
{
    public class Comment
    {

        [Key]
        public int commentId { get; set; }
        public string commentContent { get; set; }
        public int rank { get; set; }
        public int userId { get; set; }
        public int threadId { get; set; }
        public Boolean? ans { get; set; }

        public Comment()
        {

        }

        public Comment(Comment cmt)
        {
            commentId = cmt.commentId;
            commentContent = cmt.commentContent.ToString();
            rank = cmt.rank;
            userId = cmt.userId;
            threadId = cmt.threadId;
            ans = cmt.ans;
            
        }

    }
}