param([switch]$Uninstall)

$projectFilePath = Join-Path $PSScriptRoot "./App/App.csproj"
[regex]$versionRegex = "<PackageVersion>(?<PackageVersion>.*)<\/PackageVersion>"
[regex]$packageIdRegex = "<PackageId>(?<PackageId>.*)<\/PackageId>"

function GetFromFile() {
   param([string]$FilePath, [regex]$Pattern, [string]$RegexGroupName)

   $fileData = Get-Content $FilePath
   $foundLines = $fileData | Select-String -Pattern $Pattern -AllMatches

   if ($foundLines.Length -gt 1) {
      Write-Error "Project file contains more then one $RegexGroupName value"
      exit 1
   }
   if ($foundLines.Length -eq 0) {
      Write-Error "Project file doesn't contain any $RegexGroupName value"
   }

   return $foundLines.Matches[0].Groups[$RegexGroupName].Value
}

$version = GetFromFile $projectFilePath $versionRegex "PackageVersion"
$packageId = GetFromFile $projectFilePath $packageIdRegex "PackageId"

"Building app and creating package"

dotnet pack --configuration Release

"Checking if tool is installed"

$toolInstalled = (dotnet tool list --global | Where-Object { $_.Split(" ")[0] -eq $packageId })

if ($null -eq $toolInstalled) {
   "Tool is not installed"

   if (-Not $Uninstall) {
      "Installing tool"
      dotnet tool install --global --add-source "./Package" $packageId --version $version
   }
}
else {
   "Tool is installed"
   if (-Not $Uninstall) {
      "Updating tool"
      dotnet tool update --global --add-source "./Package" $packageId --version $version
   } else {
      "Uninstalling tool"
      dotnet tool uninstall --global $packageId
   }
}