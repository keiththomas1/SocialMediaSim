'use strict';


let mongoose = require('mongoose');
let Picture = mongoose.model('Pictures');

// GET
exports.readPicture = function(request, response) {
  Picture.findById(request.params.pictureId, function(error, picture) {
    if (error) {
		console.error("readPicture error with: ", picture);
		response.send(error);
	}
	console.log("readPicture request. response:", picture);
    response.json(picture);
  });
};
exports.listPicturesByTime = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listLastTenPictures error with: ", picture);
		response.send(error);
	}
	console.log("listLastTenPictures request. response:", picture);
    response.json(picture);
  })
  .where('createdDate').lt(request.params.timestamp)
  .sort({ createdDate: 'desc' })
  .limit(request.params.count);
};
exports.listPictures = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listLastTenPictures error with: ", picture);
		response.send(error);
	}
	console.log("listLastTenPictures request. response:", picture);
    response.json(picture);
  })
  .sort({ createdDate: 'desc' })
  .limit(parseInt(request.params.count, 10));
};

// POST
exports.createPicture = function(request, response) {
  let new_picture = new Picture(request.body);
  new_picture.save(function(error, picture) {
    if (error) {
		console.error("createPicture error with: ", picture);
		response.send(error);
	}
	console.log("createPicture request. response:", picture);
    response.json(picture);
  });
};

// UPDATE
exports.updatePicture = function(request, response) {
  Picture.findOneAndUpdate({_id: request.params.pictureID}, request.body, {new: true}, function(error, picture) {
    if (error) {
		console.error("updatePicture error with: ", picture);
		response.send(error);
	}
	console.log("updatePicture request. response:", picture);
    response.json(picture);
  });
};
exports.incrementLikes = function(request, response) {
	Picture.findOneAndUpdate({_id: request.params.pictureID}, {$inc: {"likes": 1}}, function(error, picture) {
		if (error) {
			console.error("incrementLikes error with: ", picture);
			response.send(error);
		}
		console.log("incrementLikes request with id=" + request.params.pictureID);
		response.json(picture);
	});
};
exports.incrementDislikes = function(request, response) {
	Picture.findOneAndUpdate({_id: request.params.pictureID}, {$inc: {"dislikes": 1}}, function(error, picture) {
		if (error) {
			console.error("incrementDislikes error with: ", picture);
			response.send(error);
		}
		console.log("incrementDislikes request with id=" + request.params.pictureID);
		response.json(picture);
	});
};

// DELETE
exports.deletePicture = function(request, response) {
  Picture.remove({
    _id: request.params.pictureID
  }, function(error, picture) {
    if (error) {
		console.error("deletePicture error with: ", picture);
		response.send(error);
	}
	console.log("deletePicture request. response:", picture);
    response.json({ message: 'picture successfully deleted' });
  });
};