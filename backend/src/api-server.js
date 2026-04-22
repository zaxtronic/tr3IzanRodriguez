require('dotenv').config();
const express = require('express');
const cors = require('cors');
const { createRepositories } = require('./repositories');
const UserService = require('./services/UserService');
const GameService = require('./services/GameService');
const ResultService = require('./services/ResultService');
const SessionConfigService = require('./services/SessionConfigService');

const userRoutes = require('./routes/userRoutes');
const gameRoutes = require('./routes/gameRoutes');
const resultRoutes = require('./routes/resultRoutes');
const sessionConfigRoutes = require('./routes/sessionConfigRoutes');

async function start() {
  const app = express();
  app.use(cors());
  app.use(express.json({ type: '*/*' }));

  const repos = await createRepositories({
    REPO_MODE: process.env.REPO_MODE,
    MONGO_URI: process.env.MONGO_URI,
  });

  const services = {
    userService: new UserService(repos.userRepo),
    gameService: new GameService(repos.gameRepo),
    resultService: new ResultService(repos.resultRepo),
    sessionConfigService: new SessionConfigService(repos.sessionConfigRepo),
  };

  app.use((req, res, next) => {
    req.services = services;
    next();
  });

  app.get('/health', (req, res) => res.json({ ok: true }));
  app.use('/api/users', userRoutes);
  app.use('/api/games', gameRoutes);
  app.use('/api/results', resultRoutes);
  app.use('/api/session-configs', sessionConfigRoutes);

  const port = process.env.API_PORT || 3001;
  app.listen(port, () => {
    console.log(`API server listening on ${port}`);
  });
}

start();
