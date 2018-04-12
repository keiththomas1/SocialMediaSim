'use strict';
module.exports = function(app) {
	let users = require('../controllers/userController');

	// POST users
	app.route('/users')
		.post(users.createUser);
		
	// GET users
	app.route('/users/:id')
		.get(users.findUser);
};