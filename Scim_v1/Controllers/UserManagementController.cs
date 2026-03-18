using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scim_v1.Context;
using Scim_v1.Models.ViewModels;
using Scim_v1.Models;

namespace Scim_v1.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserDbContext _context;

        public UserManagementController(UserDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();

                var user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
