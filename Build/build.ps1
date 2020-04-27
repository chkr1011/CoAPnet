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

Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "nuget.exe"

.\nuget.exe restore ..\Source\CoAPnet.sln

# Build and execute tests
&$msbuild ..\Source\CoAPnet.Tests\CoAPnet.Tests.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="netcoreapp3.1" /verbosity:m

vstest.console.exe ..\Source\CoAPnet.Tests\bin\Release\netcoreapp3.1\CoAPnet.Tests.dll

# Build the core library
&$msbuild ..\Source\CoAPnet\CoAPnet.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="net452" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"
&$msbuild ..\Source\CoAPnet\CoAPnet.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="netstandard1.3" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"
&$msbuild ..\Source\CoAPnet\CoAPnet.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="netstandard2.0" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"

# Build the DTLS extension
&$msbuild ..\Source\CoAPnet.Extensions.DTLS\CoAPnet.Extensions.DTLS.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="net452" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"
&$msbuild ..\Source\CoAPnet.Extensions.DTLS\CoAPnet.Extensions.DTLS.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="netstandard1.3" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"
&$msbuild ..\Source\CoAPnet.Extensions.DTLS\CoAPnet.Extensions.DTLS.csproj /t:Build /p:Configuration="Release" /p:TargetFramework="netstandard2.0" /p:FileVersion=$assemblyVersion /p:AssemblyVersion=$assemblyVersion /verbosity:m /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=".\..\..\Build\codeSigningKey.pfx"

# Create NuGet packages.

Remove-Item .\NuGet -Force -Recurse -ErrorAction SilentlyContinue

Copy-Item CoAPnet.nuspec -Destination CoAPnet.nuspec.old -Force
(Get-Content CoAPnet.nuspec) -replace '\$nugetVersion', $nugetVersion | Set-Content CoAPnet.nuspec
Copy-Item CoAPnet.Extensions.DTLS.nuspec -Destination CoAPnet.Extensions.DTLS.nuspec.old -Force
(Get-Content CoAPnet.Extensions.DTLS.nuspec) -replace '\$nugetVersion', $nugetVersion | Set-Content CoAPnet.Extensions.DTLS.nuspec

New-Item -ItemType Directory -Force -Path .\NuGet
.\nuget.exe pack CoAPnet.nuspec -Verbosity detailed -Symbols -SymbolPackageFormat snupkg -OutputDir "NuGet" -Version $nugetVersion
.\nuget.exe pack CoAPnet.Extensions.DTLS.nuspec -Verbosity detailed -Symbols -SymbolPackageFormat snupkg -OutputDir "NuGet" -Version $nugetVersion

Move-Item CoAPnet.nuspec.old -Destination CoAPnet.nuspec -Force
Move-Item CoAPnet.Extensions.DTLS.nuspec.old -Destination CoAPnet.Extensions.DTLS.nuspec -Force

Remove-Item "nuget.exe" -Force -Recurse -ErrorAction SilentlyContinue