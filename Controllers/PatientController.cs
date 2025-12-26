using Microsoft.AspNetCore.Mvc;
using smartclinic_web.Data;
using smartclinic_web.Models;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace smartclinic_web.Controllers
{
    public class PatientController : Controller
    {
        private readonly SmartClinicDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PatientController(SmartClinicDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // ------------------ DASHBOARD ------------------
        public IActionResult Dashboard()
        {
            return View();
        }

        // ------------------ PROFİL (GET) ------------------
        public IActionResult Profile(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            // Doktor listesi
            var doctors = _context.Users
                .Where(u => u.Role == "doctor")
                .Select(d => new
                {
                    d.Id,
                    FullName = d.Name + " " + d.Surname,
                    d.DoctorHospital
                })
                .ToList();

            ViewBag.Doctors = doctors;

            return View(user);
        }

        // ------------------ PROFİL (POST | KAYDET) ------------------
        [HttpPost]
        public IActionResult UpdateProfile(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

            if (user == null)
                return NotFound();

            // Güncelle
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Age = model.Age;
            user.Gender = model.Gender;
            user.BloodGroup = model.BloodGroup;
            user.DoctorName = model.DoctorName;
            user.DoctorHospital = model.DoctorHospital;

            _context.SaveChanges();

            TempData["Success"] = "Profil başarıyla güncellendi.";

            return RedirectToAction("Profile", new { id = user.Id });
        }

        // ------------------ TAHLİLLER LİSTE ------------------
        public IActionResult Tests()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var testDates = _context.TestResults
                .Where(t => t.PatientId == userId)
                .GroupBy(t => t.TestDate)
                .Select(g => g.Key)
                .OrderByDescending(d => d)
                .ToList();

            ViewBag.TestDates = testDates;

            return View();
        }
        public IActionResult TestDetails(string date)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var testDate = DateTime.Parse(date);

            var results = _context.TestResults
                .Where(t => t.PatientId == userId && t.TestDate == testDate)
                .OrderBy(t => t.TestName)
                .ToList();

            ViewBag.TestDate = testDate;

            return View(results);
        }

        // ------------------ PDF YÜKLEME SAYFASI (GET) ------------------
        public IActionResult UploadTest()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            return View();
        }

        // ------------------ PDF YÜKLEME (POST) ------------------
        [HttpPost]
        public async Task<IActionResult> UploadTest(IFormFile pdfFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            if (pdfFile == null || pdfFile.Length == 0)
            {
                TempData["Error"] = "PDF dosyası seçilmedi.";
                return RedirectToAction("UploadTest");
            }

            // PDF'i Python API'ye gönder
            using var httpClient = new HttpClient();

            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(pdfFile.OpenReadStream()), "file", pdfFile.FileName);

            var response = await httpClient.PostAsync("http://127.0.0.1:8000/api/upload/pdf", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "PDF analiz edilemedi.";
                return RedirectToAction("UploadTest");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<UploadPdfResponse>(jsonString);

            // Gelen tahlilleri DB'ye kaydet
            foreach (var item in apiResult.results)
            {
                var t = new TestResult
                {
                    PatientId = userId.Value,
                    TestDate = DateTime.Parse(apiResult.date),
                    TestName = item.name,
                    Value = item.result,
                    ReferenceRange = item.range,
                    IsOutOfRange = item.isOutOfRange
                };

                _context.TestResults.Add(t);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tahlil başarıyla yüklendi.";

            return RedirectToAction("Tests");
        }

        // ------------------ RAPORLAMA ------------------
        public IActionResult Reports()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var allTests = _context.TestResults
                .Where(t => t.PatientId == userId)
                .ToList();

            var totalTests = allTests.Count;
            var outOfRangeTests = allTests.Count(t => t.IsOutOfRange);
            var inRangeTests = allTests.Count(t => !t.IsOutOfRange);

            ViewBag.TotalTests = totalTests;
            ViewBag.OutOfRangeTests = outOfRangeTests;
            ViewBag.InRangeTests = inRangeTests;

            return View();
        }

        // ------------------ CHATBOT ------------------
        public IActionResult Chatbot()
        {
            return View();
        }

        // ------------------ CHATBOT API ------------------
        [HttpPost]
        public async Task<IActionResult> ChatMessage([FromBody] ChatRequest request)
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { message = "⚠️ Oturum bulunamadı. Lütfen giriş yapın." });

                // Kullanıcının tahlil bilgilerini al
                var userTests = _context.TestResults
                    .Where(t => t.PatientId == userId)
                    .OrderByDescending(t => t.TestDate)
                    .Take(10)
                    .ToList();

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                // Sistem mesajı oluştur
                var systemPrompt = $@"Sen SmartClinic sağlık asistanısın. Hastaya yardımcı ol.
Hasta Bilgileri:
- İsim: {user?.Name} {user?.Surname}
- Yaş: {user?.Age}
- Cinsiyet: {user?.Gender}
- Kan Grubu: {user?.BloodGroup}

