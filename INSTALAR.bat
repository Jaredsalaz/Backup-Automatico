@echo off
echo ===============================================
echo    INSTALADOR SISTEMA DE BACKUP AUTOMATICO
echo              Version 1.0.0
echo ===============================================
echo.

REM Verificar permisos de administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Este script debe ejecutarse como Administrador
    echo Haga clic derecho en el archivo y seleccione "Ejecutar como administrador"
    pause
    exit /b 1
)

echo [1/6] Verificando .NET 9.0...
dotnet --version >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: .NET 9.0 no esta instalado
    echo Descargue e instale desde: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET 9.0 encontrado

echo.
echo [2/6] Compilando proyecto...
dotnet build -c Release
if %errorLevel% neq 0 (
    echo ERROR: Error al compilar el proyecto
    pause
    exit /b 1
)
echo ✓ Compilacion exitosa

echo.
echo [3/6] Publicando servicio...
cd BackupService
dotnet publish -c Release -o "%ProgramFiles%\BackupAutomatico\Service"
if %errorLevel% neq 0 (
    echo ERROR: Error al publicar el servicio
    pause
    exit /b 1
)
echo ✓ Servicio publicado

echo.
echo [4/6] Publicando configurador...
cd ..\BackupConfigurator
dotnet publish -c Release -o "%ProgramFiles%\BackupAutomatico\Configurator"
if %errorLevel% neq 0 (
    echo ERROR: Error al publicar el configurador
    pause
    exit /b 1
)
echo ✓ Configurador publicado

echo.
echo [5/6] Instalando servicio Windows...
sc create BackupAutomatico binPath="%ProgramFiles%\BackupAutomatico\Service\BackupService.exe" start=auto
if %errorLevel% neq 0 (
    echo ADVERTENCIA: Error al crear el servicio (puede que ya exista)
)

sc description BackupAutomatico "Servicio de Backup Automatico de Bases de Datos v1.0.0"
sc start BackupAutomatico
echo ✓ Servicio instalado y iniciado

echo.
echo [6/6] Creando accesos directos...
cd ..
echo Set WshShell = WScript.CreateObject("WScript.Shell") > create_shortcut.vbs
echo Set Shortcut = WshShell.CreateShortcut("%USERPROFILE%\Desktop\Backup Configurador.lnk") >> create_shortcut.vbs
echo Shortcut.TargetPath = "%ProgramFiles%\BackupAutomatico\Configurator\BackupConfigurator.exe" >> create_shortcut.vbs
echo Shortcut.WorkingDirectory = "%ProgramFiles%\BackupAutomatico\Configurator" >> create_shortcut.vbs
echo Shortcut.Description = "Configurador de Backup Automatico" >> create_shortcut.vbs
echo Shortcut.Save >> create_shortcut.vbs
cscript create_shortcut.vbs >nul
del create_shortcut.vbs
echo ✓ Acceso directo creado en el escritorio

echo.
echo ===============================================
echo          INSTALACION COMPLETADA
echo ===============================================
echo.
echo ✓ Servicio BackupAutomatico instalado y ejecutandose
echo ✓ Configurador disponible en: %ProgramFiles%\BackupAutomatico\Configurator\
echo ✓ Acceso directo creado en el escritorio
echo.
echo PROXIMOS PASOS:
echo 1. Ejecute "Backup Configurador" desde el escritorio
echo 2. Configure sus bases de datos y servidores FTP
echo 3. El sistema funcionara automaticamente
echo.
echo Para verificar el estado del servicio:
echo   sc query BackupAutomatico
echo.
echo Para detener el servicio:
echo   sc stop BackupAutomatico
echo.
echo Para desinstalar:
echo   sc delete BackupAutomatico
echo.
pause
