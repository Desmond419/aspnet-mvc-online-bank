using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShareResources
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNum { get; set; }
        public double Balance { get; set; }
        public string Card { get; set; }
        public string Ic { get; set; }

        [Display(Name = "Admin?")]
        public bool IsAdmin { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name="Photo")]    
        public string PhotoFileName { get; set; }

        public byte[] Data { get; set; }
    }
}