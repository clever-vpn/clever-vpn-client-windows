@echo off
setlocal enabledelayedexpansion

for /f "usebackq delims=" %%i in (`powershell -Command "$xml = [xml](Get-Content -Path 'Package.appxmanifest'); $xml.Package.Identity.Version"`) do set Version=%%i

rem === 设置目录路径 ===
set "MSIX_DIR=bin/Appx/Clever-Vpn_%Version%_Test"
set "BUNDLE_NAME=bin/Appx/bundle/clevervpn.msixbundle"

rem === 检查是否存在 makeappx.exe ===
where makeappx >nul 2>&1
if errorlevel 1 (
    echo [错误] 未找到 makeappx.exe，请确保 Windows SDK 已安装并在 PATH 中。
    exit /b 1
)

rem === 构建列表文件 ===
set "BUNDLE_LIST=bundlemap.txt"
del "%BUNDLE_LIST%" >nul 2>&1



rem === 遍历 MSIX 文件并生成映射 ===

echo [Files] > "%BUNDLE_LIST%"
for %%F in ("%MSIX_DIR%\*.msix") do (
    set "FULL=%%~fF"
    set "NAME=%%~nxF"
    echo "!FULL!" "!NAME!" >> "%BUNDLE_LIST%"
)


rem === 检查是否有文件 ===
if not exist "%BUNDLE_LIST%" (
    echo [错误] 目录 "%MSIX_DIR%" 中未找到任何 .msix 文件。
    exit /b 1
)

rem === 创建 Bundle ===
echo [信息] 开始打包 bundle 到 %BUNDLE_NAME% ...
makeappx bundle /f "%BUNDLE_LIST%" /p "%BUNDLE_NAME%"

if %ERRORLEVEL%==0 (
    echo [成功] 打包完成: %BUNDLE_NAME%
) else (
    echo [失败] 打包过程中发生错误。
)

endlocal
pause

