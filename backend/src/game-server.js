require('dotenv').config();
const WebSocket = require('ws');

const port = process.env.GAME_PORT || 3002;
const wss = new WebSocket.Server({ port });
const sessionMaxPlayers = Number(process.env.GAME_MAX_PLAYERS || 2);

const sessions = new Map(); // gameId -> { players: Map(userId, { ws, state }) }
const HEARTBEAT_MS = 30000;

function getSession(gameId) {
  if (!sessions.has(gameId)) {
    sessions.set(gameId, { players: new Map() });
  }
  return sessions.get(gameId);
}

function broadcast(gameId, data, exceptUserId = null) {
  const session = sessions.get(gameId);
  if (!session) return;
  const payload = JSON.stringify(data);
  for (const [userId, p] of session.players.entries()) {
    if (exceptUserId && userId === exceptUserId) continue;
    if (p.ws.readyState === WebSocket.OPEN) p.ws.send(payload);
  }
}

wss.on('connection', (ws) => {
  let currentGameId = null;
  let currentUserId = null;
  ws.isAlive = true;

  ws.on('pong', () => {
    ws.isAlive = true;
  });

  ws.on('message', (raw) => {
    let msg;
    try { msg = JSON.parse(raw.toString()); } catch { return; }

    if (msg.type === 'join') {
      const { gameId, userId, name } = msg;
      if (!gameId || !userId) return;

      const session = getSession(gameId);
      const alreadyInSession = session.players.has(userId);
      if (!alreadyInSession && session.players.size >= sessionMaxPlayers) {
        ws.send(JSON.stringify({
          type: 'error',
          code: 'SESSION_FULL',
          message: `Session full (${sessionMaxPlayers}/${sessionMaxPlayers})`,
        }));
        ws.close(1008, 'Session full');
        return;
      }

      currentGameId = gameId;
      currentUserId = userId;

      // Reconnect logic for same user: close old socket and keep latest connection.
      const previous = session.players.get(userId);
      if (previous && previous.ws !== ws) {
        try { previous.ws.close(1000, 'Reconnected'); } catch {}
      }

      session.players.set(userId, { ws, state: { x: 0, y: 0, name } });

      // Send existing players to the new client
      const snapshot = Array.from(session.players.entries()).map(([id, p]) => ({
        userId: id,
        state: p.state,
      }));
      ws.send(JSON.stringify({ type: 'snapshot', players: snapshot }));

      broadcast(gameId, { type: 'player-joined', userId, state: { x: 0, y: 0, name } }, userId);
      return;
    }

    if (!currentGameId || !currentUserId) return;

    if (msg.type === 'move') {
      const session = getSession(currentGameId);
      const p = session.players.get(currentUserId);
      if (!p) return;
      p.state = { ...p.state, x: msg.x, y: msg.y, dir: msg.dir };
      broadcast(currentGameId, { type: 'move', userId: currentUserId, x: msg.x, y: msg.y, dir: msg.dir }, currentUserId);
      return;
    }

    if (msg.type === 'action') {
      broadcast(currentGameId, { type: 'action', userId: currentUserId, action: msg.action, data: msg.data }, currentUserId);
      return;
    }

    if (msg.type === 'state') {
      broadcast(currentGameId, { type: 'state', counter: msg.counter }, currentUserId);
      return;
    }
  });

  ws.on('close', () => {
    if (!currentGameId || !currentUserId) return;
    const session = sessions.get(currentGameId);
    if (!session) return;
    session.players.delete(currentUserId);
    broadcast(currentGameId, { type: 'player-left', userId: currentUserId });
    if (session.players.size === 0) {
      sessions.delete(currentGameId);
    }
  });
});

const heartbeat = setInterval(() => {
  wss.clients.forEach((ws) => {
    if (ws.isAlive === false) return ws.terminate();
    ws.isAlive = false;
    ws.ping();
  });
}, HEARTBEAT_MS);

wss.on('close', () => {
  clearInterval(heartbeat);
});

console.log(`Game WS server listening on ${port}`);
