# Script that renames a OMGG Template Package into a new name

param(
    [string]$NewName
)

$OldName    = "anti-afk"
$RootDir    = Get-Location
$ProjectDir = "$RootDir\$OldName"

if (-not $NewName) {
    Write-Host "Use: .\rename.ps1 NewName"
    exit
} if (-not (Test-Path $ProjectDir)) {
    Write-Host "Folder $OldName not found."
    exit
}

Write-Host "Renaming Unity project to: $NewName"

# 1. Renaming root folder
Rename-Item $ProjectDir "$NewName"

# 2. Modifying ProjectSettings
$SettingsFile = "$RootDir\$NewName\ProjectSettings\ProjectSettings.asset"

(Get-Content $SettingsFile) -replace $OldName, $NewName | Set-Content $SettingsFile

# 3. Renaming anti-afk and .meta
$PackageDir = "$RootDir\$NewName\Assets\OMGG\Package"

if (Test-Path "$PackageDir\anti-afk") {
    Rename-Item "$PackageDir\anti-afk" $NewName

    if (Test-Path "$PackageDir\anti-afk.meta") {
        Rename-Item "$PackageDir\anti-afk.meta" "$NewName.meta"
    }
}

# 4. Renaming .asmdef and .asmdef.meta
$AsmdefFile = "$PackageDir\$NewName\OMGG.anti-afk.asmdef"

if (Test-Path $AsmdefFile) {
    Rename-Item $AsmdefFile "OMGG.$NewName.asmdef"

    if (Test-Path "$AsmdefFile.meta") {
        Rename-Item "$AsmdefFile.meta" "OMGG.$NewName.asmdef.meta"
    }

    (Get-Content "$PackageDir\$NewName\OMGG.$NewName.asmdef") -replace "OMGG\.Package\.anti-afk", "OMGG.Package.$NewName" | Set-Content "$PackageDir\$NewName\OMGG.$NewName.asmdef"
}

# 5. Renaming $OldName to $NewName variables in .github/workflows
$WorkflowsDir = "$RootDir\.github\workflows"

if (Test-Path $WorkflowsDir) {
    Get-ChildItem $WorkflowsDir -Recurse -File | ForEach-Object {
        (Get-Content $_.FullName) -replace $OldName, $NewName | Set-Content $_.FullName
    }
}

# 6. Renaming $OldName to $NewName in rename.ps1 script
$RenameFile = "$RootDir\rename.ps1"

(Get-Content $RenameFile) -replace $OldName, $NewName | Set-Content $RenameFile
(Get-Content $RenameFile) -replace "anti-afk", $NewName | Set-Content $RenameFile

# 7. Renaming $OldName to $NewName in README.md
$ReadmeFile = "$RootDir\README.md"

if (Test-Path $ReadmeFile) {
    (Get-Content $ReadmeFile) -replace $OldName, $NewName | Set-Content $ReadmeFile
    (Get-Content $ReadmeFile) -replace "anti-afk", $NewName | Set-Content $ReadmeFile
}

# 8. Update Exporter.cs exportPath
$ExporterFile = "$RootDir\$NewName\Assets\Editor\Exporter.cs"

if (Test-Path $ExporterFile) {
    (Get-Content $ExporterFile) -replace "ExportedPackages/$OldName.unitypackage", "ExportedPackages/$NewName.unitypackage" | Set-Content $ExporterFile
    (Get-Content $ExporterFile) -replace "Assets/OMGG/Package/$OldName", "Assets/OMGG/Package/$NewName" | Set-Content $ExporterFile
}

Write-Host "Project successfully renamed to $NewName"
