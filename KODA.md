# KODA.md — Контекст проекта TLQ (Tarkov Lore Quests)

## 📋 Обзор проекта

**TLQ (Tarkov Lore Quests)** — серверный мод для SPT-AKI 4.0, добавляющий кастомные квесты на русском языке для всех торговцев игры.

**Назначение:** Расширение контента игры через систему кастомных квестов с использованием WTT-ServerCommonLib.

**Автор:** NLP-Core-Team / Aebisher

**Версия:** 2.5.5

**Целевая платформа:** SPT-AKI 4.0.13+

---

## 🏗️ Архитектура и технологии

### Стек технологий
- **Язык:** C# (.NET 9.0)
- **Фреймворк:** SPTarkov.Server.Core 4.0.0
- **DI-контейнер:** SPTarkov.DI 4.0.0
- **Библиотека квестов:** WTT-ServerCommonLib
- **Целевой фреймворк:** net9.0

### Структура проекта
```
TLQ/
├── TLQ.csproj              # Конфигурация проекта
├── TLQ.cs                  # Основной класс мода
├── README.md               # Документация
├── db/
│   └── CustomQuests/       # Определения квестов
│       ├── Therapist/      # Квесты Терапевта (11 квестов)
│       ├── Ragman/         # Квесты Барахольщика (10 квестов)
│       ├── Peacekeeper/    # Квесты Миротворца (6 квестов)
│       ├── Prapor/         # Квесты Прапора (8 квестов)
│       ├── Fence/          # Квесты Барахольщика (4 квеста)
│       ├── Skier/          # Квесты Лыжника (1 квест)
│       ├── Jaeger/         # Квесты Егеря (3 квеста)
│       └── Mechanic/       # Пустая директория
├── useful/                 # Вспомогательные файлы
│   ├── CommonIds.txt       # ID торговцев, боссов, локаций
│   ├── handbook.json       # Справочник предметов
│   ├── items.json          # Данные предметов
│   ├── prices.json         # Цены предметов
│   ├── quests.json         # Примеры квестов
│   ├── tarkov_data*.json   # Данные игры
└── Properties/
    └── launchSettings.json # Настройки запуска
```

### Формат квестов

Квесты определяются в JSON-файлах `quest_definitions.json` по паттерну WTT-ServerCommonLib:

**Ключевые поля:**
- `_id` — Уникальный ID квеста (24 hex символа)
- `QuestName` — Отображаемое название
- `traderId` — ID торговца (дающего квест)
- `conditions.AvailableForStart` — Условия для получения
- `conditions.AvailableForFinish` — Условия выполнения
- `conditions.Fail` — Условия провала
- `rewards.Success` — Награды за выполнение
- `rewards.Started` — Награды за получение
- `location` — Локация квеста ("any" или ID локации)
- `restartable` — Можно ли повторить после провала

**Типы условий:**
- `Level` — Минимальный уровень игрока
- `Quest` — Выполнение других квестов
- `Elimination` — Устранение целей
- `CounterCreator` — Счётчики выполнения
- `Location` — Локация выполнения
- `Kills` — Убийства с оружием/калибром

**Типы наград:**
- `Experience` — Опыт
- `TraderStanding` — Репутация торговца
- `Skill` — Прокачка навыка
- `Item` — Предметы

---

## 🔧 Сборка и запуск

