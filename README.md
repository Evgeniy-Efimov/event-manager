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

**Ответ 404 Not Found:**
```json
{
  "StatusCode": 404,
  "Message": "Event '12345678-1234-1234-1234-123456789abc' not found",
  "Timestamp": "2026-08-30T12:44:03.22Z"
}
```

#### Получение списка всех мероприятий

**Запрос:**
```http
GET http://localhost:5226/api/events
```

**Ответ 200 OK:**
```json
[
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
```

#### Создание нового мероприятия

**Запрос:**
```http
POST http://localhost:5226/api/events

{
  "title": "Event3",
  "description": "Event3 description",
  "startAt": "2026-10-01T09:00:00Z",
  "endAt": "2026-10-01T11:00:00Z"
}
```

**Ответ 201 Created:**
```
201 Created
content-length: 0 
```

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
