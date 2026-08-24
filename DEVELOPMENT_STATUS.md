# Tarjimon Office UZ — Development Status

## 2026-08-24 — 1.0 FINALIZATION: DESIGN + DISPLAY FILTER ONLY

### Ishlash rejimi — MUHIM
1.0 versiyani yakunlash bosqichidamiz.
Hozirdan boshlab faqat **2 ta ish** bajariladi:
1. **Dizayn** — 100% ga yetkazish va muzlatish.
2. **Yakuniy Display Filter** — jadvalda faqat kerakli translator/add-inlarni qoldirish, real testdan o'tkazish va muzlatish.

Boshqa ishlar hozircha qilinmaydi.
- Qidiruvga tegilmaydi.
- Duplicate merge'ga tegilmaydi.
- O'z mahsulotimizni aniqlashga tegilmaydi.
- Installer/WiX mexanizmiga tegilmaydi.
- Uninstall va Single-UAC mexanizmiga tegilmaydi.
- Yangi funksiyalar qo'shilmaydi.
- 1.0 finalizatsiyasi tugamaguncha yangi yo'nalish ochilmaydi.

### Qidiruv / aniqlash — 95% — MUZLATILGAN
- Office Add-in qidiruvi ishlayapti.
- Windows Uninstall registry orqali dasturlar aniqlanmoqda.
- AppData / Startup joylari ham tekshirilmoqda.
- Noma'lum tarjimon/add-inlarni ham topish mexanizmi ishlayapti.
- `Tarjimon Office UZ` o'z mahsulotini to'g'ri ajratmoqda.
- Qidiruv algoritmi muzlatildi.

### Duplicate merge — 95% — MUZLATILGAN
- Bir dasturga tegishli takroriy natijalarni jamlash ishlayapti.
- TransLit kabi bir mahsulotning turli manbalardan kelgan yozuvlari bitta natijaga jamlanadi.
- Duplicate merge kodi endi o'zgartirilmaydi.

### O'z mahsulotini aniqlash — 100% — MUZLATILGAN
- `Tarjimon Office UZ` begona dasturlardan to'g'ri ajratiladi.
- O'z mahsulotimiz uninstall qilinib, yangi nusxasi o'rnatilishi real testda ishladi.
- Ushbu mexanizmga tegilmaydi.

### Dizayn / UI — 100% — MUZLATILADI
- Asosiy `Office tarjimonlari` oynasi ishlayapti.
- Jadval ustunlari va checkbox mexanizmi ishlayapti.
- Mahsulot nomi, ishlab chiqaruvchi, versiya, dastur, ishonch va muallif/ishlab chiqaruvchi ma'lumotlari ko'rsatilmoqda.
- `Tarjimon Office UZ` o'z mahsuloti sifatida ajratilgan holda ko'rsatilmoqda.
- Tasdiqlash va Bekor qilish tugmalari mavjud va ishlayapti.
- Hozirgi maqsad: dizaynni 100% yakunlangan holat sifatida qabul qilish va keyingi o'zgarishlarni muzlatish.
- Dizayn bo'yicha yangi talablar 1.0 finaldan keyingi bosqichga qoldiriladi.

### Installer — 95% — MUZLATILGAN STABIL HOLAT
- MSI/Setup o'rnatilishi ishlayapti.
- O'z dasturimiz o'chirilgandan keyin yangi versiyasi qayta o'rnatilmoqda.
- Yakuniy Setup EXE MSI ichiga joylashtirilmoqda.
- Build real testda muvaffaqiyatli o'tgan.
- Installer mexanizmiga tegilmaydi.

### Uninstall — 95% — MUZLATILGAN STABIL HOLAT
- `TransLit` kabi begona tarjimonni uninstall qilish ishladi.
- `Tarjimon Office UZ`ning o'zini uninstall qilish va yangi nusxasini o'rnatish ishladi.
- Bir nechta tanlangan mahsulotni ketma-ket uninstall qilish uchun single-UAC mexanizmi qo'llanmoqda.
- Maqsad: foydalanuvchi `Tasdiqlash`ni bir marta bosadi, UAC bir marta chiqadi, keyin tanlangan uninstalllar ketma-ket bajariladi va oxirida MSI o'rnatiladi.
- 1.1.8 testida regex parser xatosi aniqlandi va MSI uninstall parseri tuzatildi.
- Uninstall mexanizmiga display filter ishini tuzatish bahonasida tegilmaydi.

### Yakuniy Display Filter — 80% — JARAYONDA
Hozirgi asosiy texnik ish shu.

Muammo:
- Qidiruv ko'p nomzodlarni to'g'ri topmoqda.
- Lekin jadvalda oddiy Windows dasturlari ham chiqib ketmoqda: Git, GitHub Desktop, Chrome, Lightshot, VS Code, Firefox, Telegram, WinRAR, 7-Zip, EaseUS, Visual Studio Tools va boshqa komponentlar.

Yechim qoidasi:
- Qidiruvni o'zgartirmaslik.
- Duplicate merge'ni o'zgartirmaslik.
- O'z mahsulotimizni aniqlashni o'zgartirmaslik.
- Uninstall kodini o'zgartirmaslik.
- Faqat jadvalga chiqarishdan oldingi yakuniy display filter kuchaytiriladi.
- Haqiqiy translator/transliterator/add-inlar saqlanadi.
- `Tarjimon Office UZ` har qanday holatda saqlanadi.
- `TransLit`, `Savodxon`, Translator/Translation, Tarjimon, Transliteration, Kirill/Lotin kabi kuchli translator signallari saqlanadi.
- Oddiy dastur faqat `convert/converter` kabi umumiy so'z sabab translator sifatida qoldirilmaydi.
- Office MUI, Proofing, Shared Components va shunga o'xshash texnik Office komponentlari jadvaldan chiqariladi.

Display filter real testdan muvaffaqiyatli o'tgach **100% deb belgilanadi va muzlatiladi**.

### Kod va loyiha tartibi — MUZLATILGAN QOIDALAR
- O'zboshimchalik bilan katta o'zgarish kiritilmaydi.
- Har bir o'zgarishdan oldin stabil holat saqlanadi.
- Xato chiqsa oldingi stabil holatga qaytish imkoniyati bo'lishi kerak.
- Root papkada vaqtinchalik `PATCH_*.bat` / `PATCH_*.ps1` fayllar ko'paytirilmaydi.
- Test tugagach vaqtinchalik skriptlar tozalanadi.
- Asosiy build uchun `FIX_BUILD_AND_BUILD.bat` saqlanadi.
- Ishlayotgan mexanizm faqat kichik, nazorat qilinadigan o'zgarish bilan yaxshilanadi.
- Build muvaffaqiyatli bo'lmaguncha yangi holat freeze qilinmaydi.
- Real test natijasi ko'rilmaguncha stabil kodga yangi o'zgarish kiritilmaydi.
- Muzlatilgan qismlarga 1.0 finalizatsiyasi davomida tegilmaydi.

## 1.0 FINAL YO'NALISHI

### Hozir bajariladi
1. **Dizayn — 100% va FREEZE**
2. **Display Filter — 80% → 100% va FREEZE**

### Hozir bajarilmaydi
- Qidiruvni qayta ishlab chiqish.
- Duplicate merge'ni qayta ishlab chiqish.
- Uninstallni qayta ishlab chiqish.
- Single-UACni qayta ishlab chiqish.
- Installer/WiXni qayta ishlab chiqish.
- Yangi qidiruv manbalari qo'shish.
- Yangi funksiyalar qo'shish.
- Yangi dizayn funksiyalari qo'shish.

### 1.0 yakuniy audit
Display Filter 100% bo'lgach:
- barcha muzlatilgan mexanizmlar buzilmaganini tekshirish;
- dizayn final holatini tekshirish;
- qidiruv natijalari kerakli translator/add-inlarni saqlab qolganini tekshirish;
- duplicate natijalar jamlanganini tekshirish;
- uninstall real testini tekshirish;
- Setup/MSI buildini tekshirish;
- yakuniy 1.0 buildni belgilash.

## Holat jadvali — 2026-08-24

| Modul | Foiz | Holat | 1.0 da |
|---|---:|---|---|
| 🔵 Qidiruv | **95%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🔵 Duplicate merge | **95%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟢 O'z mahsulotini aniqlash | **100%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟢 Dizayn / UI | **100%** | 🧊 Muzlatishga tayyor | Hozir yakunlanadi |
| 🟢 Installer / MSI / Setup | **95%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟢 Uninstall | **95%** | 🧊 Stabil / muzlatilgan | Tegilmaydi |
| 🟡 Single-UAC | **90%** | 🧊 Stabil | Tegilmaydi |
| 🟡 Display Filter | **80%** | 🔧 Jarayonda | Hozir 100% qilinadi |

### Umumiy loyiha bahosi
**≈ 95%** — bu loyiha boshqaruvi uchun taxminiy o'rtacha ko'rsatkich. Asosiy ishlaydigan mexanizmlarning katta qismi stabil/muzlatilgan; qolgan real texnik ishlar faqat **Display Filter** bilan cheklangan.

### Ranglar ma'nosi
- 🟢 **100%** — yakunlangan / muzlatilgan.
- 🔵 **90–99%** — ishlayotgan, stabil, muzlatilgan.
- 🟡 **70–89%** — jarayonda, hozirgi ish.
- 🔴 **0–69%** — muhim tugallanmagan qism.
- 🧊 **MUZLATILGAN** — 1.0 finalizatsiya davomida tegilmaydi.

## Muhim eslatma
**1.0 FINALIZATSIYA rejimi:** faqat ikkita yo'nalish — **Dizayn** va **Display Filter**. Qolgan barcha yaxshi ishlayotgan qismlar muzlatilgan. Avvalgi yaxshi natija saqlanadi; faqat nosoz joy kichik, alohida va qaytariladigan o'zgarish bilan tuzatiladi.
