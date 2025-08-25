# 🚀 INSTALACIÓN DEL SERVICIO DE BACKUP AUTOMÁTICO

## 📋 REQUISITOS PREVIOS
- Windows 10/11 o Windows Server
- .NET 9.0 Runtime instalado
- Permisos de Administrador

## 🔧 INSTALACIÓN PASO A PASO

### 1. Compilar el Servicio (si no se ha hecho)
```powershell
dotnet build BackupService
```

### 2. Abrir PowerShell como Administrador
- Buscar "PowerShell" en el menú de inicio
- Click derecho → "Ejecutar como administrador"
- Aceptar el control de cuentas de usuario (UAC)

### 3. Navegar al directorio del proyecto
```powershell
cd "c:\Users\MSI\Desktop\Backup-Automatico"
```

### 4. Ejecutar el script de instalación
```powershell
.\install-service.ps1
```

## 🎛️ GESTIÓN DEL SERVICIO

### Ver el servicio instalado
```powershell
Get-Service BackupAutomaticoService
```

### Iniciar/Detener manualmente
```powershell
Start-Service BackupAutomaticoService
Stop-Service BackupAutomaticoService
```

### Desinstalar el servicio
```powershell
.\install-service.ps1 -Uninstall
```

## 📊 MONITOREO

### 1. Administrador de Servicios
- Ejecutar: `services.msc`
- Buscar: "Servicio de Backup Automático"

### 2. Visor de Eventos de Windows
- Ejecutar: `eventvwr.msc`
- Ir a: Registros de Windows → Aplicación
- Filtrar por: "BackupService"

### 3. Logs de la aplicación
- Ubicación: `%APPDATA%\BackupAutomatico\Logs\`
- Formato: JSON estructurado
- Rotación: Mensual

## 🔍 SOLUCIÓN DE PROBLEMAS

### El servicio no inicia
1. Verificar que .NET 9.0 Runtime está instalado
2. Revisar logs en el Visor de Eventos
3. Verificar permisos de archivos
4. Comprobar configuraciones en %APPDATA%\BackupAutomatico\

### No se ejecutan los backups
1. Verificar que hay configuraciones guardadas
2. Comprobar conectividad a bases de datos
3. Verificar conectividad FTP
4. Revisar logs de la aplicación

### Cambiar configuraciones
1. Abrir BackupConfigurator.exe
2. Modificar configuraciones necesarias
3. Reiniciar el servicio:
   ```powershell
   Restart-Service BackupAutomaticoService
   ```

## 📁 UBICACIONES IMPORTANTES

- **Ejecutable del servicio:** `BackupService\bin\Debug\net9.0\BackupService.exe`
- **Configuraciones:** `%APPDATA%\BackupAutomatico\`
- **Logs:** `%APPDATA%\BackupAutomatico\Logs\`
- **Configurador:** `BackupConfigurator\bin\Debug\net9.0-windows\BackupConfigurator.exe`

## 🔄 FUNCIONAMIENTO

El servicio se ejecuta automáticamente:
- **Inicio:** Con Windows (automático)
- **Frecuencia:** Según configuración de cada backup
- **Horarios:** Configurables en el interfaz gráfico
- **Reintentos:** Automáticos en caso de fallo
- **Notificaciones:** Logs detallados
