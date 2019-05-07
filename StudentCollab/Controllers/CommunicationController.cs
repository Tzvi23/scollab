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
    }
}