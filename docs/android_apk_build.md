# Android APK Build Guide — Duel Rush

**Goal:** Build signed Android APK ready for Google Play soft-launch  
**Time:** 30–45 minutes  
**Difficulty:** Medium (mostly setup)

---

## Prerequisites

### 1. Unity 2022 LTS or Later
- Download from [unity.com](https://unity.com/download)
- Install via **Unity Hub**
- Include: **Android Build Support**, **Android SDK**, **Android NDK**

### 2. Android SDK & NDK
- Installed automatically with Unity Android module
- **Or** install manually via Android Studio:
  - Download [Android Studio](https://developer.android.com/studio)
  - SDK Manager → SDK Platforms → API 28–34
  - SDK Tools → NDK, Build-tools 34.x

### 3. Java Development Kit (JDK)
- **Windows/Mac/Linux:** Install JDK 11+ (e.g., [OpenJDK](https://openjdk.org/))
- Verify: `java -version`

### 4. ADB (Android Debug Bridge)
- Comes with Android SDK
- **Windows:** `C:\Users\<User>\AppData\Local\Android\sdk\platform-tools`
- Verify: `adb version`

---

## Step 1: Open Unity Project

1. **Open Unity Hub**
2. **Add Project** → Select your `game one/unity/` folder
3. **Wait** for project to load (first time: 2–5 minutes)
4. Ensure **version is 2022.3 LTS or later**

---

## Step 2: Configure Player Settings

1. **File → Build Settings**
2. **Select Platform: Android** (left panel)
3. **Switch Platform** (button, bottom right)
4. **Player Settings** (button)

### General Settings
- **Company Name:** DuelRush
- **Product Name:** Duel Rush
- **Version:** 0.1.0
- **Bundle Version Code:** 1

### Android Settings
- **Minimum API Level:** 28 (Android 9.0)
- **Target API Level:** 34 (Android 14)
- **Package Name:** `com.duelrush.game` (required for Play Store)
- **Install Location:** Automatic

### Graphics
- **Scripting Backend:** IL2CPP (faster, more secure)
- **ARM64:** Enable (required for 64-bit)
- **Graphics API:** OpenGLES 3 (or Vulkan for newer)

### Internet Access
- **Internet Access:** Require (for WebSocket)
- **Cleartext Traffic:** Set to `false` (production security)

### Keyboard
- **Full Screen Keyboard:** Enabled
- **Render Outside Safe Area:** Disabled

---

## Step 3: Create Keystore (Signing Certificate)

**For first build:**

1. **Back in Build Settings**
2. Scroll down → **Publishing Settings**
3. **Keystore Manager**
4. **Create New Keystore:**
   - **Password:** `duelrush_release` (or your choice)
   - **Save location:** Keep default
   - **Click Create**

5. **Create Key:**
   - **Alias:** `duelrush_key`
   - **Alias Password:** Same as keystore
   - **Validity (years):** 25
   - **Common Name:** Your name
   - **Organization:** DuelRush
   - **Click Create**

6. **Save** — Unity creates `user.keystore`

**Keep this keystore safe!** You need it for every update to the same app on Play Store.

---

## Step 4: Add Scenes to Build

If you don't have a scene yet, create one quickly in Unity menu:
- `DuelRush -> Create Demo Scene` (also adds it to Build Settings)

1. **File → Build Settings**
2. **Scenes in Build** (top section)
3. **Add Open Scene** or drag scenes from Project folder
4. **Ensure your DuelScene is at index 0** (first scene)

Example:
```
0. DuelScene
```

---

## Step 5: Build APK

### Option A: Using GUI (Easiest)

1. **File → Build Settings**
2. **Build** (button, bottom right)
3. Choose location: `Builds/Android/`
4. **Save** as `DuelRush.apk`
5. **Wait** 5–10 minutes

Console shows:
```
Built the following apk(s):
  .../DuelRush.apk

Build complete!
```

### Option B: Using Build Script (Automated)

**Windows:**
```powershell
cd "C:\Users\asdfg\OneDrive\Desktop\game one\unity"
.\build-apk.bat
```

**Mac/Linux:**
```bash
cd ~/path/to/game\ one/unity
chmod +x build-apk.sh
./build-apk.sh
```

### Option C: Command Line (Advanced)

```bash
"C:\Program Files\Unity\Hub\Editor\2022.3.XX\Editor\Unity.exe" \
  -quit \
  -batchmode \
  -projectPath "C:\Users\asdfg\OneDrive\Desktop\game one\unity" \
  -buildTarget Android \
  -executeMethod BuildScript.BuildAPK
```

---

## Step 6: Verify APK

```bash
cd Builds/Android/
ls -lh DuelRush.apk
```

Expected:
```
-rw-r--r--  1 user  group  45M Feb  4 10:00 DuelRush.apk
```

**Size:** 40–100 MB is normal (includes IL2CPP runtime)

---

## Step 7: Test on Device

### Connect Android Device

1. **Enable USB Debugging:**
   - Settings → Developer Options → USB Debugging
   - (If Developer Options hidden: Settings → About → Tap Build Number 7x)

2. **Connect via USB**

3. **Verify connection:**
   ```bash
   adb devices
   ```
   Expected:
   ```
   List of attached devices
   ABC123XYZ device
   ```

### Install APK

```bash
adb install Builds/Android/DuelRush.apk
```

Output:
```
Installing Builds/Android/DuelRush.apk
  Success
```

### Launch App

```bash
adb shell am start -n com.duelrush.game/.MainActivity
```

Or tap app icon on device.

### Check Logs

```bash
adb logcat | grep -i "duelrush\|unity\|websocket"
```

Look for:
```
✓ Environment loaded: development
✓ WebSocket connected!
```

---

## Step 8: Upload to Google Play Console

### Create App

1. Go to [play.google.com/console](https://play.google.com/console)
2. **Create App**
   - Name: Duel Rush
   - Default language: English (US)
   - Category: Games
   - Accept policies

3. **Agree** to Play Developer Agreement

### Setup App

**Left panel:**
1. **App info** → Fill out details
2. **Pricing & distribution** → Free
3. **Content rating** → Complete questionnaire
4. **Monetization** → None (or add IAP later)

### Create Release

1. **Release** → Internal testing (left)
2. **Create release**
3. **Upload APK:**
   - Select your `DuelRush.apk`
   - Wait for upload
4. **Review** → looks OK
5. **Save**

### Add Testers

1. **Testers** section
2. Add email addresses (Google accounts)
3. Share **internal test link** with testers

---

## Step 9: Monitor Crash Reports

1. **Google Play Console** → Your app
2. **Vitals** → Crashes & ANRs
3. Monitor daily for critical issues
4. Fix bugs, rebuild APK, reupload

---

## Troubleshooting

### "Build failed: Unable to find Java"
```bash
# Set JAVA_HOME
export JAVA_HOME=/usr/libexec/java_home -v 11
# Or point to your JDK installation
```

### "Cannot find Android SDK"
```
Edit → Preferences → External Tools
Set Android SDK path to:
C:\Users\<User>\AppData\Local\Android\sdk
```

### "WebSocket connection fails on device"
- Check device can reach your server (ping test)
- Ensure cleartext traffic allowed for localhost (`.env` has ws://)
- Use `wss://` (secure) for production

### "App crashes on startup"
- Check logcat: `adb logcat | grep -i unity`
- Ensure all resources (env.json, Config.cs) exist in Assets/Resources/
- Verify scene is added to Build Settings

### "APK too large (>100MB)"
- Use IL2CPP script backend (smaller than Mono)
- Remove unused assets
- Enable Asset Stripping

---

## Build Optimization Tips

### Reduce APK Size

1. **Build Settings → Player → Compression:**
   - Asset Compression: LZ4HC
   - Texture Compression: Platform default

2. **Disable unneeded modules:**
   - Edit → Project Settings → Player → Other Settings
   - Uncheck: Physics, Physics2D, Cloth, etc.

3. **Asset bundles:**
   - Move large assets to downloadable bundles

### Improve Performance

1. **Graphics:**
   - LOD Bias: 0.5
   - Max queued frames: 1
   - V Sync: Don't Sync

2. **Physics:**
   - Disable if not used
   - Reduce collision check frequency

---

## Checklist

```
Pre-Build:
[ ] Unity 2022.3 LTS installed
[ ] Android module installed
[ ] JDK 11+ installed
[ ] Package name set: com.duelrush.game
[ ] Keystore created & signed
[ ] Scenes added to Build Settings

Build:
[ ] File → Build Settings → Android
[ ] Click Build
[ ] Select Builds/Android/ folder
[ ] Wait 5–10 minutes
[ ] APK file exists

Post-Build:
[ ] APK signed (check Properties)
[ ] APK size reasonable (40–100MB)
[ ] Tested on device
[ ] No critical crashes
[ ] WebSocket connects
[ ] Config loads (check logcat)

Play Store:
[ ] App created in Play Console
[ ] APK uploaded to internal test
[ ] Testers invited
[ ] Monitoring crash dashboard
```

---

## Next Steps

1. ✅ Build APK
2. ✅ Test on device (verify WebSocket connects to your server)
3. ✅ Upload to internal testing track
4. ✅ Invite 5–10 beta testers
5. ✅ Collect feedback for 1 week
6. ✅ Move to closed testing (100–500 testers)
7. ✅ Scale UA (paid user acquisition)
8. ✅ Launch phased rollout (5% → 25% → 100%)

---

## Reference: Build Output Files

```
Builds/Android/
├── DuelRush.apk              ← Install on device
├── DuelRush.aab              ← Upload to Play Store (if built)
├── build.properties          ← Build info
└── unityLibrary/             ← Build artifacts
```

**APK:** Standalone file, sideload on device  
**AAB:** Google Play bundle, Play Store only (more optimized)

---

**Ready to build? Run: `.\build-apk.bat` (Windows) or `./build-apk.sh` (Mac/Linux)**

