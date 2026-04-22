class GameService {
  constructor(gameRepo) {
    this.gameRepo = gameRepo;
  }

  async createGame({ hostUserId, hostName = '', maxPlayers = 2, mode = 'realtime' }) {
    if (!hostUserId || typeof hostUserId !== 'string') throw new Error('hostUserId required');

    const allowedModes = new Set(['realtime', 'turns']);
    const cleanMode = typeof mode === 'string' ? mode.trim().toLowerCase() : 'realtime';
    if (!allowedModes.has(cleanMode)) throw new Error('invalid mode');

    const mp = Number(maxPlayers);
    if (!Number.isFinite(mp) || mp < 2 || mp > 8) throw new Error('invalid maxPlayers');

    const game = await this.gameRepo.create({ hostUserId: hostUserId.trim(), maxPlayers: mp, mode: cleanMode });

    // Ensure host is immediately part of the lobby so joiners can discover active sessions.
    await this.gameRepo.addPlayer(game.id, {
      userId: hostUserId.trim(),
      name: typeof hostName === 'string' ? hostName.trim() : undefined,
    });

    return this.gameRepo.getById(game.id);
  }

  async listOpen() {
    return this.gameRepo.listOpen();
  }

  async joinGame(gameId, player) {
    if (!gameId || typeof gameId !== 'string') throw new Error('gameId required');
    if (!player?.userId || typeof player.userId !== 'string') throw new Error('player.userId required');

    const clean = {
      userId: player.userId.trim(),
      name: typeof player.name === 'string' ? player.name.trim() : undefined,
    };

    return this.gameRepo.addPlayer(gameId.trim(), clean);
  }

  async updateStatus(gameId, status) {
    if (!gameId || typeof gameId !== 'string') throw new Error('gameId required');
    const allowedStatus = new Set(['open', 'running', 'finished']);
    const cleanStatus = typeof status === 'string' ? status.trim().toLowerCase() : '';
    if (!allowedStatus.has(cleanStatus)) throw new Error('invalid status');
    return this.gameRepo.updateStatus(gameId.trim(), cleanStatus);
  }
}

module.exports = GameService;
