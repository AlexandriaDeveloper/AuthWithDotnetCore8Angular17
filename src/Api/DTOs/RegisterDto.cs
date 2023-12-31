using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class RegisterDto
    {
        public string DisplayName { get; set; }
        public string DisplayImage { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

    }
}