## Шаг 1

### Дано

- DB: База данных (pg)
- **Server:** Серверное API с BL (.NET Core WebAPI)
- **UI:** Два ASP.NET MVC приложения (ASP.NET Core MVC)

### Задача

- Добавить аутентификацию и авторизацию OIDC
- Для приложений должен быть реализован SSO
- Авторизация в API по токенам


### Выбираем инструментарий

1. IS4 https://github.com/IdentityServer/IdentityServer4
  - Archived!
2. Keycloak https://github.com/keycloak/keycloak
  - Не наш стек – Java.
3. OpenIddict https://github.com/openiddict/openiddict-core
  - Подходит стек. Frameworks .net 4.6.1 - .net 7.0.
  - По сути SDK. Широкий простор для кастомизации.
  - Активно развивается


## Шаг 2

### Вопросы

- Где будет храниться информация о пользователях, по которой будет выполняться аутентификация и авторизация?
- Варианты:
  * Оставить пользователей в существующей бд: добавить в API сервера BL метод Login
  * Перести пользователей в новую бд 

### Добавляем новый кубик

- ASP.NET Core MVC ITA.OIDC.Workshop.AuthServer + OpenIddict + EF Core
- Генерация модели для OpenIddict (EF Core), миграции
- Подготовка сертификата (production mode)
- Конфигурация сервера

### Проверяем что сервере OIDC поднялся

- OIDC Discovery URL (http://localhost:5555/.well-known/openid-configuration)
- OIDC JWKS URL (http://localhost:5555/.well-known/jwks)
