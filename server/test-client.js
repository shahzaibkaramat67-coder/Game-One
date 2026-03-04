const WebSocket = require('ws');
const dotenv = require('dotenv');

dotenv.config();

const serverUrl = process.env.WS_SERVER_URL || `ws://localhost:${process.env.PORT || 3000}`;

console.log('Test Client 1: Connecting to', serverUrl);
const client1 = new WebSocket(serverUrl + '?userId=player1');
let matchStarted1 = false;

client1.on('open', () => {
  console.log('✓ Client 1 connected');
  console.log('  Enqueueing for match...');
  client1.send(JSON.stringify({ type: 'enqueue' }));
});

client1.on('message', (msg) => {
  const data = JSON.parse(msg);
  console.log('Client 1 received:', data.type, data);
  
  if (data.type === 'match_start') {
    matchStarted1 = true;
    console.log('  ✓ Match started! Sending attack...');
    setTimeout(() => {
      client1.send(JSON.stringify({ type: 'action', action: { type: 'attack' } }));
    }, 500);
  }
  if (data.type === 'match_end') {
    console.log('  ✓ Match ended! Winner:', data.winnerId);
  }
});

client1.on('error', (err) => console.error('Client 1 error:', err.message || err));
client1.on('close', () => console.log('Client 1 disconnected'));

// Client 2 connects after a short delay to trigger matchmaking
setTimeout(() => {
  console.log('\nTest Client 2: Connecting...');
  const client2 = new WebSocket(serverUrl + '?userId=player2');
  
  client2.on('open', () => {
    console.log('✓ Client 2 connected');
    console.log('  Enqueueing for match...');
    client2.send(JSON.stringify({ type: 'enqueue' }));
  });

  client2.on('message', (msg) => {
    const data = JSON.parse(msg);
    console.log('Client 2 received:', data.type, data);
    
    if (data.type === 'match_start') {
      console.log('  ✓ Match started! Sending dodge...');
      setTimeout(() => {
        client2.send(JSON.stringify({ type: 'action', action: { type: 'dodge' } }));
      }, 1000);
    }
    if (data.type === 'match_end') {
      console.log('  ✓ Match ended! Winner:', data.winnerId);
      process.exit(0);
    }
  });

  client2.on('error', (err) => console.error('Client 2 error:', err.message || err));
  client2.on('close', () => console.log('Client 2 disconnected'));
}, 2000);

// Timeout after 10s
setTimeout(() => {
  console.log('\nTest completed');
  process.exit(0);
}, 10000);
