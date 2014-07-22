using System;

namespace Elve.Driver.Nest.Models
{
    internal class OAuth2Response
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the expires in access token value.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Gets the created time of this access token.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTimeOffset Created { get; private set; }

        /// <summary>
        /// Gets the time this access token expires at.
        /// </summary>
        /// <value>
        /// The expires at.
        /// </value>
        public DateTimeOffset ExpiresAt
        {
            get { return Created + TimeSpan.FromSeconds(ExpiresIn); }
        }

        /// <summary>
        /// Gets a value indicating whether this access token is expired.
        /// </summary>
        /// <value>
        /// <c>true</c> if this access token is expired; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpired
        {
            get { return DateTimeOffset.UtcNow > ExpiresAt; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Response"/> class.
        /// </summary>
        public OAuth2Response()
        {
            Created = DateTimeOffset.UtcNow;
        }
    }
}
