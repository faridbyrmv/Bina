using Bina.Core.Dto.Homes;
using Bina.Core.Services.Implementations;
using Bina.Core.Services.Interfaces;
using Bina.Core.Validation;
using Bina.DataProvider.Entity;
using Bina.DataProvider.Repositories;
using Bina.DataProvider.Repositories.Home;
using Bina.DataProvider.Repositories.Photo;
using Bina.Mail;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rooms.Context;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("77BD25DB-C4D1-46EE-97F9-6847892262C0")), 
        ValidateIssuer = false, 
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); 

    options.AddPolicy("User", policy =>
        policy.RequireRole("User", "Admin")); 
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IBaseRepository<Homes>, HomeRepository>();
builder.Services.AddScoped<IBaseRepository<Photos>, PhotoRepository>();

builder.Services.AddScoped<ApplicationDbContext>();

builder.Services.AddValidatorsFromAssemblyContaining<RegistrationDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LogInDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateHomeDtoValidation>();


var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("connection"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "area",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
