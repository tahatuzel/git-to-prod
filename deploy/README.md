# K3s release kurulumu

`Release` workflow'u `main` push'unda backend ve frontend CI kontrollerini aynı commit için çalıştırır. Ardından EF Core bundle'ını DB VM'de uygular, iki image'ı GHCR'ye commit SHA etiketiyle gönderir ve public image'ları K3s'e dağıtır. Migration başarısızsa sonraki işler çalışmaz. `workflow_dispatch` yalnızca `main` için kabul edilir.

## GitHub ayarları

Repository secrets:

| Ad | Değer |
| --- | --- |
| `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` | Yalnızca iki EC2 instance'ına SSM SSH oturumu açabilen AWS kimliği. |
| `EC2_SSH_PRIVATE_KEY` | Terraform'da seçilen EC2 key pair'in PEM içeriği. |
| `SSH_KNOWN_HOSTS` | `DB_INSTANCE_ID` ve `K3S_SERVER_INSTANCE_ID` için önceden doğrulanmış SSH host key satırları. `StrictHostKeyChecking` kapatılmaz. |
| `DB_CONNECTION_STRING_LOCAL` | `Host=127.0.0.1;Port=5432;Database=gitToProdDB;Username=todo_app;Password=...` |
| `DB_CONNECTION_STRING_K3S` | `Host=10.42.11.159;Port=5432;Database=gitToProdDB;Username=todo_app;Password=...` |

Repository variables:

| Ad | Değer |
| --- | --- |
| `AWS_REGION` | Terraform'un çalıştırıldığı AWS region'ı. |
| `DB_INSTANCE_ID` | `terraform output instance_ids` içindeki `db` değeri. |
| `K3S_SERVER_INSTANCE_ID` | `terraform output instance_ids` içindeki `k3s-server` değeri. |
| `APPLICATION_URL` | `terraform output application_url` değeri. |
| `GHCR_PUBLIC_READY` | İlk yayında `false`; iki GHCR paketi public yapıldıktan sonra `true`. |

SSH host anahtarlarını daha önce güvenilir biçimde bağlandığın makinedeki `known_hosts` kayıtlarından al. Instance ID ile anahtarın eşleşmesini doğrulamadan GitHub'a ekleme. GitHub Actions runner'ı SSM Session Manager plugin'ini kurar ve var olan EC2 key pair ile SSH/SCP kullanır. DB VM ve K3s server için public IP veya 22 portu açılmaz.

## İlk yayın

1. Secret ve variable'ları ayarla; veritabanında `todo_app` ile `gitToProdDB` hazır olmalı. CI için PR required check'lerini GitHub branch protection'da seç.
2. `main` push'u ilk release'i başlatır. `GHCR_PUBLIC_READY` henüz `true` değilken migration ve image push çalışır; K3s deploy atlanır.
3. GitHub Packages ayarlarında `git-to-prod-backend` ve `git-to-prod-frontend` paketlerini **Public** yap. Public görünürlük kararı geri alınamayabilir; image'larda üretim secret'ı bulunmamalıdır. Backend image'ına Development appsettings dosyası kopyalanmaz.
4. `GHCR_PUBLIC_READY=true` yap ve `Release` workflow'unu `main` üzerinde elle çalıştır. CI ve idempotent bundle yeniden çalışır; ardından SHA etiketli image'lar dağıtılır. Sonraki `main` push'ları otomatik ilerler.

Dağıtım script'i K3s node listesindeki `10.42.10.50` ve `10.42.11.84` InternalIP'lerini bulur, iki Ready worker'ı `app-node=true` ile etiketler ve pod'ları yalnızca bu node'lara yerleştirir. Backend'in DB bağlantısı `todo-db` Kubernetes Secret'ındadır; Secret manifesti Git'e yazılmaz. Nuxt, API'ye `todo-backend:8080` üzerinden proxy eder. Traefik Ingress'i frontend'i `/` altında sunar; Terraform ALB'si iki worker'ın 80 portunu ve `/` health check'ini kullanır.

## Kontrol ve hata durumları

Workflow, iki Deployment'ın rollout durumunu bekler; ardından ALB üzerinden `/` ve `/api/todo-items` için HTTP başarılı yanıt arar. İlk deploy'da pod'lar hazır olmazsa `sudo k3s kubectl -n todo describe pods` ve `sudo k3s kubectl -n todo logs deployment/todo-backend` ile image pull, PostgreSQL kaynak IP ve uygulama hatalarını kontrol et. PostgreSQL `pg_hba.conf` kuralları worker kaynak IP'lerine izin vermelidir.

Bundle DB şemasını image yayınından önce ileri taşır. Image push veya rollout başarısız olursa şema otomatik geri alınmaz. İlk canlı migration öncesinde veritabanı yedeği ve geri yükleme yöntemi hazır olmalı; sonraki şema değişiklikleri önceki uygulama sürümüyle uyumlu olmalıdır. DB verisi hâlen VM root diskindedir.
