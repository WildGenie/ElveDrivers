/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using CodecoreTechnologies.Elve.DriverFramework;

namespace Elve.Driver.RainforestEagle
{
    public partial class RainforestEagleDriver
    {
        [DriverSetting("HostName", "The host name or ip address of the Rainforest EAGLE™ Gateway.", null, true)]
        public string HostNameSetting
        {
            set { _gatewayIpAddress = value; }
        }

        [DriverSetting("Poll Interval", "The interval (in seconds) between polls for current energy data.", 10, 60, "20", false)]
        public int PollingIntervalSetting
        {
            set { _pollingInterval = value * 1000; }
        }
    }
}
