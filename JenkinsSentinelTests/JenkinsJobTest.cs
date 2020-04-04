using JenkinsSentinel.src;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JenkinsSentinel.src.jenkinsdata;

namespace JenkinsSentinelTests
{
    
    
    /// <summary>
    ///This is a test class for JenkinsJobTest and is intended
    ///to contain all JenkinsJobTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JenkinsJobTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Notify if build is completed, status is red, new job has started already, NotifyWhenBuildIsComplete
        ///</summary>
        [TestMethod()]
        public void DoNotifyWhenBuildIsCompleteTest1()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenBuildIsComplete = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = true, number = 251, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "FAILED" },
            };

            bool expected = true;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Notify if build is completed, status is red, new job has not started yet, NotifyWhenBuildIsComplete
        ///</summary>
        [TestMethod()]
        public void DoNotifyWhenBuildIsCompleteTest2()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenBuildIsComplete = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = false, number = 250, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "FAILED" },
            };

            bool expected = true;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Notify if build is not completed, NotifyWhenBuildIsComplete
        ///</summary>
        [TestMethod()]
        public void DoNotNotifyWhenBuildIsCompleteTest3()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenBuildIsComplete = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = true, number = 250, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 249, result = "FAILED" },
            };

            bool expected = false;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///Notify if build is completed, status is red, new job has started already, NotifyWhenJobStateChanges
        ///</summary>
        [TestMethod()]
        public void DoNotifyWhenJobStateChangesTest1()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenJobStateChanges = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = true, number = 251, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "SUCCESS" },
            };

            bool expected = true;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Notify if build is completed, status is red, new job has not started yet, NotifyWhenJobStateChanges
        ///</summary>
        [TestMethod()]
        public void DoNotifyWhenJobStateChangesTest2()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenJobStateChanges = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = false, number = 250, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "SUCCESS" },
            };

            bool expected = true;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Notify if build is completed, status is red, new job has not started yet, NotifyWhenJobStateChanges
        ///</summary>
        [TestMethod()]
        public void DoNotNotifyWhenJobStateChangesTest3()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenJobStateChanges = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = false, number = 250, result = "FAILED" },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "FAILED" },
            };

            bool expected = false;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///Notify if build is completed, status is red, new job has started already, NotifyWhenJobStateChanges
        ///</summary>
        [TestMethod()]
        public void DoNotNotifyWhenJobStateChangesTest4()
        {
            JenkinsJob currentJob = new JenkinsJob("name", "url", false, 0)
            {
                JobStatus = "FAILED",
                LastComletedBuildNumber = 249,
                Building = true,
                NotifySettings = new NotifySettings() { NotifyWhenJobStateChanges = true }
            };


            JenkinsWorkflow NewStatus = new WorkflowJob()
            {
                lastBuild = new WorkflowRun() { building = true, number = 251, result = null },
                lastCompletedBuild = new WorkflowRun() { building = false, number = 250, result = "FAILED" },
            };

            bool expected = false;
            bool actual = currentJob.DoNotify(NewStatus);
            Assert.AreEqual(expected, actual);
        }
    }
}
