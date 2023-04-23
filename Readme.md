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
- Конфигурация сервера
- Добавляем новвые контроллеры для сконфигурированных endpoints
  * [AuthorizationController](https://github.com/openiddict/openiddict-core/blob/dev/sandbox/OpenIddict.Sandbox.AspNetCore.Server/Controllers/AuthorizationController.cs)
  * [UserinfoController](https://github.com/openiddict/openiddict-core/blob/rel/4.2.0/sandbox/OpenIddict.Sandbox.AspNetCore.Server/Controllers/UserinfoController.cs)
  * [AccountController](https://github.com/openiddict/openiddict-core/blob/rel/4.2.0/sandbox/OpenIddict.Sandbox.AspNetCore.Server/Controllers/AccountController.cs)
- Страница логина и логика переезжают в AuthServer для обеспечиваем SSO

### Проверяем что сервере OIDC поднялся

- OIDC Discovery URL (http://localhost:5555/.well-known/openid-configuration)
- OIDC JWKS URL (http://localhost:5555/.well-known/jwks)


## Шаг 3

### Изменения в AuthServer

- Определяем тип клиентов (**confidential**, public)
- Регистрируем приложения клиенты в OIDC
  * ClientAppServer1
  * ClientAppServer2
  * Postman (для отладки)

### Изменяем в клиентах
- Аутентификация по кукe остается (в куке будет хранится информация полученная от OIDC)
- Подключаем Microsoft.AspNetCore.Authentication.OpenIdConnect
- Добавляется новая схема аутентификации ITA (`AddItaAuthentication`)
- Конфигурируем подключение к серверу OIDC
- Выпиливаем страницу логина

### Проверяем
- В Postman (OIDC ACF)
- SSO Login: выполнив вход в приложении 1 выполнять вход в приложение 2 не требуется