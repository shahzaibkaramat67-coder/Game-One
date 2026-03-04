# Duel Rush - Project Summary

## What it is
Duel Rush is a lightweight 1v1 PvP duel prototype:
- Unity client (mobile-first style) for gameplay, UI, economy, and progression systems
- Node.js WebSocket server for matchmaking and authoritative hit arbitration
- Optional MongoDB persistence for leaderboard/currency endpoints (falls back to in-memory storage if MongoDB is unavailable)

## Core gameplay
- Real-time 1v1 matchmaking over WebSockets
- Short matches (default 45s) with HP-based win conditions
- Attack + dodge actions, with a dodge window that can block damage
- Server-authoritative hit results broadcast to both clients

## Meta / monetization (prototype)
- Coins (soft currency) and gems (premium currency)
- Cosmetics inventory + shop (owned items persisted via PlayerPrefs)
- Battle pass (free + premium tracks, local persistence)
- Google Play Billing stubs (simulated purchases in Unity; server-side verification stubbed)

## Live-ops and analytics (prototype)
- Daily flash offers generator (local)
- Analytics event logging endpoint (server) + client tracking calls
- Leaderboard endpoints for top players and per-player rank/rating updates

## Deployable components
- `server/` can be deployed as a standalone Node service (Dockerfile included).
- `unity/` builds to Android APK from Unity (editor build script included).

