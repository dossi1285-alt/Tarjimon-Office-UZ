# Tarjimon Office UZ — KL Office uz loading-source diagnostic
# Read-only diagnostic. It does not uninstall, disable, delete, or modify Office/add-ins.
# Run in Windows PowerShell on the test computer while Word/Excel are closed; the script opens them read-only.

$ErrorActionPreference = 'SilentlyContinue'
$Report = Join-Path ([Environment]::GetFolderPath('Desktop')) 'KL-Office-Diagnostic.txt'
$Lines = New-Object System.Collections.Generic.List[string]

function Add-Line([string]$Text = '') { [void]$Lines.Add($Text) }
function Add-Section([string]$Title) { Add-Line ''; Add-Line ('=' * 78); Add-Line $Title; Add-Line ('=' * 78) }
function Safe([scriptblock]$Block) { try { & $Block } catch { $null } }
function Registry-Addins([string]$HostName) {
    Add-Section "Registry: Office\\$HostName\\Addins"
    foreach ($view in @('Registry64','Registry32')) {
        foreach ($hive in @('LocalMachine','CurrentUser')) {
            $base = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::$hive, [Microsoft.Win32.RegistryView]::$view)
            if ($null -eq $base) { continue }
            $key = $base.OpenSubKey("SOFTWARE\Microsoft\Office\$HostName\Addins")
            if ($null -eq $key) { continue }
            foreach ($name in $key.GetSubKeyNames()) {
                $sub = $key.OpenSubKey($name)
                Add-Line "[$hive/$view] $name"
                foreach ($valueName in @('FriendlyName','Description','ProgId','Manifest','Assembly','LoadBehavior','CommandLineSafe','Connect','DllPath')) {
                    $v = $sub.GetValue($valueName, $null)
                    if ($null -ne $v) { Add-Line "  $valueName = $v" }
                }
            }
        }
    }
}

function Inspect-File([string]$Path, [string]$Source) {
    if ([string]::IsNullOrWhiteSpace($Path)) { return }
    $expanded = [Environment]::ExpandEnvironmentVariables($Path)
    if (-not [IO.File]::Exists($expanded)) { Add-Line "[$Source] FILE NOT FOUND: $expanded"; return }
    Add-Line "[$Source] FILE: $expanded"
    Safe { $i = [Diagnostics.FileVersionInfo]::GetVersionInfo($expanded); Add-Line "  ProductName=$($i.ProductName)"; Add-Line "  CompanyName=$($i.CompanyName)"; Add-Line "  FileDescription=$($i.FileDescription)"; Add-Line "  OriginalFilename=$($i.OriginalFilename)" }
    $ext = [IO.Path]::GetExtension($expanded)
    if ($ext -in @('.dotm','.dotx','.xlam','.xla')) {
        Safe {
            Add-Type -AssemblyName System.IO.Compression.FileSystem
            $zip = [IO.Compression.ZipFile]::OpenRead($expanded)
            foreach ($entry in $zip.Entries) {
                if ($entry.FullName -match '(?i)(^|/)customUI(14)?\.xml$') {
                    Add-Line "  RIBBON XML: $($entry.FullName)"
                    $reader = New-Object IO.StreamReader($entry.Open())
                    $xml = $reader.ReadToEnd(); $reader.Dispose()
                    foreach ($needle in @('KL Office','Print_Kito','Kirill','Lotin','Lotin → Kirill','Kirill → Lotin')) {
                        if ($xml -match [regex]::Escape($needle)) { Add-Line "  RIBBON MATCH: $needle" }
                    }
                    Add-Line "  Ribbon XML length=$($xml.Length)"
                }
            }
            $zip.Dispose()
        }
    }
}

function Dump-Word {
    Add-Section 'WORD runtime inspection'
    $word = New-Object -ComObject Word.Application
    $word.Visible = $false
    Add-Line "Word StartupPath = $($word.StartupPath)"
    Add-Line "NormalTemplate = $($word.NormalTemplate.FullName)"
    Safe { Add-Line "UserTemplatesPath = $($word.Options.DefaultFilePath(2))" }
    Safe { Add-Line "WorkgroupTemplatesPath = $($word.Options.DefaultFilePath(3))" }

    Add-Line ''
    Add-Line 'Word COMAddIns (currently available/registered to Word):'
    Safe {
        $word.COMAddIns.Update()
        for ($i=1; $i -le $word.COMAddIns.Count; $i++) {
            $c = $word.COMAddIns.Item($i)
            Add-Line "  ProgID=$($c.ProgID) | Description=$($c.Description) | Guid=$($c.Guid) | Connect=$($c.Connect)"
            Safe {
                $p = [Environment]::ExpandEnvironmentVariables((Get-ItemProperty -Path "Registry::HKEY_CLASSES_ROOT\$($c.ProgID)\CLSID" -Name '(default)').'(default)')
                if ($p) { Add-Line "    CLSID=$p" }
            }
        }
    }

    Add-Line ''
    Add-Line 'Word AddIns collection (templates/WLLs available to Word):'
    Safe {
        for ($i=1; $i -le $word.AddIns.Count; $i++) {
            $a = $word.AddIns.Item($i)
            Add-Line "  Name=$($a.Name) | Path=$($a.Path) | Installed=$($a.Installed) | Compiled=$($a.Compiled) | Autoload=$($a.Autoload)"
            Inspect-File (Join-Path $a.Path $a.Name) 'Word.AddIns'
        }
    }

    Add-Line ''
    Add-Line 'Word Templates collection:'
    Safe {
        for ($i=1; $i -le $word.Templates.Count; $i++) {
            $t = $word.Templates.Item($i)
            Add-Line "  Name=$($t.Name) | FullName=$($t.FullName)"
            if ($t.FullName -match '(?i)\.dotm$|\.dotx$|\.dot$') { Inspect-File $t.FullName 'Word.Template' }
        }
    }

    Add-Line ''
    Add-Line 'Word legacy CommandBars controls matching translator/ribbon captions:'
    Safe {
        foreach ($bar in $word.CommandBars) {
            foreach ($control in $bar.Controls) {
                $caption = [string]$control.Caption
                if ($caption -match '(?i)KL|Print.?Kito|Kirill|Kiril|Lotin|Latin|Tarjimon|Translator') {
                    Add-Line "  Bar=$($bar.Name) | Caption=$caption | Id=$($control.Id) | Tag=$($control.Tag) | OnAction=$($control.OnAction)"
                }
            }
        }
    }
    Safe { $word.Quit() }
    [Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}

function Dump-Excel {
    Add-Section 'EXCEL runtime inspection'
    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    Safe { Add-Line "Excel StartupPath = $($excel.Application.StartupPath)" }
    Add-Line ''
    Add-Line 'Excel COMAddIns:'
    Safe {
        $excel.COMAddIns.Update()
        for ($i=1; $i -le $excel.COMAddIns.Count; $i++) {
            $c = $excel.COMAddIns.Item($i)
            Add-Line "  ProgID=$($c.ProgID) | Description=$($c.Description) | Guid=$($c.Guid) | Connect=$($c.Connect)"
        }
    }
    Add-Line ''
    Add-Line 'Excel AddIns collection:'
    Safe {
        for ($i=1; $i -le $excel.AddIns.Count; $i++) {
            $a = $excel.AddIns.Item($i)
            Add-Line "  Name=$($a.Name) | FullName=$($a.FullName) | Installed=$($a.Installed) | ProgID=$($a.ProgID)"
            Inspect-File $a.FullName 'Excel.AddIn'
        }
    }
    Safe { $excel.Quit() }
    [Runtime.InteropServices.Marshal]::ReleaseComObject($excel) | Out-Null
}

Add-Section 'KL Office uz — read-only diagnostic'
Add-Line "Computer = $env:COMPUTERNAME"
Add-Line "User = $env:USERNAME"
Add-Line "Time = $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Add-Line 'IMPORTANT: This diagnostic only reads Office runtime objects, registry metadata and add-in/template files.'

Registry-Addins 'Word'
Registry-Addins 'Excel'
Safe { Dump-Word }
Safe { Dump-Excel }

Add-Section 'STARTUP/XLSTART filesystem snapshot'
$roots = @(
    (Join-Path $env:APPDATA 'Microsoft\Word\STARTUP'),
    (Join-Path $env:APPDATA 'Microsoft\Templates'),
    (Join-Path $env:APPDATA 'Microsoft\Excel\XLSTART')
)
foreach ($root in $roots) {
    Add-Line "PATH: $root"
    if (Test-Path $root) {
        Get-ChildItem -LiteralPath $root -File | ForEach-Object { Add-Line "  $($_.FullName)"; Inspect-File $_.FullName 'StartupFile' }
    } else { Add-Line '  NOT FOUND' }
}

$Lines | Set-Content -LiteralPath $Report -Encoding UTF8
Write-Host "Diagnostic finished: $Report" -ForegroundColor Green
Write-Host 'Open the report and send it here. Do not press Tasdiqlash and do not uninstall anything.' -ForegroundColor Yellow
Start-Process notepad.exe -ArgumentList @($Report)
