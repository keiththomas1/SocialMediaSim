'use strict';
let mongoose = require('mongoose');
let CharacterProperties = require('../utilities/characterProperties').CharacterProperties;
let Schema = mongoose.Schema;

let UserSchema = new Schema({
	userId: String,
	displayName: String,
	characterProperties: CharacterProperties,
	createdDate: {
		type: Date,
		default: Date.now
	}
});

module.exports = mongoose.model('Users', UserSchema);