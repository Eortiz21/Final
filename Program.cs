using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primera.Models;

var builder = WebApplication.CreateBuilder(args);

// Agrega controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// Obtiene la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ConexionLocalBD")
    ?? throw new InvalidOperationException("Connection string 'ConexionLocalBD' not found.");

// Registra el DbContext con SQL Server (Scoped por defecto)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configura Identity con tu DbContext
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // No requiere confirmación de correo
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Construye la aplicación web con toda la configuración previa
var app = builder.Build();






// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas de MVC
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Ruta por defecto: HomeController -> Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages (para Identity)
app.MapRazorPages();

// Ejecuta la aplicación
app.Run();
