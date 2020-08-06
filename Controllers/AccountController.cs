
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppStoreApiWithIdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppStoreApiWithIdentityServer4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> loginManager;
        private readonly RoleManager<Role> roleManager;


        public AccountController(UserManager<User> userManager,
           SignInManager<User> loginManager,
           RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.loginManager = loginManager;
            this.roleManager = roleManager;
        }
        [Authorize(Roles ="admin")]
        [Route("admin")]
        [HttpPost]
        public ActionResult<BasicResult> RegisterAdmin(RegisterModel obj)
        {
            String txt;
            User user = new User();
            user.UserName = obj.UserName;
            user.Email = obj.Email;
            user.FullName = obj.FullName;
            user.Description = obj.Description;

            IdentityResult result = userManager.CreateAsync(user, obj.Password).Result;

            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("admin").Result)
                {
                    Role role = new Role();
                    role.Name = "admin";
                    role.Description = "Perform admin operations.";
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Error while creating role!");
                        txt = "Error in Creating Role";
                    }
                }

                userManager.AddToRoleAsync(user, "admin").Wait();
                txt = "Done";
            }
            else
            {
                txt = "Error";
            }

            return new BasicResult { txt = txt };
        }

        [Authorize(Roles = "admin")]
        [Route("dev")]
        [HttpPost]
        public ActionResult<BasicResult> RegisterDev(RegisterModel obj)
        {
            String txt;
            User user = new User();
            user.UserName = obj.UserName;
            user.Email = obj.Email;
            user.FullName = obj.FullName;
            user.Description = obj.Description;


            IdentityResult result = userManager.CreateAsync(user, obj.Password).Result;

            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("dev").Result)
                {
                    Role role = new Role();
                    role.Name = "dev";
                    role.Description = "Perform Developer operations.";
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Error while creating role!");
                        txt = "Error in Creating Role";
                    }
                }

                userManager.AddToRoleAsync(user, "dev").Wait();
                txt = "Done";
            }
            else
            {
                txt = "Error";
            }

            return new BasicResult { txt = txt };
        }

        [Authorize(Roles = "admin")]
        [Route("user")]
        [HttpPost]
        public ActionResult<BasicResult> RegisterUser(RegisterModel obj)
        {
            String txt;
            User user = new User();
            user.UserName = obj.UserName;
            user.Email = obj.Email;
            user.FullName = obj.FullName;
            user.Description = obj.Description;


            IdentityResult result = userManager.CreateAsync(user, obj.Password).Result;

            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("user").Result)
                {
                    Role role = new Role();
                    role.Name = "user";
                    role.Description = "Perform user operations.";
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Error while creating role!");
                        txt = "Error in Creating Role";
                    }
                }

                userManager.AddToRoleAsync(user, "dev").Wait();
                txt = "Done";
            }
            else
            {
                txt = "Error";
            }

            return new BasicResult { txt = txt };
        }


        [Route("login")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult<BasicResult> Login(LoginModel obj)
        {
            String txt;
            var result = loginManager.PasswordSignInAsync
            (obj.UserName, obj.Password,true, false).Result;

            if (result.Succeeded)
            {
                txt="Done";
            }
            else
            {

                txt = "Invalid Login";
            }

            return new BasicResult { txt = txt };
        }

      
        [Route("logout")]
        [HttpPost]
        public ActionResult<BasicResult> LogOff()
        {
            
            loginManager.SignOutAsync().Wait();
            return new BasicResult { txt = "Done" };
        }

    }
}
