using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.DriverTestHarness;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BenqProjectorDriver.Tests
{
    [TestClass]
    public class BenqProjectorTests
    {
        #region Private Fields

        private readonly Dictionary<string, byte[]> _configFiles = new Dictionary<string, byte[]>();
        private readonly ILogger _logger = new TraceLogger(LoggerContextType.Driver, "UnitTest", LoggerVerbosity.Diagnostic);
        private readonly TestRuleDictionary _rules = new TestRuleDictionary();
        private readonly TestDeviceSettingDictionary _settings = new TestDeviceSettingDictionary();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Elve.Driver.BenqProjector.BenqProjectorDriver _driver;

        #endregion Private Fields

        #region Test Framework Methods

        /// <summary>
        /// Cleanup test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            _driver.StopDriver();
            _stopwatch.Stop();
            Trace.TraceInformation("Stopped driver instance. Total runtime: {0}", _stopwatch.Elapsed);
        }

        /// <summary>
        /// Initializes test resources.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _settings.Add(new TestDeviceSetting("NetworkAddressSetting", "172.16.1.115"));
            _settings.Add(new TestDeviceSetting("NetworkPortNumberSetting", "1070"));

            Trace.TraceInformation("Creating driver instance");
            _driver = (Elve.Driver.BenqProjector.BenqProjectorDriver)DeviceFactory.CreateAndStartDevice(
                typeof(Elve.Driver.BenqProjector.BenqProjectorDriver),
                _configFiles,
                _settings,
                _rules,
                _logger);

            _stopwatch.Reset();
            _stopwatch.Start();

            for (var count = 20; !_driver.IsReady && count > 0; count--) Thread.Sleep(1000);
        }

        #endregion Test Framework Methods

        #region Unit Tests

        [TestMethod]
        public void TestDriverIsReady()
        {
            Assert.IsTrue(_driver.IsReady, "Driver must signal it is ready");
        }

        [TestMethod]
        public void TestLampHoursNonZero()
        {
            Assert.IsTrue(_driver.LampHours.ToPrimitiveInt32() > 0, "Lamp hours should be greater than zero");
        }

        [TestMethod]
        public void TestProjectorPowerOn()
        {
            _driver.TurnPowerOn();

            for (var count = 5; !_driver.PowerState && count > 0; count--) Thread.Sleep(1000);
            Assert.IsTrue(_driver.PowerState, "Projector Power state should have been true (on)");
        }

        [TestMethod]
        public void TestAspectRatio()
        {
            var expected = new ScriptNumber(4);
            _driver.AspectRatio = expected;
            for (var count = 15; _driver.AspectRatio.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.AspectRatio.ToPrimitiveInt32(), "Projector aspect ratio not updated");

            expected = new ScriptNumber(5);
            _driver.AspectRatio = expected;
            for (var count = 15; _driver.AspectRatio.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.AspectRatio.ToPrimitiveInt32(), "Projector aspect ratio not updated");
        }

        [TestMethod]
        public void TestCurrentSource()
        {
            var expected = new ScriptNumber(7);
            _driver.CurrentSource = expected;
            for (var count = 15; _driver.CurrentSource.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.CurrentSource.ToPrimitiveInt32(), "Projector current source not updated");

            expected = new ScriptNumber(6);
            _driver.CurrentSource = expected;
            for (var count = 15; _driver.CurrentSource.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.CurrentSource.ToPrimitiveInt32(), "Projector current source not updated");

        }

        [TestMethod]
        public void TestPictureMode()
        {
            var expected = new ScriptNumber(10);
            _driver.PictureMode = expected;
            for (var count = 15; _driver.PictureMode.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.PictureMode.ToPrimitiveInt32(), "Projector picture mode not updated");

            expected = new ScriptNumber(9);
            _driver.PictureMode = expected;
            for (var count = 15; _driver.PictureMode.ToPrimitiveInt32() != expected.ToPrimitiveInt32() && count > 0; count--) Thread.Sleep(1000);
            Assert.AreEqual(expected.ToPrimitiveInt32(), _driver.PictureMode.ToPrimitiveInt32(), "Projector picture mode not updated");
        }

        [TestMethod]
        public void TestProjectorPowerOff()
        {
            _driver.TurnPowerOff();

            for (var count = 15; _driver.PowerState && count > 0; count--) Thread.Sleep(1000);
            Assert.IsFalse(_driver.PowerState, "Projector Power state should have been false (off)");
        }

        #endregion Unit Tests
    }
}