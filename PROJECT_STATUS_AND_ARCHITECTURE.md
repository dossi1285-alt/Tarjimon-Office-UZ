# Tarjimon Office UZ — loyiha holati va tasdiqlangan arxitektura

> Ushbu fayl loyiha uchun asosiy eslatma/checkpoint hisoblanadi. Har bir keyingi ish oldidan shu fayl o‘qiladi va qaysi etapga kelganimiz aniqlanadi.

## 1. Ishlash qoidasi

- Asosiy va yagona ishchi branch: `main`.
- O‘zgarishlar GitHub orqali qilinadi.
- Foydalanuvchi lokal kompyuterda asosan `Pull → Rebuild` bajaradi.
- Keraksiz vaqtinchalik, `bin`, `obj` va build natijalari source sifatida saqlanmaydi.
- Visual Studio 2026 ruscha interfeysidan foydalaniladi.
- Setup va Uninstaller source loyihalari alohida, lekin Setup o‘z ichiga yagona yangi Uninstaller EXE'ni embedded qiladi.

## 2. Hozirgacha tasdiqlangan muvaffaqiyatli natijalar

- Dastur o‘rnatilishi va Word/Excel bilan ishlashi bo‘yicha oldingi testlar muvaffaqiyatli o‘tgan.
- Versiya ko‘rsatish bilan bog‘liq muammolar tekshirilgan va versiya qiymatlarini markazlashtirish zarurligi aniqlangan.
- Windows `Программы и компоненты` orqali qo‘lda uninstall qilish sinovdan o‘tgan: dastur ham, Word/Excel add-inlari ham to‘liq o‘chgan.
- **18-test — muvaffaqiyatli.** Mustaqil Uninstaller EXE Windows `Программы и компоненты` oynasini ochdi, `Tarjimon Office UZ`ni topdi/tanladi va uninstall jarayonini boshladi. Windows Installer orqali dastur va Word/Excel add-inlari to‘liq o‘chirildi.
- WiX/MSI build tartibi bilan bog‘liq xato tuzatildi; Rebuild'da 8 ta loyiha muvaffaqiyatli build bo‘lgan holat tasdiqlandi.
- `main` va `release/1.0-installer-cleanup` tarixidagi kerakli ishlar birlashtirildi; ortiqcha/restored variantlar olib tashlandi.
- Setup'dagi eski MSI uninstall oqimi source darajasida olib tashlandi.
- Yangi Setup oqimi `InstallerFlow.cs` orqali boshqariladi.
- Setup Preflight Uninstaller EXE'ni build vaqtida publish qilib, Setup EXE ichiga embedded resource sifatida qo‘shadi.
- 19-testning dastlabki sinovida `1603` sababi aniqlandi: Uninstaller Windows uninstall jarayoni tugamasdan qaytgan va Setup yangi MSI'ni juda erta boshlagan.
- Keyingi tuzatishda Uninstaller Windows `UninstallString`/`QuietUninstallString` mexanizmini to‘g‘ridan-to‘g‘ri chaqiradigan qilib qayta yozildi. MSI bo‘lsa `/X` uninstall rejimiga o‘tkazilib, `/qn /norestart` bilan Windows Installer orqali bajariladi va mahsulot registry'dan yo‘qolguncha kutadi.

## 3. Hozirgi loyiha strukturasi

```text
Tarjimon-Office-UZ
│
├── TarjimonOfficeUZ.Core
├── TarjimonOfficeUZ.Excel
├── TarjimonOfficeUZ.Setup
├── TarjimonOfficeUZ.Setup.Preflight
│   ├── DesignRuntime.cs
│   ├── InstallerFlow.cs              ← asosiy Setup oqimi
│   └── TarjimonOfficeUZ.Setup.Preflight.csproj
├── TarjimonOfficeUZ.Setup.Wix
│   ├── Package.wxs
│   └── License.rtf                   ← Setup shartlari uchun manba
├── TarjimonOfficeUZ.Shared
├── TarjimonOfficeUZ.Tests
├── TarjimonOfficeUZ.Uninstaller       ← yagona yangi Uninstaller
├── TarjimonOfficeUZ.Word
└── TarjimonOfficeUZ.slnx
```

