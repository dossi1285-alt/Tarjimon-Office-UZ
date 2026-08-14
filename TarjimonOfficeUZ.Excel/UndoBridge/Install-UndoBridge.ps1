$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$basPath = Join-Path $root 'TarjimonOfficeUZ.UndoBridge.bas'
$xlamPath = Join-Path $root 'TarjimonOfficeUZ.UndoBridge.xlam'
$installDir = Join-Path $env:APPDATA 'Microsoft\AddIns'
$installedPath = Join-Path $installDir 'TarjimonOfficeUZ.UndoBridge.xlam'

if (-not (Test-Path $basPath)) {
    throw "VBA bridge fayli topilmadi: $basPath"
}

New-Item -ItemType Directory -Force -Path $installDir | Out-Null

$excel = $null
$workbook = $null

try {
    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    $excel.DisplayAlerts = $false

    $workbook = $excel.Workbooks.Add()

    try {
        $vbProject = $workbook.VBProject
        $vbProject.VBComponents.Import($basPath) | Out-Null
    }
    catch {
        throw @"
Excel VBA loyihasiga dasturiy kirish yopiq.

Excel'da quyidagini yoqing:
File -> Options -> Trust Center -> Trust Center Settings -> Macro Settings
-> Trust access to the VBA project object model.

Keyin Excel'ni to'liq yopib, ushbu skriptni yana ishga tushiring.

Texnik xabar: $($_.Exception.Message)
"@
    }

    if (Test-Path $xlamPath) {
        Remove-Item -Force $xlamPath
    }

    # 55 = xlOpenXMLAddIn (.xlam)
    $workbook.SaveAs($xlamPath, 55)
    $workbook.Close($false)
    $workbook = $null

    Copy-Item -Force $xlamPath $installedPath

    Write-Host "Undo bridge yaratildi:" -ForegroundColor Green
    Write-Host "  $xlamPath"
    Write-Host ""
    Write-Host "Excel AddIns papkasiga o'rnatildi:" -ForegroundColor Green
    Write-Host "  $installedPath"
    Write-Host ""
    Write-Host "Endi Excel'ni to'liq yopib qayta oching." -ForegroundColor Cyan
}
finally {
    if ($workbook -ne $null) {
        try { $workbook.Close($false) } catch {}
    }

    if ($excel -ne $null) {
        try { $excel.Quit() } catch {}
    }

    if ($workbook -ne $null) {
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($workbook)
    }

    if ($excel -ne $null) {
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel)
    }
}
