using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Seed;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Services;
using SistemaTaskWhatsapp.Services.Jobs;

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

builder.Services.AddControllers();

//Contenedor de trabajo
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();

builder.Services.AddScoped<WhatsAppFlowService>();
builder.Services.AddScoped<ISupervisorFlowService, SupervisorFlowService>();

builder.Services.AddScoped<IEmpleadoFlowService, EmpleadoFlowService>();

builder.Services.AddScoped<MessageJobs>();
builder.Services.AddScoped<MensajesService>();

//Registrar servicio de whatsapp
builder.Services.AddSingleton(new WhatsAppService(
    builder.Configuration["Twilio:AccountSid"],
    builder.Configuration["Twilio:AuthToken"],
    builder.Configuration["Twilio:WhatsAppNumber"]
));

//Función para enviar mensajes de manera automatica
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<Usuario>>();

    await DbInitializer.InicializarAsync(roleManager, userManager);
}


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

app.UseHangfireDashboard();

//Enviar mensaje todo el año de lunes a viernes a las 9:10
RecurringJob.AddOrUpdate<MessageJobs>(
    "recordatorio-empleados",
    job => job.EnviarRecordatoriosEmpleados(),
    "10 9 * * 1-5",
    TimeZoneInfo.Local
);

RecurringJob.AddOrUpdate<MessageJobs>(
    "avisar-supervisores",
    job => job.AvisarSupervisores(),
    "10 9 * * 1-5",
    TimeZoneInfo.Local
);

// minutos / hora / Dia especifico / mes / Dia de la semana 
// 0 - domingo / 1 - Lunes / 2 - martes... 6 - sabado

//Formato para solo hora
//RecurringJob.AddOrUpdate<MessageJobs>(
//    "avisar-supervisores",
//    job => job.AvisarSupervisores(),
//    Cron.Daily(12),
//    TimeZoneInfo.Local
//);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
