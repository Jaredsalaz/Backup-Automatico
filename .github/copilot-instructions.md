<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# Instrucciones para GitHub Copilot - Sistema de Backup Automático

## Contexto del Proyecto

Este es un **sistema completo de backup automático** para bases de datos con las siguientes características:

- **Lenguaje:** C# (.NET 8.0)
- **Arquitectura:** Microservicios con servicios Windows
- **Propósito:** Backup automático de BD con envío por FTP
- **Seguridad:** Proceso oculto y encriptación avanzada

## Estructura del Proyecto

### BackupCore (Librería Principal)
- **Models:** Entidades de datos (DatabaseConfig, FtpConfig, BackupConfig, BackupLog)
- **Interfaces:** Contratos de servicios (IDatabaseBackupService, IFtpService, etc.)
- **Services:** Implementaciones de lógica de negocio

### BackupService (Servicio Windows)
- **Worker:** Servicio que se ejecuta como demonio
- **Scheduler:** Programación automática con Quartz.NET
- **Security:** Funcionalidades de ocultación de proceso

### BackupConfigurator (Interfaz WinForms)
- **MainForm:** Interfaz principal de configuración
- **Forms:** Formularios específicos para configuración

## Convenciones de Código

### Naming
- **Clases:** PascalCase (ej: `DatabaseBackupService`)
- **Métodos:** PascalCase (ej: `CreateBackupAsync`)
- **Variables:** camelCase (ej: `backupFilePath`)
- **Constantes:** UPPER_CASE (ej: `DEFAULT_PORT`)

### Async/Await
- Todos los métodos de I/O deben ser asíncronos
- Usar sufijo `Async` en métodos asíncronos
- Preferir `Task<T>` sobre `void` para métodos async

### Error Handling
- Usar try-catch específicos para diferentes tipos de errores
- Loggear errores detalladamente
- No exponer información sensible en mensajes de error
- Usar excepciones personalizadas cuando sea apropiado

## Patrones de Diseño Utilizados

### Dependency Injection
- Todos los servicios se registran en el contenedor DI
- Interfaces para abstracción de dependencias
- Lifecycle apropiado (Singleton para servicios stateless)

### Repository Pattern
- `ConfigurationService` actúa como repositorio para configuraciones
- Separación entre lógica de acceso a datos y lógica de negocio

### Orchestrator Pattern
- `BackupOrchestrator` coordina operaciones complejas
- Manejo centralizado de workflows de backup

## Especificaciones Técnicas

### Bases de Datos Soportadas
- **SQL Server:** Usando `System.Data.SqlClient` (pronto migrar a Microsoft.Data.SqlClient)
- **MySQL:** Usando `MySql.Data` y `mysqldump` external process
- **PostgreSQL:** Usando `Npgsql` y `pg_dump` external process

### FTP
- **Librería:** FluentFTP
- **Estructura:** [BasePath]/SantaCruz/[Sucursal]/backups/
- **Características:** SSL/TLS opcional, creación automática de directorios

### Seguridad
- **Encriptación:** AES-256 para contraseñas y configuraciones
- **Ocultación:** Windows API para ocultar procesos
- **Almacenamiento:** Archivos en AppData del usuario

### Logging
- **Formato:** JSON estructurado
- **Rotación:** Archivos mensuales
- **Retención:** 90 días por defecto
- **Níveis:** Info, Warning, Error

## Reglas Específicas para Copilot

### Al generar código para este proyecto:

1. **Siempre usar interfaces** antes de implementaciones concretas
2. **Incluir manejo de errores** robusto con logging apropiado
3. **Usar async/await** para operaciones de I/O
4. **Validar parámetros** al inicio de métodos públicos
5. **Encriptar información sensible** antes de almacenar
6. **Documentar métodos públicos** con XML comments
7. **Seguir el patrón de configuración** existente (JSON + encriptación)

### Ejemplos de código preferido:

```csharp
// ✅ Bueno - Con validación, logging y async
public async Task<bool> CreateBackupAsync(DatabaseConfig config)
{
    if (config == null) throw new ArgumentNullException(nameof(config));
    
    try 
    {
        _logger.LogInformation("Iniciando backup para {DatabaseName}", config.Name);
        // ... implementación
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creando backup para {DatabaseName}", config.Name);
        throw;
    }
}

// ❌ Malo - Sin validación ni manejo de errores
public bool CreateBackup(DatabaseConfig config)
{
    // ... implementación directa sin checks
}
```

### Al trabajar con configuraciones:
- Siempre encriptar contraseñas usando `ISecurityService`
- Validar configuraciones antes de guardar
- Proveer valores por defecto apropiados

### Al trabajar con FTP:
- Crear estructura de directorios automáticamente
- Manejar timeouts y reintentos
- Validar conectividad antes de operaciones complejas

### Al trabajar con bases de datos:
- Usar connection strings apropiados para cada tipo
- Manejar timeouts largos para backups grandes
- Validar que las herramientas externas estén disponibles (mysqldump, pg_dump)

## Contexto de Negocio

El sistema está diseñado para **pequeñas y medianas empresas** que necesitan:
- Backups automáticos y confiables
- Estructura organizacional por sucursales
- Proceso oculto para mayor seguridad
- Facilidad de configuración sin conocimientos técnicos avanzados
- Múltiples tipos de bases de datos

Por favor, mantén este contexto en mente al sugerir código o mejoras.