### Предварительные требования
1. .NET 9.0 SDK
2. SPT-AKI 4.0.13+
3. WTT-ServerCommonLib (установлен в `D:\SPT\SPT\user\mods\WTT-ServerCommonLib\`)

### Команды сборки

```bash
# Сборка в Release режиме
dotnet build -c Release

# Сборка в Debug режиме
dotnet build -c Debug
```

### Размещение в SPT

После сборки файл `TLQ.dll` находится в:
```
bin\Release\TLQ\TLQ.dll
```

**Установка:**
1. Скопировать `db/` в `BepInEx/plugins/TLQ/db/`
2. Скопировать `TLQ.dll` в `BepInEx/plugins/TLQ/`

---

## 📝 Правила разработки

### Именование ID квестов
- Используй 24-символьные hex строки
- Для цепочек квестов используй последовательные ID (например, `67b100000000000000000001`, `67b100000000000000000002`)
- ID условий должны быть уникальными внутри квеста

### Локализация
- Все текстовые поля должны ссылаться на `ru.json` через ID (например, `"67b100000000000000000001 name"`)
- **НЕ** используй прямой текст в `quest_definitions.json`
- Файл локализации: `db/CustomQuests/<Trader>/Locales/ru.json`

### Валидация JSON
Перед каждым изменением проверяй валидность JSON:
```powershell
Get-Content "path/to/quest_definitions.json" -Raw | ConvertFrom-Json | Out-Null
```

### Паттерны квестов

**Цепочка квестов:**
- Условие `AvailableForStart` с типом `Quest` и `status: [4]` (завершён)
- `target` — ID предыдущего квеста

**Устранение с оружием:**
```json
{
  "conditionType": "Kills",
  "weapon": ["<weapon_id>"],
  "savageRole": ["<boss_name>"],
  "target": "Savage",
  "value": <количество>
}
```

**Прокачка навыка:**
```json
{
  "type": "Skill",
  "target": "<SkillName>",
  "value": <значение>
}
```

Доступные навыки: `Endurance`, `Strength`, `Vitality`, `Health`, `StressResistance`, `Metabolism`, `Immunity`, `Perception`, `Intellect`, `Attention`, `Charisma`, `Pistol`, `Revolver`, `SMG`, `Assault`, `Shotgun`, `Sniper`, `LMG`, `HMG`, `Throwing`, `Melee`, `DMR`, `AimDrills`, `TroubleShooting`, `Surgery`, `Search`, `MagDrills`, `FirstAid`, `LightVests`, `HeavyVests`, `WeaponTreatment`, `Crafting`, `HideoutManagement`, и др.

---

## 📚 Важные ID

### Торговцы
| Торговец | ID |
|----------|-----|
| Fence | 579dc571d53a0658a154fbec |
| Jaeger | 5c0647fdd443bc2504c2d371 |
| Mechanic | 5a7c2eca46aef81a7ca2145d |
| Peacekeeper | 5935c25fb3acc3127c3d8cd9 |
| Prapor | 54cb50c76803fa8b248b4571 |
| Ragman | 5ac3b934156ae10c4430e83c |
| Skier | 58330581ace78e27b8b10cee |
| Therapist | 54cb57776803fa99248b456e |

### Боссы
| Босс | ID |
|------|-----|
| Shturman | bossKojaniy |
| Sanitar | bossSanitar |
| Killa | bossKilla |
| Reshala | bossBully |
| Glukhar | bossGluhar |
| Kaban | bossBoar |

### Локации
| Локация | ID |
|---------|-----|
| Woods | 5704e3c2d2720bac5b8b4567 |
| Reserve | 5704e5fad2720bc05b8b4567 |
| Customs | 56f40101d2720b2a4d8b45d6 |
| Factory | 55f2d3fd4bdc2d5f408b4567 |

---

## 🛠️ Полезные команды

### Проверка валидности JSON
```powershell
try { 
  Get-Content "path/to/file.json" -Raw | ConvertFrom-Json | Out-Null
  Write-Host "JSON Valid" 
} catch { 
  Write-Host "JSON Invalid: $_" 
}
```

### Копирование в папку SPT
```powershell
Copy-Item "source\path" -Destination "D:\SPT\SPT\user\mods\TLQ\path" -Force
```

### Поиск квестов по названию
```powershell
Select-String -Path "quest_definitions.json" -Pattern '"QuestName":'
```

---

## 📊 Текущая статистика квестов

| Торговец | Количество квестов |
|----------|-------------------|
| Барахольщик | 10 |
| Прапор | 8 |
| Миротворец | 6 |
| Терапевт | 11 |
| Скупщик | 4 |
| Лыжник | 1 |
| Егерь | 3 |
| **ВСЕГО** | **43** |

---

## 🐛 Известные проблемы и решения

### Проблема: "Имя квеста отображается как 'Название name'"
**Причина:** В `quest_definitions.json` поле `name` содержит прямой текст вместо ссылки на локализацию.
**Решение:** Заменить на `"name": "<quest_id> name"`

### Проблема: Квест не появляется в игре
**Возможные причины:**
1. JSON невалиден
2. Файлы не скопированы в папку мода
3. Сервер не перезапущен
4. WTT-ServerCommonLib не установлен

### Проблема: Условия выполнения не срабатывают
**Проверь:**
1. ID оружия в поле `weapon`
2. ID цели в поле `savageRole`
3. ID локации в поле `target` массива Location
4. Правильность типа условия

---

## 📞 Контакты и поддержка

**GitHub:** https://github.com/sp-tarkov/server-mod-examples  
**WTT-ServerCommonLib:** https://forge.sp-tarkov.com/mod/2310/wtt-commonlib

---

**Приятной разработки!** 🎮