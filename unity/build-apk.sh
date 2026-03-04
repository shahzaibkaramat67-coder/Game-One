#!/bin/bash
# Duel Rush — Android Build Script
# Usage: ./build-apk.sh

set -e

echo "🎮 Duel Rush Android Build Script"
echo "================================="

# Check if Unity is installed
if ! command -v /Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity &> /dev/null; then
    echo "❌ Unity not found. Install Unity 2022 LTS first."
    exit 1
fi

# Check if Android SDK is set
if [ -z "$ANDROID_SDK_ROOT" ]; then
    echo "❌ ANDROID_SDK_ROOT not set. Run:"
    echo "export ANDROID_SDK_ROOT=/path/to/Android/sdk"
    exit 1
fi

# Build path
BUILD_DIR=".."
APK_PATH="$BUILD_DIR/DuelRush.apk"
APK_NAME="DuelRush.apk"
UNITY_PROJECT_PATH=$(pwd)

echo "📁 Project: $UNITY_PROJECT_PATH"
echo "🔨 Building to: $BUILD_DIR"

# Run Unity build
echo "⏳ Running Unity build..."
unity -quit \
  -batchmode \
  -projectPath "$UNITY_PROJECT_PATH" \
  -buildTarget Android \
  -executeMethod BuildScript.BuildAPK \
  -logFile -

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "📦 APK: $APK_PATH"
    echo "APK ready in parent directory: DuelRush.apk"
else
    echo "❌ Build failed. Check logs above."
    exit 1
fi
