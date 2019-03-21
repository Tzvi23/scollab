using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCollab.Models;

namespace StudentCollab.Controllers
{
    public class MainPageController : Controller
    {
        // GET: MainPage
        public ActionResult MainPage(User usr)
        {
            return View("MainPage",usr);
        }

        public ActionResult logout()
        {

            return RedirectToAction("Login","Login", new User());
        }
    }
}