using System;

namespace BackupCore.Models
{
    public class DatabaseConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; }
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Propiedad calculada para generar el connection string
        public string ConnectionString
        {
            get
            {
                return Type switch
                {
                    DatabaseType.SqlServer => $"Server={Server},{Port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=true;",
                    DatabaseType.MySQL => $"Server={Server};Port={Port};Database={Database};Uid={Username};Pwd={Password};",
                    DatabaseType.PostgreSQL => $"Host={Server};Port={Port};Database={Database};Username={Username};Password={Password};",
                    DatabaseType.Oracle => $"Data Source={Server}:{Port}/{Database};User Id={Username};Password={Password};",
                    _ => throw new NotSupportedException($"Tipo de base de datos no soportado: {Type}")
                };
            }
        }
    }

    public enum DatabaseType
    {
        SqlServer = 1,
        MySQL = 2,
        PostgreSQL = 3,
        Oracle = 4
    }
}
