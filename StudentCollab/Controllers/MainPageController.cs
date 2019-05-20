using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;
using System.Net.Mail;
using System.Text;
using System.IO;


namespace StudentCollab.Controllers
{
    public class MainPageController : Controller
    {
        //Defines
        public const int LockThreadLog = 0;
        public const int UnLockThreadLog = 1;
        public const int MoveThreadLog = 2;
        public static string DepImage = "_1";

        // GET: MainPage
        public ActionResult MainPage(User usr)
        {
            TempData["commFlag"] = 0;
            TempData["canLike"] = 0;
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

        public ActionResult backToHome()
        {
            User usr = new User((User)TempData["ur"]);

            UserDal dal = new UserDal();
            List<User> Users =
            (from x in dal.Users
             where x.UserName == usr.UserName && x.Password == usr.Password
             select x).ToList<User>();
            if (Users.Any())
            {
                switch (Users[0].rank)
                {
                    case 0:
                        AdminUser admin = new AdminUser(Users[0]);
                        ViewData["CurrentUser"] = admin.UserName;
                        ViewBag.CurrentUser = admin;
                        break;
                    case 1:
                        ManagerUser manager = new ManagerUser(Users[0]);
                        ViewData["CurrentUser"] = manager.UserName;
                        ViewBag.CurrentUser = manager;
                        break;
                    case 2:
                        User usr2 = new User(Users[0]);
                        ViewData["CurrentUser"] = usr2.UserName;
                        ViewBag.CurrentUser = usr2;
                        break;
                }
            }
            return RedirectToAction("MainPage", "MainPage", Users[0]);
        }

        public ActionResult UnionPage(User usr)
        {
            return View(usr);
        }

        public ActionResult addToUnion()
        {
            Int32 rank;
            try
            {
                rank = Int32.Parse(Request.Form["rank"]);
            }
            catch
            {
                return View("UnionPage", new User((User)TempData["usr"]));
            }

            string uName = Request.Form["username"];

            UserDal udal = new UserDal();
            List<User> usrLst =
                (from x in udal.Users
                 where x.UserName == uName
                 select x).ToList<User>();

            if (usrLst.Any() && rank < 3 && rank > 0)
            {
                usrLst[0].studentUnionRank = rank;
            }

            return View("UnionPage", new User((User)TempData["usr"]));
        }

        public ActionResult askForJoin()
        {

            User ur = new User((User)TempData["usr"]);
            MessageDal mdal = new MessageDal();
            UserDal udal = new UserDal();
            string sub = "Union " + ur.FirstName;
            Message msg = new Message()
            {
                date = DateTime.Now,
                subject = sub,
                senderName = "Union system",
                mag = " The user - " + ur.UserName + " id - " + ur.id + " want to join to the Students Union"
            };

            List<Message> messages =
                (from x in mdal.Messages
                 where x.subject == sub
                 select x).ToList<Message>();

            List<Int32> ids =
                (from x in udal.Users
                 where x.studentUnionRank == 2
                 select x.id).ToList<Int32>();

            if (!(messages.Any()))
            {
                sendThemAll(ids, msg);
            }

            return View("MyProfile", ur);
        }

        public ActionResult Cw(Int32 cId)
        {
            //TempData["CurrentUser"] = new User((User)TempData["urid"]);
            User usr = getUser();
            TempData["commFlag"] = 1;
            CommentDal cdal = new CommentDal();
            ThreadDal tdal = new ThreadDal();
            Comment c =
                (from x in cdal.Comments
                 where x.commentId == cId
                 select x).First<Comment>();
            TempData["comm"] = c.commentContent;
            Thread t =
                (from x in tdal.Threads
                 where x.ThreadId == c.threadId
                 select x).First<Thread>();
            Thread tr = new Thread(t);


            return RedirectToAction("ContentPage", tr);
        }

        public ActionResult Report(Int32 cId)
        {
            /*
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("studycollab11@gmail.com", "Stubrasil16");

            MailAddress to = new MailAddress("studycollab11@gmail.com");
            MailAddress from = new MailAddress("studycollab11@gmail.com");
            MailMessage mm = new MailMessage(from.Address, to.Address, "Report!", txt);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            */

            Message msg = (Message)TempData["Msg"];

            CommentDal cdal = new CommentDal();
            Int32 targetId =
                (from c in cdal.Comments
                 where c.commentId == cId
                 select c.userId).FirstOrDefault<Int32>();

            Int32 trdId =
                (from c in cdal.Comments
                 where c.commentId == cId
                 select c.threadId).FirstOrDefault<Int32>();

            ThreadDal tdal = new ThreadDal();
            Int32 yrId =
            (from tr in tdal.Threads
             where tr.ThreadId == trdId
             select tr.SyearId).FirstOrDefault<Int32>();

            SyearDal sdal = new SyearDal();
            Int32 dpId =
            (from sy in sdal.Syears
             where sy.SyearId == yrId
             select sy.DepartmentId).FirstOrDefault<Int32>();

            DepartmentDal dpdal = new DepartmentDal();
            Int32 instId =
            (from dp in dpdal.Departments
             where dp.DepartmentId == dpId
             select dp.InstitutionId).FirstOrDefault<Int32>();

            ManageConnectionDal mcdal = new ManageConnectionDal();
            List<Int32> mngsYr =
                (from mc in mcdal.ManageConnections
                 where mc.sYear == yrId
                 select mc.managerId).ToList<Int32>();

            List<Int32> mngsDp =
                (from mc in mcdal.ManageConnections
                where mc.department == dpId
                select mc.managerId).ToList<Int32>();

            List<Int32> mngsInst =
                (from mc in mcdal.ManageConnections
                where mc.institution == instId
                select mc.managerId).ToList<Int32>();

            UserDal udal = new UserDal();
            List<Int32> admins =
                (from u in udal.Users
                 where u.rank == 0
                 select u.id).ToList<Int32>();

            List<Int32> mngs = mngsYr;

            if (!(mngs.Any<Int32>()) || mngsYr.Contains(targetId) || mngsDp.Contains(targetId) || mngsInst.Contains(targetId) || admins.Contains(targetId))
            {
                mngs = mngsDp;

                if (!(mngs.Any<Int32>()) || mngsDp.Contains(targetId) || mngsInst.Contains(targetId) || admins.Contains(targetId))
                {
                    mngs = mngsInst;

                    if (!(mngs.Any<Int32>()) || mngsInst.Contains(targetId) || admins.Contains(targetId))
                    {
                        mngs = admins;

                    }
                }
            }

            sendThemAll(mngs, msg);

            User x = (User) TempData["urid"];
            return View(x);

        }

        private void sendThemAll(List<Int32> mIds, Message msg)
        {

            UserDal udal = new UserDal();
            MessageDal mdal = new MessageDal();

            List<String> names =
                (from x in udal.Users
                 where mIds.Contains(x.id)
                 select x.UserName).ToList<String>();

            foreach(String name in names)
            {
                msg.reciverName = name;
                mdal.Messages.Add(msg);
                mdal.SaveChanges();

            }

        }

        public ActionResult UploadFile(User usr)
        {
            User cur = new User()
            {
                UserName = "None"
            };

            if (TempData["CurrentUser"] == null)
            {
                cur = new User(usr);
                TempData["CurrentUser"] = cur;

            }
            else
            {
                if (TempData["CurrentUser"] != null)
                {
                    cur = new User((User)TempData["CurrentUser"]);
                    TempData["CurrentUser"] = cur;
                }
            }
            FileManager fm = new FileManager();
            TempData["FileManager"] = fm;
            return View("UploadFile", fm);
        }
        [HttpPost]
        public ActionResult FileUploadService(HttpPostedFileBase file)
        {
            file = Request.Files["fileupload"];
            string UplName = TempData["user"].ToString();
            if (file != null)
            {
                BinaryReader br = new BinaryReader(file.InputStream);

                Files f = new Files()
                {
                    UploaderName = UplName,
                    FileName = file.FileName,
                    Data = br.ReadBytes((int)file.ContentLength),
                    Active = true,
                    Thread = Int32.Parse(TempData["thread"].ToString())
                };
                
                FilesDal fd = new FilesDal();
                fd.files.Add(f);
                fd.SaveChanges();
            }
            UserDal udal = new UserDal();
            List<User> usr =
                (from x in udal.Users
                 where x.UserName == UplName
                 select x).ToList<User>();
            User cur = new User(usr[0]);
            return RedirectToAction("MainPage", cur);
        }
        public FileResult DownloadFile()
        {
            int UplNum = Int32.Parse(TempData["UplNum"].ToString());
            FilesDal fd = new FilesDal();

            var downlFile = fd.files.Find(UplNum);
            return File(downlFile.Data, "application/pdf", downlFile.FileName);
        }
        public ActionResult DeleteFile()
        {
            int UplNum = Int32.Parse(TempData["UplNum"].ToString());
            FilesDal fd = new FilesDal();

            var DelFile = fd.files.Find(UplNum);
            if (DelFile != null)
            {
                DelFile.Active = false;
            }
            fd.SaveChanges();
            User cur = getUser();
            return RedirectToAction("MyUploads", cur);
        }
        public ActionResult DepartmentsPage(Institution inst)
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

            if (inst != null)
            {
                BlockedDal bdal = new BlockedDal();
                List<Blocked> bd =
                (from x in bdal.Blockeds
                 where x.UserName == cur.UserName
                 select x).ToList<Blocked>();
                foreach(Blocked b in bd)
                {
                    if(b.InsId == inst.InstitutionId && b.Bdate > DateTime.Today)
                    {
                        return View("MainPage", cur);
                    }
                }


                DepartmentDal dal = new DepartmentDal();
                List<Department> Dep =
                (from x in dal.Departments
                 where x.InstitutionId == inst.InstitutionId
                 select x).ToList<Department>();

                ViewBag.DepartmentsDB = Dep;
            }



                

            return View("DepartmentsPage", cur);
        }

