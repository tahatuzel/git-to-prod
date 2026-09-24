# To-do arayüzü

Nuxt ve Nuxt UI ile hazırlanmış tek sayfalık görev uygulaması.

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
