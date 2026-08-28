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

### Testlar

- Dastur o‘rnatilishi va Word/Excel bilan ishlashi bo‘yicha oldingi testlar muvaffaqiyatli o‘tgan.
- Versiya ko‘rsatish bilan bog‘liq muammolar tekshirilgan va versiya qiymatlarini markazlashtirish zarurligi aniqlangan.
- Windows `Программы и компоненты` orqali qo‘lda uninstall qilish sinovdan o‘tgan: dastur ham, Word/Excel add-inlari ham to‘liq o‘chgan.
- **18-test — muvaffaqiyatli.** Yangi mustaqil Uninstaller EXE ishga tushirildi; u Windows `Программы и компоненты` oynasini ochdi, `Tarjimon Office UZ`ni topdi/tanladi va `Удалить` jarayonini boshladi. Keyingi `Да/Нет/Далее` qarorlari foydalanuvchiga qoldi. Windows Installer orqali dastur va Word/Excel add-inlari to‘liq o‘chirildi.
- WiX/MSI build tartibi bilan bog‘liq xato tuzatildi; `Перестроить решение` natijasida 8 ta loyiha muvaffaqiyatli build bo‘lgan holat tasdiqlandi.
- `main` va `release/1.0-installer-cleanup` tarixidagi kerakli ishlar birlashtirildi; ortiqcha/restored variantlar olib tashlandi.

## 3. Hozirgi loyiha strukturasi

```text
Tarjimon-Office-UZ
│
├── TarjimonOfficeUZ.Core
├── TarjimonOfficeUZ.Excel
├── TarjimonOfficeUZ.Setup
├── TarjimonOfficeUZ.Setup.Preflight
├── TarjimonOfficeUZ.Setup.Wix
├── TarjimonOfficeUZ.Shared
├── TarjimonOfficeUZ.Tests
├── TarjimonOfficeUZ.Uninstaller
├── TarjimonOfficeUZ.Word
└── TarjimonOfficeUZ.slnx
```

## 4. 18-testdan o‘tgan Uninstaller — o‘zgarmas komponent

`TarjimonOfficeUZ.Uninstaller` mustaqil EXE sifatida ishlaydi.

Tasdiqlangan vazifasi:

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
Tarjimon Office UZ + Word add-in + Excel add-in o‘chiriladi
```

Uninstaller o‘zi MSI uninstall jarayonini yashirin yoki majburiy rejimda boshqarmaydi. Foydalanuvchining Windows tasdiqlashlari saqlanadi.

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
Tasdiqlash:
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
Windows "Программы и компоненты"
       ↓
Tarjimon Office UZ topiladi/tanlanadi
       ↓
"Удалить"
       ↓
Foydalanuvchi: Да / Нет / Далее
       ↓
Windows Installer
       ↓
Eski dastur + Word add-in + Excel add-in o‘chadi
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

1. **Eski Uninstaller mexanizmi kerak emas.** U butunlay chiqarib tashlanishi kerak; yangi 18-testdan o‘tgan Uninstaller ishlatiladi.
2. Setup eski versiyani topganda, avval foydalanuvchidan **bitta rozilik tasdig‘i** so‘raydi.
3. **HA** bo‘lsa — yangi Uninstaller ishga tushadi.
4. Uninstaller tugagach — Setup yangi o‘rnatishni davom ettiradi.
5. **YO‘Q** bo‘lsa — Setup o‘rnatishni to‘liq bekor qiladi.
6. Eski uninstall tugagandan keyin alohida `4. Windows Installer` va `5. Setup davom etadi` kabi foydalanuvchiga ko‘rinadigan mustaqil etaplar yo‘q; ular oqimning ichki qismi.
7. Yangi o‘rnatish tugagach, foydalanuvchiga aniq yakuniy xabar chiqadi:
   **`Tarjimon Office UZ muvaffaqiyatli o‘rnatildi.`**
   va **`OK`** tugmasi.
8. `OK` bosilgach Setup tugaydi.

## 7. Keyingi ish — HOZIRGI ETAP

**Etap: 19-testga tayyorgarlik — Setup ichidagi eski uninstall oqimini yangi Uninstaller bilan almashtirish.**

Hali bajarilmagan:

- Setup'dagi eski uninstall kodining barcha joylarini inventarizatsiya qilish.
- Keraksiz eski uninstall fayllari/kodlarini olib tashlash.
- Eski versiya aniqlanganda yangi Uninstaller'ni chaqirish.
- HA/YO‘Q tasdiqlash oqimini qo‘shish.
- Uninstaller tugagach Setup'ning yangi install oqimini davom ettirish.
- YO‘Q bo‘lsa Setup'ni bekor qilish.
- Yakuniy muvaffaqiyat xabari + `OK`ni qo‘shish.
- Bularni qilgandan keyin **19-test: reinstall**.
- Keyin **20-test: boshqa/toza kompyuterda yakuniy end-to-end test**.

> **Eslatma:** Ushbu bosqichda 18-testdan muvaffaqiyatli o‘tgan Uninstaller mexanizmini o‘zboshimchalik bilan o‘zgartirmaslik kerak. Avval Setup'dagi eski uninstall oqimi aniqlanadi, keyin faqat kerakli joylar almashtiriladi.
