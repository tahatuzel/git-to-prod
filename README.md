# Git to Prod — AWS üzerinde CI/CD ve K3s dağıtımı

Bu proje, küçük bir To-do uygulamasını kullanarak **kod değişikliğinden veritabanı geçişine ve Kubernetes dağıtımına kadar** üretim benzeri bir süreci göstermeyi amaçlıyor. Uygulama özellikle basit tutuldu; odak Terraform altyapısı, GitHub Actions kapıları, EF Core migration bundle, GHCR image yayını ve K3s rollout akışında.

**Çalışma durumu:** Bu bir portföy deposudur; EC2 makineleri sürekli açık tutulmayacak ve kalıcı bir canlı demo adresi sunulmayacak. `Release` workflow'u yalnızca ilgili AWS makineleri ve PostgreSQL çalışırken tamamlanabilir.

## Mimari

```text
GitHub PR ──> Backend CI + Frontend CI
main push / manuel Release
    └─> CI kontrolleri
         └─> EF Core bundle ──SSM/SSH──> private PostgreSQL EC2
              └─> SHA etiketli backend + frontend image ──> GHCR
                   └─> SSM/SSH ──> K3s server ──> iki worker
                                                ↑
İnternet ──> HTTP ALB ──> Traefik Ingress ──> Nuxt frontend ──> .NET API ──> PostgreSQL
```

| Katman | Kullanılan yapı |
| --- | --- |
| Altyapı | Terraform; iki public, iki private subnet; ALB, NAT Gateway ve dört private EC2 |
| Kubernetes | Bir K3s server, iki worker; Traefik Ingress, iki Deployment ve ClusterIP Service |
| Veritabanı | Kubernetes dışında ayrı PostgreSQL 17 VM; EF Core 10 migration bundle |
| Uygulama | .NET 10 API, Nuxt 4 arayüz ve Nuxt sunucu tarafı API proxy'si |
| Otomasyon | GitHub Actions, GHCR, AWS Systems Manager üzerinden SSH/SCP |

EC2 makinelerine public IP veya internete açık SSH portu verilmez. ALB, worker'ların 80 portuna yönlenir; uygulama pod'ları yalnızca bu iki worker'da çalışır. Terraform, AWS kaynaklarını oluşturur; PostgreSQL ve K3s kurulumu ile ilk veritabanı hazırlığı ayrıca yapılır. VPC ağı `10.42.0.0/16` olduğundan K3s pod ağı `--cluster-cidr=10.244.0.0/16` ile kurulmalıdır; K3s'in varsayılan pod ağı VPC ile çakışır.

## Nasıl kuruldu?

1. **Altyapı:** [Terraform tanımları](terraform/) uygulanarak VPC, iki public ve iki private subnet, internet çıkışı için NAT Gateway, HTTP ALB ve private subnet'lerde dört EC2 oluşturuldu. Çıktılardan instance ID'leri, private IP'ler ve ALB adresi alındı. Terraform, makinelerin içindeki servisleri kurmuyor.
2. **Erişim:** DB ve K3s server makinelerine AWS Systems Manager oturumları üzerinden SSH ile bağlanıldı. Bu sayede EC2'lere public IP ve internete açık 22 portu gerekmedi. CI aynı kanalı SSH/SCP aktarımı için kullanıyor.
3. **Veritabanı:** Ayrı DB VM'ye PostgreSQL 17 kuruldu; uygulama için `todo_app` rolü ve `gitToProdDB` veritabanı oluşturuldu. PostgreSQL'in private ağ arayüzünü dinlemesi ve `pg_hba.conf` içinde worker bağlantılarına izin vermesi sağlandı. İki worker makinesinden `psql` ile bağlantı doğrulandı; migration bundle DB VM'de localhost üzerinden çalışıyor.
4. **K3s:** Server ve iki agent aynı VPC'deki EC2'lere kuruldu; worker'lar server token'ı ile cluster'a katıldı. İlk kurulumda K3s'in varsayılan `10.42.0.0/16` pod ağı VPC ağıyla çakıştı: worker hostu DB'ye erişirken pod'dan DB'ye TCP bağlantısı zaman aşımına uğruyordu. K3s, pod ağı `10.244.0.0/16` olacak şekilde yeniden kuruldu ve üç node'un `Ready` olduğu, pod CIDR'larının VPC dışına çıktığı doğrulandı.
5. **Yayın otomasyonu:** GitHub Actions'ta PR kontrolleri ve `main` release zinciri kuruldu. EF bundle için restore adımı eklendikten sonra CI, DB migration ve GHCR image yayını başarılı çalıştı. İki GHCR paketi public yapıldı. Yeni K3s cluster'ındaki son uygulama rollout'u henüz doğrulanmadığından, tam uçtan uca canlı yayın başarısı iddia edilmiyor.

## Release akışı

