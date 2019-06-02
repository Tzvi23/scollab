using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;

namespace StudentCollab.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Block(User usr)
        {   
            return View(new User(usr));
        }

        public ActionResult BlockUser()
        {
            User cur = new User();
            cur = (User)TempData["CurrentManager"];
            string UserName = Request.Form["username"];
            int InstId;
            int DepId;
            int YearId;
            DateTime date = DateTime.Parse(Request.Form["date"]);
            try
            {
                 InstId = Int32.Parse(Request.Form["Institution"]);
            }
            catch
            {
                InstId = -1;
            }
            try
            {
                DepId = Int32.Parse(Request.Form["Department"]);
            }
            catch
            {
                DepId = -1;
            }
            try
            {
                YearId = Int32.Parse(Request.Form["Year"]);
            }
            catch
            {
                YearId = -1;
            }
            List<ManageConnection> manageConnections = new List<ManageConnection>();
            try
            {
                ManageConnectionDal mcdal = new ManageConnectionDal();
                manageConnections =
                    (from x in mcdal.ManageConnections
                     where x.managerId == cur.id
                     select x).ToList<ManageConnection>();
            }
            catch
            {

            }
            
            
            foreach(ManageConnection mc in manageConnections)
            {
                if (InstId != -1)
                {
                    if(mc.institution == InstId)
                    {
                        BlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                }
                if (DepId != -1)
                {   
                    if(mc.department == DepId)
                    {
                        BlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        List<Department> departments = new List<Department>();
                        try
                        {
                            DepartmentDal dp = new DepartmentDal();
                            departments =
                                (from x in dp.Departments
                                 where x.DepartmentId == DepId
                                 select x).ToList<Department>();
                        }

                        catch
                        {
                            
                        }
                        
                        if(mc.institution == departments[0].InstitutionId)
                        {
                            
                            BlockFromDB(UserName, date, InstId, DepId, YearId);
                            return View("Block", cur);
                        }
                    }
                }
                if (YearId != -1)
                {
                    if(mc.sYear == YearId)
                    {
                        BlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        List<Syear> syears = new List<Syear>();
                        List<Department> departments = new List<Department>();
                        SyearDal yearid = new SyearDal();
                        DepartmentDal dp = new DepartmentDal();

                        try
                        {
                            syears =
                            (from x in yearid.Syears
                             where x.SyearId == YearId
                             select x).ToList<Syear>();

                            departments =
                                (from x in dp.Departments
                                 where x.DepartmentId == syears[0].DepartmentId
                                 select x).ToList<Department>();
                        }

                        catch
                        {

                        }
                        
                        
                        if(mc.institution == departments[0].InstitutionId || mc.department == syears[0].DepartmentId)
                        {
                            BlockFromDB(UserName, date, InstId, DepId, YearId);
                            return View("Block", cur);
                        }

                    }
                }
            }
            
            return View("Failed", cur);
        }

        public ActionResult UnBlockUser()
        {
            User cur = new User();
            cur = (User)TempData["CurrentManager"];
            string UserName = Request.Form["username"];
            int InstId;
            int DepId;
            int YearId;
            try
            {
                InstId = Int32.Parse(Request.Form["Institution"]);
            }
            catch
            {
                InstId = -1;
            }
            try
            {
                DepId = Int32.Parse(Request.Form["Department"]);
            }
            catch
            {
                DepId = -1;
            }
            try
            {
                YearId = Int32.Parse(Request.Form["Year"]);
            }
            catch
            {
                YearId = -1;
            }

            ManageConnectionDal mcdal = new ManageConnectionDal();
            List<ManageConnection> manageConnections = new List<ManageConnection>();

            try
            {
                manageConnections =
                (from x in mcdal.ManageConnections
                 where x.managerId == cur.id
                 select x).ToList<ManageConnection>();
            }
            catch
            {

            }
            

            foreach (ManageConnection mc in manageConnections)
            {
                if (InstId != -1)
                {
                    if (mc.institution == InstId)
                    {
                        UnBlockFromDB(UserName, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                }
                if (DepId != -1)
                {
                    if (mc.department == DepId)
                    {
                        UnBlockFromDB(UserName, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments = new List<Department>();
                        try
                        {
                            departments =
                            (from x in dp.Departments
                             where x.DepartmentId == DepId
                             select x).ToList<Department>();
                        }

                        catch
                        {

                        }

                        if (mc.institution == departments[0].InstitutionId)
                        {
                            UnBlockFromDB(UserName, InstId, DepId, YearId);
                            return View("Block", cur);
                        }
                    }
                }
                if (YearId != -1)
                {
                    if (mc.sYear == YearId)
                    {
                        UnBlockFromDB(UserName, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        SyearDal yearid = new SyearDal();
                        List<Syear> syears = new List<Syear>();
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments = new List<Department>();


                        try
                        {
                            syears =
                           (from x in yearid.Syears
                            where x.SyearId == YearId
                            select x).ToList<Syear>();

                            departments =
                                (from x in dp.Departments
                                 where x.DepartmentId == syears[0].DepartmentId
                                 select x).ToList<Department>();
                        }
                        catch
                        {

                        }

                        if (mc.institution == departments[0].InstitutionId || mc.department == syears[0].DepartmentId)
                        {
                            UnBlockFromDB(UserName, InstId, DepId, YearId);
                            return View("Block", cur);
                        }

                    }
                }
            }

            return View("Failed", cur);
        }
        private void BlockFromDB(string uName, DateTime date, int inst, int dep, int yeId)
        {
            Blocked blocked = new Blocked()
            {
                UserName = uName,
                InsId = inst,
                DepId = dep,
                YearId = yeId,
                Bdate = date
            };
            BlockedDal Bdal = new BlockedDal();
            Bdal.Blockeds.Add(blocked);
            Bdal.SaveChanges();
        }

        private void UnBlockFromDB(string uName, int inst, int dep, int yeId)
        {
            List<Blocked> bd = new List<Blocked>();
            BlockedDal bdal = new BlockedDal();
            try
            {
                bd =
            (from x in bdal.Blockeds
             where x.UserName == uName
             select x).ToList<Blocked>();
            }
            catch
            {

            }

            foreach (Blocked b in bd)
            {
                if (b.InsId == inst && b.DepId == dep && b.YearId == yeId && b.Bdate > DateTime.Today)
                {
                    b.InsId = -1;
                    b.DepId = -1;
                    b.YearId = -1;

                    bdal.SaveChanges();
                }
                
            }
        }

        public ActionResult ThreadsActivity(User usr)
        {
            return RedirectToAction("ViewThreadsActivity");
        }

        public ActionResult ViewThreadsActivity()
        {
            User cur = getUser();
            //cur = (User)TempData["CurrentManager"];
            int countComment = 0;

            List<ManageConnection> manageConnections = new List<ManageConnection>();
            ManageConnectionDal mcdal = new ManageConnectionDal();
            try
            {
                manageConnections =
                (from x in mcdal.ManageConnections
                 where x.managerId == cur.id
                 select x).ToList<ManageConnection>();
            }
            catch
            {

            }


            foreach (ManageConnection mc in manageConnections)
            {
                if (mc.institution != -1)
                {
                    DepartmentDal dp = new DepartmentDal();
                    SyearDal yearid = new SyearDal();
                    ThreadDal threadid = new ThreadDal();
                    CommentDal commentid = new CommentDal();
                    List<Department> departments = new List<Department>();
                    List<Syear> syears = new List<Syear>();
                    List<Thread> threads = new List<Thread>();
                    List<Comment> comments = new List<Comment>();

                    departments =
                        (from x in dp.Departments
                         where x.InstitutionId == mc.institution
                         select x).ToList<Department>();

                    foreach (Department dep in departments) { 
                    syears =
                        (from x in yearid.Syears
                         where x.DepartmentId == dep.DepartmentId
                         select x).ToList<Syear>();
                        foreach( Syear yearsC in syears)
                        {
                            threads =
                                (from x in threadid.Threads
                                 where x.SyearId == yearsC.SyearId
                                 select x).ToList<Thread>();

                            foreach(Thread th in threads)
                            {
                                comments =
                                    (from x in commentid.Comments
                                     where x.threadId == th.ThreadId
                                     select x).ToList<Comment>();

                                countComment = countComment + comments.Count();
                            }

                        }
                        
                    }


                }
                if(mc.department != -1)
                {
                    SyearDal yearid = new SyearDal();
                    ThreadDal threadid = new ThreadDal();
                    CommentDal commentid = new CommentDal();
                    List<Syear> syears = new List<Syear>();
                    List<Thread> threads = new List<Thread>();
                    List<Comment> comments = new List<Comment>();

                    try
                    {
                        syears =
                       (from x in yearid.Syears
                        where x.DepartmentId == mc.department
                        select x).ToList<Syear>();
                    }
                    catch
                    {

                    }
                   

                    foreach (Syear yearsC in syears)
                    {
                        try
                        {
                            threads =
                            (from x in threadid.Threads
                             where x.SyearId == yearsC.SyearId
                             select x).ToList<Thread>();
                        }
                        catch
                        {

                        }
                        

                        foreach (Thread th in threads)
                        {
                            try
                            {
                                comments =
                                (from x in commentid.Comments
                                 where x.threadId == th.ThreadId
                                 select x).ToList<Comment>();
                            }
                            catch
                            {

                            }
                            

                            countComment = countComment + comments.Count();

                        }
                    }
                }
                if (mc.sYear != -1)
                {
                    ThreadDal threadid = new ThreadDal();
                    CommentDal commentid = new CommentDal();
                    List<Thread> threads = new List<Thread>();
                    List<Comment> comments = new List<Comment>();

                    try
                    {
                        threads =
                        (from x in threadid.Threads
                         where x.SyearId == mc.sYear
                         select x).ToList<Thread>();
                    }
                    catch
                    {

                    }
                    

                    foreach (Thread th in threads)
                    {
                        try
                        {
                            comments =
                            (from x in commentid.Comments
                             where x.threadId == th.ThreadId
                             select x).ToList<Comment>();
                        }
                        catch
                        {

                        }
                        

                        countComment = countComment + comments.Count();

                    }
                }
            }

            using (commentCounterDal dal = new commentCounterDal())
            {
                commentCounter newCounter = new commentCounter()
                {
                    managerId = cur.id,
                    messageCounter = countComment,
                    date = DateTime.Today
                };

                dal.commentCounters.Add(newCounter);
                dal.SaveChanges();
            }
            commentCounterDal cmpCount = new commentCounterDal();
            List<commentCounter> commentCounters = new List<commentCounter>();

            try
            {
                commentCounters =
                (from x in cmpCount.commentCounters
                 where x.managerId == cur.id
                 select x).ToList<commentCounter>();
            }
            catch
            {

            }
            
            ViewBag.counters = commentCounters;
            return View("ThreadsActivity", cur);
        }

        public User getUser()
        {
            User usrid = ((User)TempData["CurrentUser"]);

            // ##### Get Users #####
            UserDal udal = new UserDal();
            List<User> usr = new List<User>();

            try
            {
                usr =
            (from x in udal.Users
             where x.UserName == usrid.UserName
             select x).ToList<User>();
            }
            catch
            {

            }
            

            User cur = new User()
            {
                UserName = "None"
            };
            if (TempData["CurrentUser"] != null)
            {
                cur = new User((User)TempData["CurrentUser"]);
                TempData["CurrentUser"] = cur;
            }

            return usr[0];
        }
    }
}