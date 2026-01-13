<div align="center">

# 🏥 SmartClinic Web - Sağlık Yönetim Sistemi

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)
![Gemini AI](https://img.shields.io/badge/Gemini%20AI-2.5%20Flash-4285F4?style=for-the-badge&logo=google&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Hastaneler, doktorlar ve hastalar için geliştirilmiş, yapay zeka destekli web tabanlı sağlık yönetim platformu.**

[Özellikler](#-özellikler) • [Kurulum](#-kurulum) • [API Dokümantasyonu](#-api-dokümantasyonu) • [Ekran Görüntüleri](#-ekran-görüntüleri) • [Katkıda Bulunma](#-katkıda-bulunma)

</div>

---

## 📋 İçindekiler

- [Proje Hakkında](#-proje-hakkında)
- [Özellikler](#-özellikler)
- [Teknoloji Yığını](#-teknoloji-yığını)
- [Kurulum](#-kurulum)
- [Proje Yapısı](#-proje-yapısı)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Veritabanı Şeması](#-veritabanı-şeması)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)

---

## 🎯 Proje Hakkında

**SmartClinic Web**, modern sağlık hizmetlerinin dijitalleştirilmesi amacıyla geliştirilen kapsamlı bir web uygulamasıdır. ASP.NET Core MVC mimarisi üzerine inşa edilen bu platform, hastane yönetimi, hasta takibi, randevu sistemi ve yapay zeka destekli sağlık asistanı özelliklerini bir arada sunmaktadır.

### 🎓 Proje Vizyonu
- Hasta-doktor iletişimini güçlendirmek
- Sağlık verilerinin güvenli yönetimini sağlamak
- Yapay zeka ile sağlık danışmanlığı sunmak
- Randevu ve tahlil süreçlerini dijitalleştirmek

---

## ✨ Özellikler

### 👨‍⚕️ Doktor Paneli
| Özellik | Açıklama |
|---------|----------|
| 📊 **Dashboard** | Günlük özet, hasta istatistikleri, yaklaşan randevular |
| 👥 **Hasta Yönetimi** | Kayıtlı hastaları listeleme, detay görüntüleme |
| 🔬 **Tahlil Takibi** | Hasta tahlil sonuçlarını inceleme ve değerlendirme |
| 📅 **Randevu Yönetimi** | Randevu onaylama, reddetme, takvim görünümü |
| 💬 **Mesajlaşma** | Hastalarla güvenli mesajlaşma sistemi |
| 👤 **Profil Yönetimi** | Kişisel bilgiler, uzmanlık alanı, hastane bilgisi |

### 🏃 Hasta Paneli
| Özellik | Açıklama |
|---------|----------|
| 📊 **Dashboard** | Kişisel sağlık özeti, bildirimler |
| 🔬 **Tahlillerim** | Tahlil sonuçlarını görüntüleme |
| 📅 **Randevu Alma** | Online randevu oluşturma ve takip |
| 💬 **Mesajlaşma** | Doktorla iletişim kurma |
| 🤖 **AI Asistan** | Gemini 2.5 Flash destekli sağlık danışmanlığı |
| 👤 **Profil** | Kişisel ve sağlık bilgileri yönetimi |

### 🤖 AI Sağlık Asistanı (Gemini 2.5 Flash)
- 💬 Doğal dil ile sağlık sorularına yanıt
- 🔬 Tahlil sonuçlarını analiz etme ve yorumlama
- 💡 Kişiselleştirilmiş sağlık önerileri
- ⚠️ Acil durumlarda doktora yönlendirme
- 🇹🇷 Tamamen Türkçe dil desteği
- 📊 Hasta profiline göre bağlamsal yanıtlar

### 🔐 Güvenlik & Kimlik Doğrulama
- Session tabanlı kimlik doğrulama
- Rol bazlı yetkilendirme (Doktor/Hasta)
- Güvenli şifre yönetimi
- CORS politikaları

---

## 🛠 Teknoloji Yığını

### Backend
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| ![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white) | 8.0 | Framework |
| ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white) | 12.0 | Programlama Dili |
| ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) | 8.0 | Web Framework |
| ![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) | 8.0 | ORM |

### Veritabanı
| Teknoloji | Açıklama |
|-----------|----------|
| ![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat&logo=sqlite&logoColor=white) | Hafif, dosya tabanlı veritabanı |

### Frontend
| Teknoloji | Açıklama |
|-----------|----------|
| ![Razor](https://img.shields.io/badge/Razor-512BD4?style=flat&logo=dotnet&logoColor=white) | View Engine |
| ![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=flat&logo=bootstrap&logoColor=white) | CSS Framework |
| ![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat&logo=javascript&logoColor=black) | Interaktif özellikler |

### Yapay Zeka
| Teknoloji | Açıklama |
|-----------|----------|
| ![Gemini](https://img.shields.io/badge/Gemini-4285F4?style=flat&logo=google&logoColor=white) | Google Gemini 2.5 Flash AI |

### Araçlar
| Araç | Açıklama |
|------|----------|
| Visual Studio / VS Code | IDE |
| Git | Versiyon Kontrolü |
| Postman | API Test |

---

## 🚀 Kurulum

### Gereksinimler

- .NET SDK 8.0 veya üzeri
- Visual Studio 2022 / VS Code
- Git

### Adım Adım Kurulum

#### 1️⃣ Projeyi Klonlayın
```bash
git clone https://github.com/hevinates/Smartclinic-Web.git
cd Smartclinic-Web
```

#### 2️⃣ Bağımlılıkları Yükleyin
```bash
dotnet restore
```

#### 3️⃣ Veritabanını Oluşturun
```bash
dotnet ef database update
```

#### 4️⃣ API Anahtarını Ayarlayın
`appsettings.json` dosyasında Gemini API anahtarınızı güncelleyin:
```json
{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY"
  }
}
```

#### 5️⃣ Uygulamayı Çalıştırın
```bash
dotnet run
```

Uygulama varsayılan olarak `http://localhost:5080` adresinde çalışacaktır.

### Docker ile Kurulum (Opsiyonel)
```bash
docker build -t smartclinic-web .
docker run -p 5080:5080 smartclinic-web
```

---

## 📁 Proje Yapısı

```
smartclinic_web/
├── 📂 Controllers/
│   ├── 📄 AuthController.cs           # Kimlik doğrulama işlemleri
│   ├── 📄 ChatbotController.cs        # AI Asistan API
│   ├── 📄 DoctorController.cs         # Doktor işlemleri
│   ├── 📄 HomeController.cs           # Ana sayfa
│   ├── 📄 PatientController.cs        # Hasta işlemleri
│   └── 📄 PatientProfileController.cs # Profil API
│
├── 📂 Views/
│   ├── 📂 Auth/                       # Giriş/Kayıt sayfaları
│   │   ├── 📄 Login.cshtml
│   │   └── 📄 Register.cshtml
│   ├── 📂 Doctor/                     # Doktor sayfaları
│   │   ├── 📄 Dashboard.cshtml
│   │   ├── 📄 Patients.cshtml
│   │   ├── 📄 Appointments.cshtml
│   │   ├── 📄 Messages.cshtml
│   │   └── 📄 Profile.cshtml
│   ├── 📂 Patient/                    # Hasta sayfaları
│   │   ├── 📄 Dashboard.cshtml
│   │   ├── 📄 Tests.cshtml
│   │   ├── 📄 Appointments.cshtml
│   │   ├── 📄 Messages.cshtml
│   │   ├── 📄 Chatbot.cshtml
│   │   └── 📄 Profile.cshtml
│   ├── 📂 Home/                       # Ana sayfa
│   │   └── 📄 Index.cshtml
│   └── 📂 Shared/                     # Paylaşılan layout'lar
│       ├── 📄 _Layout.cshtml
│       └── 📄 _ValidationScripts.cshtml
│
├── 📂 Models/
│   ├── 📄 User.cs                     # Kullanıcı modeli
│   ├── 📄 PatientProfile.cs           # Hasta profili
│   ├── 📄 Appointment.cs              # Randevu modeli
│   ├── 📄 Message.cs                  # Mesaj modeli
│   └── 📄 TestResult.cs               # Tahlil sonucu
│
├── 📂 Data/
│   └── 📄 SmartClinicDbContext.cs     # EF Core DbContext
│
├── 📂 Migrations/                     # Veritabanı migration'ları
│
├── 📂 wwwroot/                        # Statik dosyalar
│   ├── 📂 css/
│   ├── 📂 js/
│   └── 📂 images/
│
├── 📄 Program.cs                      # Uygulama giriş noktası
├── 📄 appsettings.json                # Yapılandırma
├── 📄 smartclinic.db                  # SQLite veritabanı
└── 📄 smartclinic_web.csproj          # Proje dosyası
```

---

## 🔌 API Dokümantasyonu

### Kimlik Doğrulama

#### Giriş Yap
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "hasta@example.com",
  "password": "123456"
}
```

**Yanıt:**
```json
{
  "message": "Giriş başarılı",
  "user": {
    "id": 1,
    "name": "Ahmet",
    "email": "hasta@example.com",
    "role": "patient"
  }
}
```

#### Kayıt Ol
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Ahmet",
  "surname": "Yılmaz",
  "email": "hasta@example.com",
  "password": "123456",
  "role": "patient"
}
```

#### Kullanıcı Bilgisi
```http
GET /api/auth/user/{email}
```

---

### Hasta Profili

#### Profil Getir
```http
GET /api/PatientProfile/{userId}
```

**Yanıt:**
```json
{
  "id": 1,
  "userId": 5,
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "age": 35,
  "bloodGroup": "A+",
  "height": 175,
  "weight": 70,
  "doctorId": 3,
  "doctorName": "Dr. Mehmet Öz",
  "doctorHospital": "Koru Hastanesi"
}
```

#### Profil Kaydet/Güncelle
```http
POST /api/PatientProfile
Content-Type: application/json

{
  "userId": 5,
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "age": 35,
  "bloodGroup": "A+",
  "height": 175,
  "weight": 70,
  "doctorId": 3
}
```

#### Doktor Listesi
```http
GET /api/PatientProfile/doctors
```

**Yanıt:**
```json
[
  {
    "id": 3,
    "name": "Mehmet",
    "surname": "Öz",
    "fullName": "Mehmet Öz",
    "doctorHospital": "Koru Hastanesi"
  }
]
```

---

### AI Chatbot

#### Mesaj Gönder
```http
POST /api/chatbot/message
Content-Type: application/json

{
  "message": "Baş ağrım var, ne yapmalıyım?"
}
```

**Yanıt:**
```json
{
  "message": "Baş ağrısı birçok nedenden kaynaklanabilir... 🩺"
}
```

---

### Tahlil Sonuçları

#### Hasta Tahlilleri
```http
GET /api/PatientProfile/{userId}/tests
```

**Yanıt:**
```json
[
  {
    "id": 1,
    "testName": "Hemoglobin",
    "value": "14.5",
    "referenceRange": "12-16 g/dL",
    "testDate": "2024-01-15",
    "isOutOfRange": false
  }
]
```

---

## 🗄 Veritabanı Şeması

### Users (Kullanıcılar)
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | Primary Key |
| Name | string | Ad |
| Surname | string | Soyad |
| Email | string | E-posta (unique) |
| Password | string | Şifre |
| Role | string | Rol (doctor/patient) |
| Age | int? | Yaş |
| Gender | string? | Cinsiyet |
| BloodGroup | string? | Kan Grubu |
| DoctorHospital | string? | Hastane (doktorlar için) |

### PatientProfiles (Hasta Profilleri)
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | Primary Key |
| UserId | int | Foreign Key → Users |
| FirstName | string | Ad |
| LastName | string | Soyad |
| Age | int | Yaş |
| BloodGroup | string | Kan Grubu |
| Height | double? | Boy (cm) |
| Weight | double? | Kilo (kg) |
| DoctorId | int? | Foreign Key → Users (doktor) |

### Appointments (Randevular)
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | Primary Key |
| PatientId | int | Foreign Key → Users |
| DoctorId | int | Foreign Key → Users |
| AppointmentDate | DateTime | Randevu tarihi |
| Status | string | Durum (pending/approved/rejected) |
| Notes | string? | Notlar |

### Messages (Mesajlar)
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | Primary Key |
| SenderId | int | Gönderen |
| ReceiverId | int | Alıcı |
| Content | string | Mesaj içeriği |
| SentAt | DateTime | Gönderim zamanı |
| IsRead | bool | Okundu mu? |

### TestResults (Tahlil Sonuçları)
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | Primary Key |
| PatientId | int | Foreign Key → Users |
| TestName | string | Tahlil adı |
| Value | string | Değer |
| ReferenceRange | string? | Referans aralığı |
| TestDate | DateTime | Tarih |
| IsOutOfRange | bool | Normal dışı mı? |

---

## 📸 Ekran Görüntüleri

<div align="center">

| Giriş Sayfası | Hasta Dashboard | AI Asistan |
|:-------------:|:---------------:|:----------:|
| ![Login](https://via.placeholder.com/250x150?text=Giris) | ![Dashboard](https://via.placeholder.com/250x150?text=Dashboard) | ![Chatbot](https://via.placeholder.com/250x150?text=AI+Asistan) |

| Doktor Paneli | Randevular | Mesajlar |
|:-------------:|:----------:|:--------:|
| ![Doctor](https://via.placeholder.com/250x150?text=Doktor) | ![Appointments](https://via.placeholder.com/250x150?text=Randevular) | ![Messages](https://via.placeholder.com/250x150?text=Mesajlar) |

</div>

---

## 🔧 Yapılandırma

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=smartclinic.db"
  },
  "Gemini": {
    "ApiKey": "YOUR_API_KEY_HERE"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### CORS Ayarları (Program.cs)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 🤝 Katkıda Bulunma

Katkılarınızı memnuniyetle karşılıyoruz!

### Nasıl Katkıda Bulunabilirim?

1. **Fork** edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. **Pull Request** açın

### Geliştirme Kuralları
- C# coding conventions'larını takip edin
- Yeni özellikler için XML documentation yazın
- PR açmadan önce `dotnet build` ve `dotnet test` çalıştırın
- Commit mesajlarını açıklayıcı yazın

---

## 📄 Lisans

Bu proje **MIT Lisansı** altında lisanslanmıştır.

```
MIT License

Copyright (c) 2024-2026 SmartClinic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

---

## 📞 İletişim

<div align="center">

**Geliştirici:** Hevin Ateş

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/hevinates)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/in/hevinates)
[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:hevinates@gmail.com)

</div>

---

## 🔗 İlgili Projeler

| Proje | Açıklama | Link |
|-------|----------|------|
| 📱 SmartClinic Mobil | Flutter mobil uygulaması | [GitHub](https://github.com/hevinates/Smartclinic-Mobil) |
| 🔌 SmartClinic API | PostgreSQL tabanlı REST API | [GitHub](https://github.com/hevinates/Smartclinic-Api) |

---

<div align="center">

### ⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!

**Made with ❤️ using ASP.NET Core**

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

</div>