# AWS altyapısı

Bu dizin iki public subnet (internet erişimli Application Load Balancer ve bir NAT gateway) ve iki private subnet (dört EC2) tanımlar: `k3s-server`, `k3s-worker-1`, `k3s-worker-2`, `db`. ALB, HTTP isteklerini iki worker'ın 80 portuna iletir. EC2'lere public IP atanmaz. Tek NAT gateway, tüm EC2'lere dışarı yönlü internet erişimi sağlar; bulunduğu availability zone kesilirse iki private subnet'in internet çıkışı da kesilir.

Dört EC2 aynı security group üzerinden geçici olarak birbirleriyle tüm portlarda haberleşebilir. Yalnızca worker'lara eklenen security group, ALB'den 80 portuna erişime izin verir. İnternetten 22 portuna erişim yoktur. Terraform, K3s, PostgreSQL veya uygulamayı kurmaz. Worker'ların 80 portunda `/` isteğine yanıt veren bir servis kurulana kadar ALB health check başarısız olacaktır. K3s varsayılan Traefik/ServiceLB düzeni, ingress'i bu porttan sunabilir.

## Girdiler

Seçilen AWS region'ında önceden oluşturulmuş bir EC2 key pair kullanın; private key yerel makinenizde kalır. `terraform.tfvars.example` dosyasını `terraform.tfvars` olarak kopyalayıp `aws_region` ve `ssh_key_name` değerlerini girin. Örnek region yalnızca örnektir. Yerel tfvars ve Terraform state dosyaları Git tarafından yok sayılır. ALB yalnızca HTTP (80) isteklerini kabul eder ve iki worker'a iletir. Ayrı bir domain veya sertifika gerekmez; dağıtımdan sonra `application_url` çıktısındaki public AWS bağlantısını kullanın. Bu bağlantı, worker'lardaki uygulama port 80'de yanıt vermeye başladıktan sonra çalışır.

Daha sonra uygulamak istediğinizde bu dizinde standart Terraform init/plan/apply akışını kullanabilirsiniz. **Bu dosyalar hazırlanırken Terraform çalıştırılmadı.** Ekip kullanımından önce state'i korumalı bir remote backend'e taşıyın. EC2, ALB, NAT gateway, public IPv4 adresleri, diskler ve veri aktarımı AWS maliyeti oluşturur.

## Private EC2'lere SSH

Amazon Linux 2023 AMI'sinde SSM Agent bulunur. Instance profile'a `AmazonSSMManagedInstanceCore` yetkisi verilir; NAT rotası agent'ın Systems Manager'a erişmesini sağlar. Bilgisayarınızda AWS CLI ve Session Manager plugin kurulu olmalı; AWS kimliğinizin bu instance'larda oturum başlatma yetkisi olmalı. Windows OpenSSH için `~/.ssh/config` içine şunu ekleyin:

```sshconfig
Host i-* mi-*
    User ec2-user
    IdentityFile C:/path/to/your/private-key.pem
    ProxyCommand C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe "aws ssm start-session --target %h --document-name AWS-StartSSHSession --parameters portNumber=%p"
```

Dağıtımdan sonra `instance_ids` çıktısındaki ID'lerden biriyle, örneğin `ssh i-0123456789abcdef0` komutuyla bağlanın. SSH trafiği Systems Manager tünelinden geçer; ağ üzerinden 22 portuna giriş kapalı kalır. SSM Agent çalışmazsa bu yapılandırmada başka SSH erişim yolu yoktur.

## Canlı trafik öncesi

K3s ve PostgreSQL'i kurup yapılandırın; uygulamanın ingress'ini iki worker'da 80 portundan yayınlayın, private DB adresini kullanın ve target health durumunu doğrulayın. K3s portları netleştikten sonra geniş iç ağ kuralını daraltın. Veritabanı şu anda EC2 root volume üzerindedir; önemli veri yüklemeden önce yedekleme ve kalıcılığı düzenleyin.
