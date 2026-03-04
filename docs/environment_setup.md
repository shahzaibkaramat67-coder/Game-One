# Environment Configuration Guide

## Quick Start

### Server (.env)

Copy `.env.example` to `.env` and set values:

```bash
cd server
cp .env.example .env
```

Edit `.env`:

```env
PORT=3000
NODE_ENV=development
MONGODB_URI=mongodb://127.0.0.1:27017/duelrush
```

Start:
```bash
npm start
```

### Unity (env.json + ConfigLoader)

**Auto-loading:**
- ConfigLoader reads `Assets/Resources/env.json` at startup
- Loads configuration for current environment (stored in PlayerPrefs)
- All network calls use loaded config automatically

**To switch environments in-game:**

```csharp
ConfigLoader.Instance.SetEnvironment("production");
// Restart app to apply
```

Or add Environment Switcher UI to canvas.

---

## Environment Variables

### Server (.env)

| Variable | Default | Purpose |
|----------|---------|---------|
| `PORT` | 3000 | Server port |
| `NODE_ENV` | development | Environment (development/staging/production) |
| `MONGODB_URI` | localhost | Database connection |

**Development:**
```env
PORT=3000
NODE_ENV=development
MONGODB_URI=mongodb://127.0.0.1:27017/duelrush
```

**Staging (MongoDB Atlas):**
```env
PORT=3000
NODE_ENV=staging
MONGODB_URI=mongodb+srv://user:pass@cluster.mongodb.net/duelrush_staging
```

**Production:**
```env
PORT=3000
NODE_ENV=production
MONGODB_URI=mongodb+srv://user:pass@cluster.mongodb.net/duelrush_production
```

---

### Unity (env.json)

Located at `Assets/Resources/env.json`. Defines configs for:
- `development` → localhost
- `staging` → staging.duelrush.com
- `production` → duelrush.com

Each includes:
- Server URLs (WebSocket + HTTP)
- Game settings (match duration, HP, damage)
- Monetization (gem prices, coin rewards, XP)

**Modify at runtime:**
```csharp
ConfigLoader.Instance.SetEnvironment("staging");
```

---

## Setup Checklist

**Development:**
- [ ] MongoDB running locally (or connection string set)
- [ ] `.env` exists in `server/` with correct MONGODB_URI
- [ ] `npm start` runs without errors
- [ ] `env.json` exists in `Assets/Resources/`
- [ ] ConfigLoader initializes on app start

**Staging:**
- [ ] MongoDB Atlas cluster created
- [ ] `.env` updated with staging URI
- [ ] Server deployed to staging domain
- [ ] SSL/TLS configured (WSS required)
- [ ] `env.json` updated with staging URLs

**Production:**
- [ ] `.env` has production secrets (environment variables on server, not in code)
- [ ] Server running with auto-restart
- [ ] Database replicated/backed up
- [ ] SSL certificates valid
- [ ] ConfigLoader uses production env.json

---

## Security Notes

1. **Never commit `.env`** — add to `.gitignore`
2. **Use `.env.example`** as template for team
3. **Rotate secrets regularly** (MongoDB passwords, API keys)
4. **In production:** Use environment variables, not `.env` files
5. **In cloud (Heroku/AWS):** Set vars via dashboard, not config files

Example `.gitignore`:
```
server/.env
server/node_modules/
```

---

## File Structure

```
game one/
├── server/
│   ├── .env              ← Secrets (git-ignored)
│   ├── .env.example      ← Template (committed)
│   ├── src/
│   │   └── server.js     (uses process.env.*)
│   └── package.json
│
└── unity/
    └── Assets/
        ├── Resources/
        │   ├── env.json  ← Config presets (committed)
        │   └── Config.cs ← Loaded at runtime
        └── Scripts/
            └── ConfigLoader.cs (reads env.json)
```

---

## Testing Environments

### Local Development
```bash
npm start
# Logs: "Server listening on http://localhost:3000"
# Game connects via: ws://localhost:3000
```

### Test Server Connection
```bash
# From server folder
node test-client.js
```

Expected output:
```
✓ Client 1 connected
✓ Match started!
✓ Match ended!
```

---

## Troubleshooting

**"env.json not found"**
- Check `Assets/Resources/env.json` exists
- Ensure no typos in file name

**"Cannot connect to MongoDB"**
- Verify MONGODB_URI in `.env` is correct
- Check MongoDB is running (or Atlas connection)
- Test connection: `mongo "mongodb_uri"`

**"Server listening but game can't connect"**
- Check firewall allows port 3000
- Verify Config.Instance has correct wsServerUrl
- Look at browser console for WebSocket errors

**"Crash rate high on production"**
- Check `.env` variables are set correctly
- Review server logs: `npm start 2>&1 | tee logs.txt`
- Ensure MongoDB has enough connections

