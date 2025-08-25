@echo off
title Gestor del Servicio de Backup Automatico
color 0A

:MENU
cls
echo.
echo ================================================
echo        GESTOR DEL SERVICIO DE BACKUP
echo ================================================
echo.
echo 1. Instalar Servicio
echo 2. Desinstalar Servicio
echo 3. Iniciar Servicio
echo 4. Detener Servicio
echo 5. Ver Estado del Servicio
echo 6. Abrir Configurador
echo 7. Ver Logs
echo 8. Salir
echo.
set /p choice="Selecciona una opcion (1-8): "

if "%choice%"=="1" goto INSTALL
if "%choice%"=="2" goto UNINSTALL
if "%choice%"=="3" goto START
if "%choice%"=="4" goto STOP
if "%choice%"=="5" goto STATUS
if "%choice%"=="6" goto CONFIG
if "%choice%"=="7" goto LOGS
if "%choice%"=="8" goto EXIT
goto MENU

:INSTALL
cls
echo.
echo ==========================================
echo         INSTALANDO SERVICIO
echo ==========================================
echo.
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File ""%~dp0install-service.ps1""' -Verb RunAs"
echo.
echo Revisa la ventana de PowerShell para ver el resultado.
pause
goto MENU

:UNINSTALL
cls
echo.
echo ==========================================
echo        DESINSTALANDO SERVICIO
echo ==========================================
echo.
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File ""%~dp0install-service.ps1"" -Uninstall' -Verb RunAs"
echo.
echo Revisa la ventana de PowerShell para ver el resultado.
pause
goto MENU

:START
cls
echo.
echo ==========================================
echo         INICIANDO SERVICIO
echo ==========================================
echo.
net start BackupAutomaticoService
echo.
pause
goto MENU

:STOP
cls
echo.
echo ==========================================
echo         DETENIENDO SERVICIO
echo ==========================================
echo.
net stop BackupAutomaticoService
echo.
pause
goto MENU

:STATUS
cls
echo.
echo ==========================================
echo         ESTADO DEL SERVICIO
echo ==========================================
echo.
sc query BackupAutomaticoService
echo.
echo ==========================================
echo.
powershell -Command "Get-Service BackupAutomaticoService -ErrorAction SilentlyContinue | Format-Table Name, Status, StartType -AutoSize"
echo.
pause
goto MENU

:CONFIG
cls
echo.
echo ==========================================
echo       ABRIENDO CONFIGURADOR
echo ==========================================
echo.
if exist "BackupConfigurator\bin\Release\net9.0-windows\BackupConfigurator.exe" (
    start "" "BackupConfigurator\bin\Release\net9.0-windows\BackupConfigurator.exe"
) else if exist "BackupConfigurator\bin\Debug\net9.0-windows\BackupConfigurator.exe" (
    start "" "BackupConfigurator\bin\Debug\net9.0-windows\BackupConfigurator.exe"
) else (
    echo Error: No se encontro el configurador.
    echo Compila el proyecto primero: dotnet build BackupConfigurator
)
echo.
pause
goto MENU

:LOGS
cls
echo.
echo ==========================================
echo            ABRIENDO LOGS
echo ==========================================
echo.
set "logPath=%APPDATA%\BackupAutomatico\Logs"
if exist "%logPath%" (
    explorer "%logPath%"
    echo Carpeta de logs abierta: %logPath%
) else (
    echo No se encontraron logs en: %logPath%
    echo El servicio debe ejecutarse al menos una vez para crear logs.
)
echo.
pause
goto MENU

:EXIT
cls
echo.
echo Gracias por usar el Gestor del Servicio de Backup!
echo.
timeout /t 2 /nobreak >nul
exit
