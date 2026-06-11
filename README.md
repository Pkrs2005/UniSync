# UniSync 📅

**UniSync** — агрегатор учебного расписания со встроенной системой совместного планирования и модерации изменений.

---

## Возможности MVP

* Парсинг официального расписания из `.ics` (rdcenter.ru)
* Просмотр расписания по неделям (десктоп-клиент Avalonia)
* Регистрация и авторизация (JWT)
* Система «коммитов» — студент предлагает правку, староста одобряет/отклоняет
* OpenAPI-документация: `http://localhost:5134/openapi/v1.json`
* Контейнеризация сервера и PostgreSQL через Docker Compose

---

## Быстрый старт

### 1. Поднять БД и API (Docker)

```bash
docker compose up -d --build
```

API будет доступен на `http://localhost:5134`.

### 2. Запуск клиента (без Docker)

```bash
dotnet run --project UniSync.Client/UniSync.Client.Desktop
```

### Альтернатива: только БД в Docker, API локально

```bash
docker compose up -d unisync-db
dotnet ef database update --project UniSync.Backend
dotnet run --project UniSync.Backend
```

---

## Тестовые аккаунты

| Логин    | Пароль | Роль      |
|----------|--------|-----------|
| starosta | 123456 | Moderator |
| *(любой)*| *(свой)*| Student  |

---

## Сценарий демонстрации

1. Зарегистрировать студента → войти → увидеть расписание на неделю
2. Нажать на пару → изменить аудиторию/название → «Отправить»
3. Войти как `starosta` / `123456` → «Модерация правок» → одобрить
4. Вернуться к расписанию — правка применена

---

## Стек

| Компонент | Технологии |
|-----------|------------|
| Backend   | .NET 10, ASP.NET Core, EF Core, PostgreSQL, Ical.Net |
| Client    | Avalonia UI, MVVM, CommunityToolkit.Mvvm |
| Shared    | Общие DTO (`LessonDto`, `ScheduleCommit`) |
| Infra     | Docker Compose |

---

## API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Авторизация → JWT |
| GET  | `/api/schedule?date=2026-06-11` | Расписание на неделю |
| POST | `/api/commit` | Предложить правку |
| GET  | `/api/commit` | Список правок |
| PUT  | `/api/commit/{id}/status?status=Approved` | Модерация (Moderator) |

OpenAPI: [http://localhost:5134/openapi/v1.json](http://localhost:5134/openapi/v1.json)

---

## Структура

```text
UniSync/
├── UniSync.Backend/     # Web API + парсинг ICS + БД
├── UniSync.Client/      # Avalonia desktop-клиент
├── UniSync.Shared/      # Общие модели
└── docker-compose.yml   # PostgreSQL + API
```
