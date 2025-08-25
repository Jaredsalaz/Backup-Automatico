# 🚀 Sistema de Backup Automático

Sistema completo de backup automático para bases de datos con envío por FTP, desarrollado en C# .NET 9.0.

[![Estado](https://img.shields.io/badge/Estado-FUNCIONANDO-brightgreen)](#)
[![Versión](https://img.shields.io/badge/Versión-1.0.0-blue)](#)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](#)
[![Plataforma](https://img.shields.io/badge/Plataforma-Windows-lightgrey)](#)

## � Características

- ✅ **Backup automático** de SQL Server, MySQL y PostgreSQL
- ✅ **Envío automático por FTP** con estructura organizacional
- ✅ **Interfaz gráfica** para configuración fácil
- ✅ **Servicio de Windows** para ejecución automática
- ✅ **Programación flexible** con expresiones Cron
- ✅ **Encriptación AES-256** para seguridad
- ✅ **Logs detallados** con rotación automática
- ✅ **Gestión completa** con scripts de instalación

## 🏗️ Arquitectura

### Proyectos
- **BackupCore**: Librería principal con servicios y modelos
- **BackupService**: Servicio de Windows para automatización
- **BackupConfigurator**: Interfaz gráfica WinForms

### Tecnologías
- .NET 9.0
- Quartz.NET (programación con conversión automática de Cron)
- FluentFTP (transferencia FTP verificada)
- PostgreSQL 17 (con pg_dump funcional)
- AES-256 (encriptación)
- Windows Services (con corrección de errores)

## 🚀 Instalación Rápida

### 📋 Pasos Secuenciales (IMPORTANTE: Seguir en orden)

1. **📦 Compilar** la solución completa
2. **⚙️ Ejecutar** ACTUALIZACION-FINAL.bat  
3. **🔧 Configurar** usando BackupConfigurator.exe
4. **✅ Verificar** con DIAGNOSTICO-BACKUP.bat

### 1. Compilar la Solución (REQUERIDO PRIMERO)
**Antes de ejecutar cualquier script, compile la solución:**
```bash
# Abrir Developer Command Prompt o PowerShell en la carpeta del proyecto
dotnet build --configuration Release
```
**O usando Visual Studio:**
- Abrir `BackupAutomatico.sln`
- Ir a **Build** → **Build Solution** (Ctrl+Shift+B)
- Asegurar que compile sin errores

### 2. Instalar/Actualizar Servicio
**Después de compilar**, doble click en: `ACTUALIZACION-FINAL.bat`
- ✅ Desinstala versión anterior automáticamente
- ✅ Recompila con últimas correcciones  
- ✅ Reinstala servicio actualizado
- ✅ Requiere permisos de administrador

### 3. Gestionar Servicio (Opcional)
Usar: `GESTOR-SERVICIO.bat` para:
- Ver estado del servicio
- Iniciar/detener manualmente
- Verificar configuraciones
- Revisar logs

### 3. Configurar Backups
- Ejecutar `BackupConfigurator.exe` desde:
  `BackupConfigurator\bin\Release\net9.0-windows\`
- Configurar bases de datos y cliente FTP
- Crear programaciones de backup

### 4. Verificar Funcionamiento
Usar: `DIAGNOSTICO-BACKUP.bat` para:
- Verificar estado del servicio
- Ver configuraciones actuales
- Forzar backup manual
- Revisar logs del sistema

**Fecha de finalización:** 23 de Agosto, 2025  
**Estado:** ✅ **COMPLETAMENTE FUNCIONAL Y PROBADO**

## 🎯 Características Principales

- ✅ **Backup automático** de múltiples tipos de bases de datos (SQL Server, MySQL, PostgreSQL)
- ✅ **PostgreSQL 17** instalado y funcionando con `pg_dump`
- ✅ **Envío automático** por FTP con verificación de archivos subidos exitosamente
- ✅ **Conversión automática** de expresiones Cron (5 campos → 6 campos para Quartz.NET)
- ✅ **Programación flexible** con horarios configurables (ej: `30 01 * * *`)
- ✅ **Servicio Windows** que se ejecuta como demonio con corrección de errores
- ✅ **Scripts de gestión** automatizados (ACTUALIZACION-FINAL.bat, DIAGNOSTICO-BACKUP.bat)
- ✅ **Interfaz de configuración** intuitiva en WinForms completamente funcional
- ✅ **Sistema de logs** completo con historial y monitoreo
- ✅ **Seguridad avanzada** con encriptación AES-256 y proceso oculto
- ✅ **Compresión automática** de backups para optimizar espacio
- ✅ **Retención automática** de archivos antiguos
- ✅ **Configuración por sucursal** (SantaCruz/[NombreSucursal]/backups)
- ✅ **Backup manual probado** con confirmación de subida a FTP exitosa

## 🏗️ Arquitectura del Sistema

```
BackupAutomatico/
├── BackupCore/              # Librería principal con toda la lógica
│   ├── Models/              # Modelos de datos
│   ├── Interfaces/          # Interfaces de servicios
│   └── Services/            # Implementación de servicios
├── BackupService/           # Servicio Windows (demonio)
├── BackupConfigurator/      # Interfaz de configuración WinForms
└── README.md               # Documentación
```

## 🔧 Componentes Principales

### 📊 BackupCore (Librería Principal)

**Modelos:**
- `DatabaseConfig` - Configuración de bases de datos
- `FtpConfig` - Configuración de servidores FTP
- `BackupConfig` - Configuración de tareas de backup
- `BackupLog` - Registro de actividades

**Servicios:**
- `DatabaseBackupService` - Creación de backups
- `FtpService` - Gestión de transferencias FTP
- `ConfigurationService` - Gestión de configuraciones
- `BackupOrchestrator` - Orquestador principal
- `SecurityService` - Seguridad y encriptación
- `BackupLogService` - Sistema de logs

### 🛡️ BackupService (Servicio Windows)

- Ejecuta como servicio Windows oculto
- Programación automática usando Quartz.NET
- Monitoreo continuo cada 5 minutos
- Mantenimiento automático de logs
- Ocultación del proceso para mayor seguridad

### 🖥️ BackupConfigurator (Interfaz de Usuario)

- Interfaz gráfica para configuración completa
- Gestión de bases de datos, servidores FTP y backups
- Visualización de logs y estado del sistema
- Pruebas de conexión integradas

## ⚙️ Instalación y Configuración

### 1. Requisitos del Sistema

- Windows 10/11 o Windows Server 2016+
- .NET 9.0 Runtime
- PostgreSQL 17 (se instala automáticamente si no está presente)
- Acceso de administrador para instalación del servicio
- Conexión FTP válida para pruebas

### 2. Instalación Automática (Recomendado)

**Paso 1: Compilar la solución**
```bash
# Desde la carpeta raíz del proyecto
dotnet build --configuration Release
```

**Paso 2: Ejecutar instalación**
```bash
# Ejecutar como administrador
ACTUALIZACION-FINAL.bat
```

Este script automáticamente:
- ✅ Desinstala cualquier versión anterior
- ✅ Recompila el proyecto con las últimas correcciones
- ✅ Instala el servicio actualizado
- ✅ Configura el PATH de PostgreSQL

**⚠️ Importante:** Asegúrese de que la solución compile sin errores antes de ejecutar el script.

### 3. Instalación Manual del Servicio (Alternativa)

```bash
# Compilar el proyecto
dotnet build --configuration Release

# Instalar servicio
sc create "BackupAutomaticoService" binPath="C:\path\to\BackupService.exe"
sc config "BackupAutomaticoService" start=auto
sc start "BackupAutomaticoService"
```

### 4. Verificación de la Instalación

```bash
# Ejecutar diagnóstico completo
DIAGNOSTICO-BACKUP.bat
```

### 5. Configuración de PostgreSQL

Si `pg_dump` no está disponible:
```bash
# Instalar PostgreSQL 17
winget install PostgreSQL.PostgreSQL.17

# Agregar al PATH del sistema (como administrador)
setx PATH "%PATH%;C:\Program Files\PostgreSQL\17\bin" /M
```

### 4. Configuración Inicial

**⚠️ Prerrequisito:** Asegúrese de haber compilado la solución y ejecutado `ACTUALIZACION-FINAL.bat` primero.

1. **Ejecutar el Configurador:**
   ```bash
   # Desde la carpeta compilada (después de build)
   BackupConfigurator\bin\Release\net9.0-windows\BackupConfigurator.exe
   ```

2. **Configurar Base de Datos PostgreSQL:**
   - Ir a la pestaña "Bases de Datos"
   - Hacer clic en "Agregar Base de Datos"
   - Completar información:
     - Tipo: PostgreSQL
     - Servidor: localhost (o IP del servidor)
     - Puerto: 5432
     - Base de datos: nombre_bd
     - Usuario y contraseña
   - Probar la conexión (debe ser exitosa)

3. **Configurar Servidor FTP:**
   - Ir a la pestaña "Servidores FTP"  
   - Hacer clic en "Agregar Servidor FTP"
   - Configurar:
     - Host: ftp.tipingsoft.mx (o tu servidor)
     - Puerto: 21
     - Usuario y contraseña FTP
     - Carpeta base: SantaCruz
     - Carpeta sucursal: NombreDeTuSucursal
   - Probar conexión FTP

4. **Crear Configuración de Backup:**
   - Ir a la pestaña "Configuraciones de Backup"
   - Hacer clic en "Crear Configuración de Backup"
   - Seleccionar base de datos y servidor FTP configurados
   - Configurar horario: `30 01 * * *` (1:30 AM diario)
   - Configurar retención: 3 días
   - Habilitar compresión
   - Guardar configuración

5. **Verificar Funcionamiento:**
   ```bash
   # Ejecutar diagnóstico y backup manual
   DIAGNOSTICO-BACKUP.bat
   ```

## 📁 Estructura de Carpetas en FTP

El sistema crea automáticamente la siguiente estructura:

```
[BasePath]/
└── SantaCruz/                  # Carpeta principal configurable
    └── [NombreSucursal]/       # Carpeta de sucursal configurable
        └── backups/            # Carpeta de backups
            ├── database1_backup_20250822_140000.bak
            ├── database2_backup_20250822_140000.sql.gz
            └── ...
```

## 🔐 Características de Seguridad

### Encriptación
- **Contraseñas:** Encriptadas con AES-256
- **Archivos de configuración:** Encriptados localmente
- **Clave basada en hardware:** Utiliza características del sistema

### Ocultación del Proceso
- **Proceso oculto:** No visible en Task Manager estándar
- **Nombre de proceso:** Disfrazado como servicio del sistema
- **Ventana oculta:** Sin interfaz visible durante ejecución

### Protección de Datos
- **Configuraciones seguras:** Almacenadas en AppData del usuario
- **Logs protegidos:** Acceso restringido
- **Comunicación FTP:** Soporte para SSL/TLS

## 📋 Configuración de Bases de Datos

### SQL Server
```json
{
  "Type": "SqlServer",
  "ConnectionString": "Server=localhost;Database=MiDB;Integrated Security=true;",
  "Server": "localhost",
  "Database": "MiDB",
  "Username": "",
  "Password": ""
}
```

### MySQL
```json
{
  "Type": "MySQL",
  "Server": "localhost",
  "Port": 3306,
  "Database": "midb",
  "Username": "user",
  "Password": "password"
}
```

### PostgreSQL
```json
{
  "Type": "PostgreSQL", 
  "Server": "localhost",
  "Port": 5432,
  "Database": "midb",
  "Username": "user",
  "Password": "password"
}
```

## 🕐 Programación de Tareas

### Configuración de Horarios
- **Formato:** Expresión Cron de 5 campos (`minuto hora día mes día_semana`)
- **Ejemplo:** `30 01 * * *` para backup diario a la 1:30 AM
- **Conversión automática:** El sistema convierte a formato Quartz.NET (6 campos)
- **Flexibilidad:** Cada configuración puede tener su propio horario

### Conversión Automática de Cron
El sistema incluye `ConvertToQuartzCron()` que convierte:
- **Entrada:** `30 01 * * *` (5 campos)
- **Salida:** `0 30 01 * * ?` (6 campos para Quartz.NET)
- **Automático:** Sin necesidad de configuración manual

### Ejemplos de Programación
```bash
# Diario a la 1:30 AM
30 01 * * *

# Cada día a las 2:15 AM  
15 02 * * *

# Solo lunes a viernes a las 11:30 PM
30 23 * * 1-5
```

### Automatización
- **Verificación:** Cada 5 minutos
- **Ejecución:** Automática según horario configurado
- **Reintento:** Manejo de errores con logging detallado
- **Próxima ejecución:** Se calcula automáticamente

## 📊 Sistema de Logs

### Tipos de Log
- **Exitoso:** Backup completado correctamente
- **Error:** Fallos en el proceso
- **En ejecución:** Backups en progreso
- **Cancelado:** Backups interrumpidos

### Información Registrada
- Fecha y hora de inicio/fin
- Base de datos y servidor FTP utilizados
- Tamaño del archivo generado
- Duración del proceso
- Mensajes de error detallados
- Ruta del archivo en el servidor FTP

### Limpieza Automática
- **Retención de logs:** 90 días por defecto
- **Limpieza automática:** Diaria a las 3:00 AM
- **Archivos mensuales:** Organizados por mes

## 🔧 Mantenimiento

### Limpieza Automática
- **Backups antiguos:** Eliminación según días de retención configurados
- **Logs antiguos:** Limpieza automática de registros
- **Archivos temporales:** Eliminación después de cada backup

### Monitoreo
- **Estado del servicio:** Verificación automática
- **Espacio en disco:** Monitoreo del directorio temporal
- **Conexiones:** Validación periódica de conectividad

## 🚨 Solución de Problemas

### Herramientas de Diagnóstico

**1. Diagnóstico Automático:**
```bash
# Ejecutar diagnóstico completo
DIAGNOSTICO-BACKUP.bat
```
Esto verifica:
- Estado del servicio
- Configuraciones actuales
- Logs del sistema
- Permite backup manual

**2. Actualización del Servicio:**
```bash
# Si hay errores, actualizar servicio
ACTUALIZACION-FINAL.bat
```

### Problemas Comunes Resueltos

**1. Error: `pg_dump not found`**
- ✅ **Solución implementada:** Instalación automática de PostgreSQL 17
- ✅ **PATH configurado:** `C:\Program Files\PostgreSQL\17\bin`
- ✅ **Verificación:** `pg_dump --version`

**2. Error: `TaskCanceledException`**
- ✅ **Solución implementada:** Corrección en `Worker.cs` línea 65
- ✅ **Conversión Cron:** Automática de 5 a 6 campos
- ✅ **Reinicio limpio:** Sin conflictos de archivos bloqueados

**3. Servicio no actualiza:**
- ✅ **Solución:** Usar `ACTUALIZACION-FINAL.bat`
- ✅ **Proceso:** Desinstala → Recompila → Reinstala
- ✅ **Automático:** Sin intervención manual

**4. Logs no se generan:**
- ✅ **Ubicación verificada:** `%APPDATA%\BackupAutomatico\Logs\`
- ✅ **Escritura confirmada:** Sistema de logs funcional
- ✅ **Formato JSON:** Logs estructurados

### Verificación de Funcionamiento

**Backup manual exitoso confirmado:**
- ✅ **Archivo generado:** `Eco-Game_backup_20250823_031244.sql`
- ✅ **Subida FTP:** Confirmada en servidor remoto de prueba
- ✅ **Fecha actualizada:** `23/08/2025 03:12:47 a.m.`
- ✅ **Estado:** Exitoso

### Logs de Diagnóstico

**Ubicación de logs:**
```
%APPDATA%\BackupAutomatico\Logs\
├── backup_log_202508.json
├── backup_log_202509.json
└── ...
```

**Eventos del sistema:**
```bash
# Ver logs del servicio
eventvwr.msc -> Windows Logs -> Application
# Buscar eventos de "BackupService"
```

**Ubicación de configuraciones:**
```
%APPDATA%\BackupAutomatico\Config\
├── databases.json (encriptado)
├── ftp.json (encriptado)
└── backups.json
```

## 🎛️ Configuración Avanzada

### Variables de Entorno
```bash
# Cambiar directorio de configuración
BACKUP_CONFIG_PATH=C:\CustomPath\Config

# Cambiar directorio temporal
BACKUP_TEMP_PATH=C:\CustomPath\Temp

# Nivel de logging
BACKUP_LOG_LEVEL=Debug
```

### Parámetros del Servicio
```xml
<!-- BackupService.exe.config -->
<appSettings>
  <add key="CheckIntervalMinutes" value="5" />
  <add key="MaintenanceHour" value="3" />
  <add key="LogRetentionDays" value="90" />
  <add key="DefaultCompressionEnabled" value="true" />
</appSettings>
```

## 📞 Soporte y Contacto

### Información del Sistema
- **Versión:** 1.0.0
- **Plataforma:** Windows (.NET 8.0)
- **Licencia:** Propietaria

### Características del Servicio al Cliente
- ✅ Configuración inicial incluida
- ✅ Capacitación en el uso del sistema
- ✅ Soporte técnico durante implementación
- ✅ Personalización según necesidades específicas
- ✅ Actualizaciones y mantenimiento
- ✅ Monitoreo remoto opcional

---

## 🏆 Ventajas Competitivas

1. **Seguridad Máxima:** Proceso oculto y encriptación avanzada
2. **Facilidad de Uso:** Interfaz intuitiva para configuración
3. **Flexibilidad Total:** Soporte para múltiples tipos de BD
4. **Automatización Completa:** Sin intervención manual requerida
5. **Estructura Organizacional:** Carpetas por sucursal automáticas
6. **Confiabilidad:** Sistema robusto con manejo de errores
7. **Escalabilidad:** Soporte para múltiples configuraciones simultáneas

**¡Su solución completa para backups automatizados seguros y confiables!** 🛡️

---

## 📋 Estado Actual del Proyecto

### ✅ Completado y Probado (23 Agosto 2025)

1. **✅ Sistema Base:** Arquitectura completa implementada
2. **✅ Interfaces:** BackupConfigurator funcionando correctamente
3. **✅ Servicio Windows:** Instalado y ejecutándose sin errores
4. **✅ PostgreSQL:** Versión 17 instalada, `pg_dump` funcional
5. **✅ Conversión Cron:** Algoritmo automático 5→6 campos implementado
6. **✅ FTP Upload:** Confirmado con archivo `Eco-Game_backup_20250823_031244.sql`
7. **✅ Scripts Automatizados:** `ACTUALIZACION-FINAL.bat` y `DIAGNOSTICO-BACKUP.bat`
8. **✅ Backup Manual:** Probado exitosamente desde aplicación de prueba
9. **✅ Programación:** Configurado para 1:30 AM diario (`30 01 * * *`)
10. **✅ Logging:** Sistema de logs funcionando correctamente

### 🎯 Pruebas Realizadas

- **Conexión BD:** ✅ PostgreSQL conecta correctamente
- **Generación Backup:** ✅ `pg_dump` crea archivo SQL
- **Subida FTP:** ✅ Archivo confirmado en servidor remoto
- **Servicio Windows:** ✅ Ejecuta sin TaskCanceledException
- **Conversión Cron:** ✅ `30 01 * * *` → `0 30 01 * * ?`
- **Configuración:** ✅ Interfaz guarda y carga datos encriptados
- **Programación:** ✅ Próxima ejecución calculada correctamente

### 🚀 Listo para Producción

El sistema está **100% funcional** y listo para:
- ✅ Backups automáticos diarios
- ✅ Subida automática a FTP
- ✅ Gestión de retención de archivos
- ✅ Operación desatendida 24/7
- ✅ Monitoreo y logs detallados
