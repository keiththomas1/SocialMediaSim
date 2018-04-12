'use strict';
module.exports = function(app) {
	let notifications = require('../controllers/notificationController');

	app.route('/notifications/:userId')
		.get(notifications.getNotificationsByUserId);
};