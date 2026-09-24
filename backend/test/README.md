# Testler

Depodan, .NET 10 SDK ile çalıştır:

```sh
dotnet test backend/test/UnitTests/UnitTests.csproj
dotnet test backend/test/IntegrationTests/IntegrationTests.csproj
```

Entegrasyon testleri için Docker engine çalışır durumda olmalı. Testcontainers geçici bir PostgreSQL veritabanı açar, EF Core migration'larını uygular ve HTTP API üzerinden CRUD, doğrulama ve hata yanıtlarını kontrol eder. Production connection string gerekmez; CI aynı iki komutu Docker erişimi olan bir runner'da çalıştırabilir.

Her senaryo kendi test dosyasında bulunur. Birim testleri `UnitTests/Features/TodoItem`, API senaryoları `IntegrationTests/Api`, migration testi ise `IntegrationTests/Database` altındadır. Ortak test yardımcıları `Support` klasöründedir.
