using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class OtpLogin
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int OtpCode { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
