---
name: spt-quest-creation
description: Руководство по созданию кастомных квестов для SPT-AKI через JSON без перекомпиляции
---
# Создание кастомных квестов для SPT-AK (TLQ)

## 📋 Обзор

**TLQ (Tarkov Lore Quests)** — серверный мод для SPT-AKI 4.0, добавляющий кастомные квесты на русском языке.

**Ключевое преимущество:** WTT-ServerCommonLib позволяет создавать квесты **без перекомпиляции** — только редактируй JSON файлы!

---

## 📁 Структура проекта

```
TLQ/
├── db/
│   └── CustomQuests/
│       └── [TraderName]/           ← Папка торговца (Jaeger, Prapor, etc.)
│           ├── Locales/
│           │   └── ru.json         ← ТОЛЬКО локализация
│           ├── QuestAssort/        ← ОПЦИОНАЛЬНО (для торговли предметами)
│           │   └── assort.json     ← Не нужен для квестов!
│           └── Quests/
│               └── quest_definitions.json ← ОПРЕДЕЛЕНИЯ КВЕСТОВ
├── useful/                         ← Вспомогательные файлы
│   ├── CommonIds.txt              ← ID торговцев, боссов, локаций
│   ├── items.json                 ← Все ID предметов
│   ├── quests.json                ← Ванильные квесты (примеры механик)
│   └── prices.json                ← Цены предметов
├── KODA.md                        ← Контекст проекта
└── README.md                      ← Краткое описание
```

---

## 🎯 Два файла — один квест

**Для создания квеста нужно редактировать ТОЛЬКО два файла:**

1. **`quest_definitions.json`** — логика и условия квеста
2. **`ru.json`** — весь текст на русском языке

**Ничего компилировать не нужно!** Просто отредактируй JSON и перезапусти сервер.

---

## 🆔 Резервирование и валидация ID

### ⚠️ КРИТИЧЕСКИ ВАЖНО: Формат ID

**Все ID должны быть ТОЧНО 24 hex символа!** Никаких дефисов, суффиксов или разделителей!

**ПРАВИЛЬНО:**
- `"69eded8599836012ba920001"` ✅ (24 символа)
- `"69eded8599836012ba920002"` ✅ (24 символа)
- `"69eded8599836012ba920003"` ✅ (24 символа)

**НЕПРАВИЛЬНО:**
- `"69eded8599836012ba920001-002"` ❌ (с дефисом)
- `"002"` ❌ (слишком короткий)
- `"69eded8599836012ba920001002"` ❌ (27 символов)

### Паттерн генерации ID

**Базовый ID квеста:** `69eded8599836012ba9200XX`

Где `XX` — последовательный номер квеста:
- `69eded8599836012ba920001` — квест 1
- `69eded8599836012ba920011` — квест 2 (для цепочек)
- `69eded8599836012ba920021` — квест 3

**Sub-IDs для условий и наград:** Используй последовательные номера:
- `69eded8599836012ba920001` — ID квеста
- `69eded8599836012ba920002` — первое условие
- `69eded8599836012ba920003` — второе условие
- `69eded8599836012ba920004` — первая награда
- `69eded8599836012ba920005` — вторая награда

### Проверка перед созданием

```powershell
# Проверить занятые ID
Select-String "quest_definitions.json" -Pattern '"69eded[0-9a-f]{16}"'

# Проверить валидность всех ID в файле
$content = Get-Content "quest_definitions.json" -Raw
$ids = [System.Text.RegularExpressions.Regex]::Matches($content, '"id":\s*"([0-9a-f]{24})"') | ForEach-Object { $_.Groups[1].Value }
$invalid = $ids | Where-Object { $_.Length -ne 24 }
if ($invalid.Count -gt 0) { Write-Host "INVALID IDs found: $invalid" } else { Write-Host "All IDs valid" }
```

**Правила:**
- ✅ Все ID = 24 hex символа (0-9, a-f)
- ✅ Никаких дефисов, пробелов, разделителей
- ✅ Последовательные номера для sub-ID
- ✅ Проверяй и quest_definitions.json И ru.json

---

## 📝 Файл quest_definitions.json

### Базовая структура

```json
{
  "QUEST_ID": {
    "QuestName": "Внутреннее имя (не отображается)",
    "_id": "QUEST_ID",
    "canShowNotificationsInGame": true,
    "changeQuestMessageText": "QUEST_ID changeQuestMessageText",
    "conditions": {
      "AvailableForStart": [],
      "AvailableForFinish": [],
      "Fail": []
    },
    "declinePlayerMessageText": "QUEST_ID declinePlayerMessageText",
    "description": "QUEST_ID description",
    "failMessageText": "QUEST_ID failMessageText",
    "name": "QUEST_ID name",
    "note": "QUEST_ID note",
    "traderId": "TRADER_ID",
    "location": "any",
    "type": "PickUp",
    "restartable": false,
    "rewards": {
      "Started": [],
      "Success": [],
      "Fail": []
    },
    "side": "Pmc"
  }
}
```

