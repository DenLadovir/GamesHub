## Описание
# 🎮 GamesHub

Учебный проект на **C# / ASP.NET Core** для работы с базой данных видеоигр.  
Позволяет искать, фильтровать и управлять играми, а также использовать систему ролей пользователей.

---

## ✨ Возможности

- 🔎 Поиск игр по названию  
- 📅 Фильтрация по году выхода и жанрам  
- ⏳ Отображение невышедших игр  
- ➕ Добавление игр и жанров (из JSON или вручную)  
- ✏️ Редактирование и удаление записей  
- 👤 Учётные записи пользователей (регистрация, изменение пароля)  
- 🔑 Роли: **User**, **Moderator**, **Admin**  
- 🛠️ Простая админка  

---

## 🛠️ Технологии

- C#, ASP.NET Core (MVC, Identity, Middleware, Authorization Policies)  
- Entity Framework Core (Code First, Migrations)  
- PostgreSQL  
- MediatR (CQRS)  
- REST API  

---

## 📂 Структура проекта

- `Controllers/` — MVC-контроллеры  
- `Application/` — CQRS-запросы и команды (MediatR)  
- `Models/` — модели данных  
- `Database/` — контекст EF Core и миграции  
- `Middleware/` — глобальный обработчик ошибок  

---

## 🚀 Как запустить проект

1. Установить **.NET 8 SDK** и **PostgreSQL**  
2. Создать БД (например, `gameshub`)  
3. Настроить строку подключения в `appsettings.json`:  
   ```json
   "ConnectionStrings": {
       "PostgreSQLConnection": "Host=localhost;Database=gameshub;Username=postgres;Password=yourpassword"
   }
4. Применить миграции: dotnet ef database update.
5. Запустить проект: dotnet run
6. Открыть в браузере: https://localhost:7208/
---
## Description
# 🎮 GamesHub

A learning project built with **C# / ASP.NET Core** for managing a video game database.  
Allows searching, filtering, and managing games, as well as using a user role system.

---

## ✨ Features

- 🔎 Search games by title  
- 📅 Filter by release year and genres  
- ⏳ Display upcoming (unreleased) games  
- ➕ Add games and genres (from JSON or manually)  
- ✏️ Edit and delete records  
- 👤 User accounts (registration, password change)  
- 🔑 Roles: **User**, **Moderator**, **Admin**  
- 🛠️ Simple admin panel  

---

## 🛠️ Technologies

- C#, ASP.NET Core (MVC, Identity, Middleware, Authorization Policies)  
- Entity Framework Core (Code First, Migrations)  
- PostgreSQL  
- MediatR (CQRS)  
- REST API  

---

## 📂 Project Structure

- `Controllers/` — MVC controllers  
- `Application/` — CQRS queries and commands (MediatR)  
- `Models/` — data models  
- `Database/` — EF Core context and migrations  
- `Middleware/` — global exception handler  

---

## 🚀 How to Run

1. Install **.NET 8 SDK** and **PostgreSQL**  
2. Create a database (e.g., `gameshub`)  
3. Configure the connection string in `appsettings.json`:  
   ```json
   "ConnectionStrings": {
       "PostgreSQLConnection": "Host=localhost;Database=gameshub;Username=postgres;Password=yourpassword"
4. Apply migrations: dotnet ef database update
5. Run the project: dotnet run
6. Open in a browser: https://localhost:7208/
