'use strict';
module.exports = function(app) {
	let pictures = require('../controllers/pictureController');

	// todoList Routes
	app.route('/pictures')
		.get(pictures.listAllPictures)
		.post(pictures.createPicture);
	
	app.route('/lastTenPictures')
		.get(pictures.listLastTenPictures);

	app.route('/pictures/:pictureID')
		.put(pictures.updatePicture)
		.get(pictures.readPicture)
		.delete(pictures.deletePicture);
	
	app.route('/liked/:pictureID')
		.put(pictures.incrementLikes);
	app.route('/disliked/:pictureID')
		.put(pictures.incrementDislikes);
};