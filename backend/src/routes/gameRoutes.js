const express = require('express');
const { createGame, listGames, joinGame, updateStatus } = require('../controllers/gameController');

const router = express.Router();
router.post('/', createGame);
router.get('/', listGames);
router.post('/:id/join', joinGame);
router.post('/:id/status', updateStatus);

module.exports = router;
