using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentCollab.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace StudentCollab.Tests
{
    [TestClass]
    public class ControllersTets
    {
        [TestMethod]
        public void TestLoginView()
        {
            var controller = new LoginController();
            var result = controller.Login() as ViewResult;
            Assert.AreEqual("Login", result.ViewName);
        }

        [TestMethod]
        public void TestSignupView()
        {
            var controller = new LoginController();
            var result = controller.Signup() as ViewResult;
            Assert.AreEqual("Signup", result.ViewName);
        }

        [TestMethod]
        public void TestMainPage()
        {
            var controller = new MainPageController();
            var result = controller.MainPage(new Models.User()) as ViewResult;
            Assert.AreEqual("MainPage", result.ViewName);
        }
    }
}