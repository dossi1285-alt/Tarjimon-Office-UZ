# Mavjud loyiha importi

## Manba

Foydalanuvchi taqdim etgan Visual Studio loyihasi arxivi tekshirildi.

## Aniqlangan asosiy loyihalar

- `TarjimonOfficeUZ.Core` — transliteratsiya yadrosi.
- `TarjimonOfficeUZ.Shared` — umumiy sozlamalar va UI yordamchi qismlari.
- `TarjimonOfficeUZ.Word` — Microsoft Word VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Excel` — Microsoft Excel VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Tests` — test loyihasi.
- `TarjimonOfficeUZ.Setup` — installer loyihasi.

## Transliteratsiya yadrosi

`TarjimonOfficeUZ.Core/Translation` ichida quyidagi asosiy komponentlar mavjud:

- `AlphabetRule`
- `CurrentAlphabet`
- `TranslationDirection`
- `TranslationResult`
- `ReverseTranslationCache`
- `Transliterator`

`Transliterator` lotin ↔ kirill o‘girishni bajaradi va mapping ma’lumotlarini ham qaytaradi.

## Word va Excel

Word qo‘shimchasida Ribbon tugmalari orqali lotin → kirill va kirill → lotin amallari chaqiriladi.

Excel qo‘shimchasida tanlangan kataklar bo‘yicha transliteratsiya bajariladi; formulali kataklar o‘tkazib yuboriladi.

## GitHub'ga joylash qoidasi

Visual Studio tomonidan yaratiladigan `.vs`, `bin` va `obj` kataloglari repositoryga kiritilmasligi kerak.

Arxivdagi `.pfx` vaqtinchalik kalit fayllari ham repositoryga kiritilmaydi. Ular imzolash kalitlari bilan bog‘liq bo‘lishi mumkin va GitHub'da saqlash xavfsiz emas.

Installer va designer/resx fayllari mavjud loyiha tarkibining bir qismi bo‘lib, keyingi import bosqichida alohida ko‘rib chiqiladi.
