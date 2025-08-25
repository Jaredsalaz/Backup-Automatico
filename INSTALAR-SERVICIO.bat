@echo off
echo.
echo ==========================================
echo    INSTALADOR DE SERVICIO DE BACKUP
echo ==========================================
echo.
echo Este script necesita permisos de administrador.
echo Se abrira PowerShell como administrador...
echo.
pause

REM Cambiar al directorio del script
cd /d "%~dp0"

REM Ejecutar PowerShell como administrador
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File ""%~dp0install-service.ps1""' -Verb RunAs"

echo.
echo Script de instalacion ejecutado.
echo Revisa la ventana de PowerShell para ver el resultado.
echo.
pause
