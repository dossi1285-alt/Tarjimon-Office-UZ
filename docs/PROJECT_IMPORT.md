# Tarjimon Office UZ — loyiha importi

## Manba

Foydalanuvchi taqdim etgan Visual Studio loyihasi arxivi tekshirildi.

## GitHub import qoidalari

Repositoryga haqiqiy source va loyiha konfiguratsiyasi kiritiladi. Visual Studio/build artefaktlari kiritilmaydi.

### Kiritilmaydi

- `.vs/`
- `bin/`
- `obj/`
- `*.pfx` va boshqa signing key fayllari
- vaqtinchalik `.user`, `.suo` kabi lokal sozlamalar
- tayyor build artefaktlari (`.msi`, `.exe`) source repositoryga qo‘shilmaydi

### Kiritiladi

- `.cs`
- `.csproj`
- `.slnx`
- `.vdproj`
- `.resx`
- boshqa source/resource/config fayllari

## Asosiy loyihalar

- `TarjimonOfficeUZ.Core` — transliteratsiya yadrosi.
- `TarjimonOfficeUZ.Shared` — umumiy sozlamalar va UI yordamchi qismlari.
- `TarjimonOfficeUZ.Word` — Microsoft Word VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Excel` — Microsoft Excel VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Tests` — test loyihasi.
- `TarjimonOfficeUZ.Setup` — installer loyihasi.

## Transliteratsiya yadrosi

`TarjimonOfficeUZ.Core/Translation` ichida `AlphabetRule`, `CurrentAlphabet`, `TranslationDirection`, `TranslationResult`, `ReverseTranslationCache` va `Transliterator` kabi komponentlar mavjud.

`Transliterator` lotin ↔ kirill o‘girishni bajaradi va mapping ma’lumotlarini ham qaytaradi.

## Hozirgi ish

GitHub repositoryga loyiha fayllari foydalanuvchi kompyuteridan bosqichma-bosqich import qilinmoqda. Import tugagach, ZIP va GitHub tree taqqoslanib, yetishmayotgan yoki ortiqcha fayllar aniqlanadi.
