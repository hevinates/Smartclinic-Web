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

        // LOGIN GET
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

            // ⭐ ROL BURADA SESSION'A KAYDEDİLİYOR
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetInt32("UserId", user.Id);

            if (role == "doctor")
                return RedirectToAction("Dashboard", "Doctor");

            if (role == "patient")
                return RedirectToAction("Dashboard", "Patient");

            return RedirectToAction("Index", "Home");
        }

        // REGISTER GET 
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

        // API: GET USER BY EMAIL
        [HttpGet]
        [Route("api/auth/user/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.Role
            });
        }

        // API: LOGIN (for mobile app)
        [HttpPost]
        [Route("api/auth/login")]
        public IActionResult ApiLogin([FromBody] LoginRequest request)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized(new { message = "Email veya şifre hatalı" });

            return Ok(new
            {
                message = "Giriş başarılı",
                user = new
                {
                    id = user.Id,
                    firstName = user.Name,
                    lastName = user.Surname,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
    }

    // DTO for login request
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
