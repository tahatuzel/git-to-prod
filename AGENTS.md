# Proje Rehberi

## Amaç

Bu proje, öğrenilen yazılım geliştirme pratiklerini tek bir basit To-do web uygulamasında bir araya getirmek için geliştirilecektir. Hedef, sektörde kullanılan makul standartlara uyan; bakımı kolay, anlaşılır ve çalıştırılabilir bir uygulama oluşturmaktır. Uygulama kapsamı basit tutulmalı; öğrenme hedefi, gereksiz karmaşıklık eklemek için gerekçe yapılmamalıdır.

## Ürün ve Teknoloji Hedefleri

- **Backend:** .NET ve Entity Framework Core ile PostgreSQL'e erişen bir HTTP API.
- **Backend mimarisi:** Gerektikçe ayrılmış Domain, Application, Infrastructure ve API sorumluluklarıyla Clean Architecture. Bağımlılıklar iç katmanlara doğru akmalı; iş kuralları HTTP veya veritabanı ayrıntılarına bağlanmamalıdır.
- **Frontend:** Nuxt. Tarayıcıdan gelen backend istekleri Nuxt sunucu tarafındaki proxy/server route katmanından backend API'ye aktarılmalıdır. İş kuralları frontend'de kopyalanmamalıdır.
- **Veritabanı:** PostgreSQL. Yerel geliştirmede yerel bir PostgreSQL servisi kullanılmalıdır. Üretim veritabanı Kubernetes dışında, ayrı bir VM'de çalışmalıdır.
- **Konteynerler:** Backend ve frontend için ayrı Dockerfile; yerel uygulama servislerini kök dizindeki Docker Compose dosyasıyla yönetme. Gerekirse yerel PostgreSQL servisi de Compose'a eklenebilir.
- **Testler:** Backend ve frontend için uygun birim testleri ile servisler ve veritabanı sınırlarını kapsayan entegrasyon testleri.
- **CI/CD:** GitHub Actions ile PR kontrollerini (testler ve gerekli derleme/doğrulamalar) otomatik çalıştırma; başarılı zorunlu kontroller sonrasında main'e birleştirme akışını otomatikleştirme. Main'e alınan değişiklikleri image oluşturma ve dağıtım akışına bağlama.
- **Dağıtım:** Aynı VPC içindeki toplam dört VM: bir K3s server/control-plane düğümü, iki K3s agent/worker düğümü ve Kubernetes dışında çalışan bir veritabanı VM'i. CD, Actions üzerinden K3s cluster'ındaki uygulama image'larını güncellemelidir.
- **Veritabanı geçişleri:** Dağıtım sırasında EF Core migration bundle kullanılmalıdır. Geçiş başarısız olursa uygulama sürümünü devreye alma adımı durmalıdır.

## Yapı ve Kodlama İlkeleri

- **Basitlik önceliklidir.** Önce ihtiyacı karşılayan en küçük çalışan çözümü kur. Her yeni katman, soyutlama, paket veya altyapı bileşeni mevcut ve somut bir gereksinimle gerekçelendirilmelidir; varsayımsal gelecek ihtiyaçları için ekleme yapma.
- Mevcut proje yapısını ve araçlarını incelemeden yeni klasör, katman, paket veya desen ekleme. Yeni bir soyutlama ancak somut bir ihtiyacı karşılıyorsa eklenmelidir.
- Kod sorumluluklarını anlaşılır ve gerekli ölçüde modüler tut. Kod tekrarını azalt; ancak tek seferlik veya tesadüfi benzerlikleri ortak bir soyutlamaya dönüştürme.
- Okunabilir, basit çözümleri tercih et. Genel amaçlı framework'ler, gereksiz katmanlar, aşırı yapılandırma ve erken optimizasyondan kaçın.
- Backend'de API katmanı HTTP giriş/çıkışını, Application kullanım senaryolarını, Domain iş kurallarını, Infrastructure ise EF Core ve dış kaynak bağlantılarını yönetmelidir. Bu sorumluluk sınırlarını koru; ancak her biri için ayrı proje veya ek katman oluşturmak gerçekten fayda sağlamıyorsa oluşturma.
- EF Core varken yalnızca desen gereği genel repository veya ek veri erişim katmanı oluşturma; gerekçesi varsa ekle.
- Frontend'de Nuxt'ın yerleşik klasör ve yönlendirme yaklaşımlarını izle. Proxy sorumluluğunu server route'larında tut; sunucu sırlarını tarayıcıya gönderme.
- Testleri davranış ve sınırlar üzerinden yaz. Test edilebilirlik için üretim koduna gereksiz soyutlama ekleme.
- Yeni bir tasarım veya teknoloji önerirken, hangi mevcut sorunu çözdüğünü ve daha basit seçeneğin neden yetmediğini açıkla. Bu gerekçe yoksa basit seçeneği tercih et.
- Sürümleri, image registry'sini, CI/CD hizmet ayarlarını veya dağıtım ayrıntılarını varsayma; mevcut yapı ve kullanıcı kararlarıyla uyumlu seçenekleri kullan.
- Gizli bilgileri kaynak koda veya image'lara koyma. CI/CD sırlarını uygun secret mekanizması üzerinden geçir.
- Kapsam dışı düzenlemelerden kaçın; ilgisiz kullanıcı değişikliklerini koru.

