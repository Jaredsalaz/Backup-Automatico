using BackupCore.Interfaces;
using BackupCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackupCore.Services
{
    public class BackupLogService : IBackupLogService
    {
        private readonly string _logPath;

        public BackupLogService()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _logPath = Path.Combine(appDataPath, "BackupAutomatico", "Logs");
            Directory.CreateDirectory(_logPath);
        }

        public async Task SaveLogAsync(BackupLog log)
        {
            try
            {
                string fileName = $"backup_log_{DateTime.Now:yyyyMM}.json";
                string filePath = Path.Combine(_logPath, fileName);

                List<BackupLog> logs;
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    logs = JsonSerializer.Deserialize<List<BackupLog>>(json) ?? new List<BackupLog>();
                }
                else
                {
                    logs = new List<BackupLog>();
                }

                // Remover log existente si es una actualización
                logs.RemoveAll(l => l.Id == log.Id);
                logs.Add(log);

                // Ordenar por fecha descendente
                logs = logs.OrderByDescending(l => l.StartTime).ToList();

                string updatedJson = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, updatedJson);
            }
            catch (Exception ex)
            {
                // Log del error en el event log del sistema
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("BackupAutomatico", 
                        $"Error guardando log de backup: {ex.Message}", 
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch
                {
                    // Ignorar errores del event log
                }
            }
        }

        public async Task<List<BackupLog>> GetLogsAsync(int count = 100)
        {
            try
            {
                var allLogs = new List<BackupLog>();
                var logFiles = Directory.GetFiles(_logPath, "backup_log_*.json")
                    .OrderByDescending(f => f)
                    .Take(3); // Solo los últimos 3 meses

                foreach (string file in logFiles)
                {
                    string json = await File.ReadAllTextAsync(file);
                    var logs = JsonSerializer.Deserialize<List<BackupLog>>(json) ?? new List<BackupLog>();
                    allLogs.AddRange(logs);
                }

                return allLogs
                    .OrderByDescending(l => l.StartTime)
                    .Take(count)
                    .ToList();
            }
            catch
            {
                return new List<BackupLog>();
            }
        }

        public async Task<List<BackupLog>> GetLogsByConfigAsync(string backupConfigId)
        {
            try
            {
                var allLogs = await GetLogsAsync(1000);
                return allLogs
                    .Where(l => l.BackupConfigId == backupConfigId)
                    .OrderByDescending(l => l.StartTime)
                    .ToList();
            }
            catch
            {
                return new List<BackupLog>();
            }
        }

        public async Task CleanOldLogsAsync(int retentionDays = 90)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-retentionDays);
                var logFiles = Directory.GetFiles(_logPath, "backup_log_*.json");

                foreach (string file in logFiles)
                {
                    try
                    {
                        string json = await File.ReadAllTextAsync(file);
                        var logs = JsonSerializer.Deserialize<List<BackupLog>>(json) ?? new List<BackupLog>();
                        
                        // Filtrar logs que no han expirado
                        var validLogs = logs.Where(l => l.StartTime >= cutoffDate).ToList();
                        
                        if (validLogs.Count == 0)
                        {
                            // Eliminar archivo completo si no hay logs válidos
                            File.Delete(file);
                        }
                        else if (validLogs.Count != logs.Count)
                        {
                            // Actualizar archivo con logs válidos
                            string updatedJson = JsonSerializer.Serialize(validLogs, new JsonSerializerOptions { WriteIndented = true });
                            await File.WriteAllTextAsync(file, updatedJson);
                        }
                    }
                    catch
                    {
                        // Continuar con el siguiente archivo si hay error
                    }
                }
            }
            catch
            {
                // Ignorar errores de limpieza
            }
        }
    }
}