        public ActionResult SyearsPage(Department dep)
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

            TempData["DepImg"] = DepImage;
            return View("DepartmentsPage", cur);
        }
        public ActionResult ChangeDepImage(User cur)
        {
            DepImage = Request.Form["img"];
            return RedirectToAction("MainPage", cur);
        }
        public ActionResult SyearsPage(Department dep)
        {
            if (dep != null)
            {
                BlockedDal bdal = new BlockedDal();
                List<Blocked> bd =
                (from x in bdal.Blockeds
                 where x.UserName == cur.UserName
                 select x).ToList<Blocked>();
                foreach (Blocked b in bd)
                {
                    if (b.DepId == dep.DepartmentId && b.Bdate > DateTime.Today)
                    {
                        DepartmentDal Ddal = new DepartmentDal();
                        List<Department> Dep =
                        (from x in Ddal.Departments
                         where x.InstitutionId == dep.InstitutionId
                         select x).ToList<Department>();

                        ViewBag.DepartmentsDB = Dep;

                        return View("DepartmentsPage", cur);
                    }
                }

                SyearDal dal = new SyearDal();
                List<Syear> year =
                (from x in dal.Syears
                 where x.DepartmentId == dep.DepartmentId
                 select x).ToList<Syear>();

                ViewBag.SyearDB = year;
            }


            return View(cur);
        }

