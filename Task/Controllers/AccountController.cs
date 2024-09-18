using System.Linq;
using System.Web.Mvc;
using Task.Models;
using BCrypt.Net;
using System;

namespace Task.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaskManagementDBContext _context;

        public AccountController()
        {
            _context = new TaskManagementDBContext();
        }


        public ActionResult Signup()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Signup(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View(model);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.SignupDate = DateTime.Now;

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = passwordHash,
                    Role = model.Role.ToString(),
                    SignupDate = model.SignupDate,
                    IsBlocked = false,
                    IsDeleted = false
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(Users model)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (user != null)
            {
                if (user.IsDeleted)
                {
                    ModelState.AddModelError("", "Account is deleted.");
                    return View(model);
                }

                if (user.IsBlocked)
                {
                    ModelState.AddModelError("", "Account is blocked.");
                    return View(model);
                }

                if (BCrypt.Net.BCrypt.Verify(model.PasswordHash, user.PasswordHash))
                {

                    if (user.Role == "1")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Role == "2")
                    {
                        return RedirectToAction("DashBoard", "Manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Invalid username or password");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

        // GET: Logout
        public ActionResult Logout()
        {

            return RedirectToAction("Login");
        }
    }
}