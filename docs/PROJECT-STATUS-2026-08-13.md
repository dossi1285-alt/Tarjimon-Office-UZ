# Tarjimon Office UZ — loyiha holati va keyingi reja

Sana: 2026-08-13

## Tasdiqlangan 1.0 holat

Word 1.0 bosqichi yakunlandi. Word funksionalligi real sinovlar bilan 99.9% darajada qabul qilindi. Word 1.0 kodi muzlatildi; yangi qulayliklar va takomillashtirishlar 2.0 ga qoldiriladi.

Core transliteratsiya yadrosi va mavjud regression testlar yashil holatda.

Excel 1.0 ham real sinovlar bilan yakunlandi: UsedRange/explicit selection ishlashi, Lotin ↔ Kirill, rang/format, W/C 1.0 qoidasi, formula va bo‘sh katak nazorati, tanlangan diapazon, Undo va Protected Sheet sinovlari foydalanuvchi tomonidan muvaffaqiyatli bajarildi. Excel uchun 1.0 qabul holati 99.9% deb belgilandi.

## Joriy baholash

| Modul | Holat | Bajarilish |
|---|---|---:|
| Core | Yakunlangan | 100% |
| Word | Yakunlangan, 1.0 uchun yopildi | 99.9% |
| Tests | Regression testlar mavjud va PASS | 100% |
| Excel | Yakunlangan, 1.0 uchun yopildi | 99.9% |
| Shared | Keyingi asosiy ish | 75% |
| Setup / Installer | Shared'dan keyin | 55% |
| GitHub / loyiha strukturasi | Asosiy struktura tayyor | 90% |

Joriy amaliy holat: Core + Word + Excel qismi yakunlangan; qolgan asosiy ish Shared va Setup/Installer.

## Tasdiqlangan bajarish tartibi

### 1-bosqich — Excel'ni yakunlash — TUGALLANGAN

Excel moduli Word darajasiga yaqinlashtirildi va real Excel fayllarida sinovdan o'tkazildi.

### 2-bosqich — Shared — KEYINGI ISH

Umumiy sozlamalar, resurslar, UI va yordamchi komponentlarni tekshirish. Excel va Word bilan bog'liq umumiy qismlar regressiya qilinadi.

### 3-bosqich — Setup / Installer

1. Release build.
2. MSI yaratish.
3. Toza muhitda o'rnatish.
4. Word add-in yuklanishi.
5. Excel add-in yuklanishi.
6. Uninstall/reinstall.
7. Office restartdan keyingi holat.
8. Prerequisite tekshiruvi.
9. Signing masalasini alohida ko'rib chiqish.

### 4-bosqich — Yakuniy 1.0 audit

Core + Word + Excel + Shared + Setup + Tests birgalikda tekshiriladi. Yakuniy jadval va 1.0 release holati chiqariladi.

## Muhim qoidalar

1. Word 1.0 yopilgan. Word kodiga faqat regressiya yoki kritik xato aniqlansa qaytiladi.
2. 1.0 da W/w va C/c qatnashgan, oldindan belgilangan tez-tez ishlatiladigan xalqaro so'zlar o'zgarmaydi.
3. Original katta-kichik yozilish saqlanadi.
4. Excel bitta aktiv katak holatida UsedRange, aniq ko‘p katakli selection holatida esa faqat tanlangan diapazonni tarjima qiladi.
5. Yangi qulayliklar va takomillashtirishlar 2.0 ro'yxatiga o'tkaziladi.

## Joriy ish holati

**Excel 1.0 tugallandi. Keyingi asosiy vazifa — Shared modulini tekshirish va yakunlash.**

<!-- AUTOSYNC-TEST-SHOW: 2026-08-13 19:11 Asia/Tashkent -->
