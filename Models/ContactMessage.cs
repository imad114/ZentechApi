using System;

namespace Zentech.Models
{
    public class ContactMessage
    {
        public int ContactID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
