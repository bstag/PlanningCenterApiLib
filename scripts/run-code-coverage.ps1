# PowerShell script to run code coverage analysis for Planning Center SDK

param(
    [string]$OutputDir = "TestResults",
    [string]$ReportDir = "CoverageReport",
    [int]$TargetCoverage = 80
)

Write-Host "🧪 Planning Center SDK - Code Coverage Analysis" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""

# Clean previous results
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
    Write-Host "🧹 Cleaned previous test results" -ForegroundColor Yellow
}

if (Test-Path $ReportDir) {
    Remove-Item $ReportDir -Recurse -Force
    Write-Host "🧹 Cleaned previous coverage reports" -ForegroundColor Yellow
}

Write-Host ""

# Build the solution
Write-Host "🔨 Building solution..." -ForegroundColor Cyan
dotnet build src --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful" -ForegroundColor Green
Write-Host ""

# Run tests with coverage
Write-Host "🧪 Running tests with code coverage..." -ForegroundColor Cyan
dotnet test src/PlanningCenter.Api.Client.Tests/PlanningCenter.Api.Client.Tests.csproj `
    --configuration Release `
    --no-build `
    --collect:"XPlat Code Coverage" `
    --results-directory:$OutputDir `
    --logger:"console;verbosity=normal" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Tests completed successfully" -ForegroundColor Green
Write-Host ""

# Find the coverage file
$coverageFile = Get-ChildItem -Path $OutputDir -Recurse -Filter "coverage.opencover.xml" | Select-Object -First 1

if (-not $coverageFile) {
    Write-Host "❌ Coverage file not found" -ForegroundColor Red
    exit 1
}

Write-Host "📊 Coverage file found: $($coverageFile.FullName)" -ForegroundColor Cyan
Write-Host ""

# Install ReportGenerator if not available
Write-Host "🔧 Checking ReportGenerator..." -ForegroundColor Cyan
$reportGenerator = Get-Command "reportgenerator" -ErrorAction SilentlyContinue
if (-not $reportGenerator) {
    Write-Host "📦 Installing ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to install ReportGenerator" -ForegroundColor Red
        exit 1
    }
}

# Generate HTML report
Write-Host "📈 Generating coverage report..." -ForegroundColor Cyan
reportgenerator `
    -reports:$coverageFile.FullName `
    -targetdir:$ReportDir `
    -reporttypes:"Html;TextSummary;Badges" `
    -assemblyfilters:"+PlanningCenter.Api.Client*" `
    -classfilters:"-*.Tests*;-*.TestUtilities*"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to generate coverage report" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Coverage report generated" -ForegroundColor Green
Write-Host ""

# Parse coverage summary
$summaryFile = Join-Path $ReportDir "Summary.txt"
if (Test-Path $summaryFile) {
    Write-Host "📊 Coverage Summary:" -ForegroundColor Green
    Write-Host "===================" -ForegroundColor Green
    
    $summaryContent = Get-Content $summaryFile
    $lineCoverage = $summaryContent | Where-Object { $_ -match "Line coverage:" } | Select-Object -First 1
    $branchCoverage = $summaryContent | Where-Object { $_ -match "Branch coverage:" } | Select-Object -First 1
    
    if ($lineCoverage) {
        Write-Host $lineCoverage -ForegroundColor White
        
        # Extract percentage
        if ($lineCoverage -match "(\d+\.?\d*)%") {
            $coveragePercent = [double]$matches[1]
            
            if ($coveragePercent -ge $TargetCoverage) {
                Write-Host "🎉 Target coverage of $TargetCoverage% achieved!" -ForegroundColor Green
            } else {
                $gap = $TargetCoverage - $coveragePercent
                Write-Host "⚠️ Coverage gap: $([math]::Round($gap, 2))% below target of $TargetCoverage%" -ForegroundColor Yellow
            }
        }
    }
    
    if ($branchCoverage) {
        Write-Host $branchCoverage -ForegroundColor White
    }
    
    Write-Host ""
    
    # Show detailed coverage by assembly
    Write-Host "📋 Detailed Coverage by Assembly:" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    
    $detailLines = $summaryContent | Where-Object { $_ -match "PlanningCenter\.Api\.Client" -and $_ -match "\d+\.?\d*%" }
    foreach ($line in $detailLines) {
        if ($line -match "(\d+\.?\d*)%") {
            $percent = [double]$matches[1]
            $color = if ($percent -ge 80) { "Green" } elseif ($percent -ge 60) { "Yellow" } else { "Red" }
            Write-Host $line -ForegroundColor $color
        } else {
            Write-Host $line -ForegroundColor White
        }
    }
}

Write-Host ""
Write-Host "📁 Reports generated in: $ReportDir" -ForegroundColor Cyan
Write-Host "🌐 Open $ReportDir/index.html to view detailed coverage report" -ForegroundColor Cyan
Write-Host ""

# Show uncovered areas
Write-Host "🎯 Priority Areas for Additional Tests:" -ForegroundColor Yellow
Write-Host "=======================================" -ForegroundColor Yellow
Write-Host "1. ApiConnection - Core HTTP client functionality" -ForegroundColor White
Write-Host "2. ServiceBase - Base service error handling and validation" -ForegroundColor White
Write-Host "3. Authentication - OAuth and PAT authenticators" -ForegroundColor White
Write-Host "4. PlanningCenterClient - Main client orchestration" -ForegroundColor White
Write-Host "5. Exception Handling - GlobalExceptionHandler" -ForegroundColor White
Write-Host "6. Performance Monitoring - PerformanceMonitor" -ForegroundColor White
Write-Host "7. Fluent Query Builder - FluentQueryBuilder" -ForegroundColor White
Write-Host "8. Mappers - Complex mapping logic" -ForegroundColor White
Write-Host ""

Write-Host "✅ Code coverage analysis complete!" -ForegroundColor Green