const express = require('express');
const http = require('http');
const WebSocket = require('ws');
const dotenv = require('dotenv');
const Matchmaker = require('./matchmaker');
const { connect } = require('./db');

dotenv.config();

const app = express();
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

const PORT = process.env.PORT || 3000;
const matchmaker = new Matchmaker();

app.use(express.json({ limit: '1mb' }));

// Routes
app.use('/api/billing', require('./routes/billing'));
app.use('/api/leaderboard', require('./routes/leaderboard'));
app.use('/api/analytics', require('./routes/analytics'));

app.get('/', (req, res) => res.send('Duel Rush server is running'));
app.get('/healthz', (req, res) => res.json({ ok: true }));

wss.on('connection', (ws, req) => {
  let userId;
  try {
    const url = new URL(req.url, 'http://localhost');
    userId = url.searchParams.get('userId');
  } catch {
    userId = null;
  }

  ws.id = userId || `guest_${Math.random().toString(36).slice(2, 8)}`;

  ws.on('message', (raw) => {
    try {
      const data = JSON.parse(raw);
      if (data.type === 'enqueue') matchmaker.enqueue(ws);
      else if (data.type === 'leave_queue') matchmaker.dequeue(ws);
      else if (data.type === 'action') matchmaker.handleAction(ws, data.action);
    } catch {
      // ignore malformed messages
    }
  });

  ws.on('close', () => {
    matchmaker.dequeue(ws);
  });
});

(async () => {
  server.listen(PORT, '0.0.0.0', () => {
    const addr = server.address();
    const port = addr && typeof addr === 'object' ? addr.port : PORT;
    console.log(`Server listening on http://localhost:${port}`);
  });

  try {
    await connect();
  } catch (e) {
    console.warn('DB connect failed:', e.message, '(continuing without DB)');
  }
})();

function shutdown(signal) {
  console.log(`${signal} received, shutting down...`);
  try {
    wss.close();
  } catch {}
  server.close(() => process.exit(0));
  // Force-exit if we can't close cleanly
  setTimeout(() => process.exit(1), 10_000).unref?.();
}

process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT', () => shutdown('SIGINT'));
