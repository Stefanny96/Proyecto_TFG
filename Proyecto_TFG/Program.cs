using Microsoft.EntityFrameworkCore;
using Proyecto_TFG.Functions;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;

namespace Proyecto_TFG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IMysqlFunctions, MySqlFunctions>();
            builder.Services.AddScoped<IComunes, Comunes>();
            builder.Services.AddScoped<IDocuWareConnect,DocuWareConnect>();
            builder.Services.AddScoped<IEncriptacion, Encriptacion>();
            builder.Services.AddControllers();

            //COOKIES DE AUTENTICACION
            builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
            {
                options.LoginPath = "/Index"; // Ruta donde está el login
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de vida de la cookie
                options.SlidingExpiration = true; // Renueva la cookie automáticamente con cada solicitud
                options.Cookie.Name = "YourAuthCookie";  // Asigna un nombre explícito a la cookie
                options.Cookie.HttpOnly = true;  // Protege la cookie de accesos JavaScript
                options.Cookie.SameSite = SameSiteMode.Strict; // O bien, Lax dependiendo de tus necesidades
            });

            builder.Services.AddAuthorization();

            /*// Configurar DbContext con la cadena de conexión desde appsettings.json
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
                new MySqlServerVersion(new Version(10, 11, 6)))); //version mariadb*/

            // Configurar Kestrel para producción
            if (!builder.Environment.IsDevelopment())
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5010); // Escucha en IPv4 e IPv6
                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllers();

            app.Run();
        }
    }
}

