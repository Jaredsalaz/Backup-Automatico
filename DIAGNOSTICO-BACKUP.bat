@echo off
title Diagnostico del Sistema de Backup
color 0F

:MENU
cls
echo.
echo =========================================================
echo            DIAGNOSTICO DEL SISTEMA DE BACKUP
echo =========================================================
echo.
echo 1. Ver Estado del Servicio
echo 2. Ver Configuraciones de Backup
echo 3. Ver Logs del Sistema
echo 4. Ver Eventos de Windows
echo 5. Forzar Backup Manual (PRUEBA)
echo 6. Ver Archivos FTP Enviados
echo 7. Verificar Conectividad BD y FTP
echo 8. Ver Horario Proximo Backup
echo 9. Salir
echo.
set /p choice="Selecciona una opcion (1-9): "

if "%choice%"=="1" goto SERVICE_STATUS
if "%choice%"=="2" goto CONFIGS
if "%choice%"=="3" goto LOGS
if "%choice%"=="4" goto EVENTS
if "%choice%"=="5" goto FORCE_BACKUP
if "%choice%"=="6" goto FTP_FILES
if "%choice%"=="7" goto CONNECTIVITY
if "%choice%"=="8" goto NEXT_BACKUP
if "%choice%"=="9" goto EXIT
goto MENU

:SERVICE_STATUS
cls
echo.
echo =========================================================
echo                  ESTADO DEL SERVICIO
echo =========================================================
echo.
sc query BackupAutomaticoService
echo.
echo =========================================================
echo.
powershell -Command "Get-Service BackupAutomaticoService | Format-Table Name, Status, StartType -AutoSize"
echo.
echo Procesos relacionados:
tasklist /FI "IMAGENAME eq BackupService.exe"
echo.
pause
goto MENU

:CONFIGS
cls
echo.
echo =========================================================
echo              CONFIGURACIONES DE BACKUP
echo =========================================================
echo.
echo [CONFIGURACIONES DE BACKUP]:
type "%APPDATA%\BackupAutomatico\Config\backups.json"
echo.
echo =========================================================
echo.
echo [CONFIGURACIONES DE BD]:
type "%APPDATA%\BackupAutomatico\Config\databases.json"
echo.
echo =========================================================
echo.
echo [CONFIGURACIONES FTP]:
type "%APPDATA%\BackupAutomatico\Config\ftp.json"
echo.
pause
goto MENU

:LOGS
cls
echo.
echo =========================================================
echo                  LOGS DEL SISTEMA
echo =========================================================
echo.
echo Directorio de logs: %APPDATA%\BackupAutomatico\Logs
echo.
dir "%APPDATA%\BackupAutomatico\Logs" /O:D
echo.
echo =========================================================
echo.
echo Contenido de logs (si existen):
for %%f in ("%APPDATA%\BackupAutomatico\Logs\*.log") do (
    echo.
    echo === %%f ===
    type "%%f"
    echo.
)
echo.
pause
goto MENU

:EVENTS
cls
echo.
echo =========================================================
echo              EVENTOS DE WINDOWS
echo =========================================================
echo.
echo Abriendo Visor de Eventos...
echo Buscar: "BackupService" o "BackupAutomaticoService"
start eventvwr.msc
echo.
pause
goto MENU

:FORCE_BACKUP
cls
echo.
echo =========================================================
echo                FORZAR BACKUP MANUAL
echo =========================================================
echo.
echo ATENCION: Esta funcion reinicia el servicio para
echo forzar la ejecucion inmediata de backups pendientes.
echo.
set /p confirm="¿Estas seguro? (S/N): "
if /i "%confirm%" NEQ "S" goto MENU

echo.
echo [1] Deteniendo servicio...
net stop BackupAutomaticoService
timeout /t 3 /nobreak >nul

echo [2] Reiniciando servicio...
net start BackupAutomaticoService
timeout /t 5 /nobreak >nul

echo [3] Verificando estado...
sc query BackupAutomaticoService

echo.
echo [IMPORTANTE] Revisa los logs en unos minutos para ver la actividad.
echo El servicio puede tardar unos momentos en procesar las configuraciones.
echo.
pause
goto MENU

:FTP_FILES
cls
echo.
echo =========================================================
echo               ARCHIVOS ENVIADOS POR FTP
echo =========================================================
echo.
echo Esta funcion requiere que configures la ruta local donde
echo se guardan los backups antes de enviarlos por FTP.
echo.
echo Ubicacion tipica: C:\temp\backups\ o similar
echo.
set /p backupPath="Ingresa la ruta de backups locales: "
if exist "%backupPath%" (
    dir "%backupPath%" /O:D
) else (
    echo No se encontro la ruta especificada.
)
echo.
pause
goto MENU

:CONNECTIVITY
cls
echo.
echo =========================================================
echo           VERIFICAR CONECTIVIDAD BD Y FTP
echo =========================================================
echo.
echo [INFO] Para verificar conectividad completa:
echo 1. Abrir BackupConfigurator.exe
echo 2. Ir a la tab de Bases de Datos
echo 3. Seleccionar una BD y hacer "Probar Conexion"
echo 4. Ir a la tab de Cliente FTP
echo 5. Hacer "Probar Conexion FTP"
echo.
set /p openConfig="¿Abrir configurador ahora? (S/N): "
if /i "%openConfig%"=="S" (
    if exist "BackupConfigurator\bin\Release\net9.0-windows\BackupConfigurator.exe" (
        start "" "BackupConfigurator\bin\Release\net9.0-windows\BackupConfigurator.exe"
    ) else if exist "BackupConfigurator\bin\Debug\net9.0-windows\BackupConfigurator.exe" (
        start "" "BackupConfigurator\bin\Debug\net9.0-windows\BackupConfigurator.exe"
    ) else (
        echo Error: No se encontro el configurador.
    )
)
echo.
pause
goto MENU

:NEXT_BACKUP
cls
echo.
echo =========================================================
echo              HORARIO PROXIMO BACKUP
echo =========================================================
echo.
powershell -Command "Get-Date; Write-Host 'Hora actual del sistema'"
echo.
echo Configuraciones de backup:
powershell -Command "$config = Get-Content '%APPDATA%\BackupAutomatico\Config\backups.json' | ConvertFrom-Json; $config | ForEach-Object { Write-Host \"Backup: $($_.Name)\"; Write-Host \"  Schedule: $($_.Schedule) (Cron)\"; Write-Host \"  Proximo: $($_.NextBackup)\"; Write-Host \"  Habilitado: $($_.IsEnabled)\"; Write-Host \"\" }"
echo.
pause
goto MENU

:EXIT
cls
echo.
echo Gracias por usar el Diagnostico del Sistema de Backup!
echo.
timeout /t 2 /nobreak >nul
exit
