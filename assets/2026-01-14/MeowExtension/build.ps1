# Build script for MeowExtension
# This script builds the project and creates MeowExtension.zip

Write-Host "Building MeowExtension..." -ForegroundColor Green

# Clean and build the project
dotnet clean Meow.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "Clean failed!" -ForegroundColor Red
    exit 1
}

dotnet build Meow.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Define paths
$outputDir = "bin\Debug\net8.0"
$binaryDir = "binary"
$zipFile = "MeowExtension.zip"
$zipPath = Join-Path $binaryDir $zipFile

# Check if output directory exists
if (-not (Test-Path $outputDir)) {
    Write-Host "Output directory not found: $outputDir" -ForegroundColor Red
    exit 1
}

# Always copy extension.xml from root to output directory (overwrite)
$rootExtensionXml = "extension.xml"
$outputExtensionXml = Join-Path $outputDir "extension.xml"

if (Test-Path $rootExtensionXml) {
    Copy-Item $rootExtensionXml $outputExtensionXml -Force
    Write-Host "Copied extension.xml from root to output directory" -ForegroundColor Green
} else {
    Write-Host "Warning: extension.xml not found in root directory" -ForegroundColor Yellow
}

# Create binary directory if it doesn't exist
if (-not (Test-Path $binaryDir)) {
    New-Item -ItemType Directory -Path $binaryDir -Force | Out-Null
    Write-Host "Created binary directory" -ForegroundColor Yellow
}

# Remove existing zip file if it exists
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
    Write-Host "Removed existing $zipFile" -ForegroundColor Yellow
}

# Create the zip file with required files
Write-Host "Creating $zipFile..." -ForegroundColor Green

# Files to include in the zip
$filesToZip = @(
    "$outputDir\Meow.dll",
    "$outputDir\Meow.pdb", 
    "$outputDir\extension.xml"
)

# Check if all required files exist
$missingFiles = @()
foreach ($file in $filesToZip) {
    if (-not (Test-Path $file)) {
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "Missing required files:" -ForegroundColor Red
    foreach ($file in $missingFiles) {
        Write-Host "  - $file" -ForegroundColor Red
    }
    exit 1
}

# Create the zip file
try {
    # Wait a moment for file handles to be released
    Start-Sleep -Milliseconds 500
    
    # Try to create zip, if it fails due to file lock, wait and try again
    $maxRetries = 3
    $retryCount = 0
    $zipCreated = $false
    
    while (-not $zipCreated -and $retryCount -lt $maxRetries) {
        try {
            Compress-Archive -Path $filesToZip -DestinationPath $zipPath -Force
            $zipCreated = $true
            Write-Host "Successfully created $zipPath" -ForegroundColor Green
        } catch {
            $retryCount++
            if ($retryCount -lt $maxRetries) {
                Write-Host "File locked, waiting and retrying... (attempt $retryCount of $maxRetries)" -ForegroundColor Yellow
                Start-Sleep -Milliseconds 1000
            } else {
                throw
            }
        }
    }
    
    # Show zip contents
    Write-Host "`nZip file contents:" -ForegroundColor Cyan
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $fullZipPath = Join-Path (Get-Location) $zipPath
    $zip = [System.IO.Compression.ZipFile]::OpenRead($fullZipPath)
    foreach ($entry in $zip.Entries) {
        Write-Host "  - $($entry.Name)" -ForegroundColor White
    }
    $zip.Dispose()
    
    # Show file sizes
    Write-Host "`nFile sizes:" -ForegroundColor Cyan
    foreach ($file in $filesToZip) {
        $size = (Get-Item $file).Length
        Write-Host "  - $(Split-Path $file -Leaf): $size bytes" -ForegroundColor White
    }
    
    $zipSize = (Get-Item $zipPath).Length
    Write-Host "  - ${zipFile}: $zipSize bytes" -ForegroundColor White
    
} catch {
    Write-Host "Failed to create zip file: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green
