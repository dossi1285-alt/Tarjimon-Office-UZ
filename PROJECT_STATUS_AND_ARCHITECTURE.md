# Tarjimon Office UZ — loyiha holati va tasdiqlangan arxitektura

> Ushbu fayl loyiha uchun asosiy eslatma/checkpoint hisoblanadi. Har bir keyingi ish oldidan shu fayl o‘qiladi va qaysi etapga kelganimiz aniqlanadi.

## 1. Ishlash qoidasi

- Asosiy va yagona ishchi branch: `main`.
- O‘zgarishlar GitHub orqali qilinadi.
- Foydalanuvchi lokal kompyuterda asosan `Pull → Rebuild` bajaradi.
- Keraksiz vaqtinchalik, `bin`, `obj` va build natijalari source sifatida saqlanmaydi.
- Visual Studio 2026 ruscha interfeysidan foydalaniladi.
- Setup va Uninstaller mexanizmlari alohida aralashtirilmaydi.

## 2. Hozirgacha tasdiqlangan muvaffaqiyatli natijalar

- Dastur o‘rnatilishi va Word/Excel bilan ishlashi bo‘yicha oldingi testlar muvaffaqiyatli o‘tgan.
- Versiya ko‘rsatish bilan bog‘liq muammolar tekshirilgan va versiya qiymatlarini markazlashtirish zarurligi aniqlangan.
- Windows `Программы и компоненты` orqali qo‘lda uninstall qilish sinovdan o‘tgan: dastur ham, Word/Excel add-inlari ham to‘liq o‘chgan.
- **18-test — muvaffaqiyatli.** Mustaqil Uninstaller EXE Windows `Программы и компоненты` oynasini ochdi, `Tarjimon Office UZ`ni topdi/tanladi va `Удалить` jarayonini boshladi. Keyingi `Да/Нет/Далее` qarorlari foydalanuvchiga qoldi. Windows Installer orqali dastur va Word/Excel add-inlari to‘liq o‘chirildi.
- WiX/MSI build tartibi bilan bog‘liq xato tuzatildi; Rebuild'da 8 ta loyiha muvaffaqiyatli build bo‘lgan holat tasdiqlandi.
- `main` va `release/1.0-installer-cleanup` tarixidagi kerakli ishlar birlashtirildi; ortiqcha/restored variantlar olib tashlandi.
- Setup'dagi eski MSI uninstall oqimi source darajasida olib tashlandi.
- Yangi Setup oqimi uchun `InstallerFlow.cs` yaratildi.
- Setup Preflight endi 18-testdan o‘tgan Uninstaller EXE'ni build vaqtida publish qilib, Setup EXE ichiga embedded resource sifatida qo‘shishga tayyorlangan.
- Setup'ning startup object'i yangi `InstallerFlow`ga o‘tkazildi.
- 19-testdagi birinchi sinovda aniqlangan muammo: Uninstaller `Удалить` tugmasini bosgach darhol chiqib ketgan, Windows Installer esa hali eski dastur o‘chirilishini tugatmagan. Natijada Setup yangi MSI'ni juda erta ishga tushirib, `1603` ko‘rsatgan.
- Shu sabab **yangi Uninstaller'ning o‘zi yangilandi**: endi u Windows `Программы и компоненты` orqali uninstallni boshlaydi va Windows ro‘yxatidan `Tarjimon Office UZ` yo‘qolguncha kutadi. Foydalanuvchining `Да/Нет/Далее` qarorlari o‘zgarmadi.

## 3. Hozirgi loyiha strukturasi

```text
Tarjimon-Office-UZ
│
├── TarjimonOfficeUZ.Core
├── TarjimonOfficeUZ.Excel
├── TarjimonOfficeUZ.Setup
├── TarjimonOfficeUZ.Setup.Preflight
│   ├── DesignRuntime.cs
│   ├── InstallerFlow.cs              ← yangi asosiy Setup oqimi
│   └── TarjimonOfficeUZ.Setup.Preflight.csproj
├── TarjimonOfficeUZ.Setup.Wix
├── TarjimonOfficeUZ.Shared
├── TarjimonOfficeUZ.Tests
├── TarjimonOfficeUZ.Uninstaller       ← yagona yangi Uninstaller
├── TarjimonOfficeUZ.Word
└── TarjimonOfficeUZ.slnx
```

Eski `OwnOnlyInstaller.cs`, `Program.cs` va `ProgramV110.cs` olib tashlandi. `DisplayFilterRuntime.cs` ham eski installer oqimiga tegishli bo‘lgan va endi compile qilinmaydi. Alohida eski/restored Uninstaller nusxalari ham repositoryda saqlanmaydi; `TarjimonOfficeUZ.Uninstaller` yagona Uninstaller loyihasi hisoblanadi.

## 4. Yangi Uninstaller — 18-test mexanizmi + tugashni kutish

`TarjimonOfficeUZ.Uninstaller` mustaqil EXE sifatida ishlaydi.

