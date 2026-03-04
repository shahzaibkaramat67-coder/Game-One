# Deploy to Render (server)

## 1) Push to GitHub
Render deploys from a Git repository. Push this repo to GitHub (or GitLab).

## 2) Create Render service (Blueprint)
This repo includes `render.yaml` (Render Blueprint).

In Render:
- New + → **Blueprint**
- Select your repo
- Confirm it detects the `duel-rush-server` web service

### Required env vars
- `MONGODB_URI` (recommended for persistence). If you skip it, the server will run with in-memory storage (leaderboards reset on restart).

### Ports
Render uses port `10000` by default. `render.yaml` sets `PORT=10000`.

## 3) Get your Render URL
Your service URL will look like:
- `https://YOUR-SERVICE.onrender.com`

WebSocket URL for Unity:
- `wss://YOUR-SERVICE.onrender.com`

## 4) Point Unity to the deployed server
Edit `unity/Assets/Resources/env.json`:
- `production.wsServerUrl` → `wss://YOUR-SERVICE.onrender.com`
- `production.httpServerUrl` → `https://YOUR-SERVICE.onrender.com`

Then build the Android APK.

## Notes
- For matchmaking to work, you still need 2 clients connected (or add a bot client).
- Render free instances may sleep and disconnect WebSockets; for stable multiplayer use a paid plan.

