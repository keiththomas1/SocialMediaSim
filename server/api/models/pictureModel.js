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

let CharacterColorProperties = new Schema({
	skinColor: Color3,
	hairColor: Color3,
	shirtColor: Color3,
	pantsColor: Color3
});

let CharacterSpriteProperties = new Schema({
	hairSprite: String,
	eyeSprite: String
});

let CharacterLevelProperties = new Schema({
	happinessLevel: Number,
	fitnessLevel: Number,
	styleLevel: Number
});

let CharacterProperties = new Schema({
	gender: String,
	position: Vector3,
	rotation: Number,
	scale: Number,
	spriteProperties: CharacterSpriteProperties,
	colorProperties: CharacterColorProperties,
	levelProperties: CharacterLevelProperties
});

let PictureSchema = new Schema({
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