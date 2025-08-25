using System;
using System.Linq;
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
            Console.WriteLine("=== PRUEBA DETALLADA DE BACKUP ===");
            
            try
            {
                // Configurar servicios con más logging
                var services = new ServiceCollection();
                services.AddLogging(builder => {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                });
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<IDatabaseBackupService, DatabaseBackupService>();
                services.AddSingleton<IFtpService, FtpService>();
                services.AddSingleton<IBackupLogService, BackupLogService>();
                services.AddSingleton<IBackupOrchestrator, BackupOrchestrator>();
                
                var serviceProvider = services.BuildServiceProvider();
                var configService = serviceProvider.GetRequiredService<IConfigurationService>();
                var orchestrator = serviceProvider.GetRequiredService<IBackupOrchestrator>();
                
                Console.WriteLine("✅ Servicios configurados correctamente");
                
                // Verificar configuraciones
                var backupConfigs = await configService.GetBackupConfigsAsync();
                var dbConfigs = await configService.GetDatabaseConfigsAsync();
                var ftpConfigs = await configService.GetFtpConfigsAsync();
                
                Console.WriteLine($"📋 Configuraciones cargadas:");
                Console.WriteLine($"   - Backups: {backupConfigs.Count}");
                Console.WriteLine($"   - Bases de datos: {dbConfigs.Count}");
                Console.WriteLine($"   - FTP: {ftpConfigs.Count}");
                
                foreach (var config in backupConfigs.Where(c => c.IsEnabled))
                {
                    Console.WriteLine($"\n🔄 Procesando: {config.Name}");
                    Console.WriteLine($"   - ID: {config.Id}");
                    Console.WriteLine($"   - Última ejecución: {config.LastBackup}");
                    Console.WriteLine($"   - Estado: {config.LastBackupStatus}");
                    
                    // Ejecutar backup específico
                    Console.WriteLine("   - Ejecutando backup...");
                    await orchestrator.ExecuteBackupAsync(config.Id.ToString());
                    Console.WriteLine("   - ✅ Backup completado");
                }
                
                // Verificar cambios en configuración
                Console.WriteLine("\n📊 Verificando actualizaciones...");
                var updatedConfigs = await configService.GetBackupConfigsAsync();
                foreach (var config in updatedConfigs)
                {
                    Console.WriteLine($"   - {config.Name}: {config.LastBackup} - {config.LastBackupStatus}");
                }
                
                Console.WriteLine("\n✅ Proceso completado!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Detalles: {ex}");
            }
            
            Console.WriteLine("\nPresiona cualquier tecla para cerrar...");
            Console.ReadKey();
        }
    }
}
