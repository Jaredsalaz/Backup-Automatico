# Git User Switcher Script
# Script para cambiar facilmente entre usuarios de Git

param(
    [string]$Action = "menu"
)

# Colores para output
$Red = "Red"
$Green = "Green"
$Yellow = "Yellow"
$Blue = "Cyan"
$White = "White"

# Configuraciones de usuarios predefinidas
$Users = @{
    "personal" = @{
        name = "Jaredsalaz"
        email = "tu.email.personal@ejemplo.com"
        description = "Cuenta personal"
    }
    "university" = @{
        name = "JaredUnach"
        email = "tu.email.universitario@ejemplo.edu"
        description = "Cuenta universitaria"
    }
    "work" = @{
        name = "Tu Nombre Profesional"
        email = "tu.email@empresa.com"
        description = "Cuenta de trabajo"
    }
}

function Show-Header {
    Clear-Host
    Write-Host ""
    Write-Host "================================" -ForegroundColor $Blue
    Write-Host "     GIT USER SWITCHER v1.0     " -ForegroundColor $Blue
    Write-Host "================================" -ForegroundColor $Blue
    Write-Host ""
}

function Show-CurrentUser {
    Write-Host "Configuracion actual de Git:" -ForegroundColor $Yellow
    Write-Host ""
    
    $currentName = git config --global user.name
    $currentEmail = git config --global user.email
    
    if ($currentName -and $currentEmail) {
        Write-Host "   Nombre: $currentName" -ForegroundColor $Green
        Write-Host "   Email:  $currentEmail" -ForegroundColor $Green
        
        # Verificar si coincide con algun perfil conocido
        foreach ($profileKey in $Users.Keys) {
            $user = $Users[$profileKey]
            if ($user.name -eq $currentName -and $user.email -eq $currentEmail) {
                Write-Host "   Perfil: $profileKey ($($user.description))" -ForegroundColor $Blue
                break
            }
        }
    } else {
        Write-Host "   No hay configuracion de usuario establecida" -ForegroundColor $Red
    }
    Write-Host ""
}

function Show-Menu {
    Show-Header
    Show-CurrentUser
    
    Write-Host "MENU DE OPCIONES:" -ForegroundColor $Yellow
    Write-Host ""
    Write-Host "1. Cambiar a perfil PERSONAL" -ForegroundColor $Green
    Write-Host "   ($($Users.personal.name) - $($Users.personal.email))" -ForegroundColor $White
    Write-Host ""
    Write-Host "2. Cambiar a perfil UNIVERSITARIO" -ForegroundColor $Green  
    Write-Host "   ($($Users.university.name) - $($Users.university.email))" -ForegroundColor $White
    Write-Host ""
    Write-Host "3. Cambiar a perfil TRABAJO" -ForegroundColor $Green
    Write-Host "   ($($Users.work.name) - $($Users.work.email))" -ForegroundColor $White
    Write-Host ""
    Write-Host "4. Ver configuracion actual" -ForegroundColor $Green
    Write-Host ""
    Write-Host "5. Salir" -ForegroundColor $Red
    Write-Host ""
    
    $choice = Read-Host "Selecciona una opcion (1-5)"
    return $choice
}

function Set-GitUser {
    param(
        [string]$ProfileName
    )
    
    if (-not $Users.ContainsKey($ProfileName)) {
        Write-Host "Error: Perfil '$ProfileName' no encontrado" -ForegroundColor $Red
        Write-Host ""
        return
    }
    
    $user = $Users[$ProfileName]
    
    Write-Host ""
    Write-Host "Cambiando a perfil: $ProfileName" -ForegroundColor $Yellow
    Write-Host ""
    
    try {
        git config --global user.name $user.name
        git config --global user.email $user.email
        
        Write-Host "Usuario cambiado exitosamente:" -ForegroundColor $Green
        Write-Host "   Nombre: $($user.name)" -ForegroundColor $White
        Write-Host "   Email:  $($user.email)" -ForegroundColor $White
        Write-Host "   Desc:   $($user.description)" -ForegroundColor $White
        Write-Host ""
        
        # Verificar cambio
        $newName = git config --global user.name
        $newEmail = git config --global user.email
        
        if ($newName -eq $user.name -and $newEmail -eq $user.email) {
            Write-Host "Cambio verificado correctamente" -ForegroundColor $Green
        } else {
            Write-Host "Advertencia: El cambio podria no haberse aplicado correctamente" -ForegroundColor $Yellow
        }
        
    } catch {
        Write-Host "Error al cambiar usuario: $($_.Exception.Message)" -ForegroundColor $Red
    }
    
    Write-Host ""
    Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor $Blue
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Funcion principal con menu interactivo
function Main {
    do {
        $choice = Show-Menu
        
        switch ($choice) {
            "1" {
                Set-GitUser -ProfileName "personal"
            }
            "2" {
                Set-GitUser -ProfileName "university"
            }
            "3" {
                Set-GitUser -ProfileName "work"
            }
            "4" {
                Show-Header
                Show-CurrentUser
                Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor $Blue
                $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            }
            "5" {
                Write-Host ""
                Write-Host "Saliendo del programa..." -ForegroundColor $Yellow
                Write-Host "Gracias por usar Git User Switcher!" -ForegroundColor $Green
                return
            }
            default {
                Write-Host ""
                Write-Host "Opcion invalida. Por favor selecciona 1, 2, 3, 4 o 5." -ForegroundColor $Red
                Write-Host ""
                Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor $Blue
                $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            }
        }
    } while ($true)
}

# Ejecutar funcion principal
Main
