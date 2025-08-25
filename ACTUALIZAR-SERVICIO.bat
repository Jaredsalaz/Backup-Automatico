@echo off
title Actualizar Servicio de Backup
color 0C
echo.
echo ====================================================
echo            ACTUALIZACION DEL SERVICIO
echo ====================================================
echo.
echo IMPORTANTE: Este script requiere permisos de administrador
echo.
pause

echo [1] Deteniendo servicio...
net stop BackupAutomaticoService
if %errorlevel% neq 0 (
    echo Error al detener el servicio. Verifica permisos de administrador.
    pause
    exit /b 1
)

echo [2] Desinstalando servicio...
sc delete BackupAutomaticoService
if %errorlevel% neq 0 (
    echo Error al desinstalar el servicio.
    pause
    exit /b 1
)

echo [3] Esperando que se liberen archivos...
timeout /t 5 /nobreak >nul

echo [4] Recompilando servicio...
dotnet build BackupService --configuration Release
if %errorlevel% neq 0 (
    echo Error al compilar el servicio.
    pause
    exit /b 1
)

echo [5] Reinstalando servicio...
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File \"install-service.ps1\"' -Verb RunAs"

echo.
echo ====================================================
echo     ACTUALIZACION COMPLETADA
echo ====================================================
echo.
echo El servicio ha sido actualizado con las correcciones.
echo Revisa el script de instalacion para confirmar el estado.
echo.
pause
