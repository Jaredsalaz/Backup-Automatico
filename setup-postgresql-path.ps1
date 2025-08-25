# Script para agregar PostgreSQL al PATH del sistema
Write-Host "=== CONFIGURACION DE POSTGRESQL PATH ===" -ForegroundColor Cyan

$postgresPath = "C:\Program Files\PostgreSQL\17\bin"

# Verificar que existe la ruta
if (Test-Path $postgresPath) {
    Write-Host "PostgreSQL encontrado en: $postgresPath" -ForegroundColor Green
    
    # Agregar al PATH de la sesion actual
    $env:PATH += ";$postgresPath"
    Write-Host "PostgreSQL agregado al PATH de esta sesion" -ForegroundColor Green
    
    # Verificar que funciona
    Write-Host "Verificando pg_dump..." -ForegroundColor Blue
    try {
        $version = & pg_dump --version 2>&1
        Write-Host "VERSION: $version" -ForegroundColor Green
    } catch {
        Write-Host "Error ejecutando pg_dump: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "PostgreSQL no encontrado en: $postgresPath" -ForegroundColor Red
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..."
Read-Host
