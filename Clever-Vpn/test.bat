
@echo off
setlocal

REM 使用 PowerShell 解析 XML 并输出 Version
for /f "usebackq delims=" %%i in (`powershell -Command "$xml = [xml](Get-Content -Path 'Package.appxmanifest'); $xml.Package.Identity.Version"`) do set version=%%i

echo 版本号是: %version%

endlocal
pause