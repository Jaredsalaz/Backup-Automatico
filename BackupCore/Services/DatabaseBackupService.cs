using BackupCore.Interfaces;
using BackupCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Npgsql;

namespace BackupCore.Services
{
    public class DatabaseBackupService : IDatabaseBackupService
    {
        public async Task<string> CreateBackupAsync(DatabaseConfig config, string outputPath)
        {
            string backupFileName = GenerateBackupFileName(config);
            string fullBackupPath = Path.Combine(outputPath, backupFileName);

            try
            {
                Directory.CreateDirectory(outputPath);

                switch (config.Type)
                {
                    case DatabaseType.SqlServer:
                        await CreateSqlServerBackupAsync(config, fullBackupPath);
                        break;
                    case DatabaseType.MySQL:
                        await CreateMySqlBackupAsync(config, fullBackupPath);
                        break;
                    case DatabaseType.PostgreSQL:
                        await CreatePostgreSqlBackupAsync(config, fullBackupPath);
                        break;
                    default:
                        throw new NotSupportedException($"Tipo de base de datos no soportado: {config.Type}");
                }

                if (!File.Exists(fullBackupPath))
                {
                    throw new FileNotFoundException($"El archivo de backup no se creó correctamente: {fullBackupPath}");
                }

                return fullBackupPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creando backup de {config.Name}: {ex.Message}", ex);
            }
        }

        private async Task CreateSqlServerBackupAsync(DatabaseConfig config, string backupPath)
        {
            using var connection = new SqlConnection(config.ConnectionString);
            await connection.OpenAsync();

            string backupQuery = $@"
                BACKUP DATABASE [{config.Database}] 
                TO DISK = '{backupPath}' 
                WITH FORMAT, INIT, COMPRESSION, 
                NAME = 'Full Backup of {config.Database}',
                DESCRIPTION = 'Backup automático creado el {DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

            using var command = new SqlCommand(backupQuery, connection);
            command.CommandTimeout = 0; // Sin timeout para backups grandes
            await command.ExecuteNonQueryAsync();
        }

        private async Task CreateMySqlBackupAsync(DatabaseConfig config, string backupPath)
        {
            // Para MySQL usamos mysqldump
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "mysqldump";
            process.StartInfo.Arguments = $"--host={config.Server} --port={config.Port} --user={config.Username} --password={config.Password} --single-transaction --routines --triggers --databases {config.Database}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Error en mysqldump: {error}");
            }

            await File.WriteAllTextAsync(backupPath, output);
        }

        private async Task CreatePostgreSqlBackupAsync(DatabaseConfig config, string backupPath)
        {
            // Para PostgreSQL usamos pg_dump
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "pg_dump";
            process.StartInfo.Arguments = $"-h {config.Server} -p {config.Port} -U {config.Username} -d {config.Database} -f \"{backupPath}\" --verbose";
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            // Configurar la contraseña a través de variable de entorno
            process.StartInfo.Environment["PGPASSWORD"] = config.Password;

            process.Start();
            
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Error en pg_dump: {error}");
            }
        }

        public async Task<bool> TestConnectionAsync(DatabaseConfig config)
        {
            try
            {
                switch (config.Type)
                {
                    case DatabaseType.SqlServer:
                        using (var connection = new SqlConnection(config.ConnectionString))
                        {
                            await connection.OpenAsync();
                            return connection.State == ConnectionState.Open;
                        }
                    case DatabaseType.MySQL:
                        using (var connection = new MySqlConnection(config.ConnectionString))
                        {
                            await connection.OpenAsync();
                            return connection.State == ConnectionState.Open;
                        }
                    case DatabaseType.PostgreSQL:
                        using (var connection = new NpgsqlConnection(config.ConnectionString))
                        {
                            await connection.OpenAsync();
                            return connection.State == ConnectionState.Open;
                        }
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public string GenerateBackupFileName(DatabaseConfig config)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string extension = config.Type == DatabaseType.SqlServer ? ".bak" : ".sql";
            return $"{config.Database}_backup_{timestamp}{extension}";
        }

        public string GetDefaultBackupPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string backupPath = Path.Combine(appDataPath, "BackupAutomatico", "Temp");
            Directory.CreateDirectory(backupPath);
            return backupPath;
        }
    }
}
