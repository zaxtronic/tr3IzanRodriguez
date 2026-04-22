class SessionConfigService {
  constructor(sessionConfigRepo) {
    this.sessionConfigRepo = sessionConfigRepo;
  }

  async createConfig({ name = '', maxPlayers = 2, mode = 'realtime' }) {
    const allowedModes = new Set(['realtime', 'turns']);
    const cleanMode = typeof mode === 'string' ? mode.trim().toLowerCase() : 'realtime';
    if (!allowedModes.has(cleanMode)) throw new Error('invalid mode');

    const mp = Number(maxPlayers);
    if (!Number.isFinite(mp) || mp < 2 || mp > 8) throw new Error('invalid maxPlayers');

    const cleanName = typeof name === 'string' ? name.trim() : '';
    return this.sessionConfigRepo.create({ name: cleanName, maxPlayers: mp, mode: cleanMode });
  }

  async getById(id) {
    if (!id || typeof id !== 'string') throw new Error('id required');
    return this.sessionConfigRepo.getById(id.trim());
  }
}

module.exports = SessionConfigService;