## Hedeflenen Üst Düzey Klasör Yapısı

Var olan iskeleti temel al ve uygulama büyüdükçe yalnızca ihtiyaç duyulan klasörleri ekle:

```text
/
├── backend/
│   ├── src/                 # Backend uygulama ve gerekli Clean Architecture projeleri
│   ├── test/                # Backend birim ve entegrasyon testleri
│   └── Dockerfile
├── frontend/                # Nuxt uygulaması ve kendi Dockerfile'ı
├── deploy/                  # Gerekli K3s dağıtım tanımları
├── .github/workflows/       # CI/CD iş akışları
├── docker-compose.yml       # Yerel servislerin orkestrasyonu
└── AGENTS.md                # Proje ve geliştirme yönergeleri
```

Bu yapı hedef niteliğindedir; uygulamanın gerçek gereksinimleri veya mevcut araçlar daha yalın bir düzeni destekliyorsa, aynı sorumluluk sınırlarını koruyarak gereksiz klasörler oluşturma.

## Değişiklik ve Doğrulama Akışı

1. İlgili kodu, proje yapılandırmasını ve yerel yönergeleri incele.
2. İstenen davranışı karşılayan en küçük değişikliği yap; mevcut isimlendirme ve kod biçimini izle.
3. İlgili birim/entegrasyon testlerini ve etkilenen derleme ya da statik kontrolleri çalıştır. Çalıştırılmayan doğrulamaları başarılıymış gibi bildirme.
4. Son değişiklikleri gözden geçir; ilgisiz dosya, gizli bilgi, debug artığı veya gereksiz bağımlılık olmadığını kontrol et.
5. Sonucu kısa ve açık biçimde, değişiklikleri ve doğrulama durumunu belirterek raporla.

## Oturumlar Arası İlerleme Takibi

Bu dosya, proje yönergelerinin yanında oturumlar arası ortak durum ve devir notudur. Yeni bir oturuma başlarken önce bu bölümü ve ilgili proje dosyalarını oku; tamamlanmış işleri tekrar yapmadan sıradaki adımdan devam et.

- Her anlamlı geliştirme adımının sonunda aşağıdaki yol haritasını güncelle: tamamlanan adımı `[x]`, üzerinde çalışılanı `[~]`, bekleyeni `[ ]` olarak işaretle.
- Güncellemede mevcut durumu, o adımda yapılan önemli değişikliği, doğrulama sonucunu ve sıradaki somut işi yaz. Bir engel varsa ne olduğunu ve devam etmek için gereken kararı da belirt.
- İlerleme bilgisini başka bir sohbete veya geçici notlara bırakma; bu dosyada tut ki oturum paylaşımı ve devamı için tek kaynak olsun.
- Tamamlanan adımların ayrıntılı günlüğünü biriktirme. Her adım için kısa bir sonuç tut ve aşağıdaki **Şu Anki Durum** bölümünü güncel bırak.

## Proje Yol Haritası

Adımları sırayla ve küçük, çalışır parçalar halinde ilerlet. Bir adımın kapsamı netleşince `[~]` yap; doğrulayıp tamamlayınca `[x]` yap ve sonraki adımın durumunu güncelle.

