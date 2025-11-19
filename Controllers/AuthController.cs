using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using smartclinic_web.Data;
using smartclinic_web.Models;
using System.Linq;

namespace smartclinic_web.Controllers
{
    public class AuthController : Controller
    {
        private readonly SmartClinicDbContext _context;

        public AuthController(SmartClinicDbContext context)
        {
            _context = context;
        }

        // LOGIN SAYFASI
        public IActionResult Login(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public IActionResult Login(string role, string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password && u.Role == role);

            if (user == null)
            {
                ViewBag.Role = role;
                ViewBag.Error = "Email veya şifre hatalı.";
                return View();
            }

            // *** KULLANICI ID'SINI SESSION'A KAYDET ***
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role ?? "unknown");

            // ROLE'E GÖRE DASHBOARD'A GÖNDER
            if (role == "doctor")
                return RedirectToAction("Dashboard", "Doctor");

            if (role == "patient")
                return RedirectToAction("Dashboard", "Patient");

            return RedirectToAction("Index", "Home");
        }

        // REGISTER SAYFASI
        public IActionResult Register(string role)
        {
            ViewBag.Role = role;
            return View();
        }

        // REGISTER POST
        [HttpPost]
        public IActionResult Register(string role, string name, string surname, string email, string password)
        {
            var newUser = new User
            {
                Name = name,
                Surname = surname,
                Email = email,
                Password = password,
                Role = role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login", new { role = role });
        }
    }
}
