# PAPARA Expense Tracking System
## Saha Personeli Masraf Takip ve Yönetim Uygulaması

---

## 📋 İçindekiler

1. [Proje Hakkında](#proje-hakkında)
2. [Özellikler](#özellikler)
3. [Sistem Gereksinimleri](#sistem-gereksinimleri)
4. [Teknoloji Stack'i](#teknoloji-stacki)
5. [Kurulum ve Yapılandırma](#kurulum-ve-yapılandırma)
6. [Docker ile Veritabanı Kurulumu](#docker-ile-veritabanı-kurulumu)
7. [Proje Kurulum Adımları](#proje-kurulum-adımları)
8. [Kullanıcı Rolleri ve Yetkiler](#kullanıcı-rolleri-ve-yetkiler)
9. [Kimlik Doğrulama](#kimlik-doğrulama)
10. [API Dokümantasyonu](#api-dokümantasyonu)
11. [Güvenlik ve Şifreleme](#güvenlik-ve-şifreleme)
12. [Redis Cache Sistemi](#redis-cache-sistemi)
13. [Audit Log Sistemi](#audit-log-sistemi)
14. [Raporlama](#raporlama)
15. [Test Rehberi](#test-rehberi)
16. [Sorun Giderme](#sorun-giderme)
17. [Katkıda Bulunma](#katkıda-bulunma)

---

## 🎯 Proje Hakkında

PAPARA Expense Tracking System, şirketlerin giderlerini takip edebileceği kapsamlı bir sistem sunmaktadır. Kullanıcılar giderlerini kategorilere ayırabilir, ödeme yöntemlerini seçebilir ve detaylı raporlar oluşturabilir. Proje, rol bazlı yetkilendirme sistemi ile güvenli bir yapı sunar ve modern teknolojiler kullanılarak kolay kurulum ve yönetim sağlar.

API testleri ve örnek senaryoları içeren bir PDF dosyasını [Papara_ExpenseTracking.pdf](https://github.com/user-attachments/files/20075348/Papara_ExpenseTracking.pdf) indirebilirsiniz.

### 🎪 Proje Vizyonu
Şirketlerin gider yönetimi süreçlerini dijitalleştirerek operasyonel verimliliği artırmak ve detaylı raporlama ile şeffaflık sağlamak.

### 🏢 Hedef Kullanıcılar

- **Personel:**  
  - Kendi masraflarını sisteme giren ve takip eden kullanıcılar.
  - Masraf ekleme, düzenleme ve kendi raporlarını görüntüleme yetkisine sahiptirler.

- **Admin (Yönetici):**  
  - Sistemin tümünü yönetir.
  - Kullanıcı ve rol yönetimi yapabilir.
  - Gider kategorileri ve ödeme yöntemlerini düzenleyebilir.
  - Tüm giderleri görüntüleyip onay/red işlemlerini gerçekleştirebilir.
  - Departman ve çalışan yönetimi ile tüm raporlara erişim yetkisine sahiptir.

---

## ✨ Özellikler

### 👤 Kullanıcı Yönetimi
- **Rol Bazlı Yetkilendirme:** Admin ve Personel rolleri ile güvenli erişim.
- **JWT Token ile Kimlik Doğrulama:** Tüm API isteklerinde güvenli oturum yönetimi.
- **Güvenli Şifre Yönetimi:** SHA256 hash algoritması ile şifreler güvenli şekilde saklanır.
- **Otomatik Şifre Oluşturma:** İlk kurulumda güvenli şifreler otomatik olarak üretilir.

### 💰 Gider Yönetimi
- **Kategorize Gider Girişi:** Esnek kategori sistemi ile masraflar detaylı şekilde kaydedilir.
- **Dosya Yükleme:** Fatura ve fiş gibi belgeler masraflara eklenebilir.
- **Durum Takibi:** Masraflar Beklemede, Onaylandı, Reddedildi, Ödendi statülerinde izlenir.
- **Filtreleme ve Arama:** Masraflar kategori, tarih, durum, konum, tutar, ödeme yöntemi gibi kriterlerle filtrelenebilir.

### 🔄 Onay Süreci
- **Hızlı Onay/Red İşlemleri:** Yöneticiler masrafları hızlıca onaylayabilir veya reddedebilir.
- **Ödeme Simülasyonu:** Onaylanan masraflar ödeme simülasyonuna aktarılır.

### 💳 Ödeme Sistemi
- **Sanal Banka Entegrasyonu:** Masrafların ödemesi için sanal banka simülasyonu.
- **Otomatik EFT İşlemleri:** Onaylanan masraflar için otomatik ödeme işlemleri.

### 📊 Kapsamlı Raporlama
- **Zaman Bazlı Raporlar:** Günlük, haftalık, aylık raporlar oluşturulabilir.
- **Çalışan Bazlı Analiz:** Her çalışanın masraf performansı analiz edilebilir.
- **Statü Bazlı Raporlama:** Masraflar statülerine göre raporlanabilir.
- **Kişisel Gider Raporları:** Personel kendi giderlerini detaylı görebilir.

### 🏗️ Teknik Özellikler
- **Katmanlı Mimari:** API, servis, veri erişim ve DTO katmanları ile sürdürülebilir kod yapısı.
- **Generic Repository:** Kod tekrarını azaltan, merkezi veri erişim yapısı.
- **Middleware:** Merkezi hata yönetimi ve loglama için özel middleware yapısı.
- **Audit Log:** Tüm önemli veri işlemleri otomatik olarak kaydedilir.
- **Redis Cache:** Sık erişilen veriler için performans optimizasyonu.
- **Seed Data:** İlk kurulumda örnek veriler otomatik olarak eklenir.
- **DTO/Schema Kullanımı:** Entity’ler doğrudan dışarıya açılmaz, güvenli ve kontrollü veri transferi sağlanır.
- **Serilog ile Loglama:** Tüm önemli işlemler ve hatalar detaylı şekilde loglanır.

---

## 🔧 Sistem Gereksinimleri

### Minimum Gereksinimler
- **.NET 8.0 SDK**
- **Docker Desktop**
- **Git**
- **8 GB RAM**
- **20 GB Disk Alanı**

### Önerilen Gereksinimler
- **.NET 8.0 SDK (Latest)**
- **Docker Desktop (Latest)**
- **Visual Studio 2022** veya **VS Code**
- **16 GB RAM**
- **50 GB SSD Disk**

---

## 🛠 Teknoloji Stack'i

### Backend Framework
- **Framework**: .NET 8.0 Web API
- **ORM**: Entity Framework Core
- **Architecture**: Repository Pattern + Unit of Work

### Veritabanı ve Cache
-  **Primary**: PostgreSQL / SQL Server
-  **Caching**: Redis 

### Güvenlik ve Doğrulama
- **JWT Authentication**:  JWT Bearer Token
- **FluentValidation**: Girdi doğrulamaları
- **SHA256 Encryption**: Şifre şifreleme

### Development Tools
- **AutoMapper**: DTO ve Entity dönüşümleri
- **Swagger *: API dokümantasyonu
- **Serilog**: Loglama sis(Swashbuckle)*temi
- **Docker**: Konteynerizasyon
- **Database Approach**: Code First Migration
- **Data Access**: Dapper (Raporlama için)
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Configuration**: appsettings.json
---

## 🚀 Kurulum ve Yapılandırma

### Ön Koşullar
Sistemde aşağıdaki araçların yüklü olması gerekmektedir:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/downloads)

---

## 🐳 Docker ile Veritabanı Kurulumu

### 1. SQL Server Container Kurulumu
```bash
# SQL Server container'ını başlat
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name mssql \
  -d mcr.microsoft.com/mssql/server:2019-latest
```

### 2. Redis Container Kurulumu (Persistence ile)
```bash
# Redis container'ını persistence ile başlat
docker run --name redis-expense \
  -p 6379:6379 \
  -v redis-data:/data \
  -d redis:alpine redis-server --appendonly yes
```

### 3. PostgreSQL Kurulumu (Alternatif)
```bash
# PostgreSQL container'ını başlat (isteğe bağlı)
docker run --name postgres-expense \
  -e POSTGRES_PASSWORD=YourStrong@Passw0rd \
  -e POSTGRES_DB=ExpenseTracking \
  -p 5432:5432 \
  -d postgres:15
```

### 4. Container'ları Kontrol Etme
```bash
# Çalışan container'ları göster
docker ps

# Container loglarını kontrol et
docker logs mssql
docker logs redis-expense
```

---

## 📦 Proje Kurulum Adımları

### Adım 1: Projeyi Klonlama
```bash
git clone https://github.com/nalbantseymaa/papara-bootcamp-final-case.git
cd papara-bootcamp-final-case
```

### Adım 2: Bağımlılıkları Yükleme
```bash
dotnet restore
```

### Adım 3: Connection String Yapılandırması
`appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ExpenseTracking;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "JwtConfig": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32Characters",
    "Issuer": "ExpenseTrackingAPI",
    "Audience": "ExpenseTrackingUsers",
    "AccessTokenExpiration": 60
  }
}
```

### Adım 4: Veritabanı Migration
```bash
# Migration'ları uygula ve seed data'yı yükle
dotnet ef database update --project ExpenseTracking.Api
```

### Adım 5: Projeyi Çalıştırma
```bash
# API'yi başlat
dotnet run --project ExpenseTracking.Api
```

### Adım 6: API Dokümanına Erişim
Tarayıcınızda şu adrese gidin:
- **Swagger UI**: `https://localhost:5001/swagger`
- **API Base URL**: `https://localhost:5001/api`

---

## 👥 Kullanıcı Rolleri ve Yetkiler

### 🔸 Admin Yetkileri
- Tüm sistem üzerinde tam yetki
- Kullanıcı ve rol yönetimi
- Gider  yönetimi
- Ödeme yöntemleri yönetimi
- Departman ve çalışan yönetimi
- Tüm raporlara erişim

### 🔹 Personel Yetkileri
- Kendi giderlerini ekleme/düzenleme
- Kendi gider raporlarını görüntüleme
- Profil bilgilerini güncelleme
- Sadece kendi verilerine erişim

---

## 🔐 Kimlik Doğrulama

### Varsayılan Kullanıcı Bilgileri
Sistem ilk kurulumda aşağıdaki kullanıcıları oluşturur:

#### Admin Kullanıcısı
```
Kullanıcı Adı: admin
Şifre: 123456
```

#### Personel Kullanıcısı
```
Kullanıcı Adı: employee1
Şifre: employee123
```

### Test Senaryoları
1. **Admin Girişi**: Admin kullanıcı bilgileri ile giriş yapın ve kullanıcı yönetimi ekranını kontrol edin.
2. **Gider Ekleme**: Personel rolü ile giriş yaparak yeni bir gider ekleyin.
3. **Rapor Oluşturma**: Farklı tarihler için rapor oluşturmayı test edin.

Bu README dosyası, uygulamayı sıfırdan kurmak ve kullanmak isteyenler için rehber niteliğindedir. Herhangi bir sorunuz olursa lütfen proje yöneticinizle iletişime geçin.