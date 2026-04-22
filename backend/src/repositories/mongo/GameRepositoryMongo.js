const { GameRepository } = require('../interfaces');
const { GameModel } = require('./models');

class GameRepositoryMongo extends GameRepository {
  async create(game) {
    const doc = await GameModel.create(game);
    return {
      id: doc._id.toString(),
      hostUserId: doc.hostUserId,
      status: doc.status,
      maxPlayers: doc.maxPlayers,
      mode: doc.mode,
      players: doc.players,
      createdAt: doc.createdAt,
    };
  }

  async getById(id) {
    const doc = await GameModel.findById(id);
    if (!doc) return null;
    return {
      id: doc._id.toString(),
      hostUserId: doc.hostUserId,
      status: doc.status,
      maxPlayers: doc.maxPlayers,
      mode: doc.mode,
      players: doc.players,
      createdAt: doc.createdAt,
    };
  }

  async listOpen() {
    const docs = await GameModel.find({ status: 'open' }).sort({ createdAt: -1 });
    return docs
      .filter(doc => doc.players.length < doc.maxPlayers)
      .map(doc => ({
        id: doc._id.toString(),
        hostUserId: doc.hostUserId,
        status: doc.status,
        maxPlayers: doc.maxPlayers,
        mode: doc.mode,
        players: doc.players,
        createdAt: doc.createdAt,
      }));
  }

  async addPlayer(gameId, player) {
    const doc = await GameModel.findById(gameId);
    if (!doc) return null;

    if (doc.status !== 'open') {
      throw new Error('game is not open');
    }

    const exists = doc.players.find(p => p.userId === player.userId);
    if (!exists) {
      if (doc.players.length >= doc.maxPlayers) {
        throw new Error('game is full');
      }

      doc.players.push(player);
    }

    await doc.save();
    return {
      id: doc._id.toString(),
      hostUserId: doc.hostUserId,
      status: doc.status,
      maxPlayers: doc.maxPlayers,
      mode: doc.mode,
      players: doc.players,
      createdAt: doc.createdAt,
    };
  }

  async updateStatus(gameId, status) {
    const doc = await GameModel.findById(gameId);
    if (!doc) return null;
    doc.status = status;
    await doc.save();
    return {
      id: doc._id.toString(),
      hostUserId: doc.hostUserId,
      status: doc.status,
      maxPlayers: doc.maxPlayers,
      mode: doc.mode,
      players: doc.players,
      createdAt: doc.createdAt,
    };
  }
}

module.exports = GameRepositoryMongo;
