/*
 *  Copyright 2014 Jonathan Bradshaw. All rights reserved.
 *  Redistribution and use in source and binary forms, with or without modification, is permitted.
 */

using CodecoreTechnologies.Elve.DriverFramework;
using CodecoreTechnologies.Elve.DriverFramework.Scripting;

namespace Elve.Driver.RainforestEagle
{
    public partial class RainforestEagleDriver
    {
        #region Driver Properties

        [ScriptObjectProperty("Demand", "Gets the current demand.", "the {NAME} demand", null)]
        [SupportsDriverPropertyBinding("Demand Changed", "Occurs when the power demand changes.")]
        public ScriptNumber Demand
        {
            get { return new ScriptNumber(_usageData.Demand); }
        }

        [ScriptObjectProperty("Demand Cost", "Gets the estimated demand cost per hour.", "the {NAME} cost per hour", null)]
        [SupportsDriverPropertyBinding]
        public ScriptNumber DemandCost
        {
            get { return new ScriptNumber(CalculateDemandPrice()); }
        }

        [ScriptObjectProperty("Demand Timestamp", "Gets the current demand timestamp.", "the {NAME} demand timestamp", null)]
        [SupportsDriverPropertyBinding]
        public ScriptDateTime DemandTimestamp
        {
            get { return new ScriptDateTime(_usageData.DemandTimestamp); }
        }

        [ScriptObjectProperty("Demand Units", "Gets the current demand unit.", "the {NAME} demand unit", null)]
        [SupportsDriverPropertyBinding]
        public ScriptString DemandUnits
        {
            get { return new ScriptString(_usageData.DemandUnits ?? string.Empty); }
        }

        [ScriptObjectProperty("Mac Address", "Gets the meter hardware address.", "the {NAME} hardware address", null)]
        public ScriptString MacAddress
        {
            get { return new ScriptString(_eagleReaderService != null ? _eagleReaderService.MacId : "n/a"); }
        }

        [ScriptObjectProperty("Message Text", "Gets the current message text.", "the {NAME} current message", null)]
        [SupportsDriverPropertyBinding("Message Text Changed", "Occurs when the message text changes.")]
        public ScriptString MessageText
        {
            get { return new ScriptString(_usageData.MessageText ?? string.Empty); }
        }

        [ScriptObjectProperty("Message Timestamp", "Gets the current message timestamp.", "the {NAME} message timestamp", null)]
        [SupportsDriverPropertyBinding("Message Timestamp Changed", "Occurs when the message timestamp changes.")]
        public ScriptDateTime MessageTimestamp
        {
            get { return new ScriptDateTime(_usageData.MessageTimestamp); }
        }

        [ScriptObjectProperty("Meter Status", "Gets the meter status.", "the {NAME} meter status", null)]
        [SupportsDriverPropertyBinding("Meter Status Changed", "Occurs when the meter status changes.")]
        public ScriptString MeterStatus
        {
            get { return new ScriptString(_usageData.MeterStatus ?? string.Empty); }
        }

        [ScriptObjectProperty("Price", "Gets the price per unit of power.", "the {NAME} price", null)]
        [SupportsDriverPropertyBinding("Price Changed", "Occurs when the meter price value changes.")]
        public ScriptNumber Price
        {
            get { return new ScriptNumber(_usageData.Price); }
        }

        [ScriptObjectProperty("Price Units", "Gets the pricing unit.", "the {NAME} pricing unit", null)]
        [SupportsDriverPropertyBinding]
        public ScriptString PriceUnits
        {
            get { return new ScriptString(_usageData.PriceUnits ?? string.Empty); }
        }

        [ScriptObjectProperty("Total Delivered", "Gets the total power delivered from grid.", "the {NAME} power delivered", null)]
        [SupportsDriverPropertyBinding]
        public ScriptNumber SummationDelivered
        {
            get { return new ScriptNumber(_usageData.SummationDelivered); }
        }

        [ScriptObjectProperty("Total Produced", "Gets the total power received by the grid.", "the {NAME} power produced", null)]
        [SupportsDriverPropertyBinding]
        public ScriptNumber SummationReceived
        {
            get { return new ScriptNumber(_usageData.SummationReceived); }
        }

        [ScriptObjectProperty("Usage Timestamp", "Gets the current usage timestamp.", "the {NAME} usage timestamp", null)]
        [SupportsDriverPropertyBinding]
        public ScriptDateTime UsageTimestamp
        {
            get { return new ScriptDateTime(_usageData.UsageTimestamp); }
        }

        #endregion
    }
}
