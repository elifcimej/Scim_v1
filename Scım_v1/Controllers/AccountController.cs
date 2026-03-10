using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scım_v1.Context;
using Scım_v1.Models;
using Scım_v1.Models.ViewModels;
using System.Reflection.Metadata.Ecma335;

namespace Scım_v1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDbContext _context;
        public AccountController(UserDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == model.Username && x.IsActive);

            if(user == null)
            {
                ModelState.AddModelError("  ", "Kullanıcı bulunamadı.");
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password
            );

            if (result == PasswordVerificationResult.Success)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                 ModelState.AddModelError(" ", "Kullanıcı adı veya şifre yanlış.");

                return View(model);
            }


        }
    }
}
