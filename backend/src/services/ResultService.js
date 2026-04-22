class ResultService {
  constructor(resultRepo) {
    this.resultRepo = resultRepo;
  }

  async saveResult(gameId, data) {
    if (!gameId || typeof gameId !== 'string') throw new Error('gameId required');
    if (!data || typeof data !== 'object') throw new Error('data required');

    const clean = {
      userId: typeof data.userId === 'string' ? data.userId.trim() : '',
      playerName: typeof data.playerName === 'string' ? data.playerName.trim() : '',
      score: Number.isFinite(Number(data.score)) ? Number(data.score) : 0,
      duration: Number.isFinite(Number(data.duration)) ? Number(data.duration) : 0,
      winnerId: typeof data.winnerId === 'string' ? data.winnerId.trim() : '',
    };

    if (!clean.userId) throw new Error('data.userId required');

    return this.resultRepo.create({ gameId: gameId.trim(), data: clean });
  }

  async listByGame(gameId) {
    if (!gameId || typeof gameId !== 'string') throw new Error('gameId required');
    return this.resultRepo.listByGame(gameId.trim());
  }
}

module.exports = ResultService;
