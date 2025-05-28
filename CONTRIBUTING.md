# Katkıda Bulunma Rehberi

## Güvenlik Kuralları

1. **Hassas Bilgiler**
   - Asla hassas bilgileri doğrudan kod içine yazmayın
   - API anahtarları, şifreler ve diğer sırları ortam değişkenlerinde saklayın
   - Yapılandırma dosyalarında hassas bilgileri kullanmayın

2. **Yapılandırma Dosyaları**
   - `appsettings.json` dosyasını şablon olarak kullanın
   - Hassas bilgileri ortam değişkenlerinden alın
   - Production ortamında güvenli bir secrets yönetim sistemi kullanın

3. **Kod Güvenliği**
   - SQL injection'a karşı parametreli sorgular kullanın
   - XSS saldırılarına karşı input validasyonu yapın
   - CSRF token'ları kullanın
   - Rate limiting uygulayın

4. **Veri Güvenliği**
   - Kullanıcı şifrelerini her zaman hash'leyin
   - Hassas verileri şifreleyin
   - SSL/TLS kullanın
   - Güvenlik duvarı kurallarını yapılandırın

## Geliştirme Ortamı Kurulumu

1. Projeyi klonlayın
2. `appsettings.Example.json` dosyasını `appsettings.json` olarak kopyalayın
3. `.env` dosyasını oluşturun ve gerekli değişkenleri ayarlayın
4. `setup-environment.ps1` scriptini çalıştırın
5. Projeyi derleyin ve çalıştırın

## Pull Request Süreci

1. Yeni bir branch oluşturun
2. Değişikliklerinizi yapın
3. Tüm testleri çalıştırın
4. Pull request oluşturun
5. Code review sürecini bekleyin

## Güvenlik Kontrol Listesi

- [ ] Hassas bilgiler ortam değişkenlerinde saklanıyor
- [ ] Yapılandırma dosyaları güvenli
- [ ] Kod güvenlik standartlarına uygun
- [ ] Testler başarılı
- [ ] Dokümantasyon güncel 