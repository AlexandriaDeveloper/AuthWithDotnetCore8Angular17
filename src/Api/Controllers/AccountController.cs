using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        //private readonly IMapper _mapper;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager
             //  IMapper mapper
             )
        {
            // this._mapper = mapper;
            this._tokenService = tokenService;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._roleManager = roleManager;

        }


        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return new UserDto
                {
                    Email = user.Email,
                    Token = await _tokenService.CreateToken(user),
                    DisplayName = user.DisplayName,
                    DisplayImage = user.DisplayImage
                };
            else
            {
                return Forbid();
            }
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                DisplayImage = user.DisplayImage
            };
        }



        [HttpPost("AssignUserToRole")]
        public async Task<ActionResult<UserDto>> AssignUserToRole(string userId, string roleId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("User does not exist");
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return BadRequest("Role does not exist");
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (!result.Succeeded) return BadRequest("Failed to assign user to role");
            return Ok();
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<List<IdentityRole>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(x => x).ToList();


        }
        [HttpGet("GetRoleByUserId/{userId}")]

        public async Task<ActionResult<List<string>>> GetRoles(string userId)
        {
            //Only if you are an admin or you are logedin user 
            if (User.IsInRole("Admin") || User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value == userId)
            {

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return BadRequest("User does not exist");
                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            return Unauthorized();

        }
        [HttpPost("AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> AddRole(RoleDto roleDto)
        {
            var role = await _roleManager.FindByNameAsync(roleDto.Name);
            if (role != null) return BadRequest("Role already exists");
            var result = await _roleManager.CreateAsync(new IdentityRole(roleDto.Name));
            if (!result.Succeeded) return BadRequest();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("RemoveRole")]
        public async Task<ActionResult<UserDto>> RemoveRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return BadRequest("Role does not exist");
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded) return BadRequest();
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState.FirstOrDefault().Value);
            }

            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult("Email address is in use");
            }

            var user = new ApplicationUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username,
                DisplayImage = registerDto.DisplayImage
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault().Description);


            var roleAddResult = await _userManager.AddToRolesAsync(user, registerDto.Roles);
            if (!roleAddResult.Succeeded) return BadRequest("Failed to add to role");
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateToken(user),
                Email = user.Email,
                DisplayImage = user.DisplayImage
            };
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult<UserDto>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok();

        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "User")]


        public async Task<IActionResult> GetUsers()
        {

            return Content("hello from secure controller");
        }


    }


}