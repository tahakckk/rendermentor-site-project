# Veritabanı Yapılandırması
[System.Environment]::SetEnvironmentVariable('DB_SERVER', 'your_server', 'User')
[System.Environment]::SetEnvironmentVariable('DB_NAME', 'your_database', 'User')
[System.Environment]::SetEnvironmentVariable('DB_USER', 'your_username', 'User')
[System.Environment]::SetEnvironmentVariable('DB_PASSWORD', 'your_password', 'User')

# E-posta Yapılandırması
[System.Environment]::SetEnvironmentVariable('SMTP_HOST', 'your_smtp_host', 'User')
[System.Environment]::SetEnvironmentVariable('SMTP_PORT', '587', 'User')
[System.Environment]::SetEnvironmentVariable('SMTP_USERNAME', 'your_username', 'User')
[System.Environment]::SetEnvironmentVariable('SMTP_PASSWORD', 'your_password', 'User')
[System.Environment]::SetEnvironmentVariable('EMAIL_FROM', 'your_email@example.com', 'User')
[System.Environment]::SetEnvironmentVariable('EMAIL_SUPPORT', 'support@example.com', 'User')

# Ödeme Sistemi Yapılandırması
[System.Environment]::SetEnvironmentVariable('PAYMENT_TERMINAL_USER_ID', 'your_terminal_user_id', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_TERMINAL_ID', 'your_terminal_id', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_MERCHANT_ID', 'your_merchant_id', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_PROV_USER_ID', 'your_prov_user_id', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_PROV_PASSWORD', 'your_prov_password', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_STORE_KEY', 'your_store_key', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_MODE', 'TEST', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_GATEWAY_URL', 'your_gateway_url', 'User')
[System.Environment]::SetEnvironmentVariable('PAYMENT_VERIFY_URL', 'your_verify_url', 'User')

# Güvenlik Yapılandırması
[System.Environment]::SetEnvironmentVariable('ENCRYPTION_KEY', 'your_32_character_encryption_key', 'User')
[System.Environment]::SetEnvironmentVariable('JWT_SECRET', 'your_64_character_jwt_secret', 'User')

Write-Host "Ortam değişkenleri başarıyla ayarlandı." 