# Duel Rush Server

Minimal authoritative WebSocket server for quick 1v1 matchmaking and arbitration, plus a few REST endpoints (leaderboard, analytics, billing stubs).

## Run locally

```bash
npm install
npm start
```

Defaults to `ws://localhost:3000` and `http://localhost:3000`.

### Environment variables
- `PORT` (default: `3000`)
- `MONGODB_URI` (optional; if unreachable, server continues with in-memory storage)
- `MONGO_TIMEOUT_MS` (optional; default: `2000`)

## Test WebSocket matchmaking

In a second terminal (with server running):
```bash
node test-client.js
```

## API endpoints (prototype)
- `GET /healthz`
- `GET /api/leaderboard/top/:limit`
- `GET /api/leaderboard/rank/:userId`
- `POST /api/leaderboard/update-rating` (JSON: `{ "userId": "...", "opponent": "...", "won": true }`)
- `POST /api/analytics/track` (JSON: `{ "eventName": "...", "userId": "...", "propertiesJson": "..." }`)
- `POST /api/billing/purchase-receipt` (stubbed)

Notes:
- Google Play purchase verification (`src/play-billing.js`) is still a stub and needs Google Play Developer API integration.

