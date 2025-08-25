using BackupConfigurator;
using BackupCore.Interfaces;
using BackupCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BackupConfigurator;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Configurar servicios
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        Application.Run(new MainFormNew(serviceProvider));
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        // Registrar servicios core
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();
        services.AddSingleton<IFtpService, FtpService>();
        services.AddSingleton<IBackupLogService, BackupLogService>();
        services.AddSingleton<IBackupOrchestrator, BackupOrchestrator>();
    }
}