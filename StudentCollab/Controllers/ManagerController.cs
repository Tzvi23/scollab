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

            ManageConnectionDal mcdal = new ManageConnectionDal();
            List<ManageConnection> manageConnections =
                (from x in mcdal.ManageConnections
                 where x.managerId == cur.id
                 select x).ToList<ManageConnection>();
            
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
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments =
                            (from x in dp.Departments
                             where x.DepartmentId == DepId
                             select x).ToList<Department>();
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
                        SyearDal yearid = new SyearDal();
                        List<Syear> syears =
                            (from x in yearid.Syears
                             where x.SyearId == YearId
                             select x).ToList<Syear>();
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments =
                            (from x in dp.Departments
                             where x.DepartmentId == syears[0].DepartmentId
                             select x).ToList<Department>();
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

            ManageConnectionDal mcdal = new ManageConnectionDal();
            List<ManageConnection> manageConnections =
                (from x in mcdal.ManageConnections
                 where x.managerId == cur.id
                 select x).ToList<ManageConnection>();

            foreach (ManageConnection mc in manageConnections)
            {
                if (InstId != -1)
                {
                    if (mc.institution == InstId)
                    {
                        UnBlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                }
                if (DepId != -1)
                {
                    if (mc.department == DepId)
                    {
                        UnBlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments =
                            (from x in dp.Departments
                             where x.DepartmentId == DepId
                             select x).ToList<Department>();
                        if (mc.institution == departments[0].InstitutionId)
                        {
                            UnBlockFromDB(UserName, date, InstId, DepId, YearId);
                            return View("Block", cur);
                        }
                    }
                }
                if (YearId != -1)
                {
                    if (mc.sYear == YearId)
                    {
                        UnBlockFromDB(UserName, date, InstId, DepId, YearId);
                        return View("Block", cur);
                    }
                    else
                    {
                        SyearDal yearid = new SyearDal();
                        List<Syear> syears =
                            (from x in yearid.Syears
                             where x.SyearId == YearId
                             select x).ToList<Syear>();
                        DepartmentDal dp = new DepartmentDal();
                        List<Department> departments =
                            (from x in dp.Departments
                             where x.DepartmentId == syears[0].DepartmentId
                             select x).ToList<Department>();
                        if (mc.institution == departments[0].InstitutionId || mc.department == syears[0].DepartmentId)
                        {
                            UnBlockFromDB(UserName, date, InstId, DepId, YearId);
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

        private void UnBlockFromDB(string uName, DateTime date, int inst, int dep, int yeId)
        {
            BlockedDal bdal = new BlockedDal();
            List<Blocked> bd =
            (from x in bdal.Blockeds
             where x.UserName == uName
             select x).ToList<Blocked>();

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
    }
}