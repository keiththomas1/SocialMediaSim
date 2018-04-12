'use strict';
let mongoose = require('mongoose');
let CharacterProperties = require('../utilities/characterProperties').CharacterProperties;
let Schema = mongoose.Schema;

let Vector3 = new Schema({
	x: Number,
	y: Number,
	z: Number
});
let PictureItem = new Schema({
	name: String,
	location: Vector3,
	rotation: Number,
	scale: Number
});

let PictureSchema = new Schema({
	localPictureId: String,
	playerId: String,
	playerName: String,
	backgroundName: String,
	characterProperties: CharacterProperties,
	likes: {
		type: Number,
		default: 0
	},
	dislikes: {
		type: Number,
		default: 0
	},
	totalFeedback: {
		type: Number,
		default: 0
	},
	items: [PictureItem],
	createdDate: {
		type: Date,
		default: Date.now
	},
});

module.exports = mongoose.model('Pictures', PictureSchema);