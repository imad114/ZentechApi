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
        public string? Country { get; set; }
        public string? Topic { get; set; }
        public string? Role { get; set; }
        public string? UserAgent { get; set; }  // For storing user agent
        public string? IPAddress { get; set; }  // For storing the IP address
        public string? MachineName { get; set; }  // For storing the machine name or host


    }
}
