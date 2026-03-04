# Duel Rush — Ready to Play (Local)

## 1) Start the server
```powershell
cd "C:\Users\asdfg\OneDrive\Desktop\game one\server"
npm start
```
Expected: `Server listening on http://localhost:3000`

## 2) Open Unity
Open the Unity project in `unity/` (Unity 2022 LTS or later).

## 3) Create a playable scene (fastest)
In Unity menu: `DuelRush -> Create Demo Scene`

Then press Play.

You should see:
- `WebSocket connected`
- `Match started` (once a second client connects)

## 4) Matchmaking note
Matches start when *two* clients are connected. To test:
- Run a second build (Windows build twice), or
- Run Windows + Android, or
- Use a simple bot client (ask me and I’ll add one).

## Server URL config
Unity reads URLs from `unity/Assets/Resources/env.json`:
- Development: `ws://localhost:3000`
- Production: set this to your deployed URL (e.g. Render `wss://...`)

