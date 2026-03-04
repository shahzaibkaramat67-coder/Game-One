@echo off
REM Duel Rush — Android Build Script (Windows)
REM Usage: build-apk.bat

setlocal enabledelayedexpansion

echo.
echo 🎮 Duel Rush Android Build Script
echo ==================================
echo.

REM Check if Unity Hub is installed
if not exist "C:\Program Files\Unity\Hub\Editor\*\Editor\Unity.exe" (
    echo ❌ Unity not found. Install Unity 2022 LTS via Unity Hub.
    pause
    exit /b 1
)

REM Get latest Unity version
for /f "tokens=*" %%i in ('dir /b /od "C:\Program Files\Unity\Hub\Editor" ^| findstr /r "^[0-9]" ^| tail -1') do set UNITY_VERSION=%%i
set UNITY_PATH=C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe

echo 🔍 Found Unity: %UNITY_PATH%
echo.

REM Set Android SDK (adjust path if needed)
if not defined ANDROID_SDK_ROOT (
    set ANDROID_SDK_ROOT=C:\Users\%USERNAME%\AppData\Local\Android\sdk
)

if not exist "%ANDROID_SDK_ROOT%" (
    echo ❌ Android SDK not found at: %ANDROID_SDK_ROOT%
    echo Set ANDROID_SDK_ROOT environment variable and try again.
    pause
    exit /b 1
)

echo 📱 Android SDK: %ANDROID_SDK_ROOT%
echo.

REM Build settings
set PROJECT_PATH=%cd%
set BUILD_DIR=%PROJECT_PATH%\..
set APK_PATH=%BUILD_DIR%\DuelRush.apk
set APK_NAME=DuelRush.apk

echo 📁 Project: %PROJECT_PATH%
echo 🔨 Building to: %BUILD_DIR%
echo.

REM Create build directory
if not exist "%BUILD_DIR%" mkdir "%BUILD_DIR%"

echo ⏳ Running Unity build (this may take 5-10 minutes)...
echo.

REM Run Unity build
"%UNITY_PATH%" ^
  -quit ^
  -batchmode ^
  -projectPath "%PROJECT_PATH%" ^
  -buildTarget Android ^
  -executeMethod BuildScript.BuildAPK ^
  -logFile -

if %errorlevel% equ 0 (
    echo.
    echo ✅ Build successful!
    echo 📦 APK: %APK_PATH%
    echo.
    echo APK ready in current directory: DuelRush.apk
) else (
    echo.
    echo ❌ Build failed. Check logs above.
    pause
    exit /b 1
)

pause
