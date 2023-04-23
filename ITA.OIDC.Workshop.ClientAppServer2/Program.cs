using ITA.OIDC.Workshop.ClientAppServer2.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Конфигурируем путь до API
const string apiUrl = "http://localhost:5029";

builder.Services.AddItaApiClient(apiUrl);

// Аутентификация по куке
const string basePath = "/ClientApp2";
const string authCookieName = ".ITA.ClientApp2.Cookie";
const int authCookieLifetimeDays = 1;
const string loginPageUrl = "/Home/Login";

builder.Services.AddItaAuthentication(authCookieName, basePath, authCookieLifetimeDays, loginPageUrl);

var app = builder.Build();

app.UsePathBase(new PathString(basePath));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 

app.Run();