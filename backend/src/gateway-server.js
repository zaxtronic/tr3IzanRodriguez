require('dotenv').config();
const http = require('http');
const httpProxy = require('http-proxy');

const apiPort = process.env.API_PORT || 3001;
const gamePort = process.env.GAME_PORT || 3002;
const gatewayPort = process.env.GATEWAY_PORT || 3000;

const apiTarget = process.env.API_TARGET || `http://localhost:${apiPort}`;
const gameTarget = process.env.GAME_TARGET || `ws://localhost:${gamePort}`;

const proxy = httpProxy.createProxyServer({ ws: true, changeOrigin: true });

proxy.on('error', (err, req, res) => {
  if (res && !res.headersSent) {
    res.writeHead(502, { 'Content-Type': 'application/json' });
  }
  if (res) {
    res.end(JSON.stringify({ error: 'Bad gateway', detail: err.message }));
  }
});

const server = http.createServer((req, res) => {
  if (req.url.startsWith('/api') || req.url.startsWith('/health')) {
    proxy.web(req, res, { target: apiTarget });
    return;
  }

  res.writeHead(404, { 'Content-Type': 'application/json' });
  res.end(JSON.stringify({ error: 'Not found' }));
});

server.on('upgrade', (req, socket, head) => {
  if (req.url.startsWith('/ws')) {
    proxy.ws(req, socket, head, { target: gameTarget });
    return;
  }
  socket.destroy();
});

server.listen(gatewayPort, () => {
  console.log(`Gateway listening on ${gatewayPort}`);
  console.log(`API target: ${apiTarget}`);
  console.log(`WS target: ${gameTarget}`);
});
