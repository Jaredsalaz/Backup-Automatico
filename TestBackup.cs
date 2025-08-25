using System;
using System.Threading.Tasks;
using BackupCore.Interfaces;
using BackupCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestBackup
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== PRUEBA DE BACKUP MANUAL ===");
            
            try
            {
                // Configurar servicios
                var services = new ServiceCollection();
                services.AddLogging(builder => builder.AddConsole());
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();
                services.AddSingleton<IFtpService, FtpService>();
                services.AddSingleton<IBackupLogService, BackupLogService>();
                services.AddSingleton<IBackupOrchestrator, BackupOrchestrator>();
                
                var serviceProvider = services.BuildServiceProvider();
                var orchestrator = serviceProvider.GetRequiredService<IBackupOrchestrator>();
                
                Console.WriteLine("Servicios configurados correctamente");
                
                // Ejecutar backup manual
                Console.WriteLine("Ejecutando backup manual...");
                await orchestrator.ExecuteAllScheduledBackupsAsync();
                
                Console.WriteLine("✅ Backup manual completado!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Detalles: {ex}");
            }
            
            Console.WriteLine("Presiona cualquier tecla para cerrar...");
            Console.ReadKey();
        }
    }
}
