using System;
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
                    if (new_Name.Equals(null))
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



        public ActionResult logout()
        {
            TempData["CurrentUser"] = null;
            return RedirectToAction("Login","Login", new User());
        }
    }
}