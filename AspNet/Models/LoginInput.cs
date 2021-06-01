using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CIS665Project.Models
{
    public class LoginInput
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [UIHint("password")]
        [MaxLength(50)]
        public string UserPassword { get; set; }

        public string Role { get; set; }

        public string ReturnURL { get; set; }
    }
}
