# Script to regenerate migrations for SQL Server
# Run this script from the solution root directory

Write-Host "Regenerating EF Core Migrations for SQL Server..." -ForegroundColor Cyan

# Save current location
$originalLocation = Get-Location

try {
    # Navigate to project directory
    Set-Location -Path "RazorPagesMovie"

    # Remove all existing migrations
    Write-Host "`nRemoving existing SQLite migrations..." -ForegroundColor Yellow
    if (Test-Path "Migrations") {
        Remove-Item -Path "Migrations" -Recurse -Force
        Write-Host "Migrations folder removed." -ForegroundColor Green
    }

    # Create fresh migration for SQL Server
    Write-Host "`nCreating fresh SQL Server migration..." -ForegroundColor Yellow
    $env:ASPNETCORE_ENVIRONMENT = "Production"
    dotnet ef migrations add InitialSqlServer --context RazorPagesMovieContext

    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nDone! Your migrations are now SQL Server compatible." -ForegroundColor Green
        Write-Host "`nNext steps:" -ForegroundColor Cyan
        Write-Host "1. Drop your Azure SQL Database tables with this SQL query:" -ForegroundColor White
        Write-Host "   DROP TABLE IF EXISTS [__EFMigrationsHistory];" -ForegroundColor Gray
        Write-Host "   DROP TABLE IF EXISTS [Documentary];" -ForegroundColor Gray
        Write-Host "   DROP TABLE IF EXISTS [Movie];" -ForegroundColor Gray
        Write-Host "`n2. Build and push with ACR Task:" -ForegroundColor White
        Write-Host "   az acr build --registry mltestacr6 --image razorpagesmovie:latest --image razorpagesmovie:$(Get-Date -Format 'yyyyMMdd-HHmmss') ." -ForegroundColor Gray
        Write-Host "`n3. Restart your K8s deployment:" -ForegroundColor White
        Write-Host "   kubectl rollout restart deployment razorpagesmovie" -ForegroundColor Gray
    } else {
        Write-Host "`nError creating migrations. Please check the output above." -ForegroundColor Red
    }
}
finally {
    # Return to original location
    Set-Location -Path $originalLocation
}
