# Шаг1

## Дано

- DB: База данных (pg)
- **Server:** Серверное API с BL (.NET Core WebAPI)
- **UI:** Два ASP.NET MVC приложения (ASP.NET Core MVC)

## Задача

- Добавить аутентификацию и авторизацию OIDC
- Для приложений должен быть реализован SSO
- Авторизация в API по токенам


## Выбираем инструментарий

1. IS4 https://github.com/IdentityServer/IdentityServer4
  - Archived!
2. Keycloak https://github.com/keycloak/keycloak
  - Не наш стек – Java.
3. OpenIddict https://github.com/openiddict/openiddict-core
  - Подходит стек. Frameworks .net 4.6.1 - .net 7.0.
  - По сути SDK. Широкий простор для кастомизации.
  - Активно развивается
