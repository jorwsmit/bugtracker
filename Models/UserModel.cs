using System;
using System.ComponentModel.DataAnnotations;

namespace bugtracker.Models
{
    public class UserModel
    {
        public int id { get; set; }
        [Required]
        public string email { get; set; }
        public string password { get; set; }
        public string token { get; set; }
    }
}