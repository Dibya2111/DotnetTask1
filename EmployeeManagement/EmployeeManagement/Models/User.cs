using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class User
    {
        [Key]
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        public string? UserName { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
