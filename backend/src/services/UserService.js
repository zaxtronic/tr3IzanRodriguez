const { hashPassword, verifyPassword } = require('../utils/password');

class UserService {
  constructor(userRepo) {
    this.userRepo = userRepo;
  }

  async loginOrRegister(name, password = '') {
    if (!name || typeof name !== 'string' || name.trim().length < 2) {
      throw new Error('Invalid name');
    }
    const clean = name.trim();
    const existing = await this.userRepo.getByName(clean);
    if (existing) {
      if (existing.passwordHash && password) {
        const ok = await verifyPassword(password, existing.passwordHash);
        if (!ok) throw new Error('Invalid credentials');
      }
      return existing;
    }

    const record = { name: clean };
    if (password) {
      record.passwordHash = await hashPassword(password);
    }
    return this.userRepo.create(record);
  }

  async register(name, password) {
    if (!name || typeof name !== 'string' || name.trim().length < 2) {
      throw new Error('Invalid name');
    }
    if (!password) throw new Error('password required');
    const clean = name.trim();
    const existing = await this.userRepo.getByName(clean);
    if (existing) throw new Error('User already exists');

    const passwordHash = await hashPassword(password);
    return this.userRepo.create({ name: clean, passwordHash });
  }
}

module.exports = UserService;
