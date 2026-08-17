# Tarjimon Office UZ — full project audit

Sana: 2026-08-17
Branch: `release/1.0-installer-cleanup`

## 1. Arxiv bo‘yicha holat

Foydalanuvchi yuborgan to‘liq loyiha ZIP arxivi tekshirildi. Arxivda Git metadata, Visual Studio lokal fayllari, build natijalari va source tree birga bor.

Source tree tarkibida 44 ta `.cs`, 9 ta `.resx`, 5 ta `.csproj`, 1 ta `.wixproj`, 1 ta `.vdproj` va 1 ta `.slnx` mavjud.

`.vs/`, `bin/`, `obj/`, tayyor `.msi`/`.exe` va `.pfx` signing key fayllari source repository uchun artefakt hisoblanadi. Ular arxivda mavjud bo‘lishi mumkin, lekin GitHub source sifatida commit qilinmaydi.

## 2. Git holati

Arxivdagi Git repository HEAD va remote branch bir xil commitda:

`cff4e937b16e7393a259c8c0a1b1a4d3f7fe0841`

Bu commit WiX 7 target va HeatWave prerequisite qayd etilgan holatdir.

Arxivdagi local working-tree o‘zgarishlari 35 ta faylga taalluqli ko‘rinsa ham, fayllarni CRLF/LF normalizatsiyasi bilan solishtirganda mazmuniy farq aniqlanmadi. Ya’ni bu o‘zgarishlar amalda faqat line-ending farqidir.

**Qat’iy qoida:** bunday 35 ta line-ending o‘zgarishini commit qilmaslik. Kod yoki loyiha fayliga haqiqiy o‘zgarish kiritilgandagina commit qilish.

## 3. Word3 xatosi

`TarjimonOfficeUZ.Word3.csproj` tarixda vaqtincha yaratilgan va keyin o‘chirilgan. Hozirgi release branch source tree’da u yo‘q.

Tarix:
- `ffc0b16` — vaqtinchalik `TarjimonOfficeUZ.Word3.csproj` yaratildi.
- `d04da9b` — u o‘chirildi.

**Qat’iy qoida:** `Word3.csproj`ni qayta yaratmaslik. Word loyihasi — `TarjimonOfficeUZ.Word/TarjimonOfficeUZ.Word.csproj`.

## 4. Yakuniy 1.0 arxitekturasi

1.0 uchun bitta installer bo‘ladi:

- Word: `TarjimonOfficeUZ.Word`
- Excel: `TarjimonOfficeUZ.Excel`
- Bitta MSI: `TarjimonOfficeUZ.Setup.Wix`

Alohida Word installer yoki alohida Excel installer chiqarilmaydi.

`TarjimonOfficeUZ.Setup` (`.vdproj`) faqat legacy/migratsiya loyihasi sifatida vaqtincha saqlanadi.

## 5. WiX source holati

`TarjimonOfficeUZ.Setup.Wix/TarjimonOfficeUZ.Setup.Wix.wixproj` WiX SDK 7 formatida:

`<Project Sdk="WixToolset.Sdk/7.0.0">`

WiX project Word va Excel loyihalariga ProjectReference qiladi.

`Package.wxs` esa Word va Excel output papkalarini MSI ichiga olishni hamda Word/Excel Office Add-in registry yozuvlarini yaratishni ko‘zda tutadi.

Bu source arxitekturasi to‘g‘ri yo‘nalishda, lekin **hali 1.0 installer 100% tasdiqlangan deb hisoblanmaydi**.

## 6. Asosiy blocker — Visual Studio WiX loyihasini incompatible ko‘rsatmoqda

Foydalanuvchi kompyuterida WiX Toolset 7 va HeatWave o‘rnatilgan. HeatWave Visual Studio 2026 bilan WiX 7 ni qo‘llab-quvvatlaydi.

Shunga qaramay, Visual Studio Solution Explorer’da `TarjimonOfficeUZ.Setup.Wix` hozircha `несовместимый` deb ko‘rinmoqda.