1. [x] **Temel kararlar ve iskelet:** Backend için .NET 10 `Domain` class library ve temel `TodoItem` entity'si oluşturuldu. Frontend Nuxt 4 ve npm ile başlatıldı; yerel geliştirme yönergeleri frontend README'sinde.
2. [x] **Backend dikey dilimi:** Domain, Application, Infrastructure ve API katmanları; TodoItem CRUD ve PostgreSQL erişimi hazır. Handler'lar `Result<TResponse>` ile `IResponse` uygulayan kullanım senaryosu yanıtı veya hata listesi döndürüyor; API düz başarı/hata DTO'su ve HTTP durum kodu üretiyor. `InitialCreate` migration'ı Development veritabanına uygulandı. Production connection string'i dağıtım öncesinde ayrıca girilecek.
3. [x] **Frontend ve proxy:** Nuxt UI ile tek sayfalık CRUD arayüzü hazır. İstemci düz API başarı/hata gövdesini okuyor. Geliştirmede Vite proxy, üretimde Nuxt server route backend isteklerini iletiyor.
4. [x] **Testler:** Handler birim testleri, Testcontainers PostgreSQL üzerinde migration/API entegrasyon testleri ve Nuxt HTTP proxy testi eklendi; çalıştırma adımları belgelendi.
5. [x] **Yerel konteynerler:** Backend ve frontend için ayrı Dockerfile ve `.dockerignore`, kök Compose dosyası eklendi. Frontend, `backend:8080` üzerinden API'ye; backend `host.docker.internal` üzerinden yerel PostgreSQL'e bağlanıyor. İki image build edildi ve çalışan Compose ortamında frontend ile API proxy smoke testi başarılı.
6. [~] **CI ve main akışı:** Backend/frontend CI workflow'ları PR kontrolü ve `workflow_call` olarak düzenlendi. Yeni `Release` workflow'u `main` üzerinde iki CI sonucunu bekliyor; required check/merge politikası GitHub'da ayrıca yapılandırılacak.
7. [~] **Dağıtım altyapısı:** Terraform altyapısı kullanıcı tarafından uygulandı; K3s server/worker ve PostgreSQL kuruldu. DB VM'de `todo_app`/`gitToProdDB` hazır, iki worker'dan DB bağlantısı doğrulandı. Geniş iç ağ security group kuralı ve canlı ALB uygulama kontrolü bekliyor.
8. [~] **CD ve migration dağıtımı:** İlk `main` push'unda CI kontrolleri geçti; EF bundle üretimi restore eksikliği (`NETSDK1004`) nedeniyle durdu, migration ve image yayını başlamadı. Release workflow'una backend restore adımı eklendi; düzeltmenin GitHub'da yeniden çalıştırılması ve ilk dağıtım bekliyor.

## Şu Anki Durum

- **Tamamlanan:** .NET 10 backend CRUD ve PostgreSQL akışı, Nuxt 4/Nuxt UI arayüzü ve proxy hazır. Application handler'ları ayrı `IResponse` yanıtları ve `Result<TResponse>` hata listesi kullanıyor; API düz başarı gövdesi ve `{ errors: [...] }` hata gövdesi döndürüyor. Backend/frontend Dockerfile'ları, ayrı `.dockerignore` dosyaları ve kök Compose düzeni eklendi. Frontend Compose ağında `backend:8080` adresini kullanıyor; backend Development ayarından `host.docker.internal` ile host PostgreSQL'e bağlanıyor. CORS'taki `http://localhost:3000`, tarayıcının gerçek origin'i olduğu için korundu. Production connection string testlerde kullanılmıyor.
- **Devam eden adım:** GitHub secret ve variable adları kullanıcı tarafından ayarlandı; gizli değerler doğrulanamadı. İlk `Release` çalışmasında backend/frontend CI geçti, `Build Linux migration bundle` adımı `project.assets.json` eksikliğiyle durdu. Restore adımı eklendi; DB VM'ye erişim ve migration henüz denenmedi. `GHCR_PUBLIC_READY=false` kalıyor.
- **Sıradaki iş:** Restore düzeltmesini `main`'e push edip Release'in migration ve image yayınını tamamladığını doğrula. Sonra iki GHCR paketini public yap, `GHCR_PUBLIC_READY=true` yapıp `Release` workflow'unu `main` üzerinde elle çalıştır; rollout, ALB ve proxy sonuçlarını doğrula. Canlı veri öncesi DB yedekleme/geri yüklemeyi hazırla; PR required check'lerini ve geniş iç ağ kuralını gözden geçir.
- **Paket/güvenlik durumu:** EF Core/Design `10.0.12`, Npgsql EF provider `10.0.3`, AutoMapper `16.2.0`, MediatR `14.2.0` ve global `dotnet-ef` `10.0.12`. NPM `esbuild` override `^0.28.2` ile Windows dev-server açığı giderildi. Test bağımlılıkları eklenmeden önceki 24.09.2026 denetiminde güncel registry verisiyle npm audit 0 açık; NuGet doğrudan/geçişli paket audit'i 0 açık ve 0 kullanım dışı paket gösterdi. TypeScript `6.0.3` korunuyor; mevcut typescript-eslint parser `6.1` öncesini desteklediğinden `7.0.2` uyumsuz. NuGet'te daha yeni design-time geçişli paketler var, ancak güvenlik açığı değiller.
- **Doğrulama:** Önceki yerel Compose smoke ve Linux x64 EF bundle üretimi başarılıydı. İlk GitHub Release'de backend/frontend CI geçti; bundle adımı restore öncesinde `NETSDK1004` ile durdu. Eklenen `dotnet restore backend/src/Api/Api.csproj` yerelde başarılı, workflow YAML parse ve `git diff --check` geçti. Yeni akışın GitHub'da, DB erişiminin ve K3s dağıtımının canlı doğrulaması bekliyor.
