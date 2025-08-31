using System;
using System.Globalization;
using Games.Database;
using Games.Models;
using Games.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class MainClass
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<TelegramService>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultUI();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ModeratorOrAdmin", policy =>
                policy.RequireRole("Moderator", "Admin"));
        });

        builder.Services.AddRazorPages();
        
        var app = builder.Build();
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Moderator", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id:int?}");

        app.Run();
    }
}

//app.MapStaticAssets();

//app.Use(async (context, next) =>
//{
//    Endpoint endpoint = context.GetEndpoint();
//    string rulesEndpoint = (endpoint as RouteEndpoint).RoutePattern.RawText;
//    Console.WriteLine(rulesEndpoint);
//    Console.WriteLine(endpoint);
//    await next();
//});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id:int?}");
//});

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    // Автоматическое применение миграций
//    await db.Database.MigrateAsync();

//    // Добавляем тестовые данные, если БД пустая
//    if (!db.Games.Any())
//    {
//        db.Games.Add(new Game(
//            "Hogwarts Legacy",
//            "Погрузитесь в открытый мир волшебства...",
//            "Экшен, Адвенчура, Ролевая",
//            "2023.10.02"));

//        await db.SaveChangesAsync();
//    }
//}

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    if (!await roleManager.RoleExistsAsync("Admin"))
//    {
//        await roleManager.CreateAsync(new IdentityRole("Admin"));
//        Console.WriteLine("Роль Admin создана.");
//    }

//    if (!await roleManager.RoleExistsAsync("Moderator"))
//    {
//        await roleManager.CreateAsync(new IdentityRole("Moderator"));
//        Console.WriteLine("Роль Moderator создана.");
//    }

//    if (!await roleManager.RoleExistsAsync("User"))
//    {
//        await roleManager.CreateAsync(new IdentityRole("User"));
//        Console.WriteLine("Роль User создана.");
//    }
//}

//using (var scope = app.Services.CreateScope())
//{
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    var user = await userManager.FindByNameAsync("denis.ladovir@yandex.ru"); // или FindByEmailAsync("email")

//    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
//    {
//        await userManager.AddToRoleAsync(user, "Admin");
//        Console.WriteLine("Роль Admin назначена пользователю.");
//    }
//}