Bu holatni source kodini qayta yozish bilan hal qilish kerak emas. Avval Visual Studio/HeatWave/WiX SDK/MSBuild integratsiyasi va restore holati tekshiriladi.

**Tekshiruv tartibi:**
1. Visual Studio’da WiX/HeatWave extension holatini tekshirish.
2. WiX SDK 7 NuGet/MSBuild restore holatini tekshirish.
3. `.wixproj`ni alohida reload/build qilish.
4. `bin/obj`ni tozalab, restore/rebuild qilish.
5. Faqat shundan keyin `.slnx` orqali combined build tekshirish.

## 7. WiX package ichidagi aniqlangan blocker — Excel UndoBridge

Excel kodi `TarjimonOfficeUZ.UndoBridge.xlam` faylini `%APPDATA%\\Microsoft\\AddIns` ichidan qidiradi va Excel `OnUndo` mexanizmi orqali shu xlam ichidagi `UndoLastTranslation` makrosini chaqiradi.

Source tree’da:
- `TarjimonOfficeUZ.Excel/UndoBridge/TarjimonOfficeUZ.UndoBridge.xlam`
- `Install-UndoBridge.ps1`
- `Install-UndoBridge.cmd`
- `TarjimonOfficeUZ.UndoBridge.bas`

mavjud.

Ammo hozirgi WiX `Files Include` faqat Word va Excel `bin` outputlarini harvest qiladi. `.xlam` source fayli Excel `bin` outputiga avtomatik ko‘chirilmagan.

Shuning uchun hozirgi WiX MSI real o‘rnatilganda UndoBridge avtomatik o‘rnatilishi kafolatlanmagan.

**Yechim:** 1.0 installer tarkibiga UndoBridge’ni ham kiritish va uni o‘rnatilgandan keyin Excel ishlata oladigan joyga avtomatik joylashtirish. Buni qo‘lda skript ishga tushirishga bog‘lab qo‘ymaslik kerak, chunki 1.0 acceptance mezoni bitta installer bilan Word + Excel ishlashini talab qiladi.

## 8. WiX 32-bit registry komponentlari

`Package.wxs` x64 MSI ichida 32-bit Office registry view uchun `Bitness="always32"` komponentlardan foydalanadi.

Hozir bu komponentlar `ProgramFiles64Folder` ostidagi `WORDDIR`/`EXCELDIR` bilan bog‘langan.

WiX hujjatlariga ko‘ra `always32` komponent 32-bit bo‘ladi va 64-bit joylashuv bilan aralashtirilmasligi kerak.

**Yechim:** registry-only 32-bit komponentlar uchun 32-bitga mos alohida directory yoki boshqa architecture-safe joylashuv ishlatish. Word/Excel fayllarining o‘zi esa kerakli 64-bit install folder’da qolishi mumkin.

## 9. Startup settings registry muammosi

WiX installer Office Add-in registrationni `HKLM` ostida yaratadi:

- Word `HKLM\\Software\\Microsoft\\Office\\Word\\Addins\\TarjimonOfficeUZ.Word`
- Excel `HKLM\\Software\\Microsoft\\Office\\Excel\\Addins\\TarjimonOfficeUZ.Excel`

Lekin `OfficeAddInStartupService` sozlamalarni `HKCU` ostidan ochib, shu yerga `LoadBehavior` yozadi.

Natijada installer tomonidan yaratilgan HKLM registration bilan user settings’dagi HKCU yozuvi o‘zaro moslashtirilmagan.

**Yechim:** per-user override strategiyasini aniq belgilash. Eng xavfsiz variant — installer machine-wide registrationni o‘rnatadi, foydalanuvchi sozlamasi esa HKCU’da override sifatida boshqariladi va service mavjud HKCU/HKLM holatini to‘g‘ri hisobga oladi. Bu xatti-harakat alohida test qilinadi.

## 10. Signing muammosi

Word va Excel VSTO manifestlari signed.

Word project’da `ManifestCertificateThumbprint` mavjud va local release key ishlatiladi.

