# Papara Bootcamp Final Case

## Projenin Amacı ve Genel Açıklaması
Bu proje, şirketlerin giderlerini takip edebileceği bir sistem sunmayı amaçlamaktadır. Kullanıcılar, giderlerini kategorilere ayırabilir, ödeme yöntemlerini seçebilir ve detaylı raporlar oluşturabilir. Proje, kullanıcı rolleri ve yetkilendirme sistemi ile güvenli bir yapı sunar. Ayrıca, Docker ve Entity Framework Core gibi modern teknolojiler kullanılarak kolay kurulum ve yönetim sağlanmıştır.

## Roller ve Kullanıcı Özellikleri

### Admin
- Tüm sistem üzerinde tam yetkiye sahiptir.
- Kullanıcı ve rol yönetimi yapabilir.
- Gider kategorileri ve ödeme yöntemlerini düzenleyebilir.

### Personel
- Kendi giderlerini ekleyebilir ve düzenleyebilir.
- Rapor oluşturabilir.

## API Testi
API'leri test etmek için `ExpenseTracking.Api.http` dosyasını kullanabilirsiniz. Bu dosya, Postman veya benzeri araçlarla API isteklerini kolayca test etmenizi sağlar. Örnek bir API isteği:

```http
GET https://localhost:5001/api/expenses
Authorization: Bearer <token>
```

### Örnek Testler
API testleri ve örnek senaryoları içeren bir PDF dosyasını [buradan] [Papara_ExpenseTracking.pdf](https://github.com/user-attachments/files/20075348/Papara_ExpenseTracking.pdf) indirebilirsiniz.


## MSSQL ve PostgreSQL Kurulum Notları

### MSSQL Kurulumu
Docker kullanarak MSSQL kurmak için aşağıdaki komutu çalıştırabilirsiniz:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2019-latest
```

### PostgreSQL Kurulumu
Alternatif olarak PostgreSQL kullanmak isterseniz:
```bash
docker run --name postgres -e POSTGRES_PASSWORD=YourStrong@Passw0rd -p 5432:5432 -d postgres
```

## Docker ile Proje ve Veritabanı Başlatma
1. Docker yüklü değilse [Docker](https://www.docker.com/) kurulumunu yapın.
2. MSSQL veya PostgreSQL container'ını başlatın.
3. Uygulamayı çalıştırmak için aşağıdaki komutu kullanın:
```bash
dotnet run --project ExpenseTracking.Api
```

## Projeyi Klonlama, Migration ve Veritabanı Güncelleme

### Projeyi Klonlama
```bash
git clone <repository-url>
cd papara-bootcamp-final-case
```

### Migration ve Veritabanı Güncelleme
EF Core kullanarak veritabanını güncellemek için:
```bash
dotnet ef database update --project ExpenseTracking.Api
```

## Connection String Oluşturma Adımları
`appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi veritabanı bağlantı bilgilerinizle güncelleyin. Örnek:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ExpenseTracking;User Id=sa;Password=YourStrong@Passw0rd;"
}
```

## Kullanıcı Giriş Bilgileri ve Test Senaryoları

### Kullanıcı Giriş Bilgileri
- **Admin**
  ```
  Kullanıcı Adı: admin
  Şifre: admin123
  ```
- **Personel**
  ```
  Kullanıcı Adı: employee1
  Şifre: employee123
  ```

### Test Senaryoları
1. **Admin Girişi**: Admin kullanıcı bilgileri ile giriş yapın ve kullanıcı yönetimi ekranını kontrol edin.
2. **Gider Ekleme**: Personel rolü ile giriş yaparak yeni bir gider ekleyin.
3. **Rapor Oluşturma**: Farklı tarihler için rapor oluşturmayı test edin.

Bu README dosyası, uygulamayı sıfırdan kurmak ve kullanmak isteyenler için rehber niteliğindedir. Herhangi bir sorunuz olursa lütfen proje yöneticinizle iletişime geçin.