        public ActionResult ThreadsPage(Syear year)
        {
            User cur = getUser();

            if (year != null)
            {
                BlockedDal bdal = new BlockedDal();
                List<Blocked> bd =
                (from x in bdal.Blockeds
                 where x.UserName == cur.UserName
                 select x).ToList<Blocked>();
                foreach (Blocked b in bd)
                {
                    if (b.YearId == year.SyearId && b.Bdate>DateTime.Today)
                    {
                        SyearDal Sdal = new SyearDal();
                        List<Syear> Syear =
                        (from x in Sdal.Syears
                         where x.DepartmentId == year.DepartmentId
                         select x).ToList<Syear>();

                        ViewBag.SyearDB = Syear;

                        return View("SyearsPage", cur);
                    }
                }

                ThreadDal dal = new ThreadDal();
                List<Thread> trd =
                (from x in dal.Threads
                 where x.SyearId == year.SyearId
                 select x).ToList<Thread>();

                ViewBag.ThreadDB = trd;
            }

            TempData["year"] = year;

            ManageConnectionDal mdal = new ManageConnectionDal();
            List<ManageConnection> mc =
            (from x in mdal.ManageConnections
             where x.managerId == cur.id && x.sYear == year.SyearId
             select x).ToList<ManageConnection>();

            if (mc.Any())
            {
                TempData["flagManager"] = true;

            }
            else
            {
                TempData["flagManager"] = false;
            }
            TempData["Manager"] = checkManagingAuthorityYear(year);

            return View(cur);
        }

