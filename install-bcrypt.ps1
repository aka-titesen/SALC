# Script de PowerShell para instalar BCrypt.Net-Next
# Ejecutar desde la carpeta raíz del repositorio SALC

Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "  SALC - Instalador de BCrypt.Net-Next" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si NuGet está disponible
$nugetPath = "nuget.exe"
if (-not (Get-Command $nugetPath -ErrorAction SilentlyContinue)) {
    Write-Host "??  NuGet.exe no encontrado en PATH" -ForegroundColor Yellow
    Write-Host "Descargando NuGet.exe..." -ForegroundColor Yellow
    
    $nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $nugetPath = ".\nuget.exe"
    
    try {
        Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetPath
        Write-Host "? NuGet.exe descargado correctamente" -ForegroundColor Green
    }
    catch {
        Write-Host "? Error al descargar NuGet.exe: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Instalación manual requerida:" -ForegroundColor Yellow
        Write-Host "1. Abra Visual Studio" -ForegroundColor White
        Write-Host "2. Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution" -ForegroundColor White
        Write-Host "3. Busque 'BCrypt.Net-Next'" -ForegroundColor White
        Write-Host "4. Instale la versión 4.0.3 o superior" -ForegroundColor White
        exit 1
    }
}

Write-Host ""
Write-Host "Instalando BCrypt.Net-Next..." -ForegroundColor Cyan

# Instalar el paquete
try {
    & $nugetPath install BCrypt.Net-Next -Version 4.0.3 -OutputDirectory packages
    
    Write-Host ""
    Write-Host "? BCrypt.Net-Next instalado correctamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "Pasos siguientes:" -ForegroundColor Cyan
    Write-Host "1. Abra la solución SALC.sln en Visual Studio" -ForegroundColor White
    Write-Host "2. Recompile la solución (Ctrl + Shift + B)" -ForegroundColor White
    Write-Host "3. Ejecute la aplicación (F5)" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Documentación: SALC/Docs/MIGRACION_CONTRASEÑAS.md" -ForegroundColor Cyan
}
catch {
    Write-Host "? Error al instalar BCrypt.Net-Next: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Instalación manual requerida:" -ForegroundColor Yellow
    Write-Host "1. Abra Visual Studio" -ForegroundColor White
    Write-Host "2. Tools ? NuGet Package Manager ? Manage NuGet Packages for Solution" -ForegroundColor White
    Write-Host "3. Busque 'BCrypt.Net-Next'" -ForegroundColor White
    Write-Host "4. Instale la versión 4.0.3 o superior" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "  Instalación completada" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
