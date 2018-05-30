// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Client
{
    /// <summary>
    /// Represents a new user.
    /// </summary>
    public class NewUser : User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewUser"/> class.
        /// </summary>
        /// <param name="user">The base user object.</param>
        /// <param name="registrationLink">Registration link.</param>
        public NewUser(User user, string registrationLink)
        {
            this.Email = user.Email;
            this.ExternalLoginProvider = user.ExternalLoginProvider;
            this.Id = user.Id;
            this.Password = user.Password;
            this.PhoneNumber = user.PhoneNumber;
            this.Roles = user.Roles;
            this.Username = user.Username;
            this.RegistrationLink = registrationLink;
        }

        /// <summary>
        /// Gets the registration link for this user.
        /// </summary>
        public string RegistrationLink { get; }
    }
}
