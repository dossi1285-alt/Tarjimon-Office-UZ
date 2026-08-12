# Tarjimon Office UZ — loyiha arxitekturasi

## Maqsad

Tarjimon Office UZ — o‘zbek tilidagi matnlarni kirill va lotin yozuvlari o‘rtasida o‘girishga mo‘ljallangan Office dasturlari loyihasi.

## Asosiy qismlar

- `TarjimonOfficeUZ.Core` — transliteratsiya yadrosi.
- `TarjimonOfficeUZ.Shared` — umumiy sozlamalar va yordamchi qismlar.
- `TarjimonOfficeUZ.Word` — Word VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Excel` — Excel VSTO qo‘shimchasi.
- `TarjimonOfficeUZ.Tests` — testlar.
- `TarjimonOfficeUZ.Setup` — installer.

## Rivojlantirish bosqichlari

1. Mavjud source kodni to‘liq va toza import qilish.
2. Kirill ↔ Lotin transliteratsiya yadrosini audit qilish.
3. Word va Excel integratsiyasini tekshirish.
4. Testlarni to‘liq tiklash va ishga tushirish.
5. Installer va release jarayonini tartibga solish.

## Repository qoidasi

`.vs`, `bin`, `obj`, signing key (`.pfx`) va tayyor build artefaktlari source repositoryga kiritilmaydi.

## Hozirgi holat

Mavjud loyiha source kodi foydalanuvchi kompyuteridan GitHub repositoryga bosqichma-bosqich import qilinmoqda. Import tugagach, source tree audit qilinadi.
