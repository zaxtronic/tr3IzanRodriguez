async function login(req, res) {
  const body = req.body || {};
  const { name, password } = body;
  try {
    const user = await req.services.userService.loginOrRegister(name, password);
    res.json(user);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

async function register(req, res) {
  const body = req.body || {};
  const { name, password } = body;
  try {
    const user = await req.services.userService.register(name, password);
    res.json(user);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
}

module.exports = { login, register };
