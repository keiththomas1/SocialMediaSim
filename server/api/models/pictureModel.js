'use strict';
let mongoose = require('mongoose');
let Schema = mongoose.Schema;

let Vector3 = new Schema({
	x: Number,
	y: Number,
	z: Number
});
let Color3 = new Schema({
	red: Number,
	green: Number,
	blue: Number
});
let PictureItem = new Schema({
	name: String,
	location: Vector3,
	rotation: Number,
	scale: Number
});

let PictureSchema = new Schema({
	playerName: {
		type: String
	},
	backgroundName: {
		type: String
	},
	createdDate: {
		type: Date,
		default: Date.now
	},
	avatarPosition: {
		type: Vector3
		//required: 'Missing the avatar position'
	},
	avatarRotation: Number,
	avatarScale: Number,
	skinColor: Color3,
	hairColor: Color3,
	shirtColor: Color3,
	pantsColor: Color3,
	gender: String,
	hairSprite: String,
	faceSprite: String,
	eyeSprite: String,
	bodySprite: String,
	likes: {
		type: Number,
		default: 0
	},
	dislikes: {
		type: Number,
		default: 0
	},
	items: [PictureItem]
});

module.exports = mongoose.model('Pictures', PictureSchema);