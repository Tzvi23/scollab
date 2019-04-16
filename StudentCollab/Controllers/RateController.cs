using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;

namespace StudentCollab.Controllers
{
    public class RateController : Controller
    {
        // GET: Rate
        public ActionResult Index()
        {
            return View();
        }

        private void refreshLikes(int uid, bool up)
        {

            UserDal dal = new UserDal();
            List<User> Users =
            (from x in dal.Users
             where x.id == uid
             select x).ToList<User>();
            if (up)
            {
                Users[0].Likes++;
            }
            else
            {
                Users[0].Likes--;
            }
            dal.SaveChanges();
        }

        public ActionResult addLike(String i)
        {
            
            TempData["canLike"] = true;
            Thread trd = (Thread)TempData["CurrentThread"];
            int id = (int)TempData["CurrentId" + i];
            Comment cmt = (Comment)TempData["CurrentCmt" + i];
            LikeDal ldal = new LikeDal();
            List<Like> lk =
            (from x in ldal.Likes
             where x.threadId == trd.ThreadId && x.commentId == cmt.commentId && x.usrId == id
             select x).ToList<Like>();
            if (lk.Any())
            {
               
                TempData["canLike"] = 0;

                ldal.Likes.Remove(lk[0]);
                ldal.SaveChanges();

                UserDal dal = new UserDal();
                List<User> Users =
                (from x in dal.Users
                 where x.id == id
                 select x).ToList<User>();

                int powRate = (Users[0].Likes / 10000) + 1;
                

                CommentDal cdal = new CommentDal();
                List<Comment> com =
                (from x in cdal.Comments
                 where x.threadId == trd.ThreadId && x.commentId == cmt.commentId
                 select x).ToList<Comment>();

                for(int idx = 0; idx < powRate; idx++)
                {
                    com[0].rank--;
                    refreshLikes(cmt.userId, false);
                }
                cdal.SaveChanges();
                

            }
            else
            {
                if (!canLike(id))
                {
                    TempData["canLike"] = -1;
                }
                else
                {
                    TempData["canLike"] = 0;

                    Like tmplk = new Like()
                    {
                        commentId = cmt.commentId,
                        threadId = trd.ThreadId,
                        usrId = id

                    };
                    ldal.Likes.Add(tmplk);
                    ldal.SaveChanges();

                    UserDal dal = new UserDal();
                    List<User> Users =
                    (from x in dal.Users
                     where x.id == id
                     select x).ToList<User>();

                    int powRate = (Users[0].Likes / 10000) + 1;

                    CommentDal cdal = new CommentDal();
                    List<Comment> com =
                    (from x in cdal.Comments
                     where x.threadId == trd.ThreadId && x.commentId == cmt.commentId
                     select x).ToList<Comment>();

                    for (int idx = 0; idx < powRate; idx++)
                    {
                        com[0].rank++;
                        refreshLikes(cmt.userId, true);
                    }
                    cdal.SaveChanges();
                }
            }
            return RedirectToAction("ContentPage", "MainPage", trd);
        }
        //
        private bool canLike(int id)
        {
            UserDal dal = new UserDal();
            List<User> Users =
            (from x in dal.Users
             where x.id == id
             select x).ToList<User>();
            if (Users[0].Likes >= 100)
            {
                return true;
            }

            return false;

            throw new NotImplementedException();

        }
    }
}