using System;

namespace BackupCore.Models
{
    public class BackupConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string DatabaseConfigName { get; set; } = string.Empty;
        public string FtpConfigName { get; set; } = string.Empty;
        public string Schedule { get; set; } = "0 2 * * *"; // Cron expression: 2:00 AM diario
        public bool IsEnabled { get; set; } = true;
        public bool CompressBackup { get; set; } = true;
        public int RetentionDays { get; set; } = 30; // Mantener backups por 30 días
        public DateTime LastBackup { get; set; }
        public DateTime NextBackup { get; set; }
        public string LastBackupStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        public void CalculateNextBackup()
        {
            // Se calculará usando la expresión Cron
            NextBackup = DateTime.Today.AddDays(1).AddHours(2); // Por defecto mañana a las 2 AM
        }
    }

    public enum BackupStatus
    {
        Pending,
        Running,
        Success,
        Failed,
        Cancelled
    }
}
