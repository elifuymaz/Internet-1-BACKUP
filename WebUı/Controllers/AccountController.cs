using System.Data;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUı.Models;
using WebUı.ViewModels;

namespace WebUı.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly INotyfService _notfyservice;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, INotyfService notfyservice)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notfyservice = notfyservice;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre hatalı!");
                _notfyservice.Warning("Email veya şifre hatalı!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    _notfyservice.Success("Giriş Başarılı");
                    return RedirectToAction("Index", "Admin"); // Yapılacak
                }
                else if (roles.Contains("Gazetici")) // düzenlenecek
                {
                    _notfyservice.Success("Giriş Başarılı");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _notfyservice.Success("Giriş Başarılı");
                    return RedirectToAction("Index", "Home");// düzenlenecek
                }
            
            }

            ModelState.AddModelError(string.Empty, "Email veya şifre hatalı!");
            _notfyservice.Warning("Email veya şifre hatalı!");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var usernames = model.Name.Trim() + model.Surname.Trim();
            var user = new AppUser
            {
                Email = model.Email,
                Surname = model.Surname,
                UserName = usernames,
                Name = model.Name                              
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: true);
                _notfyservice.Success("Giriş Başarılı");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _notfyservice.Warning(error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notfyservice.Success("Sistem Başarıyla Çıkıldı");
            return RedirectToAction("Index", "Home");
        }
    }
} 