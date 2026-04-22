const { GameRepository } = require('../interfaces');
const { randomUUID } = require('crypto');

class GameRepositoryMemory extends GameRepository {
  constructor() {
    super();
    this.games = new Map();
  }

  async create(game) {
    const id = randomUUID();
    const record = {
      id,
      status: 'open',
      players: [],
      createdAt: new Date().toISOString(),
      ...game,
    };
    this.games.set(id, record);
    return record;
  }

  async getById(id) {
    return this.games.get(id) || null;
  }

  async listOpen() {
    return Array.from(this.games.values())
      .filter(g => g.status === 'open' && g.players.length < g.maxPlayers)
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
  }

  async addPlayer(gameId, player) {
    const game = this.games.get(gameId);
    if (!game) return null;

    if (game.status !== 'open') {
      throw new Error('game is not open');
    }

    const exists = game.players.find(p => p.userId === player.userId);
    if (!exists) {
      if (game.players.length >= game.maxPlayers) {
        throw new Error('game is full');
      }

      game.players.push(player);
    }

    return game;
  }

  async updateStatus(gameId, status) {
    const game = this.games.get(gameId);
    if (!game) return null;
    game.status = status;
    return game;
  }
}

module.exports = GameRepositoryMemory;