### Обязательные ссылки на ru.json

Все текстовые поля должны ссылаться на ключи в `ru.json`:

| Поле | Ссылка | Описание |
|------|--------|----------|
| `name` | `"QUEST_ID name"` | Название квеста |
| `description` | `"QUEST_ID description"` | Полное описание |
| `startedMessageText` | `"QUEST_ID startedMessageText"` | При принятии |
| `successMessageText` | `"QUEST_ID successMessageText"` | При успехе |
| `failMessageText` | `"QUEST_ID failMessageText"` | При провале |
| `changeQuestMessageText` | `"QUEST_ID changeQuestMessageText"` | Цель квеста |
| `declinePlayerMessageText` | `"QUEST_ID declinePlayerMessageText"` | При отказе |
| `note` | `"QUEST_ID note"` | Заметка в интерфейсе |

---

## 🔢 СЧЕТЧИКИ — КРИТИЧЕСКИ ВАЖНО!

### Двойная конфигурация счетчика

**ОШИБКА:** Путать `value` в условии Kills и `value` в счетчике!

**ПРИМЕР (отстрел 10 диких в голову):**

```json
{
  "conditionType": "CounterCreator",
  "counter": {
    "conditions": [
      {
        "conditionType": "Kills",
        "bodyPart": ["Head"],
        "target": "Savage",
        "value": 1,        // ⚠️ СЧЕТЧИК СРАБАТЫВАЕТ КАЖДЫЙ РАЗ ПО 1
        "weapon": ["..."]
      }
    ],
    "id": "COUNTER_ID"
  },
  "id": "CONDITION_ID",
  "type": "Elimination",
  "value": 10              // ⚠️ ОБЩЕЕ КОЛИЧЕСТВО = 10
}
```

**Разбор:**
- `value: 1` в `Kills` — каждый выстрел в голову засчитывается как +1
- `value: 10` в `Elimination` — нужно 10 таких срабатываний

**ОШИБКА, которую легко допустить:**
```json
"value": 10,  // в Kills - БУДЕТ РАБОТАТЬ НЕПРАВИЛЬНО!
```

**Правило:**
- В `Kills` всегда `value: 1` (каждое убийство = 1 счет)
- В `Elimination`/`CounterCreator` ставь итоговое количество

---

## 🎯 Типы условий

### AvailableForStart (получение квеста)

| Тип | Описание | Пример |
|-----|----------|--------|
| `Level` | Уровень игрока | `{"type": "Level", "value": 15, "compareMethod": ">="}` |
| `Quest` | Выполнен другой квест | `{"type": "Quest", "target": "QUEST_ID", "status": [4]}` |
| `TraderStanding` | Репутация торговца | `{"type": "TraderStanding", "traderId": "...", "value": 0}` |

### AvailableForFinish (выполнение квеста)

| Тип | Описание | Ключевые поля |
|-----|----------|---------------|
| `HandoverItem` | Передать предмет | `target: [itemIds]`, `value: count`, `onlyFoundInRaid: true/false` |
| `FindItem` | Найти предмет | `target: [itemIds]`, `value: count`, `onlyFoundInRaid: true` |
| `CounterCreator` | Счётчик действий | Сложная вложенная структура (см. выше) |
| `Location` | Локация | `target: ["bigmap"]` (буквенное имя) |
| `AchieveSkillLevel` | Уровень навыка | `target: "Strength"`, `value: 50` |

### Fail (провал квеста)

- `ExtractPlayer` — не выбраться из рейда
- `Time` — истекло время

---

## 🏆 Типы наград

### Success (за выполнение)

```json
{
  "type": "Experience",
  "value": 20000
}
```

```json
{
  "type": "TraderStanding",
  "target": "54cb50c76803fa8b248b457b",
  "value": 0.05
}
```

```json
{
  "type": "Item",
  "items": [{
    "_id": "reward_item",
    "_tpl": "ITEM_ID",
    "upd": {"StackObjectsCount": 1}
  }],
  "value": 1
}
```

