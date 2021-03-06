﻿using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using TShockAPI;
using Community.CsharpSqlite.SQLiteClient;
using TShockAPI.DB;
using Region = TShockAPI.DB.Region;

namespace UnitTests
{
    /// <summary>
    /// Summary description for RegionManagerTest
    /// </summary>
    [TestClass]
    public class RegionManagerTest
    {
        public static IDbConnection DB;
        public static RegionManager manager;
        [TestInitialize]
        public void Initialize()
        {
            TShock.Config = new ConfigFile();
            TShock.Config.StorageType = "sqlite";

            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", "tshock.test.sqlite"));
            DB.Open();

            manager = new RegionManager(DB);
            TShock.Regions = manager;
            manager.ReloadForUnitTest("test");
        }


        [TestMethod]
        public void AddRegion()
        {
            Region r = new Region( new Rectangle(100,100,100,100), "test", true, "test");
            Assert.IsTrue(manager.AddRegion(r.Area.X, r.Area.Y, r.Area.Width, r.Area.Height, r.Name, r.WorldID));
            Assert.AreEqual(1, manager.Regions.Count);
            Assert.IsNotNull(manager.ZacksGetRegionByName("test"));

            Region r2 = new Region(new Rectangle(201, 201, 100, 100), "test2", true, "test");
            manager.AddRegion(r2.Area.X, r2.Area.Y, r2.Area.Width, r2.Area.Height, r2.Name, r2.WorldID);
            Assert.AreEqual(2, manager.Regions.Count);
            Assert.IsNotNull(manager.ZacksGetRegionByName("test2"));
        }

        [TestMethod]
        public void DeleteRegion()
        {
            Assert.IsTrue(2 == manager.Regions.Count);
            Assert.IsTrue(manager.DeleteRegion("test"));
            Assert.IsTrue(1 == manager.Regions.Count);
            Assert.IsTrue(manager.DeleteRegion("test2"));
            Assert.IsTrue(0 == manager.Regions.Count);
        }

        [TestMethod]
        public void InRegion()
        {
            Assert.IsTrue(manager.InArea(100, 100));
            Assert.IsTrue(manager.InArea(150, 150));
            Assert.IsTrue(manager.InArea(200, 200));
            Assert.IsTrue(manager.InArea(201, 201));
            Assert.IsTrue(manager.InArea(251, 251));
            Assert.IsTrue(manager.InArea(301, 301));
            Assert.IsFalse(manager.InArea(311, 311));
            Assert.IsFalse(manager.InArea(99, 99));
        }

        [TestMethod]
        public void SetRegionState()
        {
            Assert.IsTrue(manager.ZacksGetRegionByName("test").DisableBuild);
            manager.SetRegionStateTest("test", "test", false);
            Assert.IsTrue(!manager.ZacksGetRegionByName("test").DisableBuild);
            manager.SetRegionStateTest("test", "test", true);
            Assert.IsTrue(manager.ZacksGetRegionByName("test").DisableBuild);
            Assert.IsTrue(manager.ZacksGetRegionByName("test2").DisableBuild);
            manager.SetRegionStateTest("test2", "test", false);
            Assert.IsTrue(!manager.ZacksGetRegionByName("test2").DisableBuild);
            manager.SetRegionStateTest("test2", "test", true);
            Assert.IsTrue(manager.ZacksGetRegionByName("test2").DisableBuild);
        }

        [TestMethod]
        public void CanBuild()
        {
            /**
             * For now, this test is useless.  Need to implement user groups so we can alter Canbuild permission.
             */
            TSPlayer t = new TSPlayer(0);
            Assert.IsFalse( manager.CanBuild( 100,100,t) );
        }

        [TestMethod]
        public void AddUser()
        {
            /**
             * For now, this test is useless.  Need to implement users so we have names to get ids from.
             */
        }

        [TestMethod]
        public void ListID()
        {
            Assert.IsTrue(RegionManager.ListIDs("1,2,3,4,5").Count == 5);
            Assert.IsTrue(RegionManager.ListIDs("").Count == 0);
        }

        [TestMethod]
        public void ListRegions()
        {
            //needs a little more work.
        }

        [TestCleanup]
        public void Cleanup()
        {
            DB.Close();
        }
    }
}
