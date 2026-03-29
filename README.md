# e-Fatura ve e-İrsaliye mükellef servisi

GİB (Gelir İdaresi Başkanlığı) kullanıcı listelerini (VKN/TCKN, Posta Kutusu vb.) otomatik olarak çeken, işleyen ve sorgulama imkanı sunan bir mikroservis çözümüdür.

Bu depo hem bir Worker Service (veri işleyici) hem de bir Minimal API (sorgu servisi) barındırır.

Proje, veriyi kaynağından alıp son kullanıcıya sunan kadar iki ana bileşenden oluşur:

1. Worker Service (Data Synchronizer)
Görev: GİB üzerindeki güncel kullanıcı listelerini ZIP formatında indirir.

İşlem Akışı: ZIP'ten çıkarma (Unzip) -> XML Deserialization -> Veritabanı Kaydı.

2. Query API (Minimal API)
Görev: Veritabanına kaydedilen kullanıcı bilgilerini dış dünyaya açar.

Teknoloji: .NET 10 Minimal APIs, EF Core.

🛠 Tech Stack & Architecture
Framework: .NET 10
Language: C# 14
IDE: Visual Studio 2026
Architecture: Microservices & Clean Architecture
API Style: Minimal APIs
Scheduler: Quartz.NET
ORM: EF CORE
Database Engine: MSSQL Server
Infrastructure: Docker & Kubernetes uyumlu (Cloud-Native)

🚀 Dağıtım
Uygulama Cloud-Native prensiplerine uygun tasarlandığı için Dockerize edilmeye ve orkestrasyon araçlarında (AKS, vb.) çalıştırılmaya tam uyumludur.

🛠 CI/CD ve Otomasyon
Proje, GitHub Actions üzerinde tanımlanmış tam otomatik bir iş akışına (Workflow) sahiptir. Her ana sürüme veya belirli branch'lere yapılan push işlemlerinde şu adımlar izlenir:

Docker Build: Uygulama (API ve Worker için ayrı ayrı) Docker image olarak paketlenir.

GHCR Push: Oluşturulan imajlar otomatik olarak GitHub Container Registry (GHCR) üzerine itilir.

Versioning: Imajlar commit hash'i veya tag bilgisi ile etiketlenerek izlenebilirlik sağlanır.

Not: Dağıtım için ghcr.io/ufuk-cetinkaya/gib-user-api:latest benzeri imaj yollarını kullanabilirsiniz.

🚀 Dağıtım (Deployment)
Uygulamanın farklı ortamlardaki kurulum süreçleri için aşağıdaki ilgili altyapı depolarını inceleyebilirsiniz:

Cloud (AKS): Kubernetes manifestleri ile bulut ortamına dağıtım detayları için infra reposuna göz atın.

On-Prem / Local: Yerel makinelerde veya private cloud ortamlarında manuel kurulum (Docker-Desktop/K8s) için edonusum-gitops reposunu inceleyin.

🧪 Test ve Entegrasyon
Uygulamanın sunduğu uç noktaları (endpoints) test etmek için tests klasöründe bir Postman Collection bulunmaktadır.

⚠️ Bilinen Kısıtlamalar & Zayıf Yönler (Disclaimers)
Bu proje belirli kullanım senaryoları için optimize edilmiştir. Üretim (Production) ortamına almadan önce aşağıdaki noktalar dikkate alınmalıdır:

Veri Güncelleme Stratejisi: Mevcut yapıda Worker, her güncelleme döngüsünde gibuser tablosundaki tüm kayıtları silip baştan insert eder (Full Refresh).

Not: Test datası ölçeğinde sorun yaratmasa da, çok büyük canlı verilerde performans darboğazı oluşturabilir. EF Core sınırları içerisinde kalmak adına bu yöntem tercih edilmiştir.
