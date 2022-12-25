using CourseWorkData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;   // ������������ ���� ������ ApplicationContext


var builder = WebApplication.CreateBuilder(args);
// ���������� �������� ��������������
builder.Services.AddAuthentication("Cookies")  // ����� �������������� - � ������� jwt-�������
    .AddCookie(options => options.LoginPath = "/CreateLogin");
builder.Services.AddAuthorization();

// �������� ������ ����������� �� ����� ������������
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();   // ���������� middleware �������������� 
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=CreateLogin}/{id?}");
app.Map("/Home", [Authorize]() => $"Hello World!");

app.Run();
