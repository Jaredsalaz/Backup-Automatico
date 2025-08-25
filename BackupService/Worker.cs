using BackupCore.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace BackupService;

public class BackupWorker : BackgroundService
{
    private readonly ILogger<BackupWorker> _logger;
    private readonly IBackupOrchestrator _backupOrchestrator;
    private readonly IConfigurationService _configService;
    private readonly IBackupLogService _logService;
    private IScheduler? _scheduler;

    public BackupWorker(
        ILogger<BackupWorker> logger,
        IBackupOrchestrator backupOrchestrator,
        IConfigurationService configService,
        IBackupLogService logService)
    {
        _logger = logger;
        _backupOrchestrator = backupOrchestrator;
        _configService = configService;
        _logService = logService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Servicio de Backup Automático iniciado en: {time}", DateTimeOffset.Now);

            // Inicializar el programador de tareas
            _logger.LogInformation("Inicializando programador de tareas Quartz.NET...");
            StdSchedulerFactory factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start();
            _logger.LogInformation("Programador de tareas iniciado correctamente");

            // Programar tareas de backup
            _logger.LogInformation("Programando tareas de backup...");
            await ScheduleBackupJobs();

            // Ejecutar limpieza de logs cada día
            _logger.LogInformation("Programando tareas de mantenimiento...");
            await ScheduleMaintenanceJobs();

            _logger.LogInformation("Servicio completamente inicializado. Entrando en bucle principal...");

            // Mantener el servicio corriendo
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Verificar y ejecutar backups pendientes cada 5 minutos
                    _logger.LogDebug("Verificando backups pendientes...");
                    await _backupOrchestrator.ExecuteAllScheduledBackupsAsync();
                    
                    // Recargar configuraciones cada hora
                    if (DateTime.Now.Minute == 0)
                    {
                        _logger.LogInformation("Recargando configuraciones de backup...");
                        await ScheduleBackupJobs();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando verificación de backups");
                }

                try
                {
                    _logger.LogDebug("Esperando 5 minutos antes de próxima verificación...");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // El servicio se está deteniendo, esto es normal
                    _logger.LogInformation("Servicio de backup detenido");
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // El servicio se está deteniendo, esto es normal
            _logger.LogInformation("Servicio de backup detenido normalmente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico en el servicio de backup");
        }
    }

    private async Task ScheduleBackupJobs()
    {
        try
        {
            var backupConfigs = await _configService.GetBackupConfigsAsync();
            
            foreach (var config in backupConfigs.Where(c => c.IsEnabled))
            {
                // Crear un job para cada configuración de backup
                var job = JobBuilder.Create<BackupJob>()
                    .WithIdentity($"backup-{config.Id}", "backup-group")
                    .UsingJobData("BackupConfigId", config.Id)
                    .Build();

                // Convertir expresión Cron de 5 campos a 6 campos para Quartz.NET
                string quartzCronExpression = ConvertToQuartzCron(config.Schedule);
                
                // Crear trigger usando la expresión Cron
                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"trigger-{config.Id}", "backup-group")
                    .WithSchedule(CronScheduleBuilder.CronSchedule(quartzCronExpression))
                    .Build();

                // Programar o reprogramar el job
                if (await _scheduler!.CheckExists(job.Key))
                {
                    await _scheduler.DeleteJob(job.Key);
                }
                
                await _scheduler.ScheduleJob(job, trigger);
                
                _logger.LogInformation("Programado backup para {name} con horario {schedule}", 
                    config.Name, config.Schedule);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error programando trabajos de backup");
        }
    }

    /// <summary>
    /// Convierte una expresión Cron de 5 campos a 6 campos para Quartz.NET
    /// </summary>
    /// <param name="cronExpression">Expresión Cron de 5 campos (min hour day month dow)</param>
    /// <returns>Expresión Cron de 6 campos para Quartz.NET (sec min hour day month dow)</returns>
    private string ConvertToQuartzCron(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            return "0 0 2 * * ?"; // Default: diario a las 2:00 AM

        var parts = cronExpression.Trim().Split(' ');
        
        // Si ya tiene 6 campos, retornarlo tal como está
        if (parts.Length == 6)
            return cronExpression;
            
        // Si tiene 5 campos, convertir a 6 campos
        if (parts.Length == 5)
        {
            // Formato entrada: min hour day month dow
            // Formato salida: sec min hour day month dow
            string seconds = "0";  // Agregar segundos = 0
            string minutes = parts[0];
            string hours = parts[1];
            string dayOfMonth = parts[2];
            string month = parts[3];
            string dayOfWeek = parts[4];
            
            // Convertir el último campo si es necesario
            if (dayOfWeek == "*")
                dayOfWeek = "?";
                
            return $"{seconds} {minutes} {hours} {dayOfMonth} {month} {dayOfWeek}";
        }
        
        // Si tiene un formato inválido, usar default
        _logger.LogWarning("Expresión Cron inválida: {CronExpression}. Usando horario por defecto.", cronExpression);
        return "0 0 2 * * ?"; // Default: diario a las 2:00 AM
    }

    private async Task ScheduleMaintenanceJobs()
    {
        try
        {
            // Job de limpieza diaria a las 3:00 AM
            var maintenanceJob = JobBuilder.Create<MaintenanceJob>()
                .WithIdentity("maintenance", "maintenance-group")
                .Build();

            var maintenanceTrigger = TriggerBuilder.Create()
                .WithIdentity("maintenance-trigger", "maintenance-group")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(3, 0))
                .Build();

            await _scheduler!.ScheduleJob(maintenanceJob, maintenanceTrigger);
            
            _logger.LogInformation("Programado job de mantenimiento diario a las 3:00 AM");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error programando job de mantenimiento");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Deteniendo servicio de Backup Automático...");
        
        if (_scheduler != null)
        {
            await _scheduler.Shutdown();
        }
        
        await base.StopAsync(stoppingToken);
    }
}

// Job para ejecutar backups específicos
public class BackupJob : IJob
{
    private readonly IBackupOrchestrator _backupOrchestrator;
    private readonly ILogger<BackupJob> _logger;

    public BackupJob(IBackupOrchestrator backupOrchestrator, ILogger<BackupJob> logger)
    {
        _backupOrchestrator = backupOrchestrator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        string backupConfigId = context.JobDetail.JobDataMap.GetString("BackupConfigId") ?? "";
        
        try
        {
            _logger.LogInformation("Ejecutando backup programado para configuración: {id}", backupConfigId);
            await _backupOrchestrator.ExecuteBackupAsync(backupConfigId);
            _logger.LogInformation("Backup completado para configuración: {id}", backupConfigId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando backup para configuración: {id}", backupConfigId);
        }
    }
}

// Job para tareas de mantenimiento
public class MaintenanceJob : IJob
{
    private readonly IBackupLogService _logService;
    private readonly ILogger<MaintenanceJob> _logger;

    public MaintenanceJob(IBackupLogService logService, ILogger<MaintenanceJob> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Ejecutando tareas de mantenimiento");
            
            // Limpiar logs antiguos
            await _logService.CleanOldLogsAsync(90);
            
            _logger.LogInformation("Mantenimiento completado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando mantenimiento");
        }
    }
}
