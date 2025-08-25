using BackupService;
using BackupCore.Interfaces;
using BackupCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configurar para ejecutar como servicio Windows
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "BackupAutomaticoService";
});

// Registrar servicios
builder.Services.AddSingleton<ISecurityService, SecurityService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();
builder.Services.AddSingleton<IFtpService, FtpService>();
builder.Services.AddSingleton<IBackupLogService, BackupLogService>();
builder.Services.AddSingleton<IBackupOrchestrator, BackupOrchestrator>();

// Registrar el worker principal
builder.Services.AddHostedService<BackupWorker>();

var host = builder.Build();

// Ocultar proceso si es necesario
var securityService = host.Services.GetRequiredService<ISecurityService>();
securityService.HideProcess();

host.Run();
