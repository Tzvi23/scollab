﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;

namespace StudentCollab.Controllers
{
    public class MainPageController : Controller
    {
        // GET: MainPage
        public ActionResult MainPage(User usr)
        {
            User cur = new User()
            {
                UserName = "None"
            };

            if (TempData["CurrentUser"] == null)
            {
                cur = new User(usr);
                TempData["CurrentUser"] = usr;
                
            }
            else
            {
                if (TempData["CurrentUser"] != null)
                {
                    cur = new User((User)TempData["CurrentUser"]);
                    TempData["CurrentUser"] = cur;
                }
            }
            

            InstitutionDal dal = new InstitutionDal();
            List<Institution> Inst =
            (from x in dal.Institutions
             select x).ToList<Institution>();

            ViewBag.InstutionsDB = Inst;

            return View("MainPage",cur);
        }

        public ActionResult DepartmentsPage(Institution inst)
        {
            if (inst != null)
            {
                DepartmentDal dal = new DepartmentDal();
                List<Department> Dep =
                (from x in dal.Departments
                 where x.InstitutionId == inst.InstitutionId
                 select x).ToList<Department>();

                ViewBag.DepartmentsDB = Dep;
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
                

            return View("DepartmentsPage", cur);
        }

        public ActionResult SyearsPage(Department dep)
        {
            if (dep != null)
            {
                SyearDal dal = new SyearDal();
                List<Syear> year =
                (from x in dal.Syears
                 where x.DepartmentId == dep.DepartmentId
                 select x).ToList<Syear>();

                ViewBag.SyearDB = year;
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
            return View(cur);
        }

        public ActionResult ThreadsPage(Syear year)
        {
            if (year != null)
            {
                ThreadDal dal = new ThreadDal();
                List<Thread> trd =
                (from x in dal.Threads
                 where x.SyearId == year.SyearId
                 select x).ToList<Thread>();

                ViewBag.ThreadDB = trd;
            }

            TempData["year"] = year;

            User cur = getUser();
            return View(cur);
        }

        public ActionResult ContentPage(Thread thread)
        {
            if (thread != null)
            {
                // ##### Get Thread Name + Contents #####
                ContentDal dal = new ContentDal();
                List<Content> cont =
                (from x in dal.Contents
                 where x.threadId == thread.ThreadId
                 select x).ToList<Content>();

                ViewBag.ContentDb = cont;

                // ##### Get Comments #####
                CommentDal cdal = new CommentDal();
                List<Comment> com =
                (from x in cdal.Comments
                    where x.threadId == thread.ThreadId
                    select x).ToList<Comment>();

                ViewBag.CommentDb = com;

                // ##### Get Users #####
                UserDal udal = new UserDal();
                List<User> usr =
                (from x in udal.Users
                 select x).ToList<User>();

                ViewBag.UsersDb = usr;

                TempData["CurrentThread"] = new Thread(thread);
            }

            User cur = getUser();

            return View(cur);
        }

        public ActionResult chooseAnswer(Thread ctrd)
        {

            string choice = Request.Form["ansChoice"];
            int id = Int32.Parse(choice);
            using (CommentDal dl = new CommentDal())
            {
                List<Comment> answers =
                (from x in dl.Comments
                 where x.ans == true && x.threadId == ctrd.ThreadId
                 select x).ToList<Comment>();

                if (answers.Count != 0 && answers[0] != null)
                {
                    answers[0].ans = false;
                }

                

                Comment result = dl.Comments.SingleOrDefault(b => b.commentId == id);
                if (result != null)
                {
                    result.ans = true;
                    dl.SaveChanges();

                    using (ThreadDal db = new ThreadDal())
                    {
                        Thread trd = db.Threads.SingleOrDefault(b => b.ThreadId == result.threadId);
                        return RedirectToAction("ContentPage", trd);
                    }
                }
            }
            return RedirectToAction("ContentPage");


        }

        public ActionResult clearSelection(Thread ctrd)
        {
            using (CommentDal dl = new CommentDal())
            {
                List<Comment> answers =
                (from x in dl.Comments
                 where x.ans == true && x.threadId == ctrd.ThreadId
                 select x).ToList<Comment>();

                if (answers.Count != 0)
                {
                    foreach(Comment answ in answers)
                    {
                        answ.ans = false;
                    }
                }

                dl.SaveChanges();
            }
            return RedirectToAction("ContentPage", ctrd);
        }

       
        public ActionResult AdminPage()
        {
            User cur = new User()
            {
                UserName = "None"
            };
            if (TempData["CurrentUser"] != null)
            {
                cur = new User((User)TempData["CurrentUser"]);
                TempData["CurrentUser"] = cur;
            }

            //#############
            InstitutionDal dal = new InstitutionDal();
            List<Institution> Inst =
            (from x in dal.Institutions
             select x).ToList<Institution>();
            ViewBag.InstList = Inst;

            DepartmentDal dal2 = new DepartmentDal();
            List<Department> Dep =
            (from x in dal2.Departments
             select x).ToList<Department>();

            ViewBag.DepList = Dep;

            SyearDal dal3 = new SyearDal();
            List<Syear> year =
            (from x in dal3.Syears
             select x).ToList<Syear>();

            ViewBag.YearList = year;

            ThreadDal dal4 = new ThreadDal();
            List<Thread> trd =
            (from x in dal4.Threads
             select x).ToList<Thread>();

            ViewBag.threadList = trd;

            return View(cur);
        }

        public ActionResult editDepartment(Department obj)
        {
            string new_Name = Request.Form["DepartmentName"];
            string new_Id = Request.Form["InstID"];
            string conf = Request.Form["Remove"];


            //Remove Department
            if (obj.DepartmentId != 0 && conf != null && conf.Equals("yes"))
            {
                using (var context = new DepartmentDal())
                {
                    var bay = (from d in context.Departments where d.DepartmentId == obj.DepartmentId select d).Single();
                    context.Departments.Remove(bay);
                    context.SaveChanges();
                }
                return RedirectToAction("AdminPage");
            }

            if (obj.DepartmentId != 0)
            {
                DepartmentDal dal = new DepartmentDal();
                List<Department> Dep =
                (from x in dal.Departments
                 where x.DepartmentId == obj.DepartmentId
                 select x).ToList<Department>();

                if(Dep != null)
                {
                    if (new_Name != null)
                    {
                        Dep[0].DepartmentName = new_Name;
                    }
                    if (new_Name == null)
                    {
                        Dep[0].InstitutionId = Int32.Parse(new_Id);
                    }
                    dal.SaveChanges();
                }
                
            }
            else
            {
                string add_new_Name = Request.Form["NewDepartment"];
                string add_new_Id = Request.Form["NewDepartmentID"];
                try
                {
                    int id = Int32.Parse(add_new_Id);
                }
                catch
                {
                    add_new_Id = "-1";
                }
                Department newAdd = new Department()
                {
                    DepartmentName = add_new_Name,
                    InstitutionId = Int32.Parse(add_new_Id)
                };
                DepartmentDal dal = new DepartmentDal();
                dal.Departments.Add(newAdd);
                dal.SaveChanges();

            }
            return RedirectToAction("AdminPage");
        }

        public ActionResult editInstitution(Institution obj)
        {
            string new_Name = Request.Form["EditName"];
            string conf = Request.Form["Remove"];

            //Remove Institution
            if (obj.InstitutionId != 0 && conf!= null && conf.Equals("yes"))
            {
                using (var context = new InstitutionDal())
                {
                    var bay = (from d in context.Institutions where d.InstitutionId == obj.InstitutionId select d).Single();
                    context.Institutions.Remove(bay);
                    context.SaveChanges();
                }
                return RedirectToAction("AdminPage");
            }

                if (obj.InstitutionId != 0)
            {
                InstitutionDal dal = new InstitutionDal();
                List<Institution> inst =
                (from x in dal.Institutions
                 where x.InstitutionId == obj.InstitutionId
                 select x).ToList<Institution>();

                if (inst != null)
                {
                    if (new_Name != null)
                    {
                        inst[0].InstName = new_Name;
                    }
                    dal.SaveChanges();
                }

            }
            else
            {
                string add_new_Name = Request.Form["NewInstitution"];

                Institution newAdd = new Institution()
                {
                    InstName = add_new_Name,
                };
                InstitutionDal dal = new InstitutionDal();
                dal.Institutions.Add(newAdd);
                dal.SaveChanges();

            }
            return RedirectToAction("AdminPage");
        }

        public ActionResult editSyear(Syear obj)
        {
            string new_Name = Request.Form["EditYearName"];
            string new_Id = Request.Form["DepartmentID"];
            string conf = Request.Form["Remove"];


            //Remove Institution
            if (obj.SyearId != 0 && conf != null && conf.Equals("yes"))
            {
                using (var context = new SyearDal())
                {
                    var bay = (from d in context.Syears where d.SyearId == obj.SyearId select d).Single();
                    context.Syears.Remove(bay);
                    context.SaveChanges();
                }
                return RedirectToAction("AdminPage");
            }

            if (obj.DepartmentId != 0)
            {
                SyearDal dal = new SyearDal();
                List<Syear> Dep =
                (from x in dal.Syears
                 where x.SyearId == obj.SyearId
                 select x).ToList<Syear>();

                if (Dep != null)
                {
                    if (new_Name != null)
                    {
                        Dep[0].SyearName = new_Name;
                    }
                    if (new_Name.Equals(null))
                    {
                        Dep[0].DepartmentId = Int32.Parse(new_Id);
                    }
                    dal.SaveChanges();
                }

            }
            else
            {
                string add_new_Name = Request.Form["NewSyear"];
                string add_new_Id = Request.Form["NewDepartmentID"];
                try
                {
                    int id = Int32.Parse(add_new_Id);
                }
                catch
                {
                    add_new_Id = "-1";
                }
                Syear newAdd = new Syear()
                {
                    SyearName = add_new_Name,
                    DepartmentId = Int32.Parse(add_new_Id)
                };
                SyearDal dal = new SyearDal();
                dal.Syears.Add(newAdd);
                dal.SaveChanges();

            }
            return RedirectToAction("AdminPage");
        }

        public ActionResult editThread(Thread obj)
        {
            string new_Name = Request.Form["EditThreadName"];
            string new_Id = Request.Form["YearID"];
            string conf = Request.Form["Remove"];


            //Remove Thread
            if (obj.ThreadId != 0 && conf != null && conf.Equals("yes"))
            {
                using (var context = new ThreadDal())
                {
                    var bay = (from d in context.Threads where d.ThreadId == obj.ThreadId select d).Single();
                    context.Threads.Remove(bay);
                    context.SaveChanges();
                }
                return RedirectToAction("AdminPage");
            }

            if (obj.ThreadId != 0)
            {
                ThreadDal dal = new ThreadDal();
                List<Thread> Dep =
                (from x in dal.Threads
                 where x.ThreadId == obj.ThreadId
                 select x).ToList<Thread>();

                if (Dep != null)
                {
                    if (new_Name != null)
                    {
                        Dep[0].ThreadName = new_Name;
                    }
                    if (new_Name.Equals(null))
                    {
                        Dep[0].SyearId = Int32.Parse(new_Id);
                    }
                    dal.SaveChanges();
                }

            }
            else
            {
                string add_new_Name = Request.Form["NewThread"];
                string add_new_Id = Request.Form["NewSyearID"];
                try
                {
                    int id = Int32.Parse(add_new_Id);
                }
                catch
                {
                    add_new_Id = "-1";
                }
                Thread newAdd = new Thread()
                {
                    ThreadName = add_new_Name,
                    SyearId = Int32.Parse(add_new_Id)
                };
                ThreadDal dal = new ThreadDal();
                dal.Threads.Add(newAdd);
                dal.SaveChanges();

            }
            return RedirectToAction("AdminPage");
        }

        // ~~~~~~~~ New Thread ~~~~~~~~~~
        /// <summary>
        /// New Thread Page
        /// </summary>
        /// <returns>The New thread Page</returns>
        public ActionResult NewThread()
        {
            User cur = new User()
            {
                UserName = "None"
            };
            if (TempData["CurrentUser"] != null)
            {
                cur = new User((User)TempData["CurrentUser"]);
                TempData["CurrentUser"] = cur;
            }
            return View(cur);
        }
        /// <summary>
        /// Add new Thread to DataBase
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public ActionResult CreateNewThread(Syear year)
        {
            string newThreadName = Request.Form["threadName"];
            string newContent = Request.Form["contentArea"];
            string newThreadType = Request.Form["threadType"];

            User curUser = getUser();

            Thread newThread = new Thread()
            {
                ThreadName = newThreadName,
                SyearId = year.SyearId,
                ThreadType = newThreadType,
                OwnerId = curUser.id
            };

            ThreadDal trd = new ThreadDal();
            trd.Threads.Add(newThread);
            trd.SaveChanges();

            List<Thread> nThread =
                (from x in trd.Threads
                 where x.ThreadName == newThread.ThreadName && x.SyearId == newThread.SyearId
                 select x).ToList<Thread>();

            Content newContentObj = new Content()
            {
                threadName = nThread[0].ThreadName,
                threadContent = newContent,
                threadId = nThread[0].ThreadId
            };

            ContentDal cnt = new ContentDal();
            cnt.Contents.Add(newContentObj);
            cnt.SaveChanges();


            return RedirectToAction("ThreadsPage", year);
        }

        // ~~~~~~~~ New Comment ~~~~~~~~~~
        public ActionResult addComment(Content comThreadId)
        {
            string newComment = Request.Form["commentContent"];

            User usrid = ((User)TempData["CurrentUser"]);

            // ##### Get Users #####
            UserDal udal = new UserDal();
            List<User> usr =
            (from x in udal.Users
             where x.UserName == usrid.UserName
             select x).ToList<User>();

            Comment newCom = new Comment()
            {
                commentContent = newComment,
                rank = 0,
                userId = usr[0].id,
                threadId = comThreadId.threadId
            };

            CommentDal comDal = new CommentDal();
            comDal.Comments.Add(newCom);
            comDal.SaveChanges();

            ThreadDal trd = new ThreadDal();
            List<Thread> nThread =
                (from x in trd.Threads
                 where x.ThreadId == comThreadId.threadId
                 select x).ToList<Thread>();


            User cur = new User()
            {
                UserName = "None"
            };
            if (TempData["CurrentUser"] != null)
            {
                cur = new User((User)TempData["CurrentUser"]);
                TempData["CurrentUser"] = cur;
            }

            return RedirectToAction("ContentPage", nThread[0]);
        }
        // ##### Edit Thread #####
        public ActionResult EditThreadPage(Thread trd)
        {
            User usr = getUser();

            // ##### Get Thread Name + Contents #####
            ContentDal dal = new ContentDal();
            List<Content> cont =
            (from x in dal.Contents
             where x.threadId == trd.ThreadId
             select x).ToList<Content>();

            TempData["threadCont"] = cont[0];

            TempData["thread"] = trd;

            return View(usr);
        }

        public ActionResult deleteComment(Thread thread)
        {
            try
            {
                int cmtId = Int32.Parse(Request.Form["cmtDelete"]);

                using (CommentDal dl = new CommentDal())
                {
                    List<Comment> cmt = (from x in dl.Comments
                                         where x.commentId == cmtId
                                         select x).ToList<Comment>();
                    if(cmt.Count != 0)
                    {
                        dl.Comments.Remove(cmt[0]);
                        dl.SaveChanges();
                    }
                    
                }
            }
            catch
            {
                return RedirectToAction("ContentPage", thread);
            }
            return RedirectToAction("ContentPage", thread);
        }

        public ActionResult EditThreadContent(Content cont)
        {
            string newThreadName = Request.Form["threadName"];
            string newContent = Request.Form["contentArea"];
            string newThreadType = Request.Form["threadType"];
            string test = Request.Form["solved"];
            bool solved = bool.Parse(Request.Form["solved"]);
            bool locked;
            if (Request.Form["locked"] == null)
            {
                locked = false;
            }
            else locked = bool.Parse(Request.Form["locked"]);

            Thread curT = (Thread)TempData["currInst"];

            User curUser = getUser();

            using (var db = new ContentDal())
            {
                var result = db.Contents.SingleOrDefault(b => b.contentId == cont.contentId);
                if (result != null)
                {
                    result.contentId = cont.contentId;
                    result.threadName = newThreadName;
                    result.threadContent = newContent;
                    db.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    using (ThreadDal dal = new ThreadDal())
                    {
                        var result2 = dal.Threads.SingleOrDefault(b => b.ThreadId == result.threadId);
                        result2.ThreadName = newThreadName;
                        result2.ThreadType = newThreadType;
                        result2.Solved = solved;
                        result2.Locked = locked;
                        dal.SaveChanges();
                        return RedirectToAction("ContentPage", result2);
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// Get User Function !! Not ActionResult !!
        /// </summary>
        /// <returns>Current user in the system</returns>
        public User getUser()
        {
            User usrid = ((User)TempData["CurrentUser"]);

            // ##### Get Users #####
            UserDal udal = new UserDal();
            List<User> usr =
            (from x in udal.Users
             where x.UserName == usrid.UserName
             select x).ToList<User>();

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
        public ActionResult MyProfile(User usr)
        {
            User cur = new User()
            {
                UserName = "None"
            };

            if (TempData["CurrentUser"] == null)
            {
                cur = new User(usr);
                TempData["CurrentUser"] = usr;

            }
            else
            {
                if (TempData["CurrentUser"] != null)
                {
                    cur = new User((User)TempData["CurrentUser"]);
                    TempData["CurrentUser"] = cur;
                }
            }
            UserDal dal = new UserDal();
            List<User> Usr =
            (from x in dal.Users
             where x.UserName == cur.UserName
             select x).ToList<User>();

            TempData["CurrentUser"] = Usr[0];
            return View(Usr[0]);
        }
        public ActionResult EditProfile()
        {
            TempData["Name"] = TempData["CurrentUser"];
            return View(TempData["CurrentUser"]);
        }

        public ActionResult Save()
        {
            User usr = (User)TempData["Name"];
            string Name = Request.Form["Name"];
            string LastName = Request.Form["LastName"];
            string institution = Request.Form["institution"];
            string year = Request.Form["year"];
            int tmpyear = 0;
            if (year != "")
            {
                tmpyear = Int32.Parse(year);
            }

            UserDal dal = new UserDal();
            List<User> Users =
               (from x in dal.Users
                where x.UserName == usr.UserName
                select x).ToList<User>();
            if (Users[0] != null)
            {
                if (institution != "") Users[0].institution = institution;
                if (year != "") Users[0].year = tmpyear;
                if (Name != "") Users[0].FirstName = Name;
                if (LastName != "") Users[0].LastName = LastName;
            }
            dal.SaveChanges();
            TempData["CurrentUser"] = Users[0];
            return View("MyProfile", Users[0]);
        }
        public ActionResult ManageUsers(User usr)
        {
            User cur = new User()
            {
                UserName = "None"
            };

            if (TempData["CurrentUser"] == null)
            {
                cur = new User(usr);
                TempData["CurrentUser"] = usr;

            }
            else
            {
                if (TempData["CurrentUser"] != null)
                {
                    cur = new User((User)TempData["CurrentUser"]);
                    TempData["CurrentUser"] = cur;
                }
            }
            return View(cur);
        }

        public ActionResult Block()
        { 
            string UserName = Request.Form["username"];
            UserDal dal = new UserDal();
            List<User> Users =
               (from x in dal.Users
                where x.UserName == UserName
                select x).ToList<User>();
            if (Users[0] != null)
            {
                Users[0].active = false;
            }
            dal.SaveChanges();
            return View("ManageUsers", TempData["CurrentUser"]);
        }
        public ActionResult UnBlock()
        {
            string UserName = Request.Form["username"];
            UserDal dal = new UserDal();
            List<User> Users =
               (from x in dal.Users
                where x.UserName == UserName
                select x).ToList<User>();
            if (Users[0] != null)
            {
                Users[0].active = true;
            }
            dal.SaveChanges();
            return View("ManageUsers", TempData["CurrentUser"]);
        }

        public ActionResult logout()
        {
            TempData["CurrentUser"] = null;
            return RedirectToAction("Login","Login", new User());
        }
    }

}