{
  "type": "Skill",
  "target": "Strength",
  "value": 100
}
```

**Важно:** Используй **`"target"`** для названия навыка, а не `"skill"`!

**Доступные навыки:**
`Endurance`, `Strength`, `Vitality`, `Health`, `StressResistance`, `Metabolism`, `Immunity`, `Perception`, `Intellect`, `Attention`, `Charisma`, `Pistol`, `SMG`, `Assault`, `Shotgun`, `Sniper`, `Melee`, `Crafting`, `HideoutManagement`, `Search`, `FirstAid`, `Throwing`, `Strength`, `Perception`, `Charisma`, и др.

**Примеры из квестов Егеря:**
```json
// Открытый навык (видно игроку)
{
  "type": "Skill",
  "target": "Shotgun",
  "value": 75,
  "unknown": false
}

// Скрытый навык (не видно какое именно повышение)
{
  "type": "Skill",
  "target": "Strength",
  "value": 100,
  "unknown": true
}

---

## 📄 Файл ru.json

### Структура

```json
{
  "QUEST_ID name": "Название квеста",
  "QUEST_ID description": "Полное описание квеста с деталями...",
  "QUEST_ID startedMessageText": "Что нужно сделать",
  "QUEST_ID successMessageText": "Текст при успехе",
  "QUEST_ID failMessageText": "Текст при провале",
  "QUEST_ID changeQuestMessageText": "Краткая цель",
  "QUEST_ID declinePlayerMessageText": "При отказе",
  "QUEST_ID note": "Заметка в интерфейсе",
  "CONDITION_ID": "Описание условия",
  "REWARD_ID": "Описание награды"
}
```

### Важные правила

1. **Все ключи с пробелом:** `"QUEST_ID name"` не `"QUEST_IDname"`
2. **UTF-8 кодировка:** Обязательно!
3. **Пустые строки между квестами:** Для читаемости
4. **Не дублируй ключи:** Один ключ = одно значение

---

## 🛠️ Процесс создания квеста

### Шаг 1: Резервируй ID

```powershell
# Проверь какие ID уже заняты
Select-String "D:\Custom mods sources\TLQ\db\CustomQuests\Jaeger\Quests\quest_definitions.json" -Pattern '"69eded'

# Выбери свободный номер
# Например: 69eded8599836012ba920051
```

### Шаг 2: Создай условие в quest_definitions.json

1. Открой `quest_definitions.json`
2. Добавь новый квест перед закрывающей `}`
3. Используй шаблон из этого навыка
4. Добавь запятую после предыдущего квеста

### Шаг 3: Добавь локализацию в ru.json

1. Открой `ru.json`
2. Добавь все тексты для нового квеста
3. **НЕ пиши текст прямо в quest_definitions.json!**

### Шаг 4: Валидация

```powershell
# Проверка quest_definitions.json
try {
  $clean = (Get-Content "quest_definitions.json" -Raw) -replace '//.*', ''
  $json = $clean | ConvertFrom-Json
  Write-Host "quest_definitions.json: VALID - $($json.PSObject.Properties.Count) квестов"
  
  # Проверка всех ID на формат 24 hex символа
  $ids = [System.Text.RegularExpressions.Regex]::Matches($clean, '"id":\s*"([0-9a-f]{24})"') | ForEach-Object { $_.Groups[1].Value }
  if ($ids.Count -gt 0) { Write-Host "✓ Found $($ids.Count) valid IDs" }
} catch {
  Write-Host "quest_definitions.json: INVALID - $_"
}

# Проверка ru.json
try {
  $locales = Get-Content "ru.json" -Raw | ConvertFrom-Json
  Write-Host "ru.json: VALID - $($locales.PSObject.Properties.Count) ключей"
  
  # Проверка ID в локализации
  $keys = $locales.PSObject.Properties.Name
  $questIdKeys = $keys | Where-Object { $_ -match '^[0-9a-f]{24}' }
  if ($questIdKeys.Count -gt 0) { Write-Host "✓ Found $($questIdKeys.Count) quest-related keys" }
} catch {
  Write-Host "ru.json: INVALID - $_"
}
```

### Шаг 5: Копирование на сервер

```powershell
Copy-Item "D:\Custom mods sources\TLQ\db\CustomQuests\Jaeger\Quests\quest_definitions.json" -Destination "D:\SPT\SPT\user\mods\TLQ\db\CustomQuests\Jaeger\Quests\quest_definitions.json" -Force
Copy-Item "D:\Custom mods sources\TLQ\db\CustomQuests\Jaeger\Locales\ru.json" -Destination "D:\SPT\SPT\user\mods\TLQ\db\CustomQuests\Jaeger\Locales\ru.json" -Force
```

### Шаг 6: Перезапуск сервера

