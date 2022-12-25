using CourseWorkData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;   // пространство имен класса ApplicationContext


var builder = WebApplication.CreateBuilder(args);
// добавление сервисов аутентификации
builder.Services.AddAuthentication("Cookies")  // схема аутентификации - с помощью jwt-токенов
    .AddCookie(options => options.LoginPath = "/CreateLogin");
builder.Services.AddAuthorization();

// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=CreateLogin}/{id?}");
app.Map("/Home", [Authorize]() => $"Hello World!");

app.Run();
