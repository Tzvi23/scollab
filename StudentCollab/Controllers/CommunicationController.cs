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
            
            MessageDal mdal = new MessageDal();
            List<Message> msg =
            (from x in mdal.Messages
             where x.reciverName == usr.UserName
             select x).ToList<Message>();

            ViewBag.MsgDB = msg;
            

            TempData["inboxFlag"] = 0;
            return View(new User(usr));
        }

        public ActionResult SentPage(User usr)
        {

            MessageDal mdal = new MessageDal();
            List<Message> msg =
            (from x in mdal.Messages
             where x.senderName == usr.UserName
             select x).ToList<Message>();

            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 1;
            return View("InboxPage", new User(usr));
        }

        public ActionResult RecivedPage(User usr)
        {

            MessageDal mdal = new MessageDal();
            List<Message> msg =
            (from x in mdal.Messages
             where x.reciverName == usr.UserName
             select x).ToList<Message>();
            
            ViewBag.MsgDB = msg;

            TempData["inboxFlag"] = 0;
            return View("InboxPage", new User(usr));
        }

        public ActionResult sendMsg(User usr)
        {

            return View(new User(usr));
            
        }

        public ActionResult viewMsg(Int32 id)
        {
            MessageDal mdal = new MessageDal();
            List<Message> msg =
                (from x in mdal.Messages
                 where x.id == id
                 select x).ToList<Message>();

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

                        foreach(User x in users)
                        {
                            idList.Add(x.id);
                        }
                    }
                    break;
            }
            if (idList.Any())
            {
                MessageDal mdal = new MessageDal();
                foreach (int id in idList)
                {

                    UserDal dal = new UserDal();
                    List<User> user =
                    (from x in dal.Users
                     where x.id == id
                     select x).ToList<User>();

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

        public ActionResult saveMsg(User usr)
        {

            string rec = Request.Form["to"];
            string subj = Request.Form["subject"];
            string msgC = Request.Form["msgContent"];
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
                    if (yr != -1)
                    {
                        ManageConnectionDal mcdal = new ManageConnectionDal();
                        List<ManageConnection> manageConnections =
                            (from x in mcdal.ManageConnections
                             where x.managerId == usr.id && x.sYear == yr
                             select x).ToList<ManageConnection>();
                        if (manageConnections.Any())
                        {
                            saveBrodMsg(new User(usr), 0, yr, rec, subj, msgC);

                        }

                    }

                    else if (dep != -1)
                    {
                        ManageConnectionDal mcdal = new ManageConnectionDal();
                        List<ManageConnection> manageConnections =
                            (from x in mcdal.ManageConnections
                             where x.managerId == usr.id && x.department == dep
                             select x).ToList<ManageConnection>();
                        if (manageConnections.Any())
                        {
                            saveBrodMsg(new User(usr), 1, dep, rec, subj, msgC);

                        }

                    }

                    else if (inst != -1)
                    {
                        ManageConnectionDal mcdal = new ManageConnectionDal();
                        List<ManageConnection> manageConnections =
                            (from x in mcdal.ManageConnections
                             where x.managerId == usr.id && x.institution == inst
                             select x).ToList<ManageConnection>();
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

            UserDal dal = new UserDal();
            List<User> user =
            (from x in dal.Users
             where x.UserName == usr.UserName
             select x).ToList<User>();

            if (user.Any())
            {
                
                mdal.Messages.Add(ms);
                mdal.SaveChanges();

            }

            return RedirectToAction("InboxPage", new User(usr));

        }
        
    }
}