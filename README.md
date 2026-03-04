# Duel Rush (Prototype)

This repo contains:
- `unity/`: Unity client prototype (local + Android build)
- `server/`: Node.js authoritative WebSocket server + simple REST APIs

## Local play (quick)

1) Start the server:
```powershell
cd server
npm install
npm start
```
Server runs on `ws://localhost:3000` and `http://localhost:3000`.

2) Open `unity/` in Unity (2022 LTS or later), create a simple test scene (see `PLAY_NOW.md`).

Notes:
- MongoDB is optional. If MongoDB is not reachable, the server falls back to in-memory storage for leaderboard/currency APIs.
- Unity config defaults are provided at runtime. For a persistent `Config.asset`, use `DuelRush/Create Config Asset` in the Unity menu.

## Deploy (server)

### Render (recommended for quick deploy)
- Use the included Blueprint: `render.yaml`
- Guide: `docs/render_deploy.md`

### Option A: Docker (recommended)
From repo root:
```bash
docker compose up --build
```

### Option B: Node.js process
Set env vars on your host:
- `PORT`
- `MONGODB_URI` (optional)
- `MONGO_TIMEOUT_MS` (optional; defaults to `2000`)

Then:
```bash
cd server
npm ci
npm start
```

For production `wss://`, put the server behind a TLS reverse proxy (Nginx/Caddy/Cloudflare) that supports WebSockets.
