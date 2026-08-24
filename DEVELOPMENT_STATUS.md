# Tarjimon Office UZ — Development Status

## 2026-08-24 — Muzlatilgan natijalar

### Qidiruv / aniqlash
- Office Add-in qidiruvi ishlayapti.
- Windows Uninstall registry orqali dasturlar aniqlanmoqda.
- AppData / Startup joylari ham tekshirilmoqda.
- Noma'lum tarjimon/add-inlarni ham topish mexanizmi ishlayapti.
- Duplicate mahsulotlarni jamlash mexanizmi ishlayapti.
- `Tarjimon Office UZ` o'z mahsulotini to'g'ri ajratmoqda.
- Qidiruv va jamlash mexanizmi **muzlatildi**: bundan keyin faqat kichik, xavfsiz filter tuzatishlari kiritiladi.

### O'rnatish
- MSI/Setup o'rnatilishi ishlayapti.
- O'z dasturimiz o'chirilgandan keyin yangi versiyasi qayta o'rnatilmoqda.
- Installer jarayoni stabil holatda saqlanadi.

### Uninstall
- `TransLit` kabi begona tarjimonni uninstall qilish ishladi.
- `Tarjimon Office UZ`ning o'zini uninstall qilish va yangi nusxasini o'rnatish ishladi.
- Bir nechta tanlangan mahsulotni ketma-ket uninstall qilish uchun **single-UAC** mexanizmi qo'llanmoqda.
- Maqsad: foydalanuvchi `Tasdiqlash`ni bir marta bosadi, UAC bir marta chiqadi, keyin tanlangan uninstalllar ketma-ket bajariladi va oxirida MSI o'rnatiladi.
- 1.1.8 testida regex parser xatosi aniqlandi va MSI uninstall parseri tuzatildi.

### Hozirgi qolgan ish
Jadvalga qidiruv natijasida ortiqcha oddiy Windows/Office komponentlari chiqib ketmoqda. Qidiruvning o'zi o'zgartirilmaydi; faqat **yakuniy display filter** kuchaytiriladi.

### Qoidalar
- Qidiruv, duplicate merge va o'z mahsulotimizni aniqlash mexanizmini buzmaslik.
- O'zboshimchalik bilan katta o'zgarish kiritmaslik.
- Har bir o'zgarishdan oldin mavjud stabil holat saqlanadi.
- Xato chiqsa oldingi stabil holatga qaytish imkoniyati saqlanadi.

## Baholash
- Qidiruv: 95%
- Duplicate merge: 95%
- O'z mahsulotini aniqlash: 100%
- Uninstall: 95%
- Single-UAC: 90% (test davom etadi)
- Ortiqcha natijalarni display filter: 80% (keyingi ish)
- Umumiy loyiha holati: taxminan 93%
