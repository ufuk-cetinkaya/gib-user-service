# GibUserService

GİB (Gelir İdaresi Başkanlığı) kullanıcı listelerini (VKN/TCKN, Posta Kutusu vb.) otomatik olarak çeken, işleyen ve sorgulama imkanı sunan Cloud-Native yapıda bir mikroservis çözümüdür.

Bu depo hem bir Worker Service (veri işleyici) hem de bir Minimal API (sorgu servisi) barındırır.

🏗 Mimari Yapı
Proje, veriyi kaynağından alıp son kullanıcıya sunana kadar iki ana bileşenden oluşur:

1. Worker Service (Data Synchronizer)
Görev: GİB üzerindeki güncel kullanıcı listelerini ZIP formatında indirir.

İşlem Akışı: ZIP'ten çıkarma (Unzip) -> XML Deserialization -> Veritabanı Kaydı.

Zamanlama: Quartz.NET kullanılarak periyodik olarak çalıştırılır.

Teknoloji: .NET 10 Worker SDK.

2. Query API (Minimal API)
Görev: Veritabanına kaydedilen kullanıcı bilgilerini dış dünyaya açar.

Teknoloji: .NET 10 Minimal APIs, EF Core.

🛠 Teknik Özellikler
Runtime: .NET 10 & C# 14

ORM: Entity Framework Core (EF Core)

Scheduler: Quartz.NET

Altyapı: Docker & Kubernetes uyumlu (Cloud-Native), standalone çalışmaya hazır.

⚠️ Bilinen Kısıtlamalar & Zayıf Yönler (Disclaimers)
Bu proje belirli kullanım senaryoları için optimize edilmiştir. Üretim (Production) ortamına almadan önce aşağıdaki noktalar dikkate alınmalıdır:

Veri Güncelleme Stratejisi: Mevcut yapıda Worker, her güncelleme döngüsünde gibuser tablosundaki tüm kayıtları silip baştan insert eder (Full Refresh).

Not: Test datası ölçeğinde sorun yaratmasa da, çok büyük canlı verilerde performans darboğazı oluşturabilir. EF Core sınırları içerisinde kalmak adına bu yöntem tercih edilmiştir.

🚀 Dağıtım
Uygulama Cloud-Native prensiplerine uygun tasarlandığı için Dockerize edilmeye ve orkestrasyon araçlarında (AKS, vb.) çalıştırılmaya tam uyumludur.

🛠 CI/CD ve Otomasyon
Proje, GitHub Actions üzerinde tanımlanmış tam otomatik bir iş akışına (Workflow) sahiptir. Her ana sürüme veya belirli branch'lere yapılan push işlemlerinde şu adımlar izlenir:

Docker Build: Uygulama (API ve Worker için ayrı ayrı) Docker image olarak paketlenir.

GHCR Push: Oluşturulan imajlar otomatik olarak GitHub Container Registry (GHCR) üzerine itilir.

Versioning: Imajlar commit hash'i veya tag bilgisi ile etiketlenerek izlenebilirlik sağlanır.

Not: Dağıtım için ghcr.io/ufuk-cetinkaya/gib-user-api:latest benzeri imaj yollarını kullanabilirsiniz.

🧪 Test ve Entegrasyon
Uygulamanın sunduğu uç noktaları (endpoints) test etmek için proje kök dizininde bir Postman Collection bulunmaktadır.

Dosya: tests/gib-user-api.postman_collection.json

Kullanım: Bu dosyayı Postman'de içe aktararak (Import); vergi numarası, döküman tipi ve birim parametreleri ile hızlıca sorgulama yapmaya başlayabilirsiniz.
