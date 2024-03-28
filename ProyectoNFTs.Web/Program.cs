using Serilog.Events;
using Serilog;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProyectoNFTs.Infraestructure.Repository.Interfaces;
using ProyectoNFTs.Infraestructure.Repository.Implementations;
using ProyectoNFTs.Application.Services.Interfaces;
using ProyectoNFTs.Application.Services.Implementations;
using ProyectoNFTs.Application.Profiles;
using ProyectoNFTs.Infraestructure.Data;
using ProyectoNFTs.Application.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure D.I.
builder.Services.AddTransient<IRepositoryCliente, RepositoryCliente>();
builder.Services.AddTransient<IServiceCliente, ServiceCliente>();

builder.Services.AddTransient<IRepositoryPais, RepositoryPais>();
builder.Services.AddTransient<IServicePais, ServicePais>();

builder.Services.AddTransient<IRepositoryNft, RepositoryNft>();
builder.Services.AddTransient<IServiceNft, ServiceNft>();

builder.Services.AddTransient<IRepositoryTarjeta, RepositoryTarjeta>();
builder.Services.AddTransient<IServiceTarjeta, ServiceTarjeta>();

builder.Services.AddTransient<IRepositoryFactura, RepositoryFactura>();
builder.Services.AddTransient<IServiceFactura, ServiceFactura>();

// Mapping AppConfig Class to read  appsettings.json
builder.Services.Configure<AppConfig>(builder.Configuration);


// config Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ClienteProfile>();
    config.AddProfile<PaisProfile>();
    config.AddProfile<NftProfile>();
    config.AddProfile<TarjetaProfile>();
});

// Config Connection to SQLServer DataBase
builder.Services.AddDbContext<ProyectoNFTsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});




// Logger
var logger = new LoggerConfiguration()
                    //.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(LogEventLevel.Information)
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();

builder.Host.UseSerilog(logger);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
