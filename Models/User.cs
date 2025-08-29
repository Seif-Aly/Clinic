﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clinic_Complex_Management_System1.Models
{
    public class User : IdentityUser<Guid>
    {
       // [Key]
        //public Guid Id { get; set; }

        //[Required]
       // [EmailAddress]
       // public string Email { get; set; }

       // [Required]
       // public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";


    }
}
