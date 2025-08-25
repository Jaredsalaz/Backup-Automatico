using BackupCore.Interfaces;
using BackupCore.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FtpConfig = BackupCore.Models.FtpConfig; // Alias para evitar conflicto

namespace BackupCore.Services
{
    public class FtpService : IFtpService
    {
        public Task<bool> TestConnectionAsync(FtpConfig config)
        {
            try
            {
                using var client = CreateFtpClient(config);
                client.Connect();
                
                // Verificar que podemos listar el directorio
                var listing = client.GetListing(config.BasePath);
                
                return Task.FromResult(client.IsConnected);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> UploadFileAsync(FtpConfig config, string localFilePath, string remoteFileName)
        {
            try
            {
                if (!File.Exists(localFilePath))
                {
                    throw new FileNotFoundException($"Archivo local no encontrado: {localFilePath}");
                }

                using var client = CreateFtpClient(config);
                client.Connect();

                // Crear estructura de directorios si no existe
                await CreateDirectoryStructureAsync(config);

                string remotePath = config.GetFullBackupPath() + remoteFileName;
                
                // Subir archivo
                var result = client.UploadFile(localFilePath, remotePath, FtpRemoteExists.Overwrite);
                
                return result == FtpStatus.Success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error subiendo archivo a FTP: {ex.Message}", ex);
            }
        }

        public Task<bool> CreateDirectoryStructureAsync(FtpConfig config)
        {
            try
            {
                using var client = CreateFtpClient(config);
                client.Connect();

                // Crear carpeta principal (SantaCruz)
                string mainFolderPath = $"{config.BasePath.TrimEnd('/')}/SantaCruz";
                if (!client.DirectoryExists(mainFolderPath))
                {
                    client.CreateDirectory(mainFolderPath);
                }

                // Crear carpeta de sucursal
                string branchFolderPath = $"{mainFolderPath}/{config.Branch}";
                if (!client.DirectoryExists(branchFolderPath))
                {
                    client.CreateDirectory(branchFolderPath);
                }

                // Crear carpeta de backups
                string backupFolderPath = $"{branchFolderPath}/backups";
                if (!client.DirectoryExists(backupFolderPath))
                {
                    client.CreateDirectory(backupFolderPath);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creando estructura de directorios en FTP: {ex.Message}", ex);
            }
        }

        public Task<bool> DeleteOldBackupsAsync(FtpConfig config, int retentionDays)
        {
            try
            {
                using var client = CreateFtpClient(config);
                client.Connect();

                string backupPath = config.GetFullBackupPath();
                if (!client.DirectoryExists(backupPath))
                {
                    return Task.FromResult(true); // No hay directorio, no hay nada que limpiar
                }

                var files = client.GetListing(backupPath);
                var cutoffDate = DateTime.Now.AddDays(-retentionDays);

                foreach (var file in files.Where(f => f.Type == FtpObjectType.File && f.Modified < cutoffDate))
                {
                    try
                    {
                        client.DeleteFile(file.FullName);
                    }
                    catch
                    {
                        // Continuar si no se puede eliminar un archivo específico
                    }
                }

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<List<string>> ListBackupFilesAsync(FtpConfig config)
        {
            try
            {
                using var client = CreateFtpClient(config);
                client.Connect();

                string backupPath = config.GetFullBackupPath();
                if (!client.DirectoryExists(backupPath))
                {
                    return Task.FromResult(new List<string>());
                }

                var files = client.GetListing(backupPath);
                var result = files
                    .Where(f => f.Type == FtpObjectType.File)
                    .Select(f => f.Name)
                    .OrderByDescending(f => f)
                    .ToList();
                    
                return Task.FromResult(result);
            }
            catch
            {
                return Task.FromResult(new List<string>());
            }
        }

        private FtpClient CreateFtpClient(FtpConfig config)
        {
            var client = new FtpClient(config.Host, config.Username, config.Password, config.Port);
            
            if (config.UseSsl)
            {
                client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                client.Config.ValidateAnyCertificate = true;
            }
            else
            {
                client.Config.EncryptionMode = FtpEncryptionMode.None;
            }

            client.Config.ConnectTimeout = 30000;
            client.Config.ReadTimeout = 30000;
            client.Config.DataConnectionType = FtpDataConnectionType.PASV;

            return client;
        }
    }
}
