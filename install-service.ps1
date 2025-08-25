# Script de Instalación del Servicio de Backup Automático
# Ejecutar como Administrador

param(
    [Parameter(Mandatory=$false)]
    [switch]$Uninstall
)

$serviceName = "BackupAutomaticoService"
$serviceDisplayName = "Servicio de Backup Automático"
$serviceDescription = "Servicio para realizar backups automáticos de bases de datos y subirlos por FTP"

# Verificar si se ejecuta como administrador
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "[ERROR] Este script debe ejecutarse como Administrador" -ForegroundColor Red
    Write-Host "[INFO] Pasos para ejecutar como administrador:" -ForegroundColor Yellow
    Write-Host "   1. Abrir PowerShell como Administrador" -ForegroundColor White
    Write-Host "   2. Navegar a: cd '$PSScriptRoot'" -ForegroundColor White
    Write-Host "   3. Ejecutar: .\install-service.ps1" -ForegroundColor White
    pause
    exit 1
}

# Función para desinstalar el servicio
function Uninstall-BackupService {
    Write-Host "[UNINSTALL] Desinstalando servicio '$serviceName'..." -ForegroundColor Yellow
    
    # Detener el servicio si está corriendo
    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($service) {
        if ($service.Status -eq 'Running') {
            Write-Host "[STOP] Deteniendo servicio..." -ForegroundColor Yellow
            Stop-Service -Name $serviceName -Force
            Start-Sleep -Seconds 3
        }
        
        # Eliminar el servicio
        Write-Host "[DELETE] Eliminando servicio..." -ForegroundColor Yellow
        sc.exe delete $serviceName
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[SUCCESS] Servicio desinstalado correctamente" -ForegroundColor Green
        } else {
            Write-Host "[ERROR] Error al desinstalar el servicio" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "[INFO] El servicio no esta instalado" -ForegroundColor Blue
    }
}

# Función para instalar el servicio
function Install-BackupService {
    Write-Host "[INSTALL] Instalando Servicio de Backup Automatico..." -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    
    # Ruta del ejecutable del servicio
    $exePath = Join-Path $PSScriptRoot "BackupService\bin\Release\net9.0\BackupService.exe"
    
    # Verificar que el ejecutable existe
    if (-not (Test-Path $exePath)) {
        Write-Host "[ERROR] No se encontro el ejecutable del servicio en: $exePath" -ForegroundColor Red
        Write-Host "[INFO] Asegurate de haber compilado el proyecto primero:" -ForegroundColor Yellow
        Write-Host "   dotnet build BackupService" -ForegroundColor White
        pause
        exit 1
    }
    
    # Verificar si el servicio ya existe
    $existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($existingService) {
        Write-Host "[WARNING] El servicio ya existe. Desinstalando version anterior..." -ForegroundColor Yellow
        Uninstall-BackupService
        Start-Sleep -Seconds 2
    }
    
    # Crear el servicio
    Write-Host "[CREATE] Creando servicio..." -ForegroundColor Blue
    $result = sc.exe create $serviceName binpath= "`"$exePath`"" start= auto DisplayName= "`"$serviceDisplayName`""
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[ERROR] Error al crear el servicio" -ForegroundColor Red
        Write-Host $result
        pause
        exit 1
    }
    
    # Configurar descripción del servicio
    sc.exe description $serviceName "`"$serviceDescription`""
    
    # Configurar recuperación del servicio (reiniciar automáticamente si falla)
    sc.exe failure $serviceName reset= 86400 actions= restart/5000/restart/5000/restart/5000
    
    Write-Host "[SUCCESS] Servicio creado correctamente" -ForegroundColor Green
    
    # Iniciar el servicio
    Write-Host "[START] Iniciando servicio..." -ForegroundColor Blue
    $startResult = sc.exe start $serviceName
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[SUCCESS] Servicio iniciado correctamente" -ForegroundColor Green
        Start-Sleep -Seconds 3
        
        # Verificar estado del servicio
        $service = Get-Service -Name $serviceName
        Write-Host "[STATUS] Estado del servicio: $($service.Status)" -ForegroundColor Blue
        
    } else {
        Write-Host "[WARNING] El servicio se creo pero no se pudo iniciar automaticamente" -ForegroundColor Yellow
        Write-Host "   Esto puede ser normal en la primera instalacion" -ForegroundColor White
        Write-Host "   Puedes iniciarlo manualmente desde services.msc" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "[COMPLETED] INSTALACION COMPLETADA" -ForegroundColor Green
    Write-Host "=========================" -ForegroundColor Green
    Write-Host "[INFO] Informacion del servicio:" -ForegroundColor White
    Write-Host "   • Nombre: $serviceName" -ForegroundColor White
    Write-Host "   • Nombre para mostrar: $serviceDisplayName" -ForegroundColor White
    Write-Host "   • Inicio: Automatico" -ForegroundColor White
    Write-Host "   • Configuraciones: %APPDATA%\BackupAutomatico\" -ForegroundColor White
    Write-Host ""
    Write-Host "[MANAGEMENT] Gestion del servicio:" -ForegroundColor Yellow
    Write-Host "   • Ver servicios: services.msc" -ForegroundColor White
    Write-Host "   • Logs de Windows: eventvwr.msc" -ForegroundColor White
    Write-Host "   • Desinstalar: .\install-service.ps1 -Uninstall" -ForegroundColor White
}

# Ejecutar según el parámetro
if ($Uninstall) {
    Uninstall-BackupService
} else {
    Install-BackupService
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..."
pause
