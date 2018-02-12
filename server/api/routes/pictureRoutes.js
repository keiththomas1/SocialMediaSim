'use strict';
module.exports = function(app) {
	let pictures = require('../controllers/pictureController');

	// todoList Routes
	app.route('/pictures')
		.post(pictures.createPicture);
		
	app.route('/listPictures/:count')
		.get(pictures.listPictures);
	
	app.route('/listPictures/:count/:timestamp')
		.get(pictures.listPicturesByTime);

	app.route('/pictures/:pictureID')
		.put(pictures.updatePicture)
		.get(pictures.readPicture)
		.delete(pictures.deletePicture);
	
	app.route('/liked/:pictureID')
		.put(pictures.incrementLikes);
	app.route('/disliked/:pictureID')
		.put(pictures.incrementDislikes);
};