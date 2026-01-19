using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.Services;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<Usuario, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Contenedor de trabajo
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();

builder.Services.AddScoped<WhatsAppFlowService>();
builder.Services.AddScoped<ISupervisorFlowService, SupervisorFlowService>();

builder.Services.AddScoped<IEmpleadoFlowService, EmpleadoFlowService>();

//Registrar servicio de whatsapp
builder.Services.AddSingleton(new WhatsAppService(
    builder.Configuration["Twilio:AccountSid"],
    builder.Configuration["Twilio:AuthToken"],
    builder.Configuration["Twilio:WhatsAppNumber"]
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
