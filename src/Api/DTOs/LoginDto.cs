using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }

    public class RoleDto
    {
        public string Name { get; set; }
    }

    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

}