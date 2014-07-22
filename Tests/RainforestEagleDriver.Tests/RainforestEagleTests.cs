using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.DriverTestHarness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedLibrary;

namespace RainforestEagleDriver.Tests
{
    [TestClass]
    public class RainforestEagleTests
    {
        #region Private Fields

        private readonly Dictionary<string, byte[]> _configFiles = new Dictionary<string, byte[]>();
        private readonly ILogger _logger = new TraceLogger(LoggerContextType.Driver, "UnitTest", LoggerVerbosity.Diagnostic);
        private readonly TestRuleDictionary _rules = new TestRuleDictionary();
        private readonly TestDeviceSettingDictionary _settings = new TestDeviceSettingDictionary();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Driver _driver;

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
            _settings.Add(new TestDeviceSetting("HostNameSetting", "172.16.1.12"));

            Trace.TraceInformation("Creating driver instance");
            _driver = DeviceFactory.CreateAndStartDevice(
                typeof (Elve.Driver.RainforestEagle.RainforestEagleDriver),
                _configFiles,
                _settings,
                _rules,
                _logger);

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        #endregion Test Framework Methods

        #region Unit Tests

        /// <summary>
        /// Tests the driver is starts and becomes ready.
        /// </summary>
        [TestMethod]
        public void TestDriverIsReady()
        {
            var count = 20;
            while (!_driver.IsReady && count-- > 0) Thread.Sleep(1000);
            Assert.IsTrue(_driver.IsReady, "Driver must signal it is ready");
        }

        #endregion Unit Tests
    }
}
