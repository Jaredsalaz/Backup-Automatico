using BackupCore.Interfaces;
using BackupCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackupCore.Services
{
    public class BackupOrchestrator : IBackupOrchestrator
    {
        private readonly IConfigurationService _configService;
        private readonly IDatabaseBackupService _databaseService;
        private readonly IFtpService _ftpService;
        private readonly IBackupLogService _logService;

        public BackupOrchestrator(
            IConfigurationService configService,
            IDatabaseBackupService databaseService,
            IFtpService ftpService,
            IBackupLogService logService)
        {
            _configService = configService;
            _databaseService = databaseService;
            _ftpService = ftpService;
            _logService = logService;
        }

        public async Task ExecuteBackupAsync(string backupConfigId)
        {
            var backupConfig = await _configService.GetBackupConfigAsync(Guid.Parse(backupConfigId));
            if (backupConfig == null || !backupConfig.IsEnabled)
            {
                return;
            }

            // Buscar configs por nombre en lugar de ID
            var databases = await _configService.GetDatabaseConfigsAsync();
            var databaseConfig = databases.FirstOrDefault(d => d.Name == backupConfig.DatabaseConfigName);
            
            var ftpConfigs = await _configService.GetFtpConfigsAsync();
            var ftpConfig = ftpConfigs.FirstOrDefault(f => f.Name == backupConfig.FtpConfigName);

            if (databaseConfig == null || ftpConfig == null)
            {
                return;
            }

            var log = new BackupLog
            {
                BackupConfigId = backupConfigId,
                DatabaseName = databaseConfig.Name,
                FtpServer = ftpConfig.Host
            };

            try
            {
                log.MarkAsStarted();
                await _logService.SaveLogAsync(log);

                // 1. Crear backup de la base de datos
                string tempPath = _databaseService.GetDefaultBackupPath();
                string backupFilePath = await _databaseService.CreateBackupAsync(databaseConfig, tempPath);

                // 2. Comprimir si está habilitado
                if (backupConfig.CompressBackup)
                {
                    backupFilePath = await CompressBackupFile(backupFilePath);
                }

                // 3. Subir a FTP
                string backupFileName = Path.GetFileName(backupFilePath);
                bool uploadSuccess = await _ftpService.UploadFileAsync(ftpConfig, backupFilePath, backupFileName);

                if (!uploadSuccess)
                {
                    throw new Exception("Error subiendo backup al servidor FTP");
                }

                // 4. Obtener tamaño del archivo
                var fileInfo = new FileInfo(backupFilePath);
                string ftpPath = ftpConfig.GetFullBackupPath() + backupFileName;

                log.MarkAsCompleted(backupFileName, fileInfo.Length, ftpPath);

                // 5. Limpiar archivos temporales
                try
                {
                    File.Delete(backupFilePath);
                }
                catch
                {
                    // Ignorar errores de limpieza
                }

                // 6. Actualizar configuración de backup
                backupConfig.LastBackup = DateTime.Now;
                backupConfig.CalculateNextBackup();
                backupConfig.LastBackupStatus = "Exitoso";
                await _configService.SaveBackupConfigAsync(backupConfig);

                // 7. Limpiar backups antiguos en FTP
                try
                {
                    await _ftpService.DeleteOldBackupsAsync(ftpConfig, backupConfig.RetentionDays);
                }
                catch
                {
                    // Ignorar errores de limpieza de backups antiguos
                }
            }
            catch (Exception ex)
            {
                log.MarkAsFailed($"Error ejecutando backup: {ex.Message}", ex.ToString());

                // Actualizar configuración con error
                backupConfig.LastBackupStatus = $"Error: {ex.Message}";
                await _configService.SaveBackupConfigAsync(backupConfig);
            }
            finally
            {
                await _logService.SaveLogAsync(log);
            }
        }

        public async Task ExecuteAllScheduledBackupsAsync()
        {
            var pendingBackups = await GetPendingBackupsAsync();
            
            var tasks = pendingBackups.Select(backup => ExecuteBackupAsync(backup.Id.ToString()));
            await Task.WhenAll(tasks);
        }

        public async Task<List<BackupConfig>> GetPendingBackupsAsync()
        {
            var allBackups = await _configService.GetBackupConfigsAsync();
            var now = DateTime.Now;

            return allBackups
                .Where(b => b.IsEnabled && b.NextBackup <= now)
                .ToList();
        }

        private async Task<string> CompressBackupFile(string filePath)
        {
            try
            {
                string compressedPath = filePath + ".gz";
                
                using var originalFileStream = File.OpenRead(filePath);
                using var compressedFileStream = File.Create(compressedPath);
                using var compressionStream = new System.IO.Compression.GZipStream(compressedFileStream, System.IO.Compression.CompressionLevel.Optimal);
                
                await originalFileStream.CopyToAsync(compressionStream);
                
                // Eliminar archivo original
                File.Delete(filePath);
                
                return compressedPath;
            }
            catch
            {
                // Si la compresión falla, devolver el archivo original
                return filePath;
            }
        }
    }
}
