@echo off
title Solucion de Problemas - Servicio de Backup
color 0E

echo.
echo ================================================================
echo           SOLUCION DETECTADA - SERVICIO CON ERRORES
echo ================================================================
echo.
echo PROBLEMA IDENTIFICADO:
echo El servicio tiene un error en el manejo de cancelacion de tareas.
echo Ya se corrigio el codigo pero necesita reinstalacion.
echo.
echo SOLUCION REQUERIDA:
echo 1. Desinstalar servicio actual
echo 2. Recompilar con correccion
echo 3. Reinstalar servicio
echo.
echo ================================================================
echo.
echo PASOS A SEGUIR:
echo.
echo 1. Abrir PowerShell como ADMINISTRADOR
echo    - Buscar "PowerShell" en el menu de inicio
echo    - Click derecho → "Ejecutar como administrador"
echo.
echo 2. Navegar al directorio:
echo    cd "c:\Users\MSI\Desktop\Backup-Automatico"
echo.
echo 3. Desinstalar servicio actual:
echo    .\install-service.ps1 -Uninstall
echo.
echo 4. Recompilar el servicio:
echo    dotnet build BackupService --configuration Release
echo.
echo 5. Reinstalar servicio:
echo    .\install-service.ps1
echo.
echo ================================================================
echo.
echo DESPUES DE LA REINSTALACION:
echo - El servicio funcionara sin errores
echo - Los backups se ejecutaran segun programacion
echo - Los logs se generaran correctamente
echo - Podras usar DIAGNOSTICO-BACKUP.bat para monitoreo
echo.
echo ================================================================
echo.
pause

echo.
echo ¿Quieres que abra PowerShell como administrador ahora? (S/N)
set /p choice="> "

if /i "%choice%"=="S" (
    echo.
    echo Abriendo PowerShell como administrador...
    echo Ejecuta los comandos mostrados arriba.
    echo.
    powershell -Command "Start-Process PowerShell -Verb RunAs"
) else (
    echo.
    echo Ejecuta manualmente los pasos cuando estes listo.
)

echo.
pause
