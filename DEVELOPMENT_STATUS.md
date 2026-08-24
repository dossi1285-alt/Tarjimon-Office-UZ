# Tarjimon Office UZ — Development Status

## 2026-08-24 — Stabil baza va keyingi ishlar

### Qidiruv / aniqlash — MUZLATILGAN
- Office Add-in qidiruvi ishlayapti.
- Windows Uninstall registry orqali dasturlar aniqlanmoqda.
- AppData / Startup joylari ham tekshirilmoqda.
- Noma'lum tarjimon/add-inlarni ham topish mexanizmi ishlayapti.
- Duplicate mahsulotlarni jamlash mexanizmi ishlayapti.
- `Tarjimon Office UZ` o'z mahsulotini to'g'ri ajratmoqda.
- Qidiruv va jamlash mexanizmi **muzlatildi**.
- Bundan keyin qidiruv, duplicate merge yoki o'z mahsulotini aniqlash algoritmiga katta o'zgarish kiritilmaydi.

### O'rnatish — MUZLATILGAN STABIL HOLAT
- MSI/Setup o'rnatilishi ishlayapti.
- O'z dasturimiz o'chirilgandan keyin yangi versiyasi qayta o'rnatilmoqda.
- Installer jarayoni stabil holatda saqlanadi.
- O'rnatish mexanizmini faqat zarur bo'lsa va kichik xavfsiz tuzatish bilan o'zgartirish mumkin.

### Uninstall — STABIL HOLAT
- `TransLit` kabi begona tarjimonni uninstall qilish ishladi.
- `Tarjimon Office UZ`ning o'zini uninstall qilish va yangi nusxasini o'rnatish ishladi.
- Bir nechta tanlangan mahsulotni ketma-ket uninstall qilish uchun single-UAC mexanizmi qo'llanmoqda.
- Maqsad: foydalanuvchi `Tasdiqlash`ni bir marta bosadi, UAC bir marta chiqadi, keyin tanlangan uninstalllar ketma-ket bajariladi va oxirida MSI o'rnatiladi.
- 1.1.8 testida regex parser xatosi aniqlandi va MSI uninstall parseri tuzatildi.
- Uninstall mexanizmiga qidiruv/filter ishini tuzatish bahonasida tegilmaydi.

### Hozirgi muammo — YAKUNIY DISPLAY FILTER
So'nggi real testda qidiruv mexanizmi ko'plab dasturlarni topgani yana tasdiqlandi, lekin yakuniy jadvalda ortiqcha natijalar hali ham chiqmoqda: Git, GitHub Desktop, Chrome, Lightshot, VS Code, Firefox, Telegram, WinRAR, 7-Zip, EaseUS, Visual Studio Tools va boshqa oddiy Windows dasturlari.

Shuning uchun keyingi ish **faqat yakuniy display filter**ni to'g'rilash:
- Qidiruvni o'zgartirmaslik.
- Duplicate merge'ni o'zgartirmaslik.
- O'z mahsulotimizni aniqlashni o'zgartirmaslik.
- Uninstall mexanizmini o'zgartirmaslik.
- Jadvalga chiqarishdan oldin keraksiz dasturlarni filtrlash.
- Haqiqiy translator/transliterator/add-inlarni saqlab qolish.
- `Tarjimon Office UZ`ni har qanday holatda saqlash.
- `TransLit`, `Savodxon`, Translator/Translation, Tarjimon, Transliteration, Kirill/Lotin kabi kuchli translator signallarini saqlash.
- Oddiy dastur faqat `convert/converter` kabi umumiy so'z sabab tarjimon sifatida ko'rinib qolmasligi.
- Office MUI, Proofing, Shared Components kabi texnik Office komponentlarini jadvaldan chiqarish.

### Kod va loyiha tartibi — MUZLATILGAN QOIDALAR
- O'zboshimchalik bilan katta o'zgarish kiritilmaydi.
- Har bir o'zgarishdan oldin stabil holat saqlanadi.
- Xato chiqsa oldingi stabil holatga **undo kabi qaytish imkoniyati** bo'lishi kerak.
- Yangi vaqtinchalik `PATCH_*.bat` / `PATCH_*.ps1` fayllarini loyiha root papkasida ko'paytirmaslik.
- Test tugagach vaqtinchalik skriptlar tozalanadi.
- Root papkada asosiy build uchun faqat `FIX_BUILD_AND_BUILD.bat` saqlanadi.
- Ishlayotgan mexanizmni faqat kichik, nazorat qilinadigan o'zgarish bilan yaxshilash.
- Build muvaffaqiyatli bo'lmaguncha commit/freeze qilinmaydi.
- Real test natijasi ko'rilmaguncha stabil kodga yangi o'zgarish kiritilmaydi.

### Keyingi ishlar ro'yxati
1. **Display filter**ni to'g'rilash va real testdan o'tkazish.
2. Ortiqcha oddiy dasturlar jadvaldan chiqmay qolganini tasdiqlash.
3. Haqiqiy translator/add-inlar yo'qolib qolmaganini tasdiqlash.
4. Filter muvaffaqiyatli bo'lsa, shu holatni stabil/freeze qilish.
5. Keyin Preflight va WiX yakuniy holatini umumiy audit qilish.
6. Shundan keyin butun loyiha bo'yicha yakuniy tahlil: bajarilgan ishlar, qolgan ishlar, foizlar va rangli jadval.
7. `Preferences` bilan bog'liq keyingi ish hozircha **keyingi bosqichga qoldirilgan**; Preflight/WiX va stabil filter yakunlanmaguncha unga tegilmaydi.

### Baholash — hozirgi holat
- Qidiruv: **95% — muzlatilgan**
- Duplicate merge: **95% — muzlatilgan**
- O'z mahsulotini aniqlash: **100% — muzlatilgan**
- Installer: **95% — stabil/muzlatilgan**
- Uninstall: **95% — stabil**
- Single-UAC: **90% — ishlaydi, yakuniy tekshiruv qolgan**
- Ortiqcha natijalarni display filter: **hali yakunlanmagan — asosiy joriy ish**
- Umumiy loyiha holati: **taxminan 93%**

## Muhim eslatma
Mahsulot deyarli tayyor. Ishlayotgan qismlarni buzmaslik — asosiy qoida. Har bir yangi tuzatish kichik, alohida va qaytariladigan bo'lishi kerak. Avvalgi yaxshi natija saqlanadi, faqat nosoz joy tuzatiladi.
