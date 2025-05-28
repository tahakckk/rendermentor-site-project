# RenderMentor Projesi

## Güvenlik ve Kurulum

### Ortam Değişkenleri
Projeyi çalıştırmadan önce aşağıdaki adımları takip edin:

1. `setup-environment.ps1` dosyasını düzenleyin ve kendi değerlerinizi girin
2. PowerShell'i yönetici olarak çalıştırın ve scripti çalıştırın:
   ```powershell
   .\setup-environment.ps1
   ```

### Güvenlik Önlemleri
1. Tüm hassas bilgiler ortam değişkenlerinde saklanmalıdır
2. Veritabanı bağlantı bilgileri güvenli bir şekilde yönetilmelidir
3. API anahtarları ve sırlar güvenli bir şekilde saklanmalıdır
4. Kullanıcı şifreleri her zaman hash'lenmelidir
5. SSL/TLS kullanılmalıdır

### Geliştirme Ortamı
1. `appsettings.Example.json` dosyasını `appsettings.json` olarak kopyalayın
2. Ortam değişkenlerini ayarlayın
3. Projeyi derleyin ve çalıştırın

### Production Ortamı
1. Tüm hassas bilgileri Azure Key Vault veya AWS Secrets Manager gibi güvenli bir serviste saklayın
2. SSL sertifikası kullanın
3. Güvenlik duvarı kurallarını yapılandırın
4. Düzenli güvenlik denetimleri yapın

## Proje Yapısı
- `RenderMentor.Web`: Web uygulaması
- `RenderMentor.DataAccess`: Veri erişim katmanı
- `RenderMentor.Services`: İş mantığı katmanı
- `RenderMentor.Models`: Veri modelleri
- `RenderMentor.Utility`: Yardımcı sınıflar
