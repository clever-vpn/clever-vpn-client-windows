@echo off
setlocal enabledelayedexpansion

set "Version=%~1"
if "%Version%"=="" (
    for /f "usebackq delims=" %%i in (`powershell -Command "$xml = [xml](Get-Content -Path 'Package.appxmanifest'); $xml.Package.Identity.Version"`) do set Version=%%i
)

if "%Version%"=="" (
    echo [错误] 无法解析版本号，请传入参数例如 make-bundle.bat 1.0.0.0
    exit /b 1
)

rem === 设置目录路径 ===
set "MSIX_DIR=%~2"
if "%MSIX_DIR%"=="" set "MSIX_DIR=bin/Appx/Clever-Vpn_%Version%_Test"
set "BUNDLE_NAME=bin/Appx/bundle/clevervpn.msixbundle"


rem === 检查是否存在 makeappx.exe ===
where makeappx >nul 2>&1
if errorlevel 1 (
    set "SDK_BIN_DIR="
    for /f "delims=" %%D in ('dir /b /ad "C:\Program Files (x86)\Windows Kits\10\bin" 2^>nul ^| sort /r') do (
        if exist "C:\Program Files (x86)\Windows Kits\10\bin\%%D\x64\makeappx.exe" (
            set "SDK_BIN_DIR=C:\Program Files (x86)\Windows Kits\10\bin\%%D\x64"
            goto :found_makeappx
        )
    )

    :found_makeappx
    if not defined SDK_BIN_DIR (
        echo [错误] 未找到 makeappx.exe，请确保 Windows SDK 已安装并在 PATH 中。
        exit /b 1
    )

    set "PATH=%SDK_BIN_DIR%;%PATH%"
)

rem === 自动创建 bundle 目录 ===
for %%D in ("%BUNDLE_NAME%") do set "BUNDLE_DIR=%%~dpD"
if not exist "%BUNDLE_DIR%" mkdir "%BUNDLE_DIR%"

rem === 构建列表文件（临时文件，脚本自动创建与清理） ===
set "BUNDLE_LIST=%TEMP%\clevervpn_bundlemap_%RANDOM%_%RANDOM%.txt"
del "%BUNDLE_LIST%" >nul 2>&1
set /a FILE_COUNT=0



rem === 遍历 MSIX 文件并生成映射 ===

echo [Files] > "%BUNDLE_LIST%"
for %%F in ("%MSIX_DIR%\*.msix") do (
    set "FULL=%%~fF"
    set "NAME=%%~nxF"
    echo "!FULL!" "!NAME!" >> "%BUNDLE_LIST%"
    set /a FILE_COUNT+=1
)

if !FILE_COUNT! LSS 1 (
    rem Fallback: 递归搜索 bin/Appx 下所有 msix（排除 bundle 目录）
    set "MSIX_DIR=bin/Appx"
    del "%BUNDLE_LIST%" >nul 2>&1
    echo [Files] > "%BUNDLE_LIST%"
    set /a FILE_COUNT=0
    for /r "%MSIX_DIR%" %%F in (*.msix) do (
        set "FULL=%%~fF"
        echo "!FULL!" | findstr /I /C:"\bundle\" >nul
        if errorlevel 1 (
            set "NAME=%%~nxF"
            echo "!FULL!" "!NAME!" >> "%BUNDLE_LIST%"
            set /a FILE_COUNT+=1
        )
    )
)


rem === 检查是否有文件 ===
if !FILE_COUNT! LSS 1 (
    echo [错误] 目录 "%MSIX_DIR%" 中未找到任何 .msix 文件。
    del "%BUNDLE_LIST%" >nul 2>&1
    exit /b 1
)

rem === 创建 Bundle ===
echo [信息] 开始打包 bundle 到 %BUNDLE_NAME% ...
makeappx bundle /f "%BUNDLE_LIST%" /p "%BUNDLE_NAME%"
set "BUNDLE_EXIT_CODE=%ERRORLEVEL%"

if %BUNDLE_EXIT_CODE%==0 (
    echo [成功] 打包完成: %BUNDLE_NAME%
) else (
    echo [失败] 打包过程中发生错误。
)

rem === 清理临时映射文件 ===
del "%BUNDLE_LIST%" >nul 2>&1

endlocal & exit /b %BUNDLE_EXIT_CODE%

