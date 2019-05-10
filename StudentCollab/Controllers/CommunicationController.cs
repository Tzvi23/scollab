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

        public ActionResult saveMsg(User usr)
        {

            string rec = Request.Form["to"];
            string subj = Request.Form["subject"];
            string msgC = Request.Form["msgContent"];

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