        public ActionResult LockThread(Thread thread)
        {
            RecordLog(new Models.User(), LockThreadLog, thread.ThreadId);
            using (ThreadDal trdDal = new ThreadDal())
            {
                List<Thread> trd =
                    (from x in trdDal.Threads
                     where x.ThreadId == thread.ThreadId
                     select x).ToList();

                trd[0].Locked = true;
                trdDal.SaveChanges();
            }


            RecordLog(new Models.User(), LockThreadLog, thread.ThreadId);
            using (SyearDal yearDal = new SyearDal())
            {
                List<Syear> years =
                    (from x in yearDal.Syears
                     where x.SyearId == thread.SyearId
                     select x).ToList();
                return RedirectToAction("ThreadsPage", years[0]);
            }
        }

        public ActionResult UnLockThread(Thread thread)
        {

            using (ThreadDal trdDal = new ThreadDal())
            {
                List<Thread> trd =
                    (from x in trdDal.Threads
                     where x.ThreadId == thread.ThreadId
                     select x).ToList();

                trd[0].Locked = false;
                trdDal.SaveChanges();
            }

            RecordLog(new Models.User(), UnLockThreadLog, thread.ThreadId);
            using (SyearDal yearDal = new SyearDal())
            {
                List<Syear> years =
                    (from x in yearDal.Syears
                     where x.SyearId == thread.SyearId
                     select x).ToList();
                return RedirectToAction("ThreadsPage", years[0]);
            }
        }