Son Tahliller: {(userTests.Any() ? string.Join(", ", userTests.Select(t => $"{t.TestName}: {t.Value} ({t.ReferenceRange})")) : "Henüz tahlil yok")}

Kurallar:
- Tıbbi tavsiye verme, sadece bilgilendirici ol
- Cevapları kısa ve anlaşılır tut (max 3-4 cümle)
- Emojiler kullan
- Acil durumlarda doktora yönlendir
- Türkçe cevap ver";

                // Gemini API çağrısı
                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { message = "⚠️ API key yapılandırılmamış." });
                }

                var httpClient = _httpClientFactory.CreateClient();
                
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt + "\n\nKullanıcı: " + request.Message }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 500
                    }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { message = $"⚠️ API bağlantı hatası: {response.StatusCode}" });
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, options);
                var botMessage = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? "Üzgünüm, şu anda yanıt veremiyorum. 😔";

                return Json(new { message = botMessage });
            }
            catch (Exception ex)
            {
                return Json(new { message = "⚠️ Hata: " + ex.Message });
            }
        }

        // ------------------ ÖNERİLER ------------------
        public IActionResult Suggestions()
        {
            return View();
        }

        // ------------------ RANDEVU AL (GET) ------------------
        public IActionResult BookAppointment()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            
            // Kullanıcının seçili doktoru varsa onu al, yoksa tüm doktorları göster
            var doctors = _context.Users
                .Where(u => u.Role == "doctor")
                .Select(d => new
                {
                    d.Id,
                    FullName = d.Name + " " + d.Surname,
                    d.DoctorHospital,
                    Specialty = d.DoctorHospital
                })
                .ToList();

            // Kullanıcının randevularını getir
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.CreatedAt)
                .ToList();

            ViewBag.Doctors = doctors;
            ViewBag.SelectedDoctorId = user?.DoctorName != null ? 
                _context.Users.FirstOrDefault(u => u.Role == "doctor" && (u.Name + " " + u.Surname) == user.DoctorName)?.Id : null;
            ViewBag.Appointments = appointments;
            
            return View();
        }

        // ------------------ RANDEVU AL (POST) ------------------
        [HttpPost]
        public async Task<IActionResult> BookAppointment(int doctorId, DateTime appointmentDate, string appointmentTime, string reason)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var appointment = new Appointment
            {
                PatientId = userId.Value,
                DoctorId = doctorId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                Reason = reason,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Doktora mesaj gönder
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId);
            var patient = _context.Users.FirstOrDefault(u => u.Id == userId);
            
            var message = new Message
            {
                SenderId = userId.Value,
                ReceiverId = doctorId,
                Content = $"🗓️ Yeni randevu talebi!\n\nTarih: {appointmentDate:dd MMMM yyyy}\nSaat: {appointmentTime}\nSebep: {reason}",
                MessageType = "AppointmentRequest",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu talebiniz başarıyla gönderildi. Doktorunuz onayladığında bilgilendirileceksiniz.";
            return RedirectToAction("Messages");
        }

        // ------------------ RANDEVULARIM ------------------
        public IActionResult MyAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        // ------------------ MESAJLAR ------------------
        public IActionResult Messages()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            
            // Kullanıcının doktoru
            int? doctorId = null;
            if (!string.IsNullOrEmpty(user?.DoctorName))
            {
                var doctor = _context.Users.FirstOrDefault(u => 
                    u.Role == "doctor" && 
                    (u.Name + " " + u.Surname) == user.DoctorName);
                doctorId = doctor?.Id;
            }

            // Doktor bilgisi
            var doctorInfo = doctorId.HasValue ? _context.Users.FirstOrDefault(u => u.Id == doctorId) : null;

            // Mesajları getir
            var messages = _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId && m.ReceiverId == doctorId) || 
                           (m.SenderId == doctorId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.DoctorId = doctorId;
            ViewBag.DoctorInfo = doctorInfo;
            ViewBag.CurrentUserId = userId;

            return View(messages);
        }

        // ------------------ MESAJ GÖNDER ------------------
        [HttpPost]
        public async Task<IActionResult> SendMessage(int receiverId, string content)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Oturum bulunamadı" });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Mesaj boş olamaz" });

            var message = new Message
            {
                SenderId = userId.Value,
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { count = 0 });

            var count = _context.Messages
                .Count(m => m.ReceiverId == userId && !m.IsRead);

            return Json(new { count });
        }

        // ------------------ MESAJLARI OKUNDU İŞARETLE ------------------
        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsRead(int senderId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false });

            var messages = _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == userId && !m.IsRead)
                .ToList();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    // Gemini API modelleri
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class GeminiResponse
    {
        public Candidate[]? candidates { get; set; }
    }

    public class Candidate
    {
        public Content? content { get; set; }
    }

    public class Content
    {
        public Part[]? parts { get; set; }
    }

    public class Part
    {
        public string? text { get; set; }
    }
}
