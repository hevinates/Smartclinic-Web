using Microsoft.AspNetCore.Mvc;
using smartclinic_web.Data;
using System.Text;
using System.Text.Json;

namespace smartclinic_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : Controller
    {
        private readonly SmartClinicDbContext _context;
        private readonly ILogger<ChatbotController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ChatbotController(
            SmartClinicDbContext context, 
            ILogger<ChatbotController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("message")]
        public async Task<IActionResult> Message([FromBody] ChatMessageRequest request)
        {
            try
            {
                _logger.LogInformation("API çağrısı alındı. Mesaj: {Message}", request?.Message);
                
                var userId = HttpContext.Session.GetInt32("UserId");
                _logger.LogInformation("Session UserId: {UserId}", userId);
                
                if (userId == null)
                {
                    _logger.LogWarning("Kullanıcı giriş yapmamış!");
                    return Json(new { message = "⚠️ Lütfen giriş yapın." });
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                _logger.LogInformation("Kullanıcı bulundu: {UserName}", user?.Name ?? "Yok");
                
                if (user == null)
                {
                    return Json(new { message = "⚠️ Kullanıcı bulunamadı." });
                }

                var message = request?.Message?.Trim() ?? "";
                
                // Kullanıcının test sonuçlarını al
                var testResults = _context.TestResults
                    .Where(t => t.PatientId == userId)
                    .OrderByDescending(t => t.TestDate)
                    .Take(5)
                    .ToList();

                // Gemini API'ye gönder
                string response = await GetGeminiResponse(message, user, testResults);
                
                _logger.LogInformation("Yanıt gönderiliyor");

                return Json(new { message = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chatbot API hatası!");
                return Json(new { message = $"⚠️ Üzgünüm, bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        private async Task<string> GetGeminiResponse(string userMessage, Models.User user, List<Models.TestResult> testResults)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return "⚠️ API anahtarı bulunamadı.";
                }

                // Kullanıcı profil bilgilerini hazırla
                var userInfo = $"Kullanıcı Adı: {user.Name}\n" +
                              $"Yaş: {user.Age ?? 0}\n" +
                              $"Cinsiyet: {user.Gender}\n" +
                              $"Kan Grubu: {user.BloodGroup}";

                // Son test sonuçlarını ekle
                var testInfo = "";
                if (testResults.Any())
                {
                    testInfo = "\n\nSon Test Sonuçları:\n";
                    foreach (var test in testResults)
                    {
                        testInfo += $"- {test.TestName} ({test.TestDate:dd.MM.yyyy}): {test.Value}";
                        if (!string.IsNullOrEmpty(test.ReferenceRange))
                        {
                            testInfo += $" (Normal: {test.ReferenceRange})";
                        }
                        if (test.IsOutOfRange)
                        {
                            testInfo += " ⚠️ Normal dışı";
                        }
                        testInfo += "\n";
                    }
                }

                // System prompt (sağlık asistanı rolü)
                var systemPrompt = @"Sen SmartClinic sağlık asistanısın. Türkçe konuşuyorsun ve kullanıcılara sağlık konusunda yardımcı oluyorsun.

Görevlerin:
1. Kullanıcının test sonuçlarını yorumlamak
2. Sağlıklı yaşam önerileri vermek
3. Genel sağlık sorularına cevap vermek
4. Gerektiğinde doktora yönlendirmek

ÖNEMLİ KURALLAR:
- Kesinlikle teşhis koyma, sadece bilgi ver
- Ciddi durumlarda mutlaka doktora yönlendir
- Emoji kullan ama abartma (her cümlede değil)
- Kısa ve öz cevaplar ver (maksimum 300 kelime)
- Samimi ve destekleyici ol
- Tahlil değerlerini yorumlarken normal aralıkları da belirt

Kullanıcı Bilgileri:
" + userInfo + testInfo;

                // Gemini API request body
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt },
                                new { text = "Kullanıcı Sorusu: " + userMessage }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    },
                    safetySettings = new[]
                    {
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";
                
                _logger.LogInformation("Gemini API'ye istek gönderiliyor...");
                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API hatası: {Error}", errorContent);
                    return "⚠️ Üzgünüm, şu anda yanıt veremiyorum. Lütfen daha sonra tekrar deneyin.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Gemini API yanıtı alındı");

                // Parse response
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                if (jsonResponse.TryGetProperty("candidates", out var candidates) && 
                    candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var contentObj) &&
                        contentObj.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var textPart = parts[0];
                        if (textPart.TryGetProperty("text", out var textElement))
                        {
                            return textElement.GetString() ?? "Yanıt alınamadı.";
                        }
                    }
                }

                return "⚠️ Yanıt işlenemedi. Lütfen tekrar deneyin.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API çağrısında hata!");
                return "⚠️ Bağlantı hatası oluştu. Lütfen internet bağlantınızı kontrol edin.";
            }
        }
    }

    public class ChatMessageRequest
    {
        public string Message { get; set; } = "";
    }
}