Eski `OwnOnlyInstaller.cs`, `Program.cs` va `ProgramV110.cs` olib tashlangan/compile qilinmaydi. `DisplayFilterRuntime.cs` ham eski installer oqimiga tegishli bo‘lib, compile qilinmaydi. Alohida eski/restored Uninstaller nusxalari saqlanmaydi.

## 4. Yangi Uninstaller mexanizmi

Yangi Uninstaller foydalanuvchiga qo‘shimcha Windows oynalarini ochib, avtomatik bosish qilmaydi. U Windows'da ro‘yxatdan o‘tgan mahsulotning uninstall buyrug‘ini topadi va Windows Installer mexanizmini ishga tushiradi.

```text
Yangi Uninstall EXE
    ↓
Windows Registry'dan "Tarjimon Office UZ" topiladi
    ↓
QuietUninstallString / UninstallString olinadi
    ↓
Agar MSI bo‘lsa: msiexec /X ... /qn /norestart
    ↓
Windows Installer eski dastur + Word/Excel add-inlarini o‘chiradi
    ↓
Uninstaller registry'dan mahsulot yo‘qolguncha kutadi
    ↓
0 kodi
    ↓
Setup yangi MSI'ni boshlaydi
```

Bu usulning maqsadi — foydalanuvchini `Программы и компоненты`, `Удалить`, `Да/Нет/Далее` kabi qo‘shimcha bosqichlarga olib kirmaslik. Eski versiyani o‘chirishga rozilikni Setup'ning o‘zi bir marta so‘raydi.

## 5. TASDIQLANGAN YANGI SETUP ARXITEKTURASI

### 5.1. Setup ishga tushishi

```text
SETUP.EXE
  ↓
1. Texnik/dasturiy shartlar tekshiriladi
  ↓
2. Rozilik oynasi
  ↓
3. O‘rnatish papkasi
  ↓
4. Eski versiya aniqlangan holat ko‘rsatiladi
```

### 5.2. Rozilik oynasi

Rozilik oynasida:

- shartlar uzun bo‘lsa, ichki vertikal scroll mavjud;
- shartlar `License.rtf` manbasidan olinadi;
- `Roziman / qabul qilaman` checkbox'i **oldindan belgilangan**;
- foydalanuvchi galochkani olib tashlashi mumkin;
- galochka olib tashlansa `Далее` tugmasi faol bo‘lmaydi;
- qayta belgilansa davom etish mumkin.

### 5.3. O‘rnatish papkasi

```text
O‘rnatish papkasi
  ↓
Standart yo‘l ko‘rsatiladi
  ↓
[ yo‘l ] [Обзор...]
  ↓
Foydalanuvchi boshqa papkani tanlashi mumkin
```

Tanlangan yo‘l Setup tomonidan MSI'ga `INSTALLFOLDER` property orqali uzatiladi.

### 5.4. Eski versiya BOR

```text
Eski versiya aniqlanadi
  ↓
O‘rnatilgan versiya ko‘rsatiladi
  ↓
[Установить]
  ↓
Bitta Yes/No tasdig‘i
  ↓
       ┌───────────────┴───────────────┐
       │                               │
      HA                              YO‘Q
       │                               │
       ▼                               ▼
Yangi Uninstaller                 SETUP BEKOR
       │
       ▼
Windows Installer uninstall
       ↓
Uninstall tugashini kutish
       ↓
Yangi MSI install
       ↓
Core + Word + Excel + kerakli komponentlar
       ↓
"Tarjimon Office UZ muvaffaqiyatli o‘rnatildi."
       ↓
[ OK ]
       ↓
Setup yakunlanadi
```

### 5.5. Eski versiya YO‘Q

```text
Eski versiya aniqlanmadi
  ↓
[Установить]
  ↓
Yangi MSI install
  ↓
Core + Word + Excel + kerakli komponentlar
  ↓
"Tarjimon Office UZ muvaffaqiyatli o‘rnatildi."
  ↓
[ OK ]
  ↓
Setup yakunlanadi
```