Excel project’da ham `SignManifests=true` va certificate thumbprint mavjud, lekin Excel uchun project faylida ko‘rsatilgan `TarjimonOfficeUZ.Excel_1_TemporaryKey.pfx` source tree’da mavjud emas; build artefaktlari esa local certificate store holatiga bog‘liq.

ZIP ichida Word signing key fayllari ham bor, lekin `.pfx` repositoryga commit qilinmasligi kerak.

**Release blocker:** 1.0 tarqatish uchun Word va Excel VSTO signing sertifikatlari ishonchli release certificate strategiyasiga o‘tkazilishi kerak. Self-signed/development certificate bilan tayyor mahsulotni 100% release deb e’lon qilmaslik.

## 11. Test holati

`TarjimonOfficeUZ.Tests` MSTest 4.0.2 va `net10.0` targetdan foydalanadi. Core esa `.NET Framework 4.7.2`.

Current project assets’da compatibility fallback mavjud va test assembly’da 10 ta test bor.

Foydalanuvchi yuborgan Visual Studio Test Explorer natijasida testlar topilgan, lekin `Не найдены тесты для запуска` ko‘rsatilgan. Screenshot’da testlar filterlanganligi ham ko‘rinadi.

**Xulosa:** bu hozircha test kodi buzilganini anglatmaydi. Avval Test Explorer filterlarini tozalash kerak. Keyin 10/10 testni real run qilish kerak.

## 12. Hujjatlar holati

`PROJECT_RULES.md` hozirgi canonical arxitektura sifatida WiX 7 va bitta installer qoidalarini saqlaydi.

Lekin eski `README.md`, `docs/ARCHITECTURE.md`, `docs/PROJECT_IMPORT.md` va `docs/PROJECT-STATUS-2026-08-13.md`ning ayrim joylarida eski installer/import holati yozilgan.

**Yechim:** installer blockerlar hal qilingandan keyin hujjatlarni bitta yakuniy arxitektura bilan sinxronlashtirish. Hozircha eski hujjatlarni ko‘r-ko‘rona o‘zgartirmaslik.

## 13. Source code bo‘yicha hozirgi xulosa

Core transliteratsiya yadrosi va Word/Excel 1.0 funksional kodlari mavjud. Arxivdagi source tree to‘liq ko‘rinishda va GitHub release branch bilan mazmunan mos.

Hozirgi asosiy ish yangi transliteratsiya qoidalarini yozish emas.

**1.0 uchun ustuvor yo‘nalish:**

1. WiX/HeatWave/Visual Studio integration blockerini hal qilish.
2. WiX MSI’ni Word + Excel bilan real build qilish.
3. UndoBridge’ni combined installerga to‘g‘ri kiritish.
4. 32-bit/64-bit registry registrationni tuzatish va ikki Office bitness’da tekshirish.
5. HKLM/HKCU startup settingsni tuzatish.
6. Signing/trust masalasini yakunlash.
7. Toza Windows/Office muhitida bitta MSI bilan Word + Excel install test.
8. Uninstall/reinstall va Office restart regression.
9. Faqat barcha acceptance mezonlari PASS bo‘lgandan keyin 1.0 ni frozen deb e’lon qilish.

## 14. Assistant working rule

Bu audit kelajakdagi ishlar uchun canonical project memory sifatida ishlatiladi.

Har bir yangi consequential o‘zgarishdan oldin:
- GitHub repository va active branch holati tekshiriladi.
- `PROJECT_RULES.md` va ushbu audit o‘qiladi.
- Foydalanuvchidan bir xil lokal ishni qayta-qayta bajarishni so‘rashdan oldin remote history va repository state tekshiriladi.
- Butun `.csproj` yoki `.wixproj`ni qayta yozishdan saqlaniladi.
- Line-ending-only diff commit qilinmaydi.
- Temporary `Word3.csproj` kabi fayllar qayta yaratilmaydi.
- 1.0 doim Word + Excel + ONE installer tamoyili bo‘yicha baholanadi.
