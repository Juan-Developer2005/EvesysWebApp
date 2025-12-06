using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // <--- Necesario para el Login
using EvesysWebApp.Data;
using MySql.EntityFrameworkCore; // <--- Proveedor de MySQL (Oracle)
// ...

var builder = WebApplication.CreateBuilder(args);

// --- INICIO: REGISTRO DE DB CONTEXTO (Oracle MySQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configuramos el DbContext para usar MySQL con Oracle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString) // Sintaxis simple
);
// --- FIN: REGISTRO DE DB CONTEXTO ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // <-- Define la página de Login
        options.AccessDeniedPath = "/Home/AccessDenied";
        // ...
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Asegura que la DB se migre si es necesario
        await SeedData.Initialize(services); // Llama al seeder de roles
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();