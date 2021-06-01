using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIS655Project.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CIS665Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CIS655Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly Team116dbContext _context;

        public AccountController(Team116dbContext context)
        {
            _context = context;
        }

        // the returnURL captures the View the user was trying to reach before being redirected to the Login View

        public IActionResult Login(string returnURL)
        {
            // if returnURL is null or empty, it is set to "/" (i.e., Home/Index)

            returnURL = String.IsNullOrEmpty(returnURL) ? "~/Home/Index" : returnURL;

            // create a new instance of LoginInput and pass it to the Login View

            return View(new LoginInput { ReturnURL = returnURL });
        }

        // Post action (when user submits the Login form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,UserPassword,ReturnURL,Role")] LoginInput loginInput) {
            
            // check if login credentials are valid
            var aUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginInput.Username && u.Pwd == loginInput.UserPassword && u.UserRole.Role == loginInput.Role);
            var aBus = await _context.Businesses.FirstOrDefaultAsync(u => u.Email == loginInput.Username && u.Pwd == loginInput.UserPassword && u.BusinessRole.Role == loginInput.Role);
            

            //if valid
            if (aUser != null)
            {

                var uRole = await _context.UserRoles.FindAsync(aUser.UserId);
                //set claims
                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Sid, aUser.UserId.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, "User"));

                //set identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //issue authentication cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);



                // return the user to the View they were originally trying to reach or Home/Index
                return Redirect(loginInput?.ReturnURL ?? "~/Home/Index");

            }
            else if (aBus != null)
            {
                
                //set claims
                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Sid, aBus.BusId.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, "Business"));

                //set identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //issue authentication cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                
                
                if (!loginInput.ReturnURL.Contains("BIndex"))
                {
                    string url = loginInput.ReturnURL.Replace("Index", "BIndex");

                    return Redirect(url);
                }
                else
                {
                    string url = loginInput.ReturnURL;
                    return Redirect(url);
                }


                // return the user to the View they were originally trying to reach or Home/Index

            }
            else
            {
                ViewData["message"] = "Invalid credentials";
            }
            
            // return user to Login View
            return View(loginInput);
        }
        //end of login

        // GET: Sign up for an Account
        public IActionResult URegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> URegister(User user, UserRole urole)
        {
           
            
                //check for duplicate email address
                var aUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                // if no duplication
                if (aUser is null)
                {
                    // set default role to "user" and create new record
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    var newUser = _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email).Result;
                    urole.UserId = newUser.UserId;
                    urole.Role = "User";

                    //add role
                    _context.Add(urole);
                    await _context.SaveChangesAsync();

                    TempData["account"] = "User Account created successfully!";

                    // redirect to Login View

                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["email"] = "Choose a different email";
                    return View(user);
                }

        }

        // GET: Sign up for an Account
        public IActionResult BRegister()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BRegister(Business bus, BusinessRole brole)
        {

            //check for duplicate email address
            var aUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == bus.Email);

            // if no duplication
            if (aUser is null)
            {
                // set default role to "user" and create new record
                _context.Add(bus);
                await _context.SaveChangesAsync();

                var newUser = _context.Businesses.FirstOrDefaultAsync(u => u.Email == bus.Email).Result;
                brole.BusId = newUser.BusId;
                brole.Role = "Business";

                //add role
                _context.Add(brole);
                await _context.SaveChangesAsync();

                TempData["account"] = "Business Account created successfully!";

                // redirect to Login View

                return RedirectToAction(nameof(Login));
            }
            else
            {
                TempData["email"] = "Choose a different email";
                return View(bus);
            }

        }

        // method to log user out and redirect to Home View
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
