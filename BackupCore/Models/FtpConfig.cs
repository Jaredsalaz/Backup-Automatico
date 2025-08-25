using System;

namespace BackupCore.Models
{
    public class FtpConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 21;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BasePath { get; set; } = "/";
        public string Branch { get; set; } = string.Empty; // Cliente-Sucursal
        public bool UseSsl { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        public string GetFullBackupPath()
        {
            return $"{BasePath.TrimEnd('/')}/SantaCruz/{Branch}/backups/";
        }
    }
}
