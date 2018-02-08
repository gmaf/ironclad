namespace Ironclad.Client
{
    public class User : UserSummary
    {
        // Summary:
        //     Gets or sets a telephone number for the user.
        public string PhoneNumber { get; set; }

        // Summary:
        //     Gets or sets a salted and hashed representation of the password for this user.
        public string Password { get; set; }
    }
}
