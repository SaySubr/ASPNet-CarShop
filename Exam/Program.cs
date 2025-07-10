using Exam.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Exam.Services;


namespace Exam
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //сервис базы данных
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 25))));

            //сервис авторизации
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddErrorDescriber<RussianIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            //сервис корзины
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//через сколько сессия истекает
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddScoped<CartService>();

            //Сервис заказов
            builder.Services.AddScoped<OrderService>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // инициализация базы данных и ролей
            using (var scope = app.Services.CreateScope())
            {
                await RoleInitializer.SeedAsync(scope.ServiceProvider);
            }

            //cтандартный
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();

            // авторизация
            app.UseSession();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
