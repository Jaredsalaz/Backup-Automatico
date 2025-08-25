using System;

namespace BackupCore.Models
{
    public class BackupLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BackupConfigId { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string FtpServer { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public BackupStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public long BackupSizeBytes { get; set; }
        public string BackupFileName { get; set; } = string.Empty;
        public string FtpPath { get; set; } = string.Empty;
        public TimeSpan? Duration => EndTime?.Subtract(StartTime);
        
        public void MarkAsStarted()
        {
            StartTime = DateTime.Now;
            Status = BackupStatus.Running;
            Message = "Backup iniciado";
        }
        
        public void MarkAsCompleted(string fileName, long fileSize, string ftpPath)
        {
            EndTime = DateTime.Now;
            Status = BackupStatus.Success;
            BackupFileName = fileName;
            BackupSizeBytes = fileSize;
            FtpPath = ftpPath;
            Message = $"Backup completado exitosamente. Archivo: {fileName}";
        }
        
        public void MarkAsFailed(string error, string details = "")
        {
            EndTime = DateTime.Now;
            Status = BackupStatus.Failed;
            Message = error;
            ErrorDetails = details;
        }
    }
}