```text
Uninstall EXE
    ↓
Windows "Программы и компоненты"
    ↓
"Tarjimon Office UZ"ni topish/tanlash
    ↓
"Удалить"ni ishga tushirish
    ↓
Windows'ning Да / Нет / Далее oynalari
    ↓
Windows Installer
    ↓
Uninstaller Windows ro‘yxatidan dastur yo‘qolishini kutadi
    ↓
Uninstaller 0 kodi bilan tugaydi
    ↓
Setup yangi installni boshlaydi
```

Uninstaller o‘zi MSI uninstall jarayonini yashirin yoki majburiy rejimda boshqarmaydi. Foydalanuvchining Windows tasdiqlashlari saqlanadi. Asosiy qo‘shimcha vazifa — Setup yangi MSI'ni eski uninstall tugashidan oldin ishga tushirib yubormasligi uchun tugash holatini kutish.

## 5. TASDIQLANGAN YANGI SETUP ARXITEKTURASI

### A. Eski versiya YO‘Q

```text
SETUP.EXE
  ↓
Boshlang‘ich tekshiruvlar
  ↓
Eski Tarjimon Office UZ mavjud emas
  ↓
Yangi install
  ↓
Core + Word + Excel + kerakli komponentlar
  ↓
"Tarjimon Office UZ muvaffaqiyatli o‘rnatildi."
  ↓
[ OK ]
  ↓
Setup yakunlanadi
```

### B. Eski versiya BOR

```text
SETUP.EXE
  ↓
Boshlang‘ich tekshiruvlar
  ↓
Eski Tarjimon Office UZ mavjud
  ↓
Bitta tasdiqlash:
"Eski versiya mavjud. Uni o‘chirib,
yangi versiyani o‘rnatishga rozimisiz?"
  ↓
       ┌───────────────┴───────────────┐
       │                               │
      HA                              YO‘Q
       │                               │
       ▼                               ▼
Yangi Uninstaller                 SETUP BEKOR
       │
       ▼
Windows "Программы и komponentlar"
       ↓
Tarjimon Office UZ topiladi/tanlanadi
       ↓
"Удалить"
       ↓
Foydalanuvchi: Да / Нет / Далее
       ↓
Windows Installer
       ↓
Uninstaller eski dastur ro‘yxatdan yo‘qolguncha kutadi
       ↓
Yangi INSTALL davom etadi
       ↓
Core + Word + Excel + kerakli komponentlar
       ↓
"Tarjimon Office UZ muvaffaqiyatli o‘rnatildi."
       ↓
[ OK ]
       ↓
Setup yakunlanadi
```

## 6. Muhim arxitektura qarorlari

1. **Eski Uninstaller mexanizmi kerak emas.** Eski Setup MSI `/x` oqimi olib tashlandi.
2. Setup eski versiyani topganda, avval foydalanuvchidan **bitta rozilik tasdig‘i** so‘raydi.
3. **HA** bo‘lsa — yagona yangi Uninstaller embedded EXE sifatida chiqarilib ishga tushiriladi.
4. Uninstaller Windows uninstall jarayoni haqiqatan tugaganini kutadi; shundan keyingina Setup yangi MSI'ni ishga tushiradi.
5. **YO‘Q** bo‘lsa — Setup o‘rnatishni to‘liq bekor qiladi.
6. Eski uninstall uchun `msiexec /x {ProductCode}` endi Setup oqimida ishlatilmaydi.
7. Yangi o‘rnatish tugagach:
   **`Tarjimon Office UZ muvaffaqiyatli o‘rnatildi.`**
   va **`OK`** tugmasi chiqadi.
8. `OK` bosilgach Setup tugaydi.

## 7. HOZIRGI ETAP

**Etap: 19-test — birinchi real testda 1603 sababi topildi va tuzatildi.**

Aniqlangan sabab: Uninstaller Windows'dagi `Удалить`ni bosgandan keyin darhol tugagan, Setup esa Windows Installer hali uninstallni tugatmagan paytda yangi MSI'ni ishga tushirgan. Bu 1603 ga olib kelgan.

GitHub'da tuzatish tayyor. Endi lokal tekshiruv:

1. `Pull origin`
2. Visual Studio 2026'da `Сборка → Перестроить решение`
3. Build xatosi bo‘lmasa Setup EXE'ni olish.
4. Eski Tarjimon Office UZ o‘rnatilgan kompyuterda yangi Setup'ni ishga tushirish.
5. Eski versiya borligi aniqlanganda **bitta HA/YO‘Q tasdig‘i** chiqishini tekshirish.
6. **HA** → yangi Uninstaller ochilishini tekshirish.
7. Windows uninstall oynasidagi foydalanuvchi tasdiqlarini bajarish.
8. Eski dastur va Word/Excel add-inlari o‘chishini kutish.
9. Uninstall tugagach Setup avtomatik ravishda yangi installni davom ettirishini tekshirish.
10. **1603 chiqmasligi** kerak.
11. Yakunda `Tarjimon Office UZ muvaffaqiyatli o‘rnatildi.` + `OK` chiqishini tekshirish.
12. **YO‘Q** varianti ham alohida tekshiriladi: Setup bekor bo‘lishi va yangi dastur o‘rnatilmasligi kerak.

Shundan keyin bu **19-test** sifatida qayd qilinadi. Keyingi bosqich — **20-test: boshqa/toza kompyuterda yakuniy end-to-end test**.
