using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _singInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> singInManager)
        {
            _userManager = userManager;
            _singInManager = singInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) 
            {
                View(register); 
            }

            AppUser user = new AppUser
            {
                Fullname = register.Fullname,
                UserName = register.Username,
                Email = register.Email,
                

            };

           IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View(register);
                }
            }

            //await _singInManager.SignInAsync(user, false);
            //return RedirectToAction("Index", "Home");

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                View(login);
            }

            AppUser user = await _userManager.FindByEmailAsync(login.UsernameOrEmail);

            if(user is null)
            {
                user = await _userManager.FindByNameAsync(login.UsernameOrEmail);
            }

            if(user == null)
            {
                ModelState.AddModelError("", "Email Or Passwor Wrong");
                return View(login);
            }

            var result = await _singInManager.PasswordSignInAsync(user, login.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Passwor Wrong");
                return View(login);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
