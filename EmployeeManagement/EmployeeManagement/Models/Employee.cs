using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public DateTime DateOfJoining { get; set; }

        [Range(0, 9999999999.99)]
        public decimal Salary { get; set; }

        [Required]
        [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be a 10-digit number.")]
        public long PhoneNumber { get; set; }
    }
}
