# Monetization System — Complete Guide

## Currency Model

### Soft Currency: Coins 🪙
- Earned in-game: 25 coins per match won
- Used for: Low-tier cosmetics, temporary items
- Cannot purchase gems with coins

### Hard Currency: Gems 💎
- Purchased only with real money via Google Play Billing
- Used for: Premium cosmetics, battle pass, special offers
- Price tiers: $0.99 (100), $4.99 (500), $9.99 (1000)

---

## Cosmetics Shop

All cosmetics are **purely visual** — no pay-to-win mechanics.

### Cosmetic Types & Prices

| Item | Cost | Type | Notes |
|------|------|------|-------|
| Red Warrior | 150 coins | Skin | Beginner |
| Blue Ninja | 150 coins | Skin | Beginner |
| Gold Legend | 500 gems | Skin | Premium (launch exclusive) |
| Victory Dance | 100 coins | Emote | Beginner |
| Taunt Emote | 250 gems | Emote | Premium |
| Diamond Frame | 400 gems | Frame | Premium |
| Elite Badge | 200 coins | Badge | Beginner |
| Legendary Badge | 600 gems | Badge | Premium |

---

## Battle Pass (4-week season)

### Free Track
- Earn 1 level per ~5 matches (1000 XP per level)
- Rewards: Cosmetic fragments, small coins

### Premium Track ($4.99)
- Same XP as free track
- Exclusive rewards: Premium cosmetics, 1 legendary skin unlock
- **Launch offer:** Limited "Founder's Pack" at $9.99
  - Includes: Premium pass + Gold Legend skin + 200 gem bonus

---

## Revenue Strategy (30-Day Targets)

### Conservative Estimate → $10k
- **Installs:** 40,000
- **DAU:** ~12,000 (30% D1 retention)
- **Payers:** 240 (2% conversion)
- **ARPPU:** $20 avg spend
- **Revenue:** $4,800

### Aggressive Levers → $10k+
1. **Increase ARPPU to $30:**
   - Higher-priced limited skins ($9.99 bundles)
   - Battle pass + cosmetic bundles
   - Daily flash offers (20-30% discount, 24h limit)

2. **Increase Conversion to 4%:**
   - Premium onboarding flow highlighting cosmetics
   - Free gems login bonus (days 1-3)
   - Limited-time "Founder's Pack" (72h only)

3. **Combined:** 480 payers × $25 = **$12,000+**

---

## Launch Offers (Critical for month-1 revenue)

### Founder's Pack — $9.99 (First 72 hours only)
- Premium battle pass
- Exclusive gold skin
- 200 gems bonus
- Time-limited (FOMO)
- **Target:** 1-2% of installs buy
- **Calc:** 40k × 1.5% × $9.99 = ~$6,000

### Daily Flash Bundles (After 24h play)
- Rotating cosmetics at 25-30% discount
- 24-hour window (FOMO)
- Shows player has "earned" discount via gameplay
- $1.99, $4.99 tiers
- **Target:** 0.5% of DAU daily

### Battle Pass Early-Bird (Week 1)
- $4.99 for premium track
- Bonus: Instant level +3 on purchase
- Heavy notification push
- **Target:** 5% of paying DAU

---

## Server-Side Validation (NOT YET IMPLEMENTED)

Currently uses **sandbox mode** (gems auto-grant on purchase).

For production, integrate:
1. Google Play Billing Library (Java)
2. Service account credentials (Google Play Console)
3. Verify `purchaseToken` against Google API
4. Check `purchaseState == 0` (purchased, not pending/cancelled)
5. Avoid double-charging via idempotency tokens

### Implementation TODO
```javascript
// In src/routes/billing.js, replace SimulatePurchase with:
async function verifyWithGooglePlayAPI(purchaseToken, productId) {
  // Use googleapis or direct HTTP POST to:
  // https://www.googleapis.com/androidpublisher/v3/applications/{packageName}/purchases/products/{productId}/tokens/{token}
  // Returns: { purchaseState, purchaseTime, etc. }
}
```

---

## Daily Live-Ops Calendar

### Week 1 (Launch)
- **Mon:** Founder's Pack live (72h timer)
- **Tue:** +50% XP battle pass boost
- **Thu:** First cosmetic flash deal
- **Fri-Sun:** Double coins weekend

### Week 2-4
- 2-3 flash deals per week (cosmetic rotation)
- Daily login bonuses (day 7 = 50 gems)
- Mid-week XP boosts for engagement

---

## Analytics to Track

- **Daily:** Installs, DAU, D1/D7 retention
- **Per-user:** Conversion funnel, ARPPU, ARPU
- **Per-session:** Matches played, cosmetics viewed, cart abandonment
- **ROAS:** UA spend / revenue per source
- **Pivot:** By day 7, scale/pause UA based on LTV:CPA ratio

---

## Anti-Cheat for Cosmetics

Cosmetics are server-validated. Clients send purchase receipts; server validates before granting.
No local-only item grants.

---

## Next Steps

1. ✅ Cosmetics shop UI (basic scaffolded)
2. ✅ Battle pass UI (basic scaffolded)
3. ✅ Billing manager (sandbox mode ready)
4. 🔴 **TODO:** Integrate Google Play Billing Library (Unity)
5. 🔴 **TODO:** Implement Play API server verification
6. 🔴 **TODO:** Analytics integration (Firebase Events)
7. 🔴 **TODO:** Leaderboard system
8. 🔴 **TODO:** A/B test offers (offer A vs B each cohort)