        public bool checkManagingAuthority(Thread thread)
        {
            Dictionary<string, int> hierarchy = Gethierarchy(thread);
            User curUsr = getUser();

            using (ManageConnectionDal mcDal = new ManageConnectionDal())
            {
                int SyearID = hierarchy["SyearID"];
                int DepartmentID = hierarchy["DepartmentID"];
                int InstitutionID = hierarchy["InstitutionID"];
                List<ManageConnection> mc =
                    (from x in mcDal.ManageConnections
                     where x.sYear == SyearID && x.department == DepartmentID && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc.Any()) return true;

                List<ManageConnection> mc1 =
                    (from x in mcDal.ManageConnections
                     where x.sYear == -1 && x.department == DepartmentID && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc1.Any()) return true;

                List<ManageConnection> mc3 =
                    (from x in mcDal.ManageConnections
                     where x.sYear == -1 && x.department == -1 && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc3.Any()) return true;

            }

                return false;
        }
        public bool checkManagingAuthorityYear(Syear year)
        {
            Dictionary<string, int> hierarchy = GethierarchyYearBase(year);
            User curUsr = getUser();

            using (ManageConnectionDal mcDal = new ManageConnectionDal())
            {
                int SyearID = hierarchy["SyearID"];
                int DepartmentID = hierarchy["DepartmentID"];
                int InstitutionID = hierarchy["InstitutionID"];
                List<ManageConnection> mc =
                    (from x in mcDal.ManageConnections
                     where x.sYear == SyearID && x.department == DepartmentID && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc.Any()) return true;

                List<ManageConnection> mc1 =
                    (from x in mcDal.ManageConnections
                     where x.sYear == -1 && x.department == DepartmentID && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc1.Any()) return true;

                List<ManageConnection> mc3 =
                    (from x in mcDal.ManageConnections
                     where x.sYear == -1 && x.department == -1 && x.institution == InstitutionID && x.managerId == curUsr.id
                     select x).ToList();
                if (mc3.Any()) return true;

            }

            return false;
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
            
            TempData["Manager"] = checkManagingAuthority(thread);
            if (TempData["canLike"] == null) TempData["canLike"] = 0; //TODO: Check meaning

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
                        TempData["canLike"] = 0;
                        return RedirectToAction("ContentPage", trd);
                    }
                }
            }
            TempData["canLike"] = 0;
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
            TempData["canLike"] = 0;
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

            //AgreeMant Edit
            OtherDal val = new OtherDal();
            List<Other> agreemant =
                (from x in val.Others
                 select x).ToList<Other>();

            ViewBag.AgreemantObj = agreemant[0];

            FilesDal fdal = new FilesDal();
            List<Files> files =
            (from x in fdal.files
             select x).ToList<Files>();

            ViewBag.FilesList = files;

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
            string unionC = Request.Form["union"];
            
            User curUser = getUser();
            bool unionInt;
            if (unionC == "Y" || unionC == "y")
            {
                unionInt = true;
            }
            else
            {
                unionInt = false;
            }

            Thread newThread = new Thread()
            {
                ThreadName = newThreadName,
                SyearId = year.SyearId,
                ThreadType = newThreadType,
                OwnerId = curUser.id,
                Pinned = false,
                forUnion = unionInt
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

        public ActionResult unPinThread(Thread trd)
        {
            ThreadDal tdal = new ThreadDal();
            List<Thread> trds =
            (from x in tdal.Threads
             where x.ThreadId == trd.ThreadId
             select x).ToList<Thread>();

            trds[0].Pinned = false;
            tdal.SaveChanges();

            SyearDal sdal = new SyearDal();
            List<Syear> syrs =
            (from x in sdal.Syears
             where x.SyearId == trd.SyearId
             select x).ToList<Syear>();

            Syear year = new Syear(syrs[0]);

            return RedirectToAction("ThreadsPage", year);
        }

        public ActionResult PinThread(Thread trd)
        {
            ThreadDal tdal = new ThreadDal();
            List<Thread> trds =
            (from x in tdal.Threads
             where x.ThreadId == trd.ThreadId
             select x).ToList<Thread>();

            trds[0].Pinned = true;
            tdal.SaveChanges();

            SyearDal sdal = new SyearDal();
            List<Syear> syrs =
            (from x in sdal.Syears
             where x.SyearId == trd.SyearId
             select x).ToList<Syear>();

            Syear year = new Syear(syrs[0]);

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

            TempData["canLike"] = 0;

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
                TempData["canLike"] = 0;
                return RedirectToAction("ContentPage", thread);
            }
            TempData["canLike"] = 0;
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
                        TempData["canLike"] = 0;
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

            FilesDal fdal = new FilesDal();
            List<Files> FilesDB =
            (from x in fdal.files
             where x.UploaderName == cur.UserName
             select x).ToList<Files>();

            TempData["CurrentUser"] = Usr[0];
            TempData["FilesTable"] = FilesDB;
            return View(Usr[0]);
        }
        public ActionResult MyUploads(User usr)
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


            FilesDal fdal = new FilesDal();
            List<Files> FilesDB =
            (from x in fdal.files
             where x.UploaderName == cur.UserName
             select x).ToList<Files>();

            TempData["CurrentUser"] = Usr[0];
            TempData["FilesTable"] = FilesDB;
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

            ViewBag.Users = GetUsers();
            ViewBag.Institutions = GetInstitutions();
            ViewBag.Departments = GetDepartments();
            ViewBag.Syears = GetYears();
            
            User cur = getUser();
            return View(cur);
        }

        public ActionResult ManagerPage(User usr)
        {
            ViewBag.Users = GetUsers();
            ViewBag.Institutions = GetInstitutions();
            ViewBag.Departments = GetDepartments();
            ViewBag.Syears = GetYears();
            ViewBag.Threads = GetThreads();

            List<Thread> ManagingThread = new List<Thread>();
            
            foreach (Thread trd in ViewBag.Threads)
            {
                Syear obj = GetSyear(trd);
                if (checkManagingAuthorityYear(GetSyear(trd)))
                {
                    ManagingThread.Add(trd);
                }
            }
            User cur = getUser();
            ManagerLogDal dal = new ManagerLogDal();
            
            List<ManagerLog> logs =
            (from x in dal.Mlogs
                where x.userID == cur.id
                select x).ToList<ManagerLog>();
            ViewBag.Logs = logs;
            

                ViewBag.ManagingThread = ManagingThread;

            cur = getUser();
            return View(cur);
        }

        public ActionResult MoveThread(User usr)
        {
            //TODO: optional -  Add input checking
            int ThreadID;
            try
            {
                ThreadID = Int32.Parse(Request.Form["threadID"]);
            }
            catch
            {
                ThreadID = 0;
            }
            int SyearID;
            try
            {
                SyearID = Int32.Parse(Request.Form["year"]);
            }
            catch
            {
                SyearID = 0;        
            }

            List<Thread> ManagingThread = new List<Thread>();

            foreach (Thread trd in GetThreads())
            {
                Syear obj = GetSyear(trd);
                if (checkManagingAuthorityYear(GetSyear(trd)))
                {
                    ManagingThread.Add(trd);
                }
            }

            Thread res = ManagingThread.Find(b => b.ThreadId == ThreadID);
            if(res != null)
            {
                using (ThreadDal dal = new ThreadDal())
                {
                    Thread temp = dal.Threads.SingleOrDefault(b => b.ThreadId == ThreadID);
                    temp.SyearId = SyearID;
                    dal.SaveChanges();
                }
            }

            RecordLog(new Models.User(), MoveThreadLog, ThreadID, SyearID.ToString());

            return RedirectToAction("ManagerPage", usr);
        }

        public Syear GetSyear(Thread trd)
        {
            using (SyearDal dal = new SyearDal())
            {
                return dal.Syears.SingleOrDefault(b => b.SyearId == trd.SyearId);
            }
        }

        public List<User> GetUsers()
        {
            UserDal dal = new UserDal();
            List<User> Users =
                (from x in dal.Users
                 select x).ToList<User>();
            return Users;
        }

        public List<Institution> GetInstitutions()
        {
            InstitutionDal instDal = new InstitutionDal();
            List<Institution> Institutions =
                (from x in instDal.Institutions
                 select x).ToList<Institution>();
            return Institutions;
        }

        public List<Department> GetDepartments()
        {
            DepartmentDal depDal = new DepartmentDal();
            List<Department> Departments =
                (from x in depDal.Departments
                 select x).ToList<Department>();
            return Departments;
        }

        public List<Syear> GetYears()
        {
            SyearDal yearDal = new SyearDal();
            List<Syear> years =
                (from x in yearDal.Syears
                 select x).ToList<Syear>();
            return years;
        }

        public List<Thread> GetThreads()
        {
            ThreadDal threadtDal = new ThreadDal();
            List<Thread> thread =
                (from x in threadtDal.Threads
                 select x).ToList<Thread>();
            return thread;
        }

        public List<Content> GetContents() //Not in use
        {
            ContentDal contentDal = new ContentDal();
            List<Content> content =
                (from x in contentDal.Contents
                 select x).ToList<Content>();
            return content;
        }

        public Dictionary<string, int> Gethierarchy(Thread thread)
        {
            Dictionary<string, int> hierarchy = new Dictionary<string, int>();
            hierarchy.Add("ThreadID", thread.ThreadId);
            hierarchy.Add("SyearID", thread.SyearId);

            List<Syear> years = GetYears();
            Syear curYear = years.Find(b => b.SyearId == thread.SyearId);
            hierarchy.Add("DepartmentID", curYear.DepartmentId);
            List<Department> dep = GetDepartments();
            Department curDep =  dep.Find(b => b.DepartmentId == curYear.DepartmentId);
            hierarchy.Add("InstitutionID", curDep.InstitutionId);

            return hierarchy;
        }

        public Dictionary<string, int> GethierarchyYearBase(Syear year)
        {
            Dictionary<string, int> hierarchy = new Dictionary<string, int>();
            hierarchy.Add("SyearID", year.SyearId);

            List<Syear> years = GetYears();
            Syear curYear = years.Find(b => b.SyearId == year.SyearId);
            hierarchy.Add("DepartmentID", curYear.DepartmentId);
            List<Department> dep = GetDepartments();
            Department curDep = dep.Find(b => b.DepartmentId == curYear.DepartmentId);
            hierarchy.Add("InstitutionID", curDep.InstitutionId);

            return hierarchy;
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
            User cur = getUser();
            return View("ManageUsers", cur);
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
            User cur = getUser();
            return View("ManageUsers", cur);
        }

        public ActionResult makeManager()
        {
            User cur = getUser();
            string un = Request.Form["username"];
            int itt;
            int dmt;
            int yr;
            try
            {
                itt = Int32.Parse(Request.Form["institution"]);
            }
            catch
            {
                itt = -1; 
            }
            try
            {
                dmt = Int32.Parse(Request.Form["department"]);
            }
            catch
            {
                dmt = -1;
            }
            try
            {
                yr = Int32.Parse(Request.Form["year"]);
            }
            catch
            {
                yr = -1;
            }

            UserDal dal = new UserDal();
            List<User> Users =
               (from x in dal.Users
                where x.UserName == un
                select x).ToList<User>();
            if (Users[0] == null)
            {
                return RedirectToAction("ManageUsers", cur);
            }

            Users[0].rank = 1;
            dal.SaveChanges();

            ManageConnection tmpMC = new ManageConnection()
            {
                institution = itt,
                department = dmt,
                managerId = Users[0].id,
                sYear = yr
            };

            ManageConnectionDal mcdal = new ManageConnectionDal();
            mcdal.ManageConnections.Add(tmpMC);
            mcdal.SaveChanges();

            return RedirectToAction("ManageUsers", cur);
        }

        public ActionResult delManager()
        {
            //TODO: Make active field for manager
            User cur = getUser();
            string un = Request.Form["username"];
            int itt;
            int dmt;
            int yr;
            try
            {
                itt = Int32.Parse(Request.Form["institution"]);
            }
            catch
            {
                itt = -1;
            }
            try
            {
                dmt = Int32.Parse(Request.Form["department"]);
            }
            catch
            {
                dmt = -1;
            }
            try
            {
                yr = Int32.Parse(Request.Form["year"]);
            }
            catch
            {
                yr = -1;
            }
            string dl = Request.Form["del"];

            UserDal dal = new UserDal();
            List<User> Users =
               (from x in dal.Users
                where x.UserName == un
                select x).ToList<User>();
            if (Users[0] == null)
            {
                return RedirectToAction("ManageUsers", cur);
            }

            int id = Users[0].id;

            if (dl == "y" || dl == "Y")
            {
                Users[0].rank = 2;
                dal.SaveChanges();
            }
            

            ManageConnectionDal mcdal = new ManageConnectionDal();
            List<ManageConnection> manageConnections =
                (from x in mcdal.ManageConnections
                 where x.managerId == id && x.institution == itt && x.department == dmt && x.sYear == yr
                 select x).ToList<ManageConnection>();
            if (manageConnections.Any())
            {
                foreach(ManageConnection mc in manageConnections)
                {
                    mcdal.ManageConnections.Remove(mc);
                    mcdal.SaveChanges();
                }
            }

            return RedirectToAction("ManageUsers", cur);
        }

        public ActionResult logout()
        {
            TempData["CurrentUser"] = null;
            return RedirectToAction("Login","Login", new User());
        }

        public ActionResult AgreemantPage()
        {
            User cur = getUser();

            OtherDal val = new OtherDal();
            List<Other> agreemant =
                (from x in val.Others
                 select x).ToList<Other>();

            Other agr = agreemant[0];
            ViewBag.Agreemant = agr.Val;

            return View(cur);
        }

        public ActionResult UpdateAgreemant(Other agr)
        {
            OtherDal val = new OtherDal();
            List<Other> agreemant =
                (from x in val.Others
                 where x.Id == agr.Id
                 select x).ToList<Other>();

            using (OtherDal db = new OtherDal())
            {
                Other trd = db.Others.SingleOrDefault(b => b.Id == agr.Id);
                trd.Val = Request.Form["agreemantContent"];
                db.SaveChanges();
            }

            return RedirectToAction("AgreemantPage");
        }

        public bool RecordLog(User usr, int func, int trd_id, string additionString = null)
        {
            usr = getUser();
            using (ManagerLogDal dal = new ManagerLogDal())
            {                
                ManagerLog newlog = new ManagerLog
                {
                    userID = usr.id
                };

                string log;
                switch (func)
                {
                    case LockThreadLog: log = usr.UserName + " locked thread Number id : " + trd_id;
                        newlog.userLog = log;
                        newlog.logDate = DateTime.Now.ToString();
                        break;
                        
                    case UnLockThreadLog:
                        log = usr.UserName + " Unlocked thread Number id : " + trd_id;
                        newlog.userLog = log;
                        newlog.logDate = DateTime.Now.ToString();
                        break;

                    case MoveThreadLog:
                        log = usr.UserName + " Moved Thread ID: [" + trd_id + "] To Syear ID: [" + additionString + "]";
                        newlog.userLog = log;
                        newlog.logDate = DateTime.Now.ToString();
                        break;
                }
                   
                    

                dal.Mlogs.Add(newlog);
                dal.SaveChanges();
                return true;
            } 
        }
    }

}

