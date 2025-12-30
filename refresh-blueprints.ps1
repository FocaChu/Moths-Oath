# Script para facilitar o rebuild e limpeza de cache ao adicionar blueprints
# Uso: .\refresh-blueprints.ps1

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  MothsOath - Rebuild e Refresh de Blueprints" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean
Write-Host "[1/3] Limpando build anterior..." -ForegroundColor Yellow
dotnet clean --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Erro no clean" -ForegroundColor Red
    exit 1
}
Write-Host "? Clean concluído" -ForegroundColor Green
Write-Host ""

# Step 2: Build
Write-Host "[2/3] Compilando e copiando blueprints..." -ForegroundColor Yellow
dotnet build --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Erro no build" -ForegroundColor Red
    exit 1
}
Write-Host "? Build concluído" -ForegroundColor Green
Write-Host ""

# Step 3: Instruções
Write-Host "[3/3] Próximos passos:" -ForegroundColor Yellow
Write-Host "  1. PARE a aplicação Blazor se estiver rodando" -ForegroundColor White
Write-Host "  2. Limpe o cache do navegador (Ctrl+Shift+Delete)" -ForegroundColor White
Write-Host "  3. REINICIE a aplicação completamente" -ForegroundColor White
Write-Host "  4. Faça um hard refresh no navegador (Ctrl+Shift+R ou Ctrl+F5)" -ForegroundColor White
Write-Host ""
Write-Host "? Blueprints atualizados em wwwroot!" -ForegroundColor Green
Write-Host ""

# Mostrar arquivos copiados
$blueprintCount = (Get-ChildItem -Path "MothsOath.BlazorUI\wwwroot\Data\Blueprints" -Recurse -File).Count
Write-Host "Total de arquivos de blueprints: $blueprintCount" -ForegroundColor Cyan
Write-Host ""
