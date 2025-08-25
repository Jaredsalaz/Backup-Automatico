# Script para reiniciar el servicio con PostgreSQL configurado
Write-Host "=== REINICIO DEL SERVICIO DE BACKUP ===" -ForegroundColor Cyan

try {
    Write-Host "🔄 Deteniendo servicio BackupAutomaticoService..." -ForegroundColor Yellow
    Stop-Service "BackupAutomaticoService" -Force -ErrorAction Stop
    Write-Host "✅ Servicio detenido" -ForegroundColor Green
    
    Write-Host "🔄 Iniciando servicio BackupAutomaticoService..." -ForegroundColor Yellow
    Start-Service "BackupAutomaticoService" -ErrorAction Stop
    Write-Host "✅ Servicio iniciado" -ForegroundColor Green
    
    # Verificar estado
    $servicio = Get-Service "BackupAutomaticoService"
    Write-Host "📊 Estado del servicio: $($servicio.Status)" -ForegroundColor Blue
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "⚠️  Este script requiere permisos de administrador" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..."
Read-Host
