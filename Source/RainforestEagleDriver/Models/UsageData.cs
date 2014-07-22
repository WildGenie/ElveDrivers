using System;

namespace Elve.Driver.RainforestEagle.Models
{
    /// <summary>
    /// Represents the results from the Rainforest Eagle for current usage data.
    /// </summary>
    internal sealed class UsageData
    {
        #region Private Fields

        private DateTime _demandTimestamp;
        private DateTime _messageTimestamp;
        private DateTime _usageTimestamp;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        ///     Gets or sets the demand.
        /// </summary>
        /// <value>
        ///     The demand.
        /// </value>
        public double Demand { get; set; }

        /// <summary>
        ///     Gets or sets the demand timestamp.
        /// </summary>
        /// <value>
        ///     The demand timestamp.
        /// </value>
        public DateTime DemandTimestamp
        {
            get { return _demandTimestamp; }
            set { _demandTimestamp = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        }

        /// <summary>
        ///     Gets or sets the demand units.
        /// </summary>
        /// <value>
        ///     The demand units.
        /// </value>
        public string DemandUnits { get; set; }

        /// <summary>
        ///     Gets or sets the message confirmed.
        /// </summary>
        /// <value>
        ///     The message confirmed.
        /// </value>
        public string MessageConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets the message confirm required.
        /// </summary>
        /// <value>
        ///     The message confirm required.
        /// </value>
        public string MessageConfirmRequired { get; set; }

        /// <summary>
        ///     Gets or sets the message identifier.
        /// </summary>
        /// <value>
        ///     The message identifier.
        /// </value>
        public string MessageId { get; set; }

        /// <summary>
        ///     Gets or sets the message priority.
        /// </summary>
        /// <value>
        ///     The message priority.
        /// </value>
        public string MessagePriority { get; set; }

        /// <summary>
        ///     Gets or sets the message queue.
        /// </summary>
        /// <value>
        ///     The message queue.
        /// </value>
        public string MessageQueue { get; set; }

        /// <summary>
        ///     Gets or sets the message read.
        /// </summary>
        /// <value>
        ///     The message read.
        /// </value>
        public string MessageRead { get; set; }

        /// <summary>
        ///     Gets or sets the message text.
        /// </summary>
        /// <value>
        ///     The message text.
        /// </value>
        public string MessageText { get; set; }

        /// <summary>
        ///     Gets or sets the message timestamp.
        /// </summary>
        /// <value>
        ///     The message timestamp.
        /// </value>
        public DateTime MessageTimestamp
        {
            get { return _messageTimestamp; }
            set { _messageTimestamp = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        }

        /// <summary>
        ///     Gets or sets the meter status.
        /// </summary>
        /// <value>
        ///     The meter status.
        /// </value>
        public string MeterStatus { get; set; }
        /// <summary>
        ///     Gets or sets the price.
        /// </summary>
        /// <value>
        ///     The price.
        /// </value>
        public double Price { get; set; }

        /// <summary>
        ///     Gets or sets the price label.
        /// </summary>
        /// <value>
        ///     The price label.
        /// </value>
        public string PriceLabel { get; set; }

        public string PriceUnits { get; set; }

        /// <summary>
        ///     Gets or sets the summation delivered.
        /// </summary>
        /// <value>
        ///     The summation delivered.
        /// </value>
        public double SummationDelivered { get; set; }

        /// <summary>
        ///     Gets or sets the summation received.
        /// </summary>
        /// <value>
        ///     The summation received.
        /// </value>
        public double SummationReceived { get; set; }

        /// <summary>
        ///     Gets or sets the summation units.
        /// </summary>
        /// <value>
        ///     The summation units.
        /// </value>
        public string SummationUnits { get; set; }

        /// <summary>
        ///     Gets or sets the usage timestamp.
        /// </summary>
        /// <value>
        ///     The usage timestamp.
        /// </value>
        public DateTime UsageTimestamp
        {
            get { return _usageTimestamp; }
            set { _usageTimestamp = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        }

        #endregion Public Properties
    }
}