## 6. Foydalanuvchi bosadigan asosiy tugmalar kamaytirildi

Yangi maqsad:

1. `Далее` — shartlar oynasidan keyin.
2. O‘rnatish joyini kerak bo‘lsa `Обзор...` orqali tanlash.
3. Eski versiya bo‘lsa — **bitta Yes/No tasdig‘i**.
4. Shundan keyin uninstall + install **avtonom**.
5. Ish tugaganda faqat **OK**.

Ya'ni Windows uninstall oynasidagi qo‘shimcha `Удалить`, `Да`, `Далее` bosishlari Setup oqimining bir qismi sifatida endi foydalanuvchiga yuklanmaydi.

## 7. Muhim qarorlar

1. Eski Uninstaller mexanizmi qayta ishlatilmaydi.
2. Yagona yangi `TarjimonOfficeUZ.Uninstaller` ishlatiladi.
3. Setup eski versiyani topganda foydalanuvchidan bitta tasdiq oladi.
4. **HA** → Uninstaller → Windows Installer uninstall → tugashini kutish → yangi MSI install.
5. **YO‘Q** → Setup to‘liq bekor qilinadi.
6. Uninstaller MSI uninstallni Windows Installer orqali bajaradi.
7. Setup yangi MSI'ni uninstall tugamasdan ishga tushirmaydi.
8. Yakunda `Tarjimon Office UZ muvaffaqiyatli o‘rnatildi.` va `OK` chiqadi.
9. `OK` bosilgach Setup tugaydi.

## 8. HOZIRGI ETAP

**Etap: 19-test — yangi arxitektura qayta sinovga tayyor.**

GitHub'da quyidagi o‘zgarishlar bajarildi:

- yangi Uninstaller Windows Registry'dagi Windows uninstall buyrug‘idan foydalanadigan qilindi;
- UIAutomation va `Программы и компоненты`ni avtomatik boshqarish mexanizmi olib tashlandi;
- Setup'ning rozilik oynasiga scroll qilinadigan shartlar qo‘shildi;
- checkbox oldindan `checked` holatda bo‘ldi, lekin foydalanuvchi uni olib tashlay oladi;
- checkbox olinmasa `Далее` bloklanadi;
- o‘rnatish papkasi aniq ko‘rsatiladi va `Обзор...` bilan o‘zgartiriladi;
- eski versiya aniqlanganda bitta tasdiqlashdan keyin uninstall + install avtomatik davom etadi;
- yakuniy `OK` saqlangan.

### Lokal test tartibi

1. `Pull origin`.
2. Visual Studio 2026 → `Сборка → Перестроить решение`.
3. Build xatosi bo‘lmasa `TarjimonOfficeUZSetup.exe`ni oling.
4. **19-A:** eski `Tarjimon Office UZ` o‘rnatilgan kompyuterda Setup'ni ishga tushiring.
5. Rozilik checkbox'i tayyor `checked` ekanini tekshiring.
6. Galochkani olib tashlab `Далее` bloklanishini tekshiring.
7. Galochkani qayta qo‘yib davom eting.
8. O‘rnatish papkasi ko‘rinishini va `Обзор...` ishlashini tekshiring.
9. Eski versiya `1.1.0` kabi to‘g‘ri aniqlanishini tekshiring.
10. `Установить` → `Да`.
11. Keyingi uninstall + yangi install bosqichlari **avtonom** bajarilishini kuting.
12. `1603` chiqmasligi kerak.
13. Word va Excel add-inlari yangi versiyada mavjudligini tekshiring.
14. Yakunda `O‘rnatish tugadi` + `OK` chiqishini tekshiring.
15. **19-B:** eski versiya yo‘q holatda ham xuddi shu Setup'ni sinang.
16. **19-C:** eski versiya bor holatda `Yo‘q` ni bosib Setup bekor bo‘lishini tekshiring.

Keyingi bosqich: **20-test — boshqa/toza kompyuterda yakuniy end-to-end test.**
