using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;
using System.Net.Mail;
using System.Text;


namespace StudentCollab.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View("Login",new User());
        }


        public ActionResult Contact(User getUsr)
        {

            return View(getUsr);
        }

        public ActionResult EmailConfirm(User usr)
        {
            string ConfCode;
            string un;
            string comp;

            try
            {
                ConfCode = Request.Form["Ccode"];
                un = TempData["UserName"].ToString();
                comp = TempData["ConfirmCode"].ToString();
            }
            catch
            {
                return View("Login");
            }

            if (Int32.Parse(ConfCode) == Int32.Parse(comp))
            {
                try
                {
                    UserDal dal = new UserDal();
                    List<User> Users =
                    (from x in dal.Users
                     where x.UserName == un
                     select x).ToList<User>();
                    if (Users[0] != null)
                    {
                        Users[0].EmailConfirmed = true;
                        Users[0].active = true;
                    }
                    dal.SaveChanges();
                }
                catch
                {
                    return View("Login");
                }
                return RedirectToAction("Login");
            }
            return View("Login");
            /*
            Random rnd = new Random();
            int code = rnd.Next(1000000, 9999999);
            TempData["UserName"] = un;
            TempData["ConfirmCode"] = code;
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("studycollab11@gmail.com", "Stubrasil16");

            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress("studycollab11@gmail.com");
            MailMessage mm = new MailMessage(from.Address, to.Address, "Wellcome to StudyCollab", "Your verification code is: " + code + "\nThank you, SCteam.");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            return RedirectToAction("EmailConfirm");
            */
        }

        public ActionResult Submit()
        {

            string username = Request.Form["username"];
            string password = Request.Form["pass"]; //same name

            if (!(username == null || password == null))
            {
                List<User> Users = new List<User>();
                UserDal dal = new UserDal();
                try
                {
                    Users =
                    (from x in dal.Users
                     where x.UserName == username && x.Password == password
                     select x).ToList<User>();
                }
                catch
                {

                }

                if (Users.Any())
                {
                    if (!((bool) Users[0].EmailConfirmed))
                    {
                        Random rnd = new Random();
                        int code = rnd.Next(1000000, 9999999);
                        TempData["UserName"] = username;
                        TempData["Email"] = Users[0].Email;
                        TempData["ConfirmCode"] = code;
                        SmtpClient client = new SmtpClient();
                        client.Port = 587;
                        client.Host = "smtp.gmail.com";
                        client.EnableSsl = true;
                        client.Timeout = 10000;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential("studycollab11@gmail.com", "Stubrasil16");

                        MailAddress to = new MailAddress(TempData["Email"].ToString());
                        MailAddress from = new MailAddress("studycollab11@gmail.com");
                        MailMessage mm = new MailMessage(from.Address, to.Address, "Wellcome to StudyCollab", "Your verification code is: " + code + "\nThank you, SCteam.");
                        mm.BodyEncoding = UTF8Encoding.UTF8;
                        mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                        client.Send(mm);

                        return View("EmailConfirm", new User(Users[0]));
                    }
                    //Creates the user class as defined by rank
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
                            User usr = new User(Users[0]);
                            ViewData["CurrentUser"] = usr.UserName;
                            ViewBag.CurrentUser = usr;
                            break;
                    }
                    if (Users[0].active == true)
                    {
                        //return RedirectToAction("Contact", Users[0]);
                        return RedirectToAction("MainPage", "MainPage", Users[0]);
                    }
                    else
                    {
                        return RedirectToAction("BadLogin");
                    }
                }
                else
                {
                    return RedirectToAction("BadLogin");
                }

            }
            
            else return RedirectToAction("Login");



        }

        public ActionResult Signup()
        {
            

            return View("Signup");
        }

        public ActionResult SignupCont()
        {

            string username = Request.Form["Username"];
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string passwordConfirm = Request.Form["passwordConfirm"];
            string institution = null;
            if (!(Request.Form["institution"].Equals("")))
            {
                institution = Request.Form["institution"];
            }
            int? year = null;
            try
            {
                 year = Int32.Parse(Request.Form["year"]);
            }
            catch
            {
                year = null;
            }
            //if the password not the same return to the sign up form
            if (!password.Equals(passwordConfirm))return RedirectToAction("Signup");
            else
            {
                UserDal dal = new UserDal();
                User tempUsr = new User()
                {
                    UserName = username,
                    Password = password,
                    Email = email,
                    rank = 2,
                    institution = institution,
                    year = year,
                    EmailConfirmed = false,
                    Likes = 0,
                    studentUnionRank = 0,
                    downloadCounter = 0,
                    uploadCounter = 0

                };

                //Checks if a user with the same user name exists
                List<User> Users =
                (from x in dal.Users
                 where x.UserName == username
                 select x).ToList<User>();
                if(!(Users.Any()))
                {
                    Random rnd = new Random();
                    int code = rnd.Next(1000000, 9999999);
                    TempData["UserName"] = username;
                    TempData["Email"] = tempUsr.Email;
                    TempData["ConfirmCode"] = code;
                    SmtpClient client = new SmtpClient();
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("studycollab11@gmail.com", "Stubrasil16");

                    MailAddress to = new MailAddress(tempUsr.Email);
                    MailAddress from = new MailAddress("studycollab11@gmail.com");
                    MailMessage mm = new MailMessage(from.Address, to.Address, "Wellcome to StudyCollab", "Your verification code is: " + code + "\nThank you, SCteam.");
                    mm.BodyEncoding = UTF8Encoding.UTF8;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mm);

                    dal.Users.Add(tempUsr);
                    dal.SaveChanges();
                    ViewBag.model = tempUsr;
                    return  View("EmailConfirm", new User(tempUsr));
                }

            }

            return RedirectToAction("Signup");
        }

        public ActionResult BadLogin()
        {
            return View();
        }
    }
}