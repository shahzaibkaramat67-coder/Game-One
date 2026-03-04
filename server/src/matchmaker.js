const { v4: uuidv4 } = require('uuid');

class Matchmaker {
  constructor() {
    this.queue = [];
    this.matches = new Map();
  }

  enqueue(client) {
    if (!client || client.matchId) return;
    if (this.queue.includes(client)) return;
    this.queue.push(client);
    this.tryMatch();
  }

  dequeue(client) {
    this.queue = this.queue.filter(c => c !== client);
  }

  tryMatch() {
    while (this.queue.length >= 2) {
      const a = this.queue.shift();
      const b = this.queue.shift();
      this.createMatch(a, b);
    }
  }

  createMatch(a, b) {
    const id = uuidv4();
    const match = {
      id,
      players: [a, b],
      state: {
        hp: new Map([[a.id, 100], [b.id, 100]]),
        lastDodge: new Map([[a.id, 0], [b.id, 0]]),
      },
    };
    this.matches.set(id, match);

    a.matchId = id;
    b.matchId = id;

    try { a.send(JSON.stringify({ type: 'match_start', matchId: id, selfId: a.id, opponentId: b.id })); } catch {}
    try { b.send(JSON.stringify({ type: 'match_start', matchId: id, selfId: b.id, opponentId: a.id })); } catch {}
  }

  endMatch(id, winnerId) {
    const match = this.matches.get(id);
    if (!match) return;

    const payload = JSON.stringify({ type: 'match_end', winnerId });
    match.players.forEach(p => {
      try { p.send(payload); } catch {}
      p.matchId = null;
    });
    this.matches.delete(id);
  }

  handleAction(client, msg) {
    const match = this.matches.get(client.matchId);
    if (!match) return;
    const opponent = match.players.find(p => p !== client);
    if (!opponent) return;

    if (msg.type === 'dodge') {
      match.state.lastDodge.set(client.id, Date.now());
      const payload = JSON.stringify({ type: 'player_dodged', playerId: client.id });
      match.players.forEach(p => { try { p.send(payload); } catch {} });
      return;
    }

    if (msg.type === 'attack') {
      const now = Date.now();
      const lastDodge = match.state.lastDodge.get(opponent.id) || 0;
      const dodgeWindow = 500;

      let damage = 12;
      if (now - lastDodge <= dodgeWindow) damage = 0;

      const newHp = Math.max(0, (match.state.hp.get(opponent.id) || 0) - damage);
      match.state.hp.set(opponent.id, newHp);

      const payload = JSON.stringify({
        type: 'hit',
        attackerId: client.id,
        targetId: opponent.id,
        damage,
        hp: newHp,
      });
      match.players.forEach(p => { try { p.send(payload); } catch {} });

      if (newHp <= 0) this.endMatch(match.id, client.id);
    }
  }
}

module.exports = Matchmaker;

