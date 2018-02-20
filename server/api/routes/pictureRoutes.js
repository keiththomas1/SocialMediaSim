'use strict';
module.exports = function(app) {
	let pictures = require('../controllers/pictureController');

	// POST pictures
	app.route('/pictures')
		.post(pictures.createPicture);
		
	// GET pictures
	app.route('/listUserPictures/:username')
		.get(pictures.listPicturesByUser);
	app.route('/listPictures/:count')
		.get(pictures.listPictures);
	app.route('/listPictures/:count/:timestamp')
		.get(pictures.listPicturesByTime);
	app.route('/listFeedbackNeededPictures/:count')
		.get(pictures.listFeedbackNeededPictures);
	
	// Picture modifications
	app.route('/pictures/:pictureID')
		.put(pictures.updatePicture)
		.get(pictures.readPicture);
	app.route('/liked/:pictureID')
		.put(pictures.incrementLikes);
	app.route('/disliked/:pictureID')
		.put(pictures.incrementDislikes);
};