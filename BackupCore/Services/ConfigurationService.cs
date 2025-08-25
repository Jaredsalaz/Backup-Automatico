using BackupCore.Interfaces;
using BackupCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace BackupCore.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private readonly ISecurityService _securityService;

        public ConfigurationService(ISecurityService securityService)
        {
            _securityService = securityService;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _configPath = Path.Combine(appDataPath, "BackupAutomatico", "Config");
            Directory.CreateDirectory(_configPath);
        }

        #region Database Configs

        public async Task<List<DatabaseConfig>> GetDatabaseConfigsAsync()
        {
            string filePath = Path.Combine(_configPath, "databases.json");
            if (!File.Exists(filePath))
                return new List<DatabaseConfig>();

            string json = await File.ReadAllTextAsync(filePath);
            string decryptedJson = _securityService.DecryptString(json);
            
            var configs = JsonSerializer.Deserialize<List<DatabaseConfig>>(decryptedJson) ?? new List<DatabaseConfig>();
            
            // Desencriptar contraseñas
            foreach (var config in configs)
            {
                if (!string.IsNullOrEmpty(config.Password))
                {
                    config.Password = _securityService.DecryptString(config.Password);
                }
            }

            return configs;
        }

        public async Task SaveDatabaseConfigAsync(DatabaseConfig config)
        {
            var configs = await GetDatabaseConfigsAsync();
            var existingConfig = configs.FirstOrDefault(c => c.Id == config.Id);

            // Encriptar contraseña antes de guardar
            var configToSave = new DatabaseConfig
            {
                Id = config.Id,
                Name = config.Name,
                Type = config.Type,
                Server = config.Server,
                Database = config.Database,
                Username = config.Username,
                Password = !string.IsNullOrEmpty(config.Password) ? _securityService.EncryptString(config.Password) : "",
                Port = config.Port,
                IsActive = config.IsActive,
                CreatedAt = existingConfig?.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (existingConfig != null)
            {
                configs.Remove(existingConfig);
            }
            configs.Add(configToSave);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string encryptedJson = _securityService.EncryptString(json);
            
            string filePath = Path.Combine(_configPath, "databases.json");
            await File.WriteAllTextAsync(filePath, encryptedJson);
        }

        public async Task DeleteDatabaseConfigAsync(Guid id)
        {
            var configs = await GetDatabaseConfigsAsync();
            configs.RemoveAll(c => c.Id == id);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string encryptedJson = _securityService.EncryptString(json);
            
            string filePath = Path.Combine(_configPath, "databases.json");
            await File.WriteAllTextAsync(filePath, encryptedJson);
        }

        public async Task<DatabaseConfig?> GetDatabaseConfigAsync(Guid id)
        {
            var configs = await GetDatabaseConfigsAsync();
            return configs.FirstOrDefault(c => c.Id == id);
        }

        #endregion

        #region FTP Configs

        public async Task<List<FtpConfig>> GetFtpConfigsAsync()
        {
            string filePath = Path.Combine(_configPath, "ftp.json");
            if (!File.Exists(filePath))
                return new List<FtpConfig>();

            string json = await File.ReadAllTextAsync(filePath);
            string decryptedJson = _securityService.DecryptString(json);
            
            var configs = JsonSerializer.Deserialize<List<FtpConfig>>(decryptedJson) ?? new List<FtpConfig>();
            
            // Desencriptar contraseñas
            foreach (var config in configs)
            {
                if (!string.IsNullOrEmpty(config.Password))
                {
                    config.Password = _securityService.DecryptString(config.Password);
                }
            }

            return configs;
        }

        public async Task SaveFtpConfigAsync(FtpConfig config)
        {
            var configs = await GetFtpConfigsAsync();
            var existingConfig = configs.FirstOrDefault(c => c.Id == config.Id);

            // Encriptar contraseña antes de guardar
            var configToSave = new FtpConfig
            {
                Id = config.Id,
                Name = config.Name,
                Host = config.Host,
                Port = config.Port,
                Username = config.Username,
                Password = !string.IsNullOrEmpty(config.Password) ? _securityService.EncryptString(config.Password) : "",
                BasePath = config.BasePath,
                Branch = config.Branch,
                UseSsl = config.UseSsl,
                IsActive = config.IsActive,
                CreatedAt = existingConfig?.CreatedAt ?? DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            if (existingConfig != null)
            {
                configs.Remove(existingConfig);
            }
            configs.Add(configToSave);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string encryptedJson = _securityService.EncryptString(json);
            
            string filePath = Path.Combine(_configPath, "ftp.json");
            await File.WriteAllTextAsync(filePath, encryptedJson);
        }

        public async Task DeleteFtpConfigAsync(Guid id)
        {
            var configs = await GetFtpConfigsAsync();
            configs.RemoveAll(c => c.Id == id);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string encryptedJson = _securityService.EncryptString(json);
            
            string filePath = Path.Combine(_configPath, "ftp.json");
            await File.WriteAllTextAsync(filePath, encryptedJson);
        }

        public async Task<FtpConfig?> GetFtpConfigAsync(Guid id)
        {
            var configs = await GetFtpConfigsAsync();
            return configs.FirstOrDefault(c => c.Id == id);
        }

        #endregion

        #region Backup Configs

        public async Task<List<BackupConfig>> GetBackupConfigsAsync()
        {
            string filePath = Path.Combine(_configPath, "backups.json");
            if (!File.Exists(filePath))
                return new List<BackupConfig>();

            string json = await File.ReadAllTextAsync(filePath);
            var configs = JsonSerializer.Deserialize<List<BackupConfig>>(json) ?? new List<BackupConfig>();
            return configs;
        }

        public async Task SaveBackupConfigAsync(BackupConfig config)
        {
            var configs = await GetBackupConfigsAsync();
            var existingConfig = configs.FirstOrDefault(c => c.Id == config.Id);

            if (existingConfig != null)
            {
                configs.Remove(existingConfig);
            }
            
            // Calcular próximo backup si es necesario
            if (config.NextBackup == default)
            {
                config.CalculateNextBackup();
            }
            
            configs.Add(config);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(_configPath, "backups.json");
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task UpdateDatabaseConfigAsync(DatabaseConfig config)
        {
            config.UpdatedAt = DateTime.Now;
            await SaveDatabaseConfigAsync(config);
        }

        public async Task UpdateFtpConfigAsync(FtpConfig config)
        {
            config.UpdatedAt = DateTime.Now;
            await SaveFtpConfigAsync(config);
        }

        public async Task UpdateBackupConfigAsync(BackupConfig config)
        {
            config.UpdatedAt = DateTime.Now;
            await SaveBackupConfigAsync(config);
        }

        public async Task DeleteBackupConfigAsync(Guid id)
        {
            var configs = await GetBackupConfigsAsync();
            configs.RemoveAll(c => c.Id == id);

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(_configPath, "backups.json");
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<BackupConfig?> GetBackupConfigAsync(Guid id)
        {
            var configs = await GetBackupConfigsAsync();
            return configs.FirstOrDefault(c => c.Id == id);
        }

        #endregion
    }
}
