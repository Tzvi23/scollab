using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudentCollab.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using StudentCollab.Models;
using StudentCollab.Dal;

namespace StudentCollab.Tests
{
    [TestClass]
    public class ControllersTets
    {
        public TestContext TestContext { get; set; }
        public string testingThreadHeader = "Testing Thread !@#23343";
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
            var controller = new MainPageControllerTest();
            var result = controller.MainPage(new Models.User()) as ViewResult;
            Assert.AreEqual("MainPage", result.ViewName);
        }

        [TestMethod]
        public void TestBackToHome()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };

            //Act
            var result = (RedirectToRouteResult)controller.backToHome(cur);
            //Assert
            Assert.AreEqual("MainPage", result.RouteValues["controller"]);
        }

        // < !!!!--FAILING--!!! >
        /*[TestMethod]
        public void TestReport()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            string txt = "Test String";
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            //Act
            var result = controller.Report(txt, cur) as ViewResult;
            //Assert
            Assert.AreEqual("Report", result.View);
        }*/

        [TestMethod]
        public void TestUploadFile()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            //Act
            var result = controller.UploadFile(cur) as ViewResult;
            //Assert
            Assert.AreEqual("UploadFile", result.ViewName);
        }

        [TestMethod]
        public void TestDepartmentsPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            Institution inst = new Institution
            {
                InstitutionId = 1,
                InstName = "SCE"
            };
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            //Act
            var result = controller.DepartmentsPage(inst, cur) as ViewResult;
            //Assert
            Assert.AreEqual("DepartmentsPage", result.ViewName);
        }

        [TestMethod]
        public void TestSyearsPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            Department inst = new Department
            {
                DepartmentId = 1
            };
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            //Act
            var result = controller.SyearsPage(inst, cur) as ViewResult;
            //Assert
            Assert.AreEqual("SyearsPage", result.ViewName);
        }

        [TestMethod]
        public void TestThreadsPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            Syear inst = new Syear
            {
                SyearId = 1
            };
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            //Act
            var result = controller.ThreadsPage(inst, cur) as ViewResult;
            //Assert
            Assert.AreEqual("ThreadsPage", result.ViewName);
        }

        [TestMethod]
        public void TestLockThread()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            Thread test_Thread = new Thread
            {
                ThreadName = "Testing Thread",
                SyearId = 1,
                ThreadType = "[Question]",
                OwnerId = 1
            };
            //Add test thread to DB
            using (ThreadDal trdDal = new ThreadDal())
            {
                trdDal.Threads.Add(test_Thread);
                trdDal.SaveChanges();
            }

            //Act
            controller.LockThread(test_Thread);

            //Assert
            using (ThreadDal trdDal = new ThreadDal())
            {
                List<Thread> testTrd =
                    (from x in trdDal.Threads
                     where x.ThreadName == test_Thread.ThreadName
                     select x).ToList();
                Assert.AreEqual(testTrd[0].Locked, true);

                //Cleanup
                trdDal.Threads.Remove(testTrd[0]);
                trdDal.SaveChanges();
            }


        }

        [TestMethod]
        public void TestUnLockThread()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            Thread test_Thread = new Thread
            {
                ThreadName = testingThreadHeader,
                SyearId = 1,
                ThreadType = "[Question]",
                OwnerId = 1
            };
            //Add test thread to DB
            using (ThreadDal trdDal = new ThreadDal())
            {
                trdDal.Threads.Add(test_Thread);
                trdDal.SaveChanges();
            }

            //Act
            controller.UnLockThread(test_Thread);

            //Assert
            using (ThreadDal trdDal = new ThreadDal())
            {
                List<Thread> testTrd =
                    (from x in trdDal.Threads
                     where x.ThreadName == test_Thread.ThreadName
                     select x).ToList();
                Assert.AreEqual(testTrd[0].Locked, false);

                //Cleanup

                trdDal.Threads.Remove(testTrd[0]);
                trdDal.SaveChanges();
            }

        }

        [TestMethod]
        public void TestAdminPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();

            //Act
            var result = controller.AdminPage() as ViewResult;

            //Assert
            Assert.AreEqual("AdminPage", result.ViewName);
        }

        [TestMethod]
        public void TestNewThread()
        {
            //Arrange
            var controller = new MainPageControllerTest();

            //Act
            var result = controller.NewThread() as ViewResult;

            //Assert
            Assert.AreEqual("NewThread", result.ViewName);
        }

        [TestMethod]
        public void TestCreateNewThread()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            string content = "Test Content TESTING!!!!";
            Syear inst = new Syear
            {
                SyearId = 1
            };
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            Thread test_Thread = new Thread
            {
                ThreadName = testingThreadHeader,
                SyearId = 1,
                ThreadType = "[Question]",
                OwnerId = 1
            };

            //Act
            controller.CreateNewThread(inst, test_Thread, cur, content);

            //Assert
            using (ContentDal cnt = new ContentDal())
            {
                List<Content> cont =
                    (from x in cnt.Contents
                     where x.threadName == test_Thread.ThreadName
                     select x).ToList();
                Assert.AreEqual(content, cont[0].threadContent);

                //Cleanup
                cnt.Contents.Remove(cont[0]);
                cnt.SaveChanges();
            }

            //Cleanup Remove testing Thread
            //Add test thread to DB
            using (ThreadDal trdDal = new ThreadDal())
            {
                var result = trdDal.Threads.SingleOrDefault(b => b.ThreadName == test_Thread.ThreadName);
                trdDal.Threads.Remove(result);
                trdDal.SaveChanges();
            }

        }

        //[TestMethod]
        //public void TestEditThreadPage()
        //{
        //    //Arrange
        //    var controller = new MainPageControllerTest();
        //}

        [TestMethod]
        public void TestManagerPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };

            //Act
            var result = controller.ManagerPage(cur) as ViewResult;

            //Assert
            Assert.AreEqual("ManagerPage", result.ViewName);
        }

        [TestMethod]
        public void TestAgreemantPage()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            // Act
            var result = controller.AgreemantPage(cur) as ViewResult;

            //Assert
            Assert.AreEqual("AgreemantPage", result.ViewName);
        }

        [TestMethod]
        public void TestUpdateAgreemant()
        {
            //Arrange
            var controller = new MainPageControllerTest();
            User cur = new User
            {
                UserName = "tzvi",
                Password = "1234"
            };
            Other testOther = new Other();
            testOther.Id = 2;
            

            //Act
            controller.UpdateAgreemant(testOther, "NewAgreement Testing 123#@!");

            //Assert
            using (OtherDal dal = new OtherDal())
            {
                Other val = dal.Others.SingleOrDefault(b => b.Id == testOther.Id);
                Assert.AreEqual("NewAgreement Testing 123#@!", val.Val);

                controller.UpdateAgreemant(testOther, "Do not Delete - Line for testing");
            }

        }


    }
}