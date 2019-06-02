using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;


namespace StudentCollab.Controllers
{
    public class CommunicationController : Controller
    {
        // GET: Communication
        public ActionResult InboxPage(User usr)
        {
            List<Message> msg = new List<Message>();
            try
            {
                MessageDal mdal = new MessageDal();
                msg =
                (from x in mdal.Messages
                 where (x.reciverName == usr.UserName) && (x.subject != "Alert") && (x.senderName != "Union system") && (x.senderName != "Follow system")
                 select x).ToList<Message>();


            }

            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 0;
            return View(new User(usr));
        }

        public ActionResult SentPage(User usr)
        {
            List<Message> msg = new List<Message>();


            MessageDal mdal = new MessageDal();

            try
            {
                msg =
                (from x in mdal.Messages
                 where x.senderName == usr.UserName
                 select x).ToList<Message>();
            }
            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 1;
            return View("InboxPage", new User(usr));
        }
        
        public ActionResult AlertPage(User usr)
        {


            List<Message> msg = new List<Message>();
            MessageDal mdal = new MessageDal();

            try
            {
                msg =
                (from x in mdal.Messages
                 where x.reciverName == usr.UserName && x.subject == "Alert" && x.senderName == "AlertSys"
                 select x).ToList<Message>();
            }
            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 2;
            return View("InboxPage", new User(usr));
        }

        public ActionResult UnionPage(User usr)
        {

            List<Message> msg = new List<Message>();
            MessageDal mdal = new MessageDal();
            try
            {
                msg =
                (from x in mdal.Messages
                 where x.reciverName == usr.UserName && x.senderName == "Union system"
                 select x).ToList<Message>();
            }
            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 3;
            return View("InboxPage", new User(usr));
        }

        public ActionResult RecivedPage(User usr)
        {
            List<Message> msg = new List<Message>();
            try
            {
                MessageDal mdal = new MessageDal();
                msg =
                (from x in mdal.Messages
                 where (x.reciverName == usr.UserName) && (x.subject != "Alert") && (x.senderName != "Union system") && (x.senderName != "Follow system")
                 select x).ToList<Message>();


            }

            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 0;
            return View("InboxPage", new User(usr));
        }

        public ActionResult followPage(User usr)
        {
            List<Message> msg = new List<Message>();
            try
            {
                MessageDal mdal = new MessageDal();
                msg =
                (from x in mdal.Messages
                 where x.senderName == "Follow system"
                 select x).ToList<Message>();


            }

            catch
            {
                return View("Error", new User(usr));
            }

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 4;
            return View("InboxPage", new User(usr));
        }

        public ActionResult sendMsg(User usr)
        {

            return View(new User(usr));
            
        }

        public ActionResult deleteMsg(Int32 id)
        {
            List<Message> msg = new List<Message>();
            MessageDal mdal = new MessageDal();
            try
            {
                msg =
                (from x in mdal.Messages
                 where x.id == id
                 select x).ToList<Message>();
            }
            catch
            {
                User Eusr = new User((User)TempData["mdl"]);
                return View("Error", new User(Eusr));
            }

            Message ms = msg[0];
            mdal.Messages.Remove(ms);
            mdal.SaveChanges();

            User usr = new User((User)TempData["mdl"]);
            return RedirectToAction("InboxPage", usr);
        }

        public ActionResult viewMsg(Int32 id)
        {
            List<Message> msg = new List<Message>();
            try
            {
                MessageDal mdal = new MessageDal();
                msg =
                    (from x in mdal.Messages
                     where x.id == id
                     select x).ToList<Message>();

            }
            catch
            {
                User Eusr = new User((User)TempData["mdl"]);
                return RedirectToAction("InboxPage", Eusr);
            }

            TempData["mid"] = new Message(msg[0]);
            User usr = new User((User)TempData["mdl"]);
            return View(usr);

        }

