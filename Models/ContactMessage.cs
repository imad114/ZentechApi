using System;

namespace Zentech.Models
{
    public class ContactMessage
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumbre { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Country { get; set; }

    }
}
