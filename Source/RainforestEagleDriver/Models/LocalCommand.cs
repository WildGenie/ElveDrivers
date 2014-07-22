namespace Elve.Driver.RainforestEagle.Models
{
    /// <summary>
    /// Represents a command sent to the Eagle Rainforest.
    /// </summary>
    internal sealed class LocalCommand
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Mac identifier (required).
        /// </summary>
        public string MacId { get; set; }

        /// <summary>
        /// Gets or sets the command name (required).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the period for usage results (only used for certain commands).
        /// </summary>
        public string Period { get; set; }

        #endregion Public Properties
    }
}