1. `main` hedefli pull request'lerde backend unit/entegrasyon testleri ve image build; frontend lint, typecheck, test ve image build kontrolleri çalışır. `main` push'u veya `workflow_dispatch`, [Release workflow'unu](.github/workflows/release.yml) başlatır ve aynı commit için bu kontrolleri yeniden çalıştırır.
2. Kontroller geçerse `Infrastructure` migration projesi ve `Api` startup projesinden self-contained Linux x64 EF Core bundle üretilir. Bundle, SSM üzerinden SSH/SCP ile DB VM'ye taşınır ve PostgreSQL'e yerel bağlantıyla uygulanır.
3. Migration başarılı olursa backend ve frontend Docker image'ları commit SHA etiketiyle `ghcr.io/tahatuzel/git-to-prod-backend` ve `ghcr.io/tahatuzel/git-to-prod-frontend` paketlerine gönderilir. Paketler public ise K3s image pull kimliği gerekmez.
4. Dağıtım, worker'ları Kubernetes `InternalIP` değerlerinden bulur, etiketler, DB bağlantı bilgisini Kubernetes Secret'a yazar ve [manifestleri](deploy/app.yaml) uygular. İki Deployment için rollout beklenir; ardından ALB üzerinden `/` ve Nuxt proxy'si üzerinden `/api/todo-items` kontrol edilir.

Migration başarısızsa image yayını ve rollout başlamaz. Migration sonrasında image yayını veya rollout başarısız olursa veritabanı şeması otomatik geri alınmaz; önemli veri öncesinde denenmiş bir yedek/geri yükleme süreci gerekir. İlk GHCR yayınında `GHCR_PUBLIC_READY=false` ile K3s adımı atlanabilir; iki paket public yapıldıktan sonra bu değişken `true` olur.

## Yapılandırma

Release için GitHub repository **secrets**: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `EC2_SSH_PRIVATE_KEY`, `SSH_KNOWN_HOSTS`, `DB_CONNECTION_STRING_LOCAL`, `DB_CONNECTION_STRING_K3S`.

GitHub repository **variables**: `AWS_REGION`, `DB_INSTANCE_ID`, `K3S_SERVER_INSTANCE_ID`, `APPLICATION_URL`, `GHCR_PUBLIC_READY`. Instance ID'leri ve ALB adresi [Terraform çıktılarından](terraform/outputs.tf) alınır. DB bağlantı dizeleri ve SSH anahtarları Git'e yazılmaz; SSH host anahtarları `SSH_KNOWN_HOSTS` ile doğrulanır. AWS kimliğinin SSM oturum yetkisi yalnızca DB ve K3s server instance'larıyla sınırlandırılmalıdır.

Altyapıyı yeniden kurarken [Terraform örnek girdilerini](terraform/terraform.tfvars.example) kullanın; K3s server kurulumunda yukarıdaki pod CIDR'ını seçin. `todo_app` rolü ve `gitToProdDB` veritabanı DB VM'de önceden hazırlanmalıdır. Mevcut [dağıtım script'i](deploy/apply.sh) worker InternalIP'leri olarak `10.42.10.50` ve `10.42.11.84` değerlerini bekler; yeni Terraform çıktıları farklıysa script ve DB ağ izinleri birlikte güncellenmelidir.

## Depo düzeni ve yerel doğrulama

| Yol | Amaç |
| --- | --- |
| [`terraform/`](terraform/) | VPC, ağ, güvenlik grupları, EC2 ve ALB tanımları |
| [`.github/workflows/`](.github/workflows/) | PR kontrolleri ve release kapıları |
| [`deploy/`](deploy/) | Namespace, K3s manifestleri ve dağıtım script'leri |
| [`backend/`](backend/) | .NET API, EF Core migration ve testler |
| [`frontend/`](frontend/) | Nuxt arayüzü, API proxy'si ve test |
| [`docker-compose.yml`](docker-compose.yml) | Yerel frontend/backend konteynerleri |

Yerel geliştirmede .NET 10 SDK, Node.js 22, npm 11 ve PostgreSQL gerekir; entegrasyon testleri için Docker da çalışmalıdır. Compose, backend'in host makinedeki PostgreSQL'e erişmesini bekler. Yerel bağlantı ayarı [`appsettings.Development.json`](backend/src/Api/appsettings.Development.json) içindedir; gerçek üretim kimlik bilgileri için kullanmayın.

```sh
dotnet test backend/test/UnitTests/UnitTests.csproj
dotnet test backend/test/IntegrationTests/IntegrationTests.csproj
cd frontend && npm ci && npm run lint && npm run typecheck && npm test
```

Yerel uygulamayı PostgreSQL hazırken `docker compose up --build` ile başlatıp `http://localhost:3000` adresinden açabilirsiniz. Nuxt, `/api/todo-items` isteklerini backend'e sunucu tarafında iletir.

## Portföy demosunun sınırları

GitHub'da CI, EF bundle migration ve GHCR image yayını çalıştırıldı. İlk K3s rollout'u, VPC ve pod CIDR çakışması nedeniyle tamamlanmadı; K3s aynı makinelerde `10.244.0.0/16` ağıyla yeniden kuruldu. Yeni cluster'daki son rollout ve ALB uçtan uca kontrolü henüz doğrulanmadı. EC2'ler kapalıyken `main` push'ları `Release` workflow'unun migration/dağıtım adımlarında başarısız olur; bu dönemde [yalnızca Release workflow'u devre dışı bırakılabilir](https://docs.github.com/en/actions/managing-workflow-runs-and-deployments/managing-workflow-runs/disabling-and-enabling-a-workflow) ve altyapı yeniden açıldığında tekrar etkinleştirilebilir. EC2 durdurmak [EBS depolama](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/how-ec2-instance-stop-start-works.html), [ALB](https://aws.amazon.com/elasticloadbalancing/pricing/) ve [NAT Gateway](https://docs.aws.amazon.com/vpc/latest/userguide/nat-gateway-pricing.html) maliyetlerini bitirmez; maliyet durumunu ayrıca kontrol edin.
