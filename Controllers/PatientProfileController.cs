using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartclinic_web.Data;
using smartclinic_web.Models;

namespace smartclinic_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientProfileController : ControllerBase
    {
        private readonly SmartClinicDbContext _context;

        public PatientProfileController(SmartClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientProfile/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _context.PatientProfiles
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound(new { message = "Profil bulunamadı" });
            }

            return Ok(new
            {
                profile.Id,
                profile.UserId,
                profile.FirstName,
                profile.LastName,
                profile.Age,
                profile.BloodGroup,
                profile.Height,
                profile.Weight,
                profile.DoctorId,
                DoctorName = profile.Doctor != null ? $"{profile.Doctor.Name} {profile.Doctor.Surname}" : null,
                DoctorHospital = profile.Doctor?.DoctorHospital
            });
        }

        // POST: api/PatientProfile
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProfile([FromBody] PatientProfile model)
        {
            var existingProfile = await _context.PatientProfiles
                .FirstOrDefaultAsync(p => p.UserId == model.UserId);

            if (existingProfile != null)
            {
                // Güncelle
                existingProfile.FirstName = model.FirstName;
                existingProfile.LastName = model.LastName;
                existingProfile.Age = model.Age;
                existingProfile.BloodGroup = model.BloodGroup;
                existingProfile.Height = model.Height;
                existingProfile.Weight = model.Weight;
                existingProfile.DoctorId = model.DoctorId;
                existingProfile.UpdatedAt = DateTime.Now;

                _context.PatientProfiles.Update(existingProfile);
            }
            else
            {
                // Yeni oluştur
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                _context.PatientProfiles.Add(model);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla kaydedildi", profileId = existingProfile?.Id ?? model.Id });
        }

        // GET: api/PatientProfile/doctors
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _context.Users
                .Where(u => u.Role == "doctor")
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Surname,
                    FullName = d.Name + " " + d.Surname,
                    d.DoctorHospital
                })
                .ToListAsync();

            return Ok(doctors);
        }
    }
}
