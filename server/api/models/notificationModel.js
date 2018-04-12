'use strict';
let mongoose = require('mongoose');
let CharacterProperties = require('../utilities/characterProperties').CharacterProperties;
let Schema = mongoose.Schema;

let NotificationSchema = new Schema({
	userId: String,
	otherUserId: String,
	pictureId: String,
	liked: Boolean,
	characterProperties: CharacterProperties,
	createdDate: {
		type: Date,
		default: Date.now
	}
});

module.exports = mongoose.model('Notifications', NotificationSchema);