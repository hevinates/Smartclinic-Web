using Microsoft.AspNetCore.Mvc;
using smartclinic_web.Data;
using smartclinic_web.Models;
using System.Linq;

namespace smartclinic_web.Controllers
{
    public class PatientController : Controller
    {
        private readonly SmartClinicDbContext _context;

        public PatientController(SmartClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // PROFİL SAYFASINI GETİR
        public IActionResult Profile(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Doktor listesi
            var doctors = _context.Users
                .Where(u => u.Role == "doctor")
                .Select(d => new { 
                    d.Id, 
                    FullName = d.Name + " " + d.Surname, 
                    d.DoctorHospital 
                })
                .ToList();

            ViewBag.Doctors = doctors;

            return View(user);
        }


        // PROFİLİ KAYDET (POST)
        
        [HttpPost]
        public IActionResult UpdateProfile(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

            if (user == null)
                return NotFound();

            // Güncelleme
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Age = model.Age;
            user.Gender = model.Gender;
            user.BloodGroup = model.BloodGroup;
            user.DoctorName = model.DoctorName;
            user.DoctorHospital = model.DoctorHospital;

            _context.SaveChanges();

            // 🔥 Success mesajı vermek istiyorsan TempData kullanmalısın
            TempData["Success"] = "Profil başarıyla güncellendi.";

            // 🔥 ÖNEMLİ → View döndürme, redirect yap
            return RedirectToAction("Profile", new { id = user.Id });
        }


        public IActionResult Tests()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult Chatbot()
        {
            return View();
        }

        public IActionResult Suggestions()
        {
            return View();
        }
    }
}
