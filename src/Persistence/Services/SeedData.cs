using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Models;


using Auth.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Services
{
    public class SeedData
    {


        public SeedData()
        {

        }
        public static void EnsureSeedData(ApplicationContext context, UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {



            context.Database.Migrate();
            if (!roleMgr.Roles.Any())
            {
                roleMgr.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                roleMgr.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
            }


            if (userMgr.Users.Any()) return;

            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    DisplayImage = "default.jpg",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                else
                {
                    userMgr.AddToRoleAsync(alice, "User");
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(ClaimTypes.Name , "Alice Smith"),
                            new Claim("DisplayImage", alice.DisplayImage),
                            new Claim(ClaimTypes.Role, "User"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                else
                {
                    userMgr.AddToRoleAsync(alice, "Admin");
                }


            }
            else
            {

            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    DisplayImage = "default.jpg",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                else
                {
                    userMgr.AddToRoleAsync(bob, "Admin");
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(ClaimTypes.Name, "Bob Smith"),
                            new Claim("DisplayImage", bob.DisplayImage),
                            new Claim(ClaimTypes.Role, "Admin"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                // Log.Debug("bob created");
            }
            else
            {
                //  Log.Debug("bob already exists");
            }
        }
    }
}