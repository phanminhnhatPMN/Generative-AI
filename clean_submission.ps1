# Script to clean bin and obj directories before submitting
Write-Host "Cleaning bin and obj folders..." -ForegroundColor Cyan

Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | ForEach-Object {
    Write-Host "Removing: $($_.FullName)" -ForegroundColor Yellow
    Remove-Item -Path $_.FullName -Recurse -Force
}

Write-Host "Done! Project is clean and ready for submission." -ForegroundColor Green

