# 1 — Assistant language rule / Russian UI terminology

## Permanent rule

For the Tarjimon Office UZ project, the user must NOT have to repeatedly ask the assistant to use Russian UI terminology.

The user's Visual Studio, GitHub Desktop and Windows interface is Russian. When giving instructions about where to click, which menu to open, or which button/command to use, use the Russian UI label first whenever it is known.

Keep the explanation in Uzbek unless the user asks for another language.

### Examples

- **Сборка** — Build
- **Пересобрать решение** — Rebuild Solution
- **Очистить решение** — Clean Solution
- **Восстановить пакеты NuGet** — Restore NuGet Packages
- **Обозреватель решений** — Solution Explorer
- **Вывод** — Output
- **Система управления версиями** — Source Control
- **Получить изменения** / **Pull origin** — synchronize from GitHub, using the exact Russian label visible in the user's GitHub Desktop when known
- **Зафиксировать изменения** / Commit
- **Отправить изменения** / Push
- **Параметры** — Settings
- **Собрать** — Build selected project
- **Перезагрузить проект** — Reload Project

### If the exact Russian label is uncertain

Explain the action in Uzbek and put the likely Russian UI label in parentheses. Do not make the user ask again for Russian terminology.

### Project workflow reminder

GitHub Desktop is installed and is the normal Git workflow for this project. GitHub is the canonical project source/history. Do not forget this when giving instructions or planning changes.

### Priority

This is a permanent project instruction. Read it together with `PROJECT_RULES.md` before consequential project work.
