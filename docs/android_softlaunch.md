# Android Soft-Launch & Build Guide

## Prerequisites

- Unity 2022 LTS or later
- Android SDK (API 28+)
- Google Play Console account (free to create)
- Keystore file for signing (generate in Unity)

---

## Step 1: Unity Build Settings

1. **File → Build Settings**
2. Select **Android** platform
3. **Player Settings:**
   - **Minimum API Level:** 28
   - **Target API Level:** 34
   - **Package Name:** `com.duelrush.game` (or your company domain)
   - **Version Code:** 1
   - **Version Name:** 0.1.0

4. **Graphics:**
   - Set to optimal for mobile (reduce draw calls)

5. **Network:**
   - Ensure WebSocket support (ws:// protocol)

---

## Step 2: Create Keystore for Signing

1. **Edit → Project Settings → Player → Publishing Settings**
2. **Keystore Manager:**
   - Click "Create New"
   - Set password: `duelrush_release` (or your choice)
   - Alias: `duelrush_key`
   - Alias password: same
3. **Save** — Unity will create `user.keystore` in project

---

## Step 3: Configure Google Play Billing

1. **Open Google Play Console:** https://play.google.com/console
2. **Create App:**
   - Name: `Duel Rush`
   - Category: Games
   - Accept required policies

3. **Monetization Setup:**
   - Go to Monetization → Billing Setup
   - Add merchant account (if needed)
   - Create products:
     - `gems_100`, `gems_500`, `gems_1000` (non-consumable)
     - `battle_pass` (non-consumable, one per season)
     - `founder_pack` (non-consumable, limited time)

4. **Get Package Name** and save it (e.g., `com.duelrush.game`)

---

## Step 4: Integrate Google Play Billing in Unity

1. **Window → TextMesh Pro → Import TMP Essential Resources**
2. **Add Google Play Billing (via Unity Package Manager):**
   ```
   Window → Package Manager → + → Add package from git URL
   https://github.com/google/play-billing-unity.git
   ```

3. **In your BillingManager.cs, initialize:**
   ```csharp
   using Google.Play.Billing;

   void Start() {
       var billingClient = new BillingClientStateListener();
       billingClient.StartConnection();
   }
   ```

4. **Sandbox Testing:**
   - Add test account in Google Play Console
   - Purchases will be free in sandbox

---

## Step 5: Build APK for Testing

1. **File → Build Settings → Android**
2. **Build:**
   - Click "Build and Run"
   - Unity auto-installs APK on connected device
   - Or save APK to disk for distribution

3. **Test Locally:**
   - Ensure device can reach `ws://localhost:3000` (server on same network)
   - Or change server URL to production domain

---

## Step 6: Internal Testing on Google Play

1. **Google Play Console → Internal Testing:**
   - Upload signed APK (minimum version 0.1.0)
   - Add yourself as tester
   - Share link with team

2. **Test Accounts:**
   - Add test accounts in Play Console
   - Purchases are free for testers
   - Refunds instant

---

## Step 7: Closed Testing (Soft-Launch)

1. **Google Play Console → Closed Testing:**
   - Create closed testing track
   - Upload APK
   - Add 100–500 testers (friends, community, influencers)
   - Collect feedback for 1–2 weeks

2. **Monitor KPIs:**
   - Install count, daily active users
   - Crash reports, ANRs
   - Purchase funnel (views → cart → purchase)
   - Retention (D1, D7)

---

## Step 8: Production Launch

Once you have:
- ✅ >90% D1 retention in closed testing
- ✅ >2% conversion to payers
- ✅ No critical crashes
- ✅ Localization (if targeting multiple regions)

1. **Create Release Track:**
   - Same APK as closed testing
   - Phased rollout: 5% → 25% → 100% (over 1 week)

2. **Release Notes:**
   ```
   Duel Rush — Season 1 Launch

   30–60 second 1v1 duels.
   Compete, collect cosmetics, rank up.

   No ads. Pure skill-based PvP.
   ```

3. **Monitor:**
   - Crash rates (target: <0.1%)
   - ARPPU, conversion (target: 2–4% payers)
   - D1/D7 retention (target: 30–40% / 15–20%)

---

## Server & Backend for Soft-Launch

### Prepare Production Server

1. **Deploy Node server** to AWS EC2 / Heroku / DigitalOcean:
   ```bash
   git clone <your-repo>
   cd server
   npm install
   # Set env vars: MONGODB_URI, PORT
   npm start
   ```

2. **Configure MongoDB:**
   - Use MongoDB Atlas (free tier for testing)
   - Create database `duelrush_prod`
   - White-list IP ranges

3. **Update client server URL:**
   - In `WebSocketClient.cs`: change `ws://localhost:3000` to `wss://your-domain.com`
   - In `BillingManager.cs`: change `http://localhost:3000` to `https://your-domain.com`
   - Use WSS (secure WebSocket) in production

4. **SSL Certificate:**
   - Use Let's Encrypt (free)
   - Nginx reverse proxy for termination

---

## Analytics & Live-Ops Dashboard

Set up **Firebase Console** for real-time monitoring:

1. **Add Firebase to app:** https://firebase.google.com/console
2. **Download `google-services.json`** → Place in `Assets/Plugins/Android/`
3. **In code:**
   ```csharp
   Firebase.Analytics.FirebaseAnalytics.LogEvent("match_complete", ...);
   ```

4. **Dashboard:**
   - Daily installs, DAU, retention
   - Purchase funnel
   - Custom events (cosmetics bought, battle pass level, etc.)

---

## UA (User Acquisition) Strategy for Soft-Launch

### Budget: $3,000 (conservative)

**Channels (by effectiveness):**
1. **Facebook/Instagram Ads** (50% budget)
   - Target: Hardcore gamers, ages 18–35, interest in PvP
   - CPI target: $0.25–$0.50
   - Conversion: 2–3%

2. **TikTok Organic** (free but effort-heavy)
   - Post clips of duels, emotes, cosmetics
   - Hashtags: #PvPGaming #MobileGaming #DuelRush
   - Goal: 50k–100k impressions = 5–10% CTR

3. **YouTube Pre-roll** (20% budget)
   - Target: Gaming channels 100k–1M subs
   - Sponsor videos about mobile PvP
   - CPI ~$0.50–$1.00

4. **Influencer Partnerships** (20% budget)
   - 10–20 micro-influencers (10k–100k followers)
   - Free game + $100–200 sponsorship
   - Goal: 10k installs from their audiences

5. **Discord Communities** (10% budget)
   - Post in gaming/mobile gaming Discords
   - Offer free Founder's Pack codes (first 24h)
   - Build community for launch

---

## Metrics to Collect (First Week)

| Metric | Target | Method |
|--------|--------|--------|
| Installs | 5k–10k | Google Play Console |
| D1 Retention | >30% | Firebase |
| Matches/DAU | >3 | Server logs |
| Shop views | >50% DAU | Analytics event |
| Conversion | 2–3% | Firebase purchase events |
| ARPPU | $20 | Firebase purchase value |

---

## Soft-Launch Success Criteria

🟢 **Continue to global launch if:**
- ✅ D1 retention >30%
- ✅ Conversion >2%
- ✅ ARPPU >$15
- ✅ <5 crashes per 10k installs

🔴 **Pause & iterate if:**
- ❌ D1 retention <20%
- ❌ Conversion <1%
- ❌ Crash rate >10%

---

## Iteration Checklist (Based on Feedback)

- [ ] Reduce match duration if retention dips (30s instead of 45s)
- [ ] Add daily login bonus (20 coins, day 7 = 100)
- [ ] Lower cosmetic prices by 20% if shop views >50% but purchases <1%
- [ ] Add tutorial for first-time players
- [ ] Fix any reported bugs immediately
- [ ] Add new cosmetics mid-week if sales slow

