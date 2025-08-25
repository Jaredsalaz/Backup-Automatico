@echo off
title Actualizacion FINAL del Servicio
color 0D

echo.
echo ================================================================
echo            ACTUALIZACION FINAL - CORRECCION CRON
echo ================================================================
echo.
echo PROBLEMA: El servicio bloquea archivos y no se puede recompilar
echo SOLUCION: Proceso completo de actualizacion forzada
echo.
echo Este script ejecutara:
echo 1. Desinstalar servicio (PowerShell Admin)
echo 2. Esperar liberacion de archivos
echo 3. Recompilar con correccion
echo 4. Reinstalar servicio (PowerShell Admin)
echo.
echo ================================================================
echo.
pause

echo [PASO 1] Ejecutando desinstalacion en PowerShell Admin...
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command \"cd \\\"%cd%\\\"; .\install-service.ps1 -Uninstall; pause\"' -Verb RunAs"

echo.
echo [PASO 2] Esperando 10 segundos para liberar archivos...
timeout /t 10 /nobreak

echo [PASO 3] Recompilando servicio...
dotnet build BackupService --configuration Release

if %errorlevel% neq 0 (
    echo.
    echo ERROR: No se pudo compilar. Intenta manualmente:
    echo 1. Verificar que el servicio este detenido
    echo 2. Ejecutar: dotnet build BackupService --configuration Release
    pause
    exit /b 1
)

echo.
echo [PASO 4] Ejecutando instalacion en PowerShell Admin...
powershell -Command "Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command \"cd \\\"%cd%\\\"; .\install-service.ps1; pause\"' -Verb RunAs"

echo.
echo ================================================================
echo              ACTUALIZACION COMPLETADA
echo ================================================================
echo.
echo VERIFICACION:
echo 1. El servicio debe estar ejecutandose sin errores
echo 2. Usa DIAGNOSTICO-BACKUP.bat para verificar logs
echo 3. Fuerza un backup manual para probar
echo.
echo La expresion Cron "30 01 * * *" ahora se convertira automaticamente
echo a "0 30 01 * * ?" para funcionar con Quartz.NET
echo.
pause
