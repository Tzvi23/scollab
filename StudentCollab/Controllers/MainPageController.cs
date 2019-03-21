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
            InstitutionDal dal = new InstitutionDal();
            List<Institution> Inst =
            (from x in dal.Institutions
             select x).ToList<Institution>();

            ViewBag.InstutionsDB = Inst;

            return View("MainPage",usr);
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

            return View();
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

            return View();
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

            return View();
        }

        public ActionResult logout()
        {

            return RedirectToAction("Login","Login", new User());
        }
    }
}