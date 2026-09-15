# EventManager

ASP.NET Core Web API сервис для управления мероприятиями

## Основные возможности

Сервис предоставляет RESTful API для управления мероприятиями с базовыми CRUD-операциями

- Создание новых мероприятий с основной информацией (заголовок, описание, время)
- Обновление существующих мероприятий
- Удаление мероприятий
- Swagger документация

### Примеры запросов и ответов

#### Получение мероприятия по ID

**Запрос:**
```http
GET http://localhost:5226/api/events/12345678-1234-1234-1234-123456789abc
```

**Ответ 200 OK:**
```json
{
  "id": "12345678-1234-1234-1234-123456789abc",
  "title": "Event1",
  "description": "Event1 details",
  "startAt": "2026-09-15T10:00:00Z",
  "endAt": "2026-09-15T17:00:00Z"
}
```

#### Получение списка мероприятий

**Запрос с фильтрацией и пагинацией:**
```http
GET http://localhost:5226/api/events?Page=1&PageSize=10&Title=Event&From=2026-09-01T00:00:00Z&To=2026-09-30T23:59:59Z
```

**Query параметры:**
| Параметр | Тип       | Описание                                   |
| -------- | --------- | ------------------------------------------ |
| Page     | int?      | Номер страницы (начиная с 1)               |
| PageSize | int?      | Размер страницы (по умолчанию 10)          |
| Title    | string?   | Фильтр по заголовку (частичное совпадение) |
| From     | DateTime? | Начало периода (фильтр по StartAt)         |
| To       | DateTime? | Конец периода (фильтр по EndAt)            |

**Ответ 200 OK:**
```json
{
  "totalCount": 2,
  "page": 1,
  "pageSize": 10,
  "results": [
    {
      "id": "12345678-1234-1234-1234-123456789abc",
      "title": "Event1",
      "description": "Event1 details",
      "startAt": "2026-09-15T10:00:00Z",
      "endAt": "2026-09-15T17:00:00Z"
    },
    {
      "id": "87654321-4321-4321-4321-cba987654321",
      "title": "Event2",
      "description": "Event2 details",
      "startAt": "2026-09-16T14:00:00Z",
      "endAt": "2026-09-16T15:30:00Z"
    }
  ]
}
```

#### Создание нового мероприятия

**Запрос:**
```http
POST http://localhost:5226/api/events
```

```json
{
  "title": "Event3",
  "description": "Event3 description",
  "startAt": "2026-10-01T09:00:00Z",
  "endAt": "2026-10-01T11:00:00Z"
}
```

**Ответ 201 Created:**
```json
{
  "id": "d2e02a3e-89eb-4a4d-84da-9a4e7171cc2a",
  "title": "Event3",
  "description": "Event3 description",
  "startAt": "2026-10-01T09:00:00Z",
  "endAt": "2026-10-01T11:00:00Z"
}
```

#### Формат ответа при ошибках

```json
{
  "title": "Resource not found",
  "status": 404,
  "detail": "Event '3fa85f64-5717-4562-b3fc-2c963f66afa6' not found"
}
```

| Поле   | Тип     | Описание                         |
| ------ | ------- | -------------------------------- |
| title  | string  | Сообщение об ошибке              |
| status | int     | HTTP код ответа                  |
| detail | string? | Описание ошибки                  |

## Технологии и архитектура

- **Язык**: C#
- **Фреймворк**: .NET 10
- **Документация**: Swagger
- **Архитектурный стиль**: RESTful
- **Формат данных**: JSON

### Структура проекта

- **EventManager.Domain**: доменные модели бизнес-логики
- **EventManager.Application**: бизнес-логика, DTO модели, маппинг
- **EventManager.WebApi**: HTTP контроллеры, обработка ошибок в запросах

## Запуск сервиса

### Предварительные требования

- .NET 10 SDK
- Git

### Шаги для запуска

#### 1. Клонирование репозитория

```bash
git clone https://github.com/Evgeniy-Efimov/event-manager.git EventManager
cd EventManager
```

#### 2. Запуск API

```bash
dotnet run --project EventManager.WebApi
```

#### 3. Проверка работы

Swagger UI будет доступен по адресу http://localhost:5226/swagger/index.html

## Запуск тестов

#### Запуск Unit тестов

```bash
dotnet test EventManager.UnitTests
```
