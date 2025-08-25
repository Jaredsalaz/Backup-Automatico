using BackupCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackupCore.Interfaces
{
    public interface IDatabaseBackupService
    {
        Task<string> CreateBackupAsync(DatabaseConfig config, string outputPath);
        Task<bool> TestConnectionAsync(DatabaseConfig config);
        string GenerateBackupFileName(DatabaseConfig config);
        string GetDefaultBackupPath();
    }

    public interface IFtpService
    {
        Task<bool> TestConnectionAsync(FtpConfig config);
        Task<bool> UploadFileAsync(FtpConfig config, string localFilePath, string remoteFileName);
        Task<bool> CreateDirectoryStructureAsync(FtpConfig config);
        Task<bool> DeleteOldBackupsAsync(FtpConfig config, int retentionDays);
        Task<List<string>> ListBackupFilesAsync(FtpConfig config);
    }

    public interface IConfigurationService
    {
        Task<List<DatabaseConfig>> GetDatabaseConfigsAsync();
        Task<List<FtpConfig>> GetFtpConfigsAsync();
        Task<List<BackupConfig>> GetBackupConfigsAsync();
        Task SaveDatabaseConfigAsync(DatabaseConfig config);
        Task SaveFtpConfigAsync(FtpConfig config);
        Task SaveBackupConfigAsync(BackupConfig config);
        Task UpdateDatabaseConfigAsync(DatabaseConfig config);
        Task UpdateFtpConfigAsync(FtpConfig config);
        Task UpdateBackupConfigAsync(BackupConfig config);
        Task DeleteDatabaseConfigAsync(Guid id);
        Task DeleteFtpConfigAsync(Guid id);
        Task DeleteBackupConfigAsync(Guid id);
        Task<DatabaseConfig?> GetDatabaseConfigAsync(Guid id);
        Task<FtpConfig?> GetFtpConfigAsync(Guid id);
        Task<BackupConfig?> GetBackupConfigAsync(Guid id);
    }

    public interface IBackupLogService
    {
        Task SaveLogAsync(BackupLog log);
        Task<List<BackupLog>> GetLogsAsync(int count = 100);
        Task<List<BackupLog>> GetLogsByConfigAsync(string backupConfigId);
        Task CleanOldLogsAsync(int retentionDays = 90);
    }

    public interface IBackupOrchestrator
    {
        Task ExecuteBackupAsync(string backupConfigId);
        Task ExecuteAllScheduledBackupsAsync();
        Task<List<BackupConfig>> GetPendingBackupsAsync();
    }

    public interface ISecurityService
    {
        string EncryptString(string plainText);
        string DecryptString(string cipherText);
        void HideProcess();
        bool IsProcessHidden();
    }
}
