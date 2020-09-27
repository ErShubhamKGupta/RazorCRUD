using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int Mobile { get; set; }

        public string Company { get; set; }
    }
}
