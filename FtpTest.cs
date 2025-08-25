using System;
using System.Threading.Tasks;
using BackupCore.Interfaces;
using BackupCore.Services;
using BackupCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FtpTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== VERIFICANDO ARCHIVOS EN FTP ===");
            
            try
            {
                // Configurar servicios
                var services = new ServiceCollection();
                services.AddLogging(builder => builder.AddConsole());
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<IFtpService, FtpService>();
                
                var serviceProvider = services.BuildServiceProvider();
                var configService = serviceProvider.GetRequiredService<IConfigurationService>();
                var ftpService = serviceProvider.GetRequiredService<IFtpService>();
                
                Console.WriteLine("Obteniendo configuraciones...");
                
                // Obtener configuraciones FTP
                var ftpConfigs = await configService.GetFtpConfigsAsync();
                var backupConfigs = await configService.GetBackupConfigsAsync();
                
                Console.WriteLine($"Configuraciones FTP encontradas: {ftpConfigs.Count}");
                Console.WriteLine($"Configuraciones Backup encontradas: {backupConfigs.Count}");
                
                foreach (var backupConfig in backupConfigs.Where(c => c.IsEnabled))
                {
                    var ftpConfig = ftpConfigs.FirstOrDefault(f => f.Name == backupConfig.FtpConfigName);
                    if (ftpConfig != null)
                    {
                        Console.WriteLine($"\n--- Verificando FTP para: {backupConfig.Name} ---");
                        Console.WriteLine($"Servidor FTP: {ftpConfig.Host}");
                        
                        try
                        {
                            // Listar archivos en el directorio de backup
                            var files = await ftpService.ListDirectoryAsync(ftpConfig, ".");
                            Console.WriteLine($"Archivos encontrados: {files.Count}");
                            
                            foreach (var file in files.Take(10)) // Mostrar solo los primeros 10
                            {
                                Console.WriteLine($"  - {file}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error listando directorio FTP: {ex.Message}");
                        }
                    }
                }
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
