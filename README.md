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

### Token Alma İşlemi

#### Login Request
```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "123456"
}
```

#### Login Response
```json
{
  "serverDate": "2025-06-01T16:58:20.683908Z",
  "referenceNo": "c8a5dd41-80f8-4049-aeea-1f739036f033",
  "success": true,
  "message": "Success",
  "response": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userName": "admin",
    "expiration": "2025-06-02T09:38:20.6837886Z"
  }
}
```

### API İsteklerinde Token Kullanımı
```http
GET /api/expenses
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
---

## 📚 API Dokümantasyonu

### 🔐 Kimlik Doğrulama
```http
POST /api/auth/login          # Giriş yap
POST /api/auth/refresh        # Token yenile
```

### 👤 Kullanıcı Yönetimi
```http
GET    /api/users             # Kullanıcıları listele
POST   /api/users             # Yeni kullanıcı oluştur
GET    /api/users/{id}        # Kullanıcı detayı
PUT    /api/users/{id}        # Kullanıcı güncelle
DELETE /api/users/{id}        # Kullanıcı sil
```

### 💰 Gider Yönetimi
```http
GET    /api/expenses                                    # Giderleri listele
POST   /api/expenses                                    # Yeni gider oluştur
GET    /api/expenses/{id}                              # Gider detayı
PUT    /api/expenses/{id}                              # Gider güncelle (sadece beklemede olanlar)
DELETE /api/expenses/{id}                              # Gider sil
GET    /api/expenses/ByParameters?categoryId=2&paymentMethodId=2&minAmount=12&maxAmount=100&status=Approved&location=izmir

# Onay İşlemleri
PUT    /api/expenses/approve/{expenseId}               # Gideri onayla
PUT    /api/expenses/reject/{expenseId}                # Gideri reddet
```

### 📄 Gider Dosyaları
```http
GET    /api/expensefiles      # Dosyaları listele
POST   /api/expensefiles      # Dosya yükle
GET    /api/expensefiles/{id} # Dosya detayı
PUT    /api/expensefiles/{id} # Dosya güncelle
DELETE /api/expensefiles/{id} # Dosya sil
```

### 🏷️ Kategori Yönetimi
```http
GET    /api/categories        # Kategorileri listele
POST   /api/categories        # Yeni kategori oluştur
GET    /api/categories/{id}   # Kategori detayı
PUT    /api/categories/{id}   # Kategori güncelle
DELETE /api/categories/{id}   # Kategori sil
```

### 💳 Ödeme Yöntemleri
```http
GET    /api/payment-methods        # Ödeme yöntemlerini listele
POST   /api/payment-methods        # Yeni ödeme yöntemi oluştur
GET    /api/payment-methods/{id}   # Ödeme yöntemi detayı
PUT    /api/payment-methods/{id}   # Ödeme yöntemi güncelle
DELETE /api/payment-methods/{id}   # Ödeme yöntemi sil
```

### 🏢 Departman Yönetimi
```http
GET    /api/departments        # Departmanları listele
POST   /api/departments        # Yeni departman oluştur
GET    /api/departments/{id}   # Departman detayı
PUT    /api/departments/{id}   # Departman güncelle
DELETE /api/departments/{id}   # Departman sil
```

### 👨‍💼 Çalışan Yönetimi
```http
GET    /api/employees                                          # Çalışanları listele
POST   /api/employees                                          # Yeni çalışan oluştur
GET    /api/employees/{id}                                     # Çalışan detayı
PUT    /api/employees/{id}                                     # Çalışan güncelle
DELETE /api/employees/{id}                                     # Çalışan sil
GET    /api/employees/ByParameters?departmentId=2&minSalary=102&MaxSalary=500
```

### 📍 Adres Yönetimi
```http
GET    /api/addresses                                          # Adresleri listele
GET    /api/addresses/{id}                                     # Adres detayı
PUT    /api/addresses/{id}                                     # Adres güncelle
DELETE /api/addresses/{id}                                     # Adres sil
GET    /api/addresses/ByParameters?city=Ankara&zipCode=06000
POST   /api/employees/{employeeId}/addresses                   # Çalışana adres ekle
POST   /api/departments/{departmentId}/addresses               # Departmana adres ekle
```

### 📞 Telefon Yönetimi
```http
GET    /api/phones            # Telefonları listele
GET    /api/phones/{id}       # Telefon detayı
PUT    /api/phones/{id}       # Telefon güncelle
DELETE /api/phones/{id}       # Telefon sil
POST   /api/employees/{id}/phones      # Çalışana telefon ekle
POST   /api/departments/{id}/phones    # Departmana telefon ekle
POST   /api/managers/{id}/phones       # Yöneticiye telefon ekle
```

### 👨‍💼 Yönetici İşlemleri
```http
GET    /api/managers          # Yöneticileri listele
POST   /api/managers          # Yeni yönetici oluştur
GET    /api/managers/{id}     # Yönetici detayı
PUT    /api/managers/{id}     # Yönetici güncelle
DELETE /api/managers/{id}     # Yönetici sil
```

### 📊 Raporlama
```http
# Genel Şirket Raporu (period: Daily, Weekly, Monthly)
GET /api/Reports/company/total?period=Daily

