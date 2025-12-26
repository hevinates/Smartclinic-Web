using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using smartclinic_web.Data;
using smartclinic_web.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        // ------------------ RANDEVULARIM ------------------
        public IActionResult Appointments()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        // ------------------ HASTALARIM ------------------
        public IActionResult Patients()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            // Doktorun adını al
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId);
            if (doctor == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            string doctorFullName = $"{doctor.Name} {doctor.Surname}";

            // Bu doktoru seçen hastaları al
            var patients = _context.Users
                .Where(u => u.Role == "patient" && u.DoctorName == doctorFullName)
                .ToList();

            // Her hasta için yaklaşan randevuları al
            var today = DateTime.Today;
            var patientAppointments = new Dictionary<int, Appointment?>();

            foreach (var patient in patients)
            {
                var nextAppointment = _context.Appointments
                    .Where(a => a.PatientId == patient.Id 
                             && a.DoctorId == doctorId 
                             && a.AppointmentDate >= today
                             && (a.Status == "Pending" || a.Status == "Approved"))
                    .OrderBy(a => a.AppointmentDate)
                    .FirstOrDefault();

                patientAppointments[patient.Id] = nextAppointment;
            }

            ViewBag.PatientAppointments = patientAppointments;

            return View(patients);
        }

        // ------------------ ANALİZLER ------------------
        public IActionResult Analytics()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            // Doktorun adını al
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId);
            if (doctor == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            string doctorFullName = $"{doctor.Name} {doctor.Surname}";

            // İstatistikler
            var totalPatients = _context.Users.Count(u => u.Role == "patient" && u.DoctorName == doctorFullName);
            var totalAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId);
            var pendingAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Pending");
            var approvedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Approved");
            var completedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Completed");
            var rejectedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Rejected");

            // Bu ay ve geçen ayın randevu sayıları
            var today = DateTime.Today;
            var thisMonthStart = new DateTime(today.Year, today.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            
            var thisMonthAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.AppointmentDate >= thisMonthStart);
            var lastMonthAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.AppointmentDate >= lastMonthStart && a.AppointmentDate < thisMonthStart);

            // Bugünkü randevular
            var todayAppointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today && (a.Status == "Approved" || a.Status == "Pending"))
                .OrderBy(a => a.AppointmentTime)
                .ToList();

            // Yaklaşan randevular (gelecek 7 gün)
            var nextWeekEnd = today.AddDays(7);
            var upcomingAppointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate > today && a.AppointmentDate <= nextWeekEnd && (a.Status == "Approved" || a.Status == "Pending"))
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Take(5)
                .ToList();

            // Cinsiyet dağılımı
            var malePatients = _context.Users.Count(u => u.Role == "patient" && u.DoctorName == doctorFullName && u.Gender == "Erkek");
            var femalePatients = _context.Users.Count(u => u.Role == "patient" && u.DoctorName == doctorFullName && u.Gender == "Kadın");

            // Yaş grupları
            var patients = _context.Users.Where(u => u.Role == "patient" && u.DoctorName == doctorFullName).ToList();
            var age0_18 = patients.Count(p => p.Age.HasValue && p.Age < 18);
            var age18_35 = patients.Count(p => p.Age.HasValue && p.Age >= 18 && p.Age < 35);
            var age35_50 = patients.Count(p => p.Age.HasValue && p.Age >= 35 && p.Age < 50);
            var age50Plus = patients.Count(p => p.Age.HasValue && p.Age >= 50);

            ViewBag.TotalPatients = totalPatients;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.PendingAppointments = pendingAppointments;
            ViewBag.ApprovedAppointments = approvedAppointments;
            ViewBag.CompletedAppointments = completedAppointments;
            ViewBag.RejectedAppointments = rejectedAppointments;
            ViewBag.ThisMonthAppointments = thisMonthAppointments;
            ViewBag.LastMonthAppointments = lastMonthAppointments;
            ViewBag.TodayAppointments = todayAppointments;
            ViewBag.UpcomingAppointments = upcomingAppointments;
            ViewBag.MalePatients = malePatients;
            ViewBag.FemalePatients = femalePatients;
            ViewBag.Age0_18 = age0_18;
            ViewBag.Age18_35 = age18_35;
            ViewBag.Age35_50 = age35_50;
            ViewBag.Age50Plus = age50Plus;

            return View();
        }

        // ------------------ RANDEVU ONAYLA ------------------
        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int appointmentId)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return Json(new { success = false, message = "Oturum bulunamadı" });

            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.Id == appointmentId && a.DoctorId == doctorId);

            if (appointment == null)
                return Json(new { success = false, message = "Randevu bulunamadı" });

            appointment.Status = "Approved";
            await _context.SaveChangesAsync();

            // Hastaya onay mesajı gönder
            var message = new Message
            {
                SenderId = doctorId.Value,
                ReceiverId = appointment.PatientId,
                Content = $"✅ Randevunuz onaylandı!\n\nTarih: {appointment.AppointmentDate:dd MMMM yyyy}\nSaat: {appointment.AppointmentTime}\n\nLütfen randevu saatine zamanında gelin.",
                MessageType = "AppointmentApproval",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu onaylandı ve hasta bilgilendirildi" });
        }

        // ------------------ RANDEVU REDDET ------------------
        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int appointmentId, string? note)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return Json(new { success = false, message = "Oturum bulunamadı" });

            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.Id == appointmentId && a.DoctorId == doctorId);

            if (appointment == null)
                return Json(new { success = false, message = "Randevu bulunamadı" });

            appointment.Status = "Rejected";
            appointment.DoctorNote = note;
            await _context.SaveChangesAsync();

            // Hastaya red mesajı gönder
            var message = new Message
            {
                SenderId = doctorId.Value,
                ReceiverId = appointment.PatientId,
                Content = $"❌ Randevunuz reddedildi.\n\nTarih: {appointment.AppointmentDate:dd MMMM yyyy}\nSaat: {appointment.AppointmentTime}\n\n{(string.IsNullOrEmpty(note) ? "Lütfen başka bir tarih için randevu talep edin." : $"Not: {note}")}",
                MessageType = "AppointmentApproval",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu reddedildi ve hasta bilgilendirildi" });
        }

        // ------------------ MESAJLAR ------------------
        public IActionResult Messages(int? patientId)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            // Doktorun tüm hastalarını al
            var patients = _context.Users
                .Where(u => u.Role == "patient" && u.DoctorName == 
                    _context.Users.Where(d => d.Id == doctorId).Select(d => d.Name + " " + d.Surname).FirstOrDefault())
                .ToList();

            // Seçili hasta
            User? selectedPatient = null;
            List<Message> messages = new();

            if (patientId.HasValue)
            {
                selectedPatient = _context.Users.FirstOrDefault(u => u.Id == patientId);
                
                messages = _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => (m.SenderId == doctorId && m.ReceiverId == patientId) || 
                               (m.SenderId == patientId && m.ReceiverId == doctorId))
                    .OrderBy(m => m.SentAt)
                    .ToList();
            }

            ViewBag.Patients = patients;
            ViewBag.SelectedPatient = selectedPatient;
            ViewBag.CurrentUserId = doctorId;

            return View(messages);
        }

        // ------------------ MESAJ GÖNDER ------------------
        [HttpPost]
        public async Task<IActionResult> SendMessage(int receiverId, string content)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return Json(new { success = false, message = "Oturum bulunamadı" });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Mesaj boş olamaz" });

            var message = new Message
            {
                SenderId = doctorId.Value,
                ReceiverId = receiverId,
                Content = content,
                MessageType = "Normal",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mesaj gönderildi" });
        }

        // ------------------ OKUNMAMIŞ MESAJ SAYISI ------------------
        [HttpGet]
        public IActionResult GetUnreadMessageCount()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return Json(new { count = 0 });

            var count = _context.Messages
                .Count(m => m.ReceiverId == doctorId && !m.IsRead);

            return Json(new { count });
        }

        // ------------------ MESAJLARI OKUNDU İŞARETLE ------------------
        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsRead(int senderId)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return Json(new { success = false });

            var messages = _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == doctorId && !m.IsRead)
                .ToList();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ------------------ HASTA TAHLİLLERİ ------------------
        public IActionResult PatientTests(int patientId)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            // Hastanın bilgilerini al
            var patient = _context.Users.FirstOrDefault(u => u.Id == patientId && u.Role == "patient");
            if (patient == null) return NotFound();

            // Doktorun adını al ve kontrol et
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId);
            string doctorFullName = $"{doctor.Name} {doctor.Surname}";

            // Bu hasta, bu doktorun hastası mı kontrol et
            if (patient.DoctorName != doctorFullName)
            {
                TempData["Error"] = "Bu hasta sizin hastanız değil.";
                return RedirectToAction("Patients");
            }

            // Hastanın tahlil tarihlerini al
            var testDates = _context.TestResults
                .Where(t => t.PatientId == patientId)
                .GroupBy(t => t.TestDate)
                .Select(g => g.Key)
                .OrderByDescending(d => d)
                .ToList();

            ViewBag.TestDates = testDates;
            ViewBag.Patient = patient;

            return View(testDates);
        }

        // ------------------ HASTA TAHLİL DETAYLARI ------------------
        public IActionResult PatientTestDetails(int patientId, string date)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Auth", new { role = "doctor" });

            // Hastanın bilgilerini al
            var patient = _context.Users.FirstOrDefault(u => u.Id == patientId && u.Role == "patient");
            if (patient == null) return NotFound();

            // Doktorun adını al ve kontrol et
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId);
            string doctorFullName = $"{doctor.Name} {doctor.Surname}";

            // Bu hasta, bu doktorun hastası mı kontrol et
            if (patient.DoctorName != doctorFullName)
            {
                TempData["Error"] = "Bu hasta sizin hastanız değil.";
                return RedirectToAction("Patients");
            }

            var testDate = DateTime.Parse(date);

            // Tahlil sonuçlarını al
            var results = _context.TestResults
                .Where(t => t.PatientId == patientId && t.TestDate == testDate)
                .OrderBy(t => t.TestName)
                .ToList();

            ViewBag.TestDate = testDate;
            ViewBag.Patient = patient;

            return View(results);
        }

        // ==================== API ENDPOINTS ====================

        // API: GET ANALYTICS
        [HttpGet]
        [Route("api/doctors/{doctorId}/analytics")]
        public IActionResult GetAnalytics(int doctorId)
        {
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId && u.Role == "doctor");
            if (doctor == null)
                return NotFound(new { message = "Doktor bulunamadı" });

            // Hasta sayısını patient_profiles tablosundan al
            var totalPatients = _context.PatientProfiles.Count(p => p.DoctorId == doctorId);

            // İstatistikler
            var totalAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId);
            var pendingAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Pending");
            var approvedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Approved");
            var completedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Completed");
            var rejectedAppointments = _context.Appointments.Count(a => a.DoctorId == doctorId && a.Status == "Rejected");

            // Bugünkü randevular
            var today = DateTime.Today;
            var todayAppointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today && (a.Status == "Approved" || a.Status == "Pending"))
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new {
                    patientName = a.Patient != null ? $"{a.Patient.Name} {a.Patient.Surname}" : "Bilinmeyen",
                    appointmentTime = a.AppointmentTime,
                    status = a.Status
                })
                .ToList();

            // Cinsiyet dağılımı
            var patientProfiles = _context.PatientProfiles
                .Include(p => p.User)
                .Where(p => p.DoctorId == doctorId)
                .ToList();

            var malePatients = patientProfiles.Count(p => p.User != null && p.User.Gender == "Erkek");
            var femalePatients = patientProfiles.Count(p => p.User != null && p.User.Gender == "Kadın");

            // Yaş grupları
            var age0_18 = patientProfiles.Count(p => p.Age.HasValue && p.Age < 18);
            var age18_35 = patientProfiles.Count(p => p.Age.HasValue && p.Age >= 18 && p.Age < 35);
            var age35_50 = patientProfiles.Count(p => p.Age.HasValue && p.Age >= 35 && p.Age < 50);
            var age50Plus = patientProfiles.Count(p => p.Age.HasValue && p.Age >= 50);

            return Ok(new {
                totalPatients,
                totalAppointments,
                pendingAppointments,
                approvedAppointments,
                completedAppointments,
                rejectedAppointments,
                todayAppointments,
                malePatients,
                femalePatients,
                age0_18,
                age18_35,
                age35_50,
                age50Plus
            });
        }

        // API: GET DOCTOR BY EMAIL
        [HttpGet]
        [Route("api/doctors/email/{email}")]
        public IActionResult GetDoctorByEmail(string email)
        {
            var doctor = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "doctor");
            
            if (doctor == null)
                return NotFound(new { message = "Doktor bulunamadı" });

            return Ok(new
            {
                doctor.Id,
                firstName = doctor.Name,
                lastName = doctor.Surname,
                doctor.Email,
                hospital = doctor.DoctorHospital
            });
        }

        // API: UPDATE DOCTOR PROFILE
        [HttpPut]
        [Route("api/doctors/email/{email}")]
        public async Task<IActionResult> UpdateDoctorProfile(string email, [FromBody] DoctorUpdateDto dto)
        {
            var doctor = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "doctor");
            
            if (doctor == null)
                return NotFound(new { message = "Doktor bulunamadı" });

            doctor.Name = dto.FirstName;
            doctor.Surname = dto.LastName;
            doctor.DoctorHospital = dto.Hospital;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi" });
        }
    }

    // DTO for doctor update
    public class DoctorUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;
    }
}
