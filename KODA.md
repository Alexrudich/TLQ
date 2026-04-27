# KODA.md — Контекст проекта TLQ

## 📋 Обзор

**TLQ (Tarkov Lore Quests)** — серверный мод для SPT-AKI 4.0, добавляющий кастомные квесты на русском языке.

**Автор:** NLP-Core-Team  
**Версия:** 2.5.5  
**Платформа:** SPT-AKI 4.0.13+

---

## 🏗️ Архитектура

**Стек:**
- C# (.NET 9.0)
- SPTarkov.Server.Core 4.0.0
- WTT-ServerCommonLib

**Ключевое преимущество:** Квесты создаются **без перекомпиляции** — только JSON!

---

## 📁 Структура

```
TLQ/
├── db/CustomQuests/[Trader]/
│   ├── Locales/ru.json
│   ├── Quests/quest_definitions.json
│   └── QuestAssort/assort.json (опционально)
├── useful/
│   ├── CommonIds.txt
│   ├── items.json
│   ├── quests.json
│   └── prices.json
├── .koda/skills/spt-quest-creation.md  ← НАВЫК СОЗДАНИЯ
├── README.md
└── TLQ.csproj
```

---

## 📚 Документация

**Полное руководство по созданию квестов:**

👉 **`.koda/skills/spt-quest-creation.md`**

Включает:
- Паттерн резервирования ID
- Структуру квестов
- Типы условий и наград
- Шаблоны квестов
- Отладку и валидацию

---

## 🛠️ Полезные файлы

| Файл | Описание |
|------|----------|
| `useful/CommonIds.txt` | ID торговцев, боссов, локаций |
| `useful/items.json` | Все ID предметов |
| `useful/quests.json` | Ванильные квесты (примеры) |
| `useful/prices.json` | Цены предметов |

---

## 🔗 Ссылки

- **WTT-ServerCommonLib:** https://forge.sp-tarkov.com/mod/2310/wtt-commonlib
- **Примеры квестов:** https://github.com/sp-tarkov/server-mod-examples

---

**Приятной разработки!** 🎮
