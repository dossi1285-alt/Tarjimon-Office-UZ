# Tarjimon Office UZ — loyiha holati va keyingi reja

Sana: 2026-08-13

## Tasdiqlangan holat

Word 1.0 bosqichi yakunlandi. Word funksionalligi real sinovlar bilan 99.9% darajada qabul qilindi. Core transliteratsiya yadrosi va mavjud regression testlar ham yashil holatda.

## Joriy baholash

| Modul | Holat | Bajarilish |
|---|---|---:|
| Core | Yakunlangan | 100% |
| Word | Yakunlangan, 1.0 uchun yopildi | 99.9% |
| Tests | Regression testlar mavjud va PASS | 100% |
| Excel | Keyingi asosiy ish | 65% |
| Shared | Excel/umumiy UI va sozlamalardan keyin tekshiriladi | 75% |
| Setup / Installer | Excel va Shared'dan keyin | 55% |
| GitHub / loyiha strukturasi | Asosiy struktura tayyor | 90% |

Umumiy joriy baho: taxminan 84%.

## Tasdiqlangan bajarish tartibi

### 1-bosqich — Excel'ni yakunlash

Excel modulini Word darajasiga olib chiqish.

Tekshiruvlar:

1. Excel add-in yuklanishi.
2. Ribbon ko'rinishi va tugmalar ishlashi.
3. Lotin → Kirill.
4. Kirill → Lotin.
5. Bir nechta tanlangan kataklar.
6. Bitta katak.
7. Bo'sh kataklar.
8. Raqamlar va formulalar.
9. Formula kataklarini o'zgartirmaslik.
10. Protected worksheet holati.
11. Aralash matn.
12. O'zbek maxsus harflari.
13. e'lon/E'lon konteksti.
14. W/C xalqaro so'zlari.
15. Undo orqali qaytarish.
16. Katta tanlov/range bilan ishlash.
17. Xatolik holatlari.
18. Real Excel faylida yakuniy test.

Qabul mezoni: Excel 1.0 uchun kamida 99% amaliy tayyorlik va barcha kritik testlar PASS.

### 2-bosqich — Shared

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

## Muhim qoida

Word 1.0 yopilgan. Word kodiga faqat regressiya yoki kritik xato aniqlansa qaytiladi. Yangi qulayliklar va takomillashtirishlar 2.0 ro'yxatiga o'tkaziladi.

## Joriy ish holati

**Boshladik: keyingi asosiy vazifa — Excel modulini yakunlash.**
