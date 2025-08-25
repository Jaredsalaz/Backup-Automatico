@echo off
title Corregir Error de Expresion Cron
color 0A

echo.
echo =========================================================
echo         CORRECCION DE ERROR DE EXPRESION CRON
echo =========================================================
echo.
echo PROBLEMA SOLUCIONADO:
echo - Error: "Unexpected end of expression" en Cron
echo - Causa: Quartz.NET necesita 6 campos, no 5
echo - Solucion: Convertidor automatico agregado
echo.
echo ESTE SCRIPT:
echo 1. Detiene el servicio
echo 2. Recompila con la correccion
echo 3. Reinicia el servicio
echo.
echo REQUIERE: Permisos de administrador
echo =========================================================
echo.
pause

echo [1] Deteniendo servicio...
net stop BackupAutomaticoService

echo [2] Recompilando con correccion...
dotnet build BackupService --configuration Release

echo [3] Iniciando servicio...
net start BackupAutomaticoService

echo [4] Verificando estado...
sc query BackupAutomaticoService

echo.
echo =========================================================
echo              CORRECCION APLICADA
echo =========================================================
echo.
echo El servicio ahora puede:
echo - Interpretar expresiones Cron de 5 o 6 campos
echo - Convertir automaticamente al formato Quartz.NET
echo - Ejecutar backups segun programacion
echo - Generar logs correctamente
echo.
echo PROBEMOS:
echo 1. Usa DIAGNOSTICO-BACKUP.bat
echo 2. Opcion 5: Forzar Backup Manual
echo 3. Opcion 3: Ver Logs (deberian aparecer)
echo.
pause
