const express = require('express');
const { createSessionConfig, getSessionConfig } = require('../controllers/sessionConfigController');

const router = express.Router();
router.post('/', createSessionConfig);
router.get('/:id', getSessionConfig);

module.exports = router;