        private void saveBrodMsg(User usr, int level, int brodId, string rec, string subj, string msgC)
        {
            List<int> idList = new List<int>();
            switch (level)
            {
                case 0:
                    /*
                    SyearDal sdal = new SyearDal();
                    List<Syear> syears =
                        (from x in sdal.Syears
                         where brodId == x.SyearId
                         select x).ToList<Syear>();
                    if (syears.Any())
                    {
                        UserDal dal = new UserDal();
                        List<User> users = 
                            (from x in dal.Users
                             where x.year == )
                    }*///continue after connecting user to yr
                    break;

                case 1:
                    break;

                case 2:
                    try
                    {
                        InstitutionDal idal = new InstitutionDal();
                        List<Institution> institutions =
                            (from x in idal.Institutions
                             where brodId == x.InstitutionId
                             select x).ToList<Institution>();
                        if (institutions.Any())
                        {
                            Institution insti = institutions[0];
                            UserDal dal = new UserDal();
                            List<User> users =
                                (from y in dal.Users
                                 where y.institution.ToLower() == insti.InstName.ToLower()
                                 select y).ToList<User>();

                            foreach (User x in users)
                            {
                                idList.Add(x.id);
                            }
                        }
                    }
                    catch
                    {
                        break;
                    }
                    
                    break;
            }
            if (idList.Any())
            {
                MessageDal mdal = new MessageDal();
                foreach (int id in idList)
                {
                    List<User> user = new List<User>();

                    try
                    {
                        UserDal dal = new UserDal();
                        user =
                        (from x in dal.Users
                         where x.id == id
                         select x).ToList<User>();
                    }
                    catch
                    {

                    }

                    if (user.Any())
                    {
                        Message ms = new Message()
                        {
                            date = DateTime.Now,
                            senderName = usr.UserName,
                            reciverName = user[0].UserName,
                            mag = msgC,
                            subject = subj
                        };

                        mdal.Messages.Add(ms);
                        mdal.SaveChanges();

                    }
                }
                
            }

        }

        public ActionResult follow(Int32 i)
        {
            ThreadDal tdal = new ThreadDal();
            CommentDal cdal = new CommentDal();
            FollowDal fdal = new FollowDal();
            List<Follow> follows = new List<Follow>();
            List<Comment> comments = new List<Comment>();
            List<Thread> threads = new List<Thread>();
            User ur = new User((User)TempData["urid"]);

            try
            {
                

                comments =
                (from x in cdal.Comments
                 where x.commentId == i
                 select x).ToList<Comment>();
                int on = comments[0].userId;
                int tid = comments[0].threadId;
                int fwr = ur.id;

                threads =
                    (from x in tdal.Threads
                     where x.ThreadId == tid
                     select x).ToList<Thread>();

                Follow f = new Follow()
                {
                    followOn = on,
                    follower = fwr

                };
                fdal.Follows.Add(f);
                fdal.SaveChanges();

                


            }
            catch
            {

            }

            Thread t = new Thread(threads[0]);

            return RedirectToAction("ContentPage", "MainPage", t);
            
        }

        public ActionResult unFollow(Int32 i)
        {
            ThreadDal tdal = new ThreadDal();
            CommentDal cdal = new CommentDal();
            FollowDal fdal = new FollowDal();
            List<Follow> follows = new List<Follow>();
            List<Comment> comments = new List<Comment>();
            List<Thread> threads = new List<Thread>();
            User ur = new User((User)TempData["urid"]);

            try
            {
                comments =
                (from x in cdal.Comments
                 where x.commentId == i
                 select x).ToList<Comment>();
                int on = comments[0].userId;
                int tid = comments[0].threadId;
                int fwr = ur.id;

                threads =
                    (from x in tdal.Threads
                     where x.ThreadId == tid
                     select x).ToList<Thread>();

                follows =
                    (from y in fdal.Follows
                     where y.followOn == @on && y.follower == fwr
                     select y).ToList<Follow>();

                fdal.Follows.Remove(follows[0]);
                fdal.SaveChanges();

            }

            catch
            {

            }
            
            Thread t = new Thread(threads[0]);

            return RedirectToAction("ContentPage", "MainPage", t);

        }

        public ActionResult saveMsg(User usr)
        {
            string rec;
            string subj;
            string msgC;
            try
            {
                rec = Request.Form["to"];
                subj = Request.Form["subject"];
                msgC = Request.Form["msgContent"];
            }
            catch
            {
                return View("Error", new User(usr));
            }
            int inst, dep, yr;
            try
            {
                inst = Int32.Parse(Request.Form["inst"]);
            }
            catch
            {
                inst = -1;
            }
            try
            {
                dep = Int32.Parse(Request.Form["dep"]);
            }
            catch
            {
                dep = -1;
            }
            try
            {
                yr = Int32.Parse(Request.Form["yr"]);
            }
            catch
            {
                yr = -1;
            }

            if(yr+dep+inst > -3)
            {
                if(usr.rank == 0)
                {
                    if(yr != -1)
                    {
                        saveBrodMsg(new User(usr), 0, yr, rec, subj, msgC);

                    }

                    else if(dep != -1)
                    {
                        saveBrodMsg(new User(usr), 1, dep, rec, subj, msgC);

                    }

                    else if (inst != -1)
                    {
                        saveBrodMsg(new User(usr), 2, inst, rec, subj, msgC);

                    }
                    return RedirectToAction("InboxPage", new User(usr));

                }
                
                if(usr.rank == 1)
                {
                    List<ManageConnection> manageConnections = new List<ManageConnection>();
                    if (yr != -1)
                    {
                        try
                        {
                            ManageConnectionDal mcdal = new ManageConnectionDal();
                            manageConnections =
                                (from x in mcdal.ManageConnections
                                 where x.managerId == usr.id && x.sYear == yr
                                 select x).ToList<ManageConnection>();
                        }
                        catch
                        {

                        }

                        if (manageConnections.Any())
                        {
                            saveBrodMsg(new User(usr), 0, yr, rec, subj, msgC);

                        }

                    }

                    else if (dep != -1)
                    {
                        try
                        {
                            ManageConnectionDal mcdal = new ManageConnectionDal();
                            manageConnections =
                                (from x in mcdal.ManageConnections
                                 where x.managerId == usr.id && x.department == dep
                                 select x).ToList<ManageConnection>();
                        }
                        catch
                        {

                        }
                        if (manageConnections.Any())
                        {
                            saveBrodMsg(new User(usr), 1, dep, rec, subj, msgC);

                        }

                    }

                    else if (inst != -1)
                    {
                        try
                        {
                            ManageConnectionDal mcdal = new ManageConnectionDal();
                            manageConnections =
                                (from x in mcdal.ManageConnections
                                 where x.managerId == usr.id && x.institution == inst
                                 select x).ToList<ManageConnection>();
                        }
                        catch
                        {

                        }

                        if (manageConnections.Any())
                        {
                            saveBrodMsg(new User(usr), 2, inst, rec, subj, msgC);

                        }

                    }
                    return RedirectToAction("InboxPage", new User(usr));

                }

            }

            MessageDal mdal = new MessageDal();
            Message ms = new Message()
            {
                date = DateTime.Now,
                senderName = usr.UserName,
                reciverName = rec,
                mag = msgC,
                subject = subj
            };

            List<User> user = new List<User>();
            try
            {
                UserDal dal = new UserDal();
                user =
                (from x in dal.Users
                 where x.UserName == usr.UserName
                 select x).ToList<User>();
            }
            catch
            {

            }
            

            if (user.Any())
            {
                
                mdal.Messages.Add(ms);
                mdal.SaveChanges();

            }

            return RedirectToAction("InboxPage", new User(usr));

        }
        
    }
}