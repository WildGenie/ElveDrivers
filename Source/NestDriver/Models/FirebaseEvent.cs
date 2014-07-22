namespace Elve.Driver.Nest.Models
{
    internal class FirebaseEvent<T>
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        //public Dictionary<string, T> Data { get; set; }
        public T Data { get; set; }
    }
}