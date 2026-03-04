# Duel Rush - Quick Start

## Server
```powershell
cd server
npm install
npm start
```
Open in browser: `http://localhost:3000` (should say the server is running).

## Unity (fastest)
1. Open the Unity project in `unity/` (Unity 2022 LTS+).
2. Unity menu: `DuelRush -> Create Demo Scene`
3. Press Play.

## Multiplayer note
Matchmaking starts when **2 clients** are connected.

## Changing server URL
Edit `unity/Assets/Resources/env.json`:
- `development` points to localhost
- set `production` to your deployed URL (Render uses `https://...` + `wss://...`)

