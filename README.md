
# SmartClinic Web Uygulaması

Bu proje, **SmartClinic** ekosisteminin **ASP.NET Core MVC** ile geliştirilmiş **web uygulaması** kısmını içermektedir. Uygulama hem **hasta** hem de **doktor** kullanıcıları için tasarlanmış olup, modern bir sağlık yönetim sistemi sunar.

---

## 🌐 Proje Özeti

SmartClinic Web uygulaması, kullanıcıların **tahlil sonuçlarını yükleyip görüntüleyebildiği**, doktorların ise **hasta verilerini yönetebildiği** bir dijital sağlık platformudur.
Uygulama **ASP.NET Core MVC**, **Entity Framework Core**, **SQLite** ve **Python FastAPI PDF Ayrıştırma Servisi** kullanılarak geliştirilmiştir.

---

## 🏗️ Proje Yapısı

```
smartclinic_web/           # Web uygulaması ana klasörü
├── Controllers/           # Giriş, Hasta ve Doktor logic
│   ├── AuthController.cs
│   ├── PatientController.cs
│   └── DoctorController.cs
│
├── Models/                # DB tabloları ve API modelleri
│   ├── User.cs
│   ├── TestResult.cs
│   └── UploadPdfResponse.cs
│
├── Data/
│   └── SmartClinicDbContext.cs   # EF Core DB Context
│
├── Views/                 # Razor View dosyaları
│   ├── Auth/
│   ├── Patient/
│   ├── Doctor/
│   └── Shared/ (_Layout.cshtml)
│
├── Migrations/            # Code-First migration dosyaları
│
├── wwwroot/               # Statik dosyalar
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/           # PDF upload klasörü
│
└── Program.cs             # Uygulama başlangıç dosyası
```

---

## ⚙️ Özellikler

### 👩‍⚕️ Doktor Paneli

* Hasta listesinin görüntülenebilmesi için hazır altyapı.
* Tahlil verilerinin görüntülenmesi.
* Profili görüntüleme.
* Genişletilebilir esnek yapı.

### 🧑‍⚕️ Hasta Paneli

* PDF formatında tahlil yükleme.
* Yüklenen PDF’in FastAPI servisinde otomatik ayrıştırılması.
* Test sonuçlarını **tarih bazlı** listeleme.
* Referans dışı sonuçların **kırmızı renkte ve uyarılı** gösterimi.
* Sonuç detay sayfasında **referans dışı filtreleme (ON/OFF)**.
* Profil düzenleme ve doktor seçme (doktorun hastanesi otomatik doldurulur).

### 🔐 Kimlik Doğrulama

* Kullanıcı kayıt ve giriş işlemleri web üzerinde yapılır.
* Roller → `doctor` ve `patient`.
* Login işleminden sonra rol bazlı yönlendirme:

  * `Patient/Dashboard`
  * `Doctor/Dashboard`
* Session yönetimi:

  * `UserId`
  * `UserRole`
* Çıkış → session sıfırlanır.

---

## 🧠 Veri Akışı

1. Hasta PDF tahlil dosyasını yükler.
2. Web uygulaması PDF’i **Python FastAPI** servisine gönderir.
3. Servis:

   * Tahlil adlarını çıkarır,
   * laboratuvar sonuçlarını çözer,
   * referans aralığını parse eder,
   * referans dışı olup olmadığını hesaplar.
4. Web uygulaması dönen JSON’u `TestResult` tablosuna kaydeder.
5. Hasta sonuçları tarih bazında listeleyebilir.
6. Detay ekranında:

   * referans dışı değerler kırmızı
   * filtre ON/OFF modu ile sadece referans dışı değerler gösterilebilir.

---

## 🔌 API Bağlantısı

Web uygulaması aşağıdaki servislere bağlanır:

| Endpoint                | Açıklama                            |
| ----------------------- | ----------------------------------- |
| `POST /api/upload/pdf`  | PDF dosyasını ayrıştıran servis     |
| `GET /api/tests`        | Tüm test tarihlerini listeleme      |
| `GET /api/tests/{date}` | Belirli tarihin sonuçlarını getirme |

Bu uç noktalar **FastAPI** üzerinde çalışır.

---

## 🧩 Kullanılan Teknolojiler

| Katman          | Teknoloji                   |
| --------------- | --------------------------- |
| Backend         | ASP.NET Core MVC            |
| ORM             | Entity Framework Core       |
| Veritabanı      | SQLite                      |
| UI              | Razor Pages + Bootstrap     |
| Oturum Yönetimi | ASP.NET Session             |
| PDF Ayrıştırma  | Python FastAPI (pdfplumber) |
| Statik Dosyalar | wwwroot                     |

---

## 🚀 Kurulum

### 1. Bağımlılıkları yükle

```bash
dotnet restore
```

### 2. Migration oluştur ve DB’yi hazırla

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Web uygulamasını çalıştır

```bash
dotnet run
```

### 4. PDF Ayrıştırma Servisini başlat (opsiyonel)

```bash
uvicorn main:app --reload
```

---

## 🧾 Örnek Veri (FastAPI’den dönen JSON)

```json
{
  "date": "25.11.2025",
  "results": [
    {
      "name": "Hemoglobin (HGB)",
      "result": "10.4",
      "range": "11.5-15.5",
      "isOutOfRange": true
    }
  ]
}
```

---

## 👩‍💻 Geliştirici

**Hevin Ateş**
ASP.NET Core & Flutter Entegrasyon Geliştiricisi

---

## 🏥 SmartClinic Web

Modern, güvenli ve kullanıcı dostu bir dijital sağlık yönetim platformu.

---

Hazır!
İstersen aynı README’nin **İngilizce versiyonunu** da hazırlayabilirim.
