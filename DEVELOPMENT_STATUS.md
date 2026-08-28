# Tarjimon Office UZ — Development Status

## 2026-08-24 — 1.0 FINALIZATION: DESIGN FROZEN + DISPLAY FILTER ONLY

### Ishlash rejimi — MUHIM
1.0 versiyani yakunlash bosqichidamiz.
Hozirdan boshlab faqat **1 ta faol ish** qoladi:
1. **Yakuniy Display Filter** — jadvalda faqat kerakli translator/add-inlarni qoldirish, real testdan o'tkazish va muzlatish.

**Dizayn 100% yakunlandi va MUZLATILDI.**

Boshqa ishlar hozircha qilinmaydi.
- Qidiruvga tegilmaydi.
- Duplicate merge'ga tegilmaydi.
- O'z mahsulotimizni aniqlashga tegilmaydi.
- Installer/WiX mexanizmiga tegilmaydi.
- Uninstall va Single-UAC mexanizmiga tegilmaydi.
- Dizayn qayta o'zgartirilmaydi.
- Yangi funksiyalar qo'shilmaydi.
- Display Filter 100% bo'lmaguncha yangi yo'nalish ochilmaydi.

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

### Dizayn / UI — 100% — MUZLATILGAN
- Asosiy `Office tarjimonlari` oynasi ishlaydi.
- Jadval ustunlari va checkbox mexanizmi saqlangan.
- `Tasdiqlash` va `Bekor qilish` tugmalaridan keyingi bo'sh joy ixcham va nazoratli holatga keltirildi.
- Tugmalar pastki panelga yopishib qolmaydi va ortiqcha oq joy qoldirilmaydi.
- Mavjud **ko'k** rang saqlangan va **yashil** aksent qo'shilgan.
- `Tasdiqlash` tugmasi yashil aksentda.
- `Bekor qilish` tugmasi ko'k aksentda.
- `Office tarjimonlarini aniqlash` subtitri yashil aksentda.
- `Tarjimon Office UZ` setup EXE uchun Word + Excel kombinatsiyasidagi ko'k/yashil icon tayyorlangan.
- Setup icon build vaqtida `.ico.b64` manbasidan qayta yaratiladi va `ApplicationIcon` sifatida ishlatiladi.
- Dizayn kodi alohida `DesignRuntime.cs` orqali qo'llanadi.
- Qidiruv, merge, uninstall va boshqa muzlatilgan mexanizmlarga dizayn tuzatishlari bilan tegilmaydi.
- Dizayn real UI ko'rinishida tekshirildi va foydalanuvchi tomonidan **100% qabul qilindi**.

### Installer — 95% — MUZLATILGAN STABIL HOLAT
- MSI/Setup o'rnatilishi ishlayapti.
- O'z dasturimiz o'chirilgandan keyin yangi versiyasi qayta o'rnatilmoqda.
- Yakuniy Setup EXE MSI ichiga joylashtirilmoqda.
- Build real testda muvaffaqiyatli o'tgan.
- Installer mexanizmiga tegilmaydi; faqat setup icon uchun build asset ishlatiladi.

### Uninstall — 95% — MUZLATILGAN STABIL HOLAT
- `TransLit` kabi begona tarjimonni uninstall qilish ishladi.
- `Tarjimon Office UZ`ning o'zini uninstall qilish va yangi nusxasini o'rnatish ishladi.
- Bir nechta tanlangan mahsulotni ketma-ket uninstall qilish uchun single-UAC mexanizmi qo'llanmoqda.
- Maqsad: foydalanuvchi `Tasdiqlash`ni bir marta bosadi, UAC bir marta chiqadi, keyin tanlangan uninstalllar ketma-ket bajariladi va oxirida MSI o'rnatiladi.
- 1.1.8 testida regex parser xatosi aniqlandi va MSI uninstall parseri tuzatildi.
- Uninstall mexanizmiga display filter ishini tuzatish bahonasida tegilmaydi.

### Yakuniy Display Filter — 80% — JARAYONDA
Hozirgi yagona faol texnik ish shu.

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
- **Dizayn 100% muzlatilgan: unga qayta o'zgarish kiritilmaydi.**

## 1.0 FINAL YO'NALISHI

### Hozir bajariladi
1. **Display Filter — 80% → 100% va FREEZE**

### Hozir bajarilmaydi
- Qidiruvni qayta ishlab chiqish.
- Duplicate merge'ni qayta ishlab chiqish.
- Uninstallni qayta ishlab chiqish.
- Single-UACni qayta ishlab chiqish.
- Installer/WiXni qayta ishlab chiqish.
- Yangi qidiruv manbalari qo'shish.
- Yangi funksiyalar qo'shish.
- Dizaynni qayta o'zgartirish.

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
| 🟢 Dizayn / UI | **100%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟢 Installer / MSI / Setup | **95%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟢 Uninstall | **95%** | 🧊 Muzlatilgan | Tegilmaydi |
| 🟡 Single-UAC | **90%** | 🧊 Stabil | Tegilmaydi |
| 🟡 Display Filter | **80%** | 🔧 Jarayonda | Hozir 100% qilinadi |

### Umumiy loyiha bahosi
**≈ 95%** — bu loyiha boshqaruvi uchun taxminiy o'rtacha ko'rsatkich. Asosiy ishlaydigan mexanizmlarning katta qismi stabil/muzlatilgan; qolgan real texnik ish faqat **Display Filter** bilan cheklangan.

### Ranglar ma'nosi
- 🟢 **100%** — yakunlangan / muzlatilgan.
- 🔵 **90–99%** — ishlayotgan, stabil, muzlatilgan.
- 🟡 **70–89%** — jarayonda, hozirgi ish.
- 🔴 **0–69%** — muhim tugallanmagan qism.
- 🧊 **MUZLATILGAN** — 1.0 finalizatsiya davomida tegilmaydi.

## Muhim eslatma
**1.0 FINALIZATSIYA rejimi:** dizayn yakunlandi va muzlatildi. Endi faqat **Display Filter** ustida ishlanadi. Qolgan barcha yaxshi ishlayotgan qismlar muzlatilgan. Avvalgi yaxshi natija saqlanadi; faqat nosoz joy kichik, alohida va qaytariladigan o'zgarish bilan tuzatiladi.