```powershell
# Останови сервер
# Запусти сервер заново
# Проверь логи на ошибки
```

---

## 🔍 Вспомогательные файлы

### items.json

Все ID предметов игры. Используй для поиска:

```powershell
# Найти предмет по названию
Select-String "D:\Custom mods sources\TLQ\useful\items.json" -Pattern "Army Cap" -Context 0,2
```

### quests.json

Ванильные квесты Escape from Tarkov. Примеры всех механик:

```powershell
# Найти квест по названию
Select-String "D:\Custom mods sources\TLQ\useful\quests.json" -Pattern "Test Drive" -Context 0,50
```

**Примеры механик:**
- Убийства с калибром оружия
- Убийства по времени суток
- Вынос предметов из рейда
- Выживание в рейде
- Использование предметов

### CommonIds.txt

ID торговцев, боссов, локаций:

```
Traders:
Jaeger: 5c0647fdd443bc2504c2d371
Prapor: 54cb50c76803fa8b248b4571

Maps:
Woods: 5704e3c2d2720bac5b8b4567
Customs: 56f40101d2720b2a4d8b45d6

Bosses:
Shturman: bossKojaniy
Killa: bossKilla
```

### prices.json

Цены предметов для баланса наград.

---

## 📋 Шаблоны квестов

### Шаблон 1: Убить X целей в голову

```json
{
  "QUEST_ID": {
    "QuestName": "Убийство целей",
    "_id": "QUEST_ID",
    "traderId": "5c0647fdd443bc2504c2d371",
    "location": "any",
    "type": "Elimination",
    "conditions": {
      "AvailableForStart": [
        {"type": "Level", "value": 15, "compareMethod": ">="}
      ],
      "AvailableForFinish": [
        {
          "conditionType": "CounterCreator",
          "counter": {
            "conditions": [
              {
                "conditionType": "Kills",
                "bodyPart": ["Head"],
                "target": "Savage",
                "value": 1,
                "weapon": ["WEAPON_ID"]
              }
            ],
            "id": "COUNTER_ID"
          },
          "id": "CONDITION_ID",
          "type": "Elimination",
          "value": 10
        }
      ],
      "Fail": []
    },
    "rewards": {
      "Success": [
        {"type": "Experience", "value": 20000},
        {"type": "TraderStanding", "target": "5c0647fdd443bc2504c2d371", "value": 0.05}
      ]
    }
  }
}
```

### Шаблон 2: Передать X предметов (найденных в рейде)

```json
{
  "QUEST_ID": {
    "QuestName": "Сбор предметов",
    "_id": "QUEST_ID",
    "traderId": "5c0647fdd443bc2504c2d371",
    "type": "PickUp",
    "conditions": {
      "AvailableForStart": [
        {"type": "Level", "value": 15, "compareMethod": ">="}
      ],
      "AvailableForFinish": [
        {
          "conditionType": "HandoverItem",
          "target": ["ITEM_ID_1", "ITEM_ID_2"],
          "value": 15,
          "onlyFoundInRaid": true
        }
      ],
      "Fail": []
    },
    "rewards": {
      "Success": [
        {"type": "Experience", "value": 30000}
      ]
    }
  }
}
```

### Шаблон 3: Цепочка квестов

```json
{
  "QUEST_ID_PART_2": {
    "QuestName": "Часть 2",
    "_id": "QUEST_ID_PART_2",
    "conditions": {
      "AvailableForStart": [
        {"type": "Level", "value": 15, "compareMethod": ">="},
        {"type": "Quest", "target": "QUEST_ID_PART_1", "status": [4]}
      ]
    }
  }
}
```

`status: [4]` означает "квест завершён".

---

## 🐛 Частые ошибки

### Ошибка 0: Удаление и пересоздание файлов quest_definitions.json

**⚠️ КРИТИЧЕСКИ ВАЖНОЕ ПРАВИЛО:**

**НИКОГДА не удаляй и не пересоздавай файл `quest_definitions.json`!**

Когда добавляешь новый квест:
- ✅ **ДОПИСЫВАЙ** новый квест в конец существующего файла (перед закрывающей `}`)
- ✅ Используй `edit_file` с `old_string`/`new_string` для вставки
- ✅ Добавь запятую после предыдущего квеста

**НЕПРАВИЛЬНО:**
```powershell
# ❌ УДАЛИТЬ файл и создать заново
Remove-Item quest_definitions.json
New-Item quest_definitions.json  # ОШИБКА!
```

**ПРАВИЛЬНО:**
```powershell
# ✅ Добавить в конец файла перед закрывающей }
# Используем edit_file для вставки нового квеста
```

