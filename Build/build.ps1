param([string]$assemblyVersion, [string]$nugetVersion)

if ([string]::IsNullOrEmpty($assemblyVersion)) {$assemblyVersion = "0.0.1"}
if ([string]::IsNullOrEmpty($nugetVersion)) {$nugetVersion = "0.0.1"}

$vswhere = ${Env:\ProgramFiles(x86)} + '\Microsoft Visual Studio\Installer\vswhere'
$msbuild = &$vswhere -products * -requires Microsoft.Component.MSBuild -latest -find MSBuild\**\Bin\MSBuild.exe

Write-Host
Write-Host "Assembly version = $assemblyVersion"
Write-Host "Nuget version    = $nugetVersion"
Write-Host "MSBuild path     = $msbuild"
Write-Host

# Cleanup
Get-ChildItem -Path ".\..\" -Filter "*.nupkg" -Recurse | Remove-Item
Get-ChildItem -Path ".\..\" -Filter "*.snupkg" -Recurse | Remove-Item

$certificate = ".\..\..\Build\codeSigningKey.pfx"

# Build the DTLS extension
&$msbuild ..\Source\CoAPnet.Extensions.DTLS\CoAPnet.Extensions.DTLS.csproj /t:Clean /t:Restore /t:Build /p:Configuration="Release" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /p:PackageVersion=$nugetVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=$certificate

# Build the core library
&$msbuild ..\Source\CoAPnet\CoAPnet.csproj /t:Clean /t:Restore /t:Build /p:Configuration="Release" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /p:PackageVersion=$nugetVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=$certificate

# Build and execute tests
&$msbuild ..\Source\CoAPnet.Tests\CoAPnet.Tests.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="net6.0" /verbosity:m

vstest.console.exe ..\Source\CoAPnet.Tests\bin\Release\net6.0\CoAPnet.Tests.dll