# Statüye Göre Rapor
GET /api/Reports/company/by-status?period=Daily

# Çalışan Bazlı Rapor
GET /api/Reports/company/by-employee?period=Weekly&employeeId=2

# Kendi Masraflarım (Personel için)
GET /api/Reports/GetEmployeeExpenses
```

---

## 🔒 Güvenlik ve Şifreleme

### Şifre Güvenliği
- **SHA256 Hash Algoritması**: Tüm şifreler güvenli şekilde hashlenir
- **Rastgele Şifre Oluşturma**: İlk kurulumda güvenli şifreler otomatik oluşturulur
- **Şifreler Hiçbir Zaman Açık Saklanmaz**: Veritabanında sadece hash değeri tutulur

### JWT Token Güvenliği
- **Güvenli Secret Key**: En az 32 karakter uzunluğunda
- **Token Süresi Yönetimi**: Configurable expiration time
- **Refresh Token**: Otomatik token yenileme

### API Güvenliği
- **Rol Bazlı Yetkilendirme**: Her endpoint için uygun rol kontrolü
- **Input Validation**: FluentValidation ile güçlü doğrulama
- **CORS Policy**: Güvenli cross-origin istekler

---

## 🚀 Redis Cache Sistemi

### Cache Edilen Veriler
Redis ile performans optimizasyonu için şu veriler cache'lenir:

- **Expense Categories (Gider Kategorileri)**
- **Payment Methods (Ödeme Yöntemleri)**
- **Departments (Departmanlar)**

### Redis Kullanımının Avantajları

#### 🔥 Performans Artışı
- Sık sorgulanan master veriler RAM'de tutulur
- Veritabanı yükü azalır
- Yanıt süreleri dramatik olarak kısalır

#### ⚡ Hız Optimizasyonu
- Kategori listesi gibi sık erişilen veriler milisaniyede döner
- Network trafiği azalır
- Concurrent user desteği artar

#### 🔄 Veri Tutarlılığı
- Cache invalidation stratejileri ile güncel veri garantisi
- TTL (Time To Live) ayarları ile otomatik temizlik
- Master data değiştiğinde cache otomatik güncellenir

### Redis Yapılandırması
```json
{
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "ExpenseTracking",
    "DefaultTTL": 3600
  }
}
```

---

## 📋 Audit Log Sistemi

### Otomatik İşlem Kayıtları
Sistemimizde tüm veri ekleme, güncelleme ve silme işlemleri otomatik olarak kaydedilir:

#### 📝 Kayıt Edilen Bilgiler
- **Kullanıcı Bilgisi**: Hangi kullanıcı işlemi gerçekleştirdi
- **İşlem Türü**: Create, Update, Delete
- **Tablo/Entity**: Hangi veri üzerinde işlem yapıldı
- **Eski/Yeni Değerler**: Değişiklik detayları
- **Timestamp**: İşlem zamanı
- **IP Adresi**: İsteğin geldiği adres

#### 🎯 Kullanım Alanları
- **Güvenlik İzleme**: Şüpheli aktivitelerin tespiti
- **Şeffaflık**: İşlem geçmişinin takibi
- **Yasal Gereklilikler**: Compliance ve audit ihtiyaçları
- **Hata Ayıklama**: Sorunların kökenini bulma

#### 🔍 Erişim
Audit log kayıtları sistem yöneticileri tarafından incelenebilir ve gerektiğinde raporlanabilir.

---

## 📊 Raporlama Sistemi

### Şirket Genel Raporları

#### 📈 Toplam Gider Raporu
```http
GET /api/Reports/company/total?period=Daily
GET /api/Reports/company/total?period=Weekly
GET /api/Reports/company/total?period=Monthly
```

#### 📊 Statü Bazlı Raporlar
```http
GET /api/Reports/company/by-status?period=Daily
```
- Onaylanan giderler
- Reddedilen giderler
- Bekleyen giderler

### Çalışan Bazlı Raporlar

#### 👤 Çalışan Detay Raporu
```http
GET /api/Reports/company/by-employee?period=Weekly&employeeId=2
```

#### 📱 Kişisel Gider Raporu
```http
GET /api/Reports/GetEmployeeExpenses
```
*Not: Çalışanlar sadece kendi giderlerini görebilir*

### Rapor Özellikleri
- **Zaman Bazlı Filtreleme**: Günlük, haftalık, aylık
- **Kategori Bazlı Analiz**: Hangi kategoride ne kadar harcandı
- **Trend Analizi**: Gider artış/azalış trendleri
- **Export Özelliği**: Excel/PDF formatında indirme

---

## 🧪 Test Rehberi

### 1. 🔐 Admin Girişi Testi
```bash
# Admin rolü  ile giriş yap
POST /api/auth/login
{
  "userName": "admin",
  "password": "123456"
}
```

### 2. 💰 Gider Ekleme Testi
```bash
# Personel rolü ile gider ekle
POST /api/expenses
{
  "amount": 100.50,
  "description": "Yakıt gideri",
  "categoryId": 1,
  "paymentMethodId": 1,
  "location": "Istanbul"
}
```

### 3. ✅ Gider Onaylama Testi
```bash
# Admin ile gider onayla
PUT /api/expenses/approve/1
```

### 4. 📊 Rapor Testi
```bash
# Günlük rapor al
GET /api/Reports/company/total?period=Daily
```
---

## 🐛 Sorun Giderme

### Yaygın Sorunlar ve Çözümleri

#### Docker Container Başlatma Sorunu
```bash
# Container'ları kontrol et
docker ps -a

# Durdurulmuş container'ı başlat
docker start mssql
docker start redis-expense

# Container loglarını kontrol et
docker logs mssql
```

#### Veritabanı Bağlantı Hatası
```bash
# Connection string'i kontrol et
# appsettings.json dosyasındaki bilgileri doğrula
# SQL Server container'ının çalıştığından emin ol
docker ps | grep mssql
```

#### Migration Hatası
```bash
# Mevcut migration'ları kontrol et
dotnet ef migrations list

# Database'i temizle ve yeniden oluştur
dotnet ef database drop
dotnet ef database update
```

#### Redis Bağlantı Sorunu
```bash
# Redis container'ını kontrol et
docker ps | grep redis

# Redis'e bağlantı testi
docker exec -it redis-expense redis-cli ping
```

#### JWT Token Hatası
```bash
# Token süresini kontrol et
# Secret key yapılandırmasını doğrula
# appsettings.json'daki JwtConfig bölümünü kontrol et
```

### Log Dosyaları
Sistem logları Serilog ile kaydedilir. Hata ayıklama için log dosyalarını kontrol edin:
- **Application Logs**: `logs/` dizini
- **Error Logs**: Kritik hatalar ayrı dosyada tutulur

---

## 🏗️ Mimari ve Tasarım Prensipleri

### Katmanlı Mimari
- **API Layer**: Controller'lar ve middleware'ler
- **Service Layer**: İş mantığı ve servisler
- **Repository Layer**: Veri erişim katmanı
- **Schema Layer**: DTO ve response modelleri

### Tasarım Desenleri
- **Generic Repository Pattern**: Kod tekrarını azaltma
- **Unit of Work Pattern**: İşlem yönetimi
- **Dependency Injection**: Gevşek bağlılık
- **Middleware Pattern**: Cross-cutting concerns

### Kod Kalitesi
- **Clean Code**: Anlamlı isimlendirme
- **SOLID Principles**: Genişletilebilir tasarım
- **Single Responsibility**: Her class tek sorumlu
- **Defensive Programming**: Güvenli kod yazımı

---

## 🤝 Katkıda Bulunma

### Geliştirme Standartları
- **Anlamlı değişken isimleri**
- **25 satırı geçmeyen metotlar**
- **Hardcoded değerler yerine constants**
- **Kapsamlı unit testler**
- **API dokümantasyonu güncellemeleri**

### Pull Request Süreci
1. Feature branch oluşturun
2. Değişikliklerinizi test edin
3. Code review sürecini takip edin
4. Dokümantasyonu güncelleyin

---

## 🙏 Teşekkürler

Bu proje **Patika.dev** ve **Papara** işbirliği ile düzenlenen **Kadın Yazılımcı Bootcamp** bitirme projesi olarak geliştirilmiştir.

**Papara** ve **Patika.dev**'e sağladıkları eğitim fırsatı için teşekkür ederiz.


---

## 📞 İletişim ve Destek
Bu README dosyası, uygulamayı sıfırdan kurmak ve kullanmak isteyenler için rehber niteliğindedir. 

Herhangi bir sorunuz olursa lütfen aşağıdaki kanallardan ulaşın:

**Proje Deposu**: https://github.com/nalbantseymaa/papara-bootcamp-final-case.git

**LinkedIn**: https://www.linkedin.com/in/nalbantseyma/

---

*Son güncelleme: Haziran 2025*  
