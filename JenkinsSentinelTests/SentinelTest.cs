using JenkinsSentinel.src;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JenkinsSentinelTests
{
    /// <summary>
    ///This is a test class for SentinelTest and is intended
    ///to contain all SentinelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SentinelTest
    {
        public SentinelTest()
        {
            sentinel = new Sentinel();
        }

        private Sentinel sentinel;

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


        [TestMethod()]
        public void MoveZeroJobIndexUpTest()
        {
            sentinel = new Sentinel();
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 0),
                new JenkinsJob("j", "u2", false, 1),
                new JenkinsJob("j", "u3", false, 2),
                new JenkinsJob("j", "u4", false, 3),
            };
            JenkinsJob Job = sentinel.Jobs[0];
            sentinel.MoveJobIndexUp(Job);
            Assert.AreEqual(sentinel.Jobs[0].Index, 0);
            Assert.AreEqual(sentinel.Jobs[1].Index, 1);
        }

        [TestMethod()]
        public void MoveJobIndexUpTest()
        {
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 0),
                new JenkinsJob("j", "u2", false, 1),
                new JenkinsJob("j", "u3", false, 2),
                new JenkinsJob("j", "u4", false, 3),
            };
            JenkinsJob Job = sentinel.Jobs[1];
            sentinel.MoveJobIndexUp(Job);
            Assert.AreEqual(sentinel.Jobs[0].Index, 1);
            Assert.AreEqual(sentinel.Jobs[1].Index, 0);
        }

        [TestMethod()]
        public void MoveInnerJobIndexUpTest()
        {
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 0),
                new JenkinsJob("j", "u2", false, 1),
                new JenkinsJob("j", "u3", false, 2),
                new JenkinsJob("j", "u4", false, 3),
            };
            JenkinsJob Job = sentinel.Jobs[2];
            sentinel.MoveJobIndexUp(Job);
            Assert.AreEqual(sentinel.Jobs[0].Index, 0);
            Assert.AreEqual(sentinel.Jobs[1].Index, 2);
            Assert.AreEqual(sentinel.Jobs[2].Index, 1);
            Assert.AreEqual(sentinel.Jobs[3].Index, 3);
        }

        [TestMethod()]
        public void MoveJobIndexUpMultipleTimesTest()
        {
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 0),
                new JenkinsJob("j", "u2", false, 1),
                new JenkinsJob("j", "u3", false, 2),
                new JenkinsJob("j", "u4", false, 3),
            };
            JenkinsJob Job = sentinel.Jobs[3];
            sentinel.MoveJobIndexUp(Job);
            sentinel.MoveJobIndexUp(Job);
            sentinel.MoveJobIndexUp(Job);
            sentinel.MoveJobIndexUp(Job); // At this point it is already on 0 position
            Assert.AreEqual(sentinel.Jobs[0].Index, 1);
            Assert.AreEqual(sentinel.Jobs[1].Index, 2);
            Assert.AreEqual(sentinel.Jobs[2].Index, 3);
            Assert.AreEqual(sentinel.Jobs[3].Index, 0);
        }

        [TestMethod()]
        public void MoveJobIndexDownMultipleTimesTest()
        {
            sentinel.LastIndex = 3;
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 0),
                new JenkinsJob("j", "u2", false, 1),
                new JenkinsJob("j", "u3", false, 2),
                new JenkinsJob("j", "u4", false, 3),
            };
            JenkinsJob Job = sentinel.Jobs[0];
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job); // At this point it is already on 10 position
            Assert.AreEqual(sentinel.Jobs[0].Index, 3);
            Assert.AreEqual(sentinel.Jobs[1].Index, 0);
            Assert.AreEqual(sentinel.Jobs[2].Index, 1);
            Assert.AreEqual(sentinel.Jobs[3].Index, 2);
        }


        [TestMethod()]
        public void MoveJobIndexDownMultipleTimesWithRemovedItemsTest()
        {
            sentinel.LastIndex = 15;
            sentinel.Jobs = new System.Collections.Generic.List<JenkinsJob>()
            { 
                new JenkinsJob("j", "u1", false, 1),
                new JenkinsJob("j", "u2", false, 3),
                new JenkinsJob("j", "u3", false, 7),
                new JenkinsJob("j", "u4", false, 10),
            };
            JenkinsJob Job = sentinel.Jobs[0];
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job);
            sentinel.MoveJobIndexDown(Job); // At this point it is already on 10 position
            Assert.AreEqual(sentinel.Jobs[0].Index, 10);
            Assert.AreEqual(sentinel.Jobs[1].Index, 1);
            Assert.AreEqual(sentinel.Jobs[2].Index, 3);
            Assert.AreEqual(sentinel.Jobs[3].Index, 7);
        }
    }
}
