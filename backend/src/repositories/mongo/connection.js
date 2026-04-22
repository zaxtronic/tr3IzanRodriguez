const mongoose = require('mongoose');

async function connectMongo(uri) {
  if (mongoose.connection.readyState === 1) return;
  await mongoose.connect(uri, {
    autoIndex: true,
  });
}

module.exports = { connectMongo, mongoose };
