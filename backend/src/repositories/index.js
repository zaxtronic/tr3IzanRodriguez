const UserRepositoryMemory = require('./memory/UserRepositoryMemory');
const GameRepositoryMemory = require('./memory/GameRepositoryMemory');
const ResultRepositoryMemory = require('./memory/ResultRepositoryMemory');
const SessionConfigRepositoryMemory = require('./memory/SessionConfigRepositoryMemory');

const UserRepositoryMongo = require('./mongo/UserRepositoryMongo');
const GameRepositoryMongo = require('./mongo/GameRepositoryMongo');
const ResultRepositoryMongo = require('./mongo/ResultRepositoryMongo');
const SessionConfigRepositoryMongo = require('./mongo/SessionConfigRepositoryMongo');
const { connectMongo } = require('./mongo/connection');

async function createRepositories(config) {
  const mode = config.REPO_MODE || 'memory';

  if (mode === 'mongo') {
    await connectMongo(config.MONGO_URI);
    return {
      userRepo: new UserRepositoryMongo(),
      gameRepo: new GameRepositoryMongo(),
      resultRepo: new ResultRepositoryMongo(),
      sessionConfigRepo: new SessionConfigRepositoryMongo(),
    };
  }

  return {
    userRepo: new UserRepositoryMemory(),
    gameRepo: new GameRepositoryMemory(),
    resultRepo: new ResultRepositoryMemory(),
    sessionConfigRepo: new SessionConfigRepositoryMemory(),
  };
}

module.exports = { createRepositories };
