using CourseWorkData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;

namespace CourseWorkData.Controllers
{
    public class LoginController : Controller
    {
        private ApplicationContext db;
        public LoginController(ApplicationContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult CreateLogin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLogin(LoginModel model)
        {
            var us = (from user in db.LoginModels
                          where (user.Password == model.Password)
                          select user).FirstOrDefault();

            if (us != null)
                {
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultRoleClaimType, us.URole.ToString()) };
                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                    return RedirectToAction("Index", "Home");
                }

            ModelState.AddModelError("", "Некорректный пароль");          
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("CreateLogin");
        }
    }
}
