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

let CharacterColorProperties = new Schema({
	skinColor: Color3,
	hairColor: Color3,
	shirtColor: Color3,
	pantsColor: Color3
});

let CharacterSpriteProperties = new Schema({
	hairSprite: String,
	eyeSprite: String,
	birthmark: String
});

let CharacterLevelProperties = new Schema({
	avatarLevel: Number,
	happinessLevel: Number,
	fitnessLevel: Number,
	styleLevel: Number
});

let CharacterProperties = new Schema({
	gender: String,
	position: Vector3,
	rotation: Number,
	scale: Number,
	poseName: String,
	spriteProperties: CharacterSpriteProperties,
	colorProperties: CharacterColorProperties,
	levelProperties: CharacterLevelProperties
});

module.exports = {
	CharacterProperties
};