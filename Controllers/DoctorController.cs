using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using smartclinic_web.Data;
using smartclinic_web.Models;
using System.Linq;

namespace smartclinic_web.Controllers
{
    public class DoctorController : Controller
    {
        private readonly SmartClinicDbContext _context;

        public DoctorController(SmartClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // ⭐ PROFİL SAYFASI (GET)
        public IActionResult Profile()
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Auth", new { role = "doctor" });

            var doctor = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "doctor");

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // ⭐ PROFİL GÜNCELLEME (POST)
        [HttpPost]
        public IActionResult Profile(User model)
        {
            int? id = HttpContext.Session.GetInt32("UserId");

            if (id == null)
                return RedirectToAction("Login", "Auth", new { role = "doctor" });

            var doctor = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "doctor");

            if (doctor == null)
                return NotFound();

            doctor.Name = model.Name;
            doctor.Surname = model.Surname;
            doctor.DoctorHospital = model.DoctorHospital;

            _context.SaveChanges();

            TempData["Success"] = "Profil başarıyla güncellendi.";

            return RedirectToAction("Profile");
        }
    }
}
