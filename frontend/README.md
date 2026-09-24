# To-do arayüzü

Nuxt ve Nuxt UI ile hazırlanmış tek sayfalık görev uygulaması.

## Docker Compose ile çalıştırma

1. Projenin kök dizininde `docker compose up --build` komutunu çalıştır.
2. Uygulamayı `http://localhost:3000` adresinde aç.
3. Durdurmak için `docker compose down` komutunu çalıştır.

Compose, frontend isteklerini Docker ağı içindeki `backend:8080` adresine proxy eder. Backend host makinedeki PostgreSQL'e bağlanır; `backend/src/Api/appsettings.Development.json` içindeki connection string konteynerden erişmek için `host.docker.internal` kullanır. PostgreSQL'in Docker dışından gelen bağlantıları kabul etmesi ve 5432 portuna erişimin açık olması gerekir. Yerel uygulamayı Docker dışında `dotnet run` ile çalıştırırken connection string'deki host'u `localhost` yap. Backend CORS ayarındaki `http://localhost:3000` tarayıcının kullandığı frontend origin'idir; Nuxt backend çağrılarını sunucu tarafında proxy ettiği için burada Docker servis adı kullanılmaz.

Backend ayrıca `http://localhost:5157` adresinde erişilebilir.

## Yerelde çalıştırma

1. Backend'i kök dizinden başlat: `dotnet run --project backend/src/Api/Api.csproj`
2. Frontend klasöründe bağımlılıkları kur: `npm install`
3. Geliştirme sunucusunu başlat: `npm run dev`
4. Uygulamayı `http://localhost:3000` adresinde aç.

Geliştirmede Vite `/api/todo-items` isteklerini varsayılan olarak `http://localhost:5157` adresine iletir. Başka bir backend adresi için `NUXT_API_BASE_URL` ortam değişkeni kullanılabilir. Üretim Nuxt sunucusu aynı değişkeni kullanarak API isteklerini backend'e proxy eder.

## Komutlar

- `npm run dev` — geliştirme sunucusu
- `npm run typecheck` — Nuxt/TypeScript kontrolleri
- `npm run lint` — ESLint
- `npm test` — üretim Nuxt sunucusunun HTTP proxy testi (sahte backend kullanır)
- `npm run build` — üretim derlemesi