**Почему это важно:**
- Потеря истории изменений
- Риск потерять существующие квесты при ошибке
- Лишняя работа по пересозданию

---

### Ошибка 1: Пропуск запятой

```json
{
  "quest1": { ... }    // ЗАПЯТАЯ ОБЯЗАТЕЛЬНА!
  "quest2": { ... }
}
```

### Ошибка 2: Прямой текст вместо ссылки

```json
// ПЛОХО:
"name": "Трофеи часть 1"

// ХОРОШО:
"name": "69eded8599836012ba920001 name"
```

### Ошибка 3: Неправильная кодировка

- **Обязательно UTF-8**
- PowerShell: `Set-Content -Encoding UTF8`

### Ошибка 4: Путаница в value счетчика

```json
// ОШИБКА:
"value": 10  // в Kills - будет засчитывать только 10 убийств за раз

// ПРАВИЛЬНО:
"value": 1   // в Kills
"value": 10  // в Elimination (общее количество)
```

### Ошибка 5: Невалидный ID

**ТРЕБОВАНИЕ:** Все ID = 24 hex символа (0-9, a-f), БЕЗ дефисов!

```json
// ПЛОХО:
"id": "69eded8599836012ba920001-002"  // ❌ с дефисом
"id": "002"  // ❌ слишком короткий

// ХОРОШО:
"id": "69eded8599836012ba920002"  // ✅ 24 символа
"id": "69eded8599836012ba920003"  // ✅ 24 символа
```

**Проверка:**
```powershell
# Найти все невалидные ID
Select-String "quest_definitions.json" -Pattern '"id":\s*"[0-9a-f-]{2,}"' | Where-Object { $_.Line -notmatch '"[0-9a-f]{24}"' }
```

### Ошибка 6: Забыл проверить ru.json

**Нужно проверять И quest_definitions.json И ru.json на одинаковые ID!**

```powershell
# Проверить соответствие ID между файлами
$questIds = (Get-Content "quest_definitions.json" -Raw | ConvertFrom-Json).PSObject.Properties.Name
$localeKeys = (Get-Content "ru.json" -Raw | ConvertFrom-Json).PSObject.Properties.Name
foreach ($id in $questIds) {
  if ($localeKeys -notcontains "$id name") { Write-Host "⚠ Missing: $id name" }
}
```

---

## 🧪 Отладка

### 1. Проверь логи сервера

Ищи сообщения о загрузке квестов:
```
[TLQ] Quests loaded!
```

### 2. Проверь валидность JSON

```powershell
try {
  $json = Get-Content "quest_definitions.json" -Raw | ConvertFrom-Json
  Write-Host "Квестов: $($json.PSObject.Properties.Count)"
} catch {
  Write-Host "Ошибка: $_"
}
```

### 3. Проверь наличие всех ключей локализации

```powershell
$questId = "69eded8599836012ba920001"
$locales = Get-Content "ru.json" -Raw | ConvertFrom-Json
$requiredKeys = @(
  "$questId name",
  "$questId description",
  "$questId startedMessageText",
  "$questId successMessageText",
  "$questId failMessageText"
)
foreach ($key in $requiredKeys) {
  if ($locales.PSObject.Properties.Name -contains $key) {
    Write-Host "✓ $key"
  } else {
    Write-Host "✗ $key - ОТСУТСТВУЕТ!"
  }
}
```

### 4. Кэш профиля

Если квесты не появляются:
- Останови сервер
- Удали папку `D:\SPT\SPT\user\profiles\YOUR_PROFILE_ID`
- Запусти сервер заново

---

## 📚 Полезные команды PowerShell

```powershell
# Проверка валидности JSON
try { Get-Content "file.json" -Raw | ConvertFrom-Json | Out-Null; Write-Host "VALID" } catch { Write-Host "INVALID: $_" }

# Поиск квеста по названию
Select-String "quest_definitions.json" -Pattern "Название квеста"

# Поиск ID предмета
Select-String "items.json" -Pattern "Army Cap" -Context 0,2

# Копирование файлов на сервер
Copy-Item "source\file.json" -Destination "D:\SPT\SPT\user\mods\TLQ\path\file.json" -Force

# Подсчёт квестов
(Get-Content "quest_definitions.json" -Raw | ConvertFrom-Json).PSObject.Properties.Count
```

---

## 📞 Контакты

**Автор:** NLP-Core-Team  
**WTT-ServerCommonLib:** https://forge.sp-tarkov.com/mod/2310/wtt-commonlib

---

**Приятной разработки квестов!** 🎮
