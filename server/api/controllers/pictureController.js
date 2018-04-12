'use strict';


let mongoose = require('mongoose');
let Picture = mongoose.model('Pictures');
let notificationController = require('./notificationController');

// GET
exports.readPicture = function(request, response) {
  Picture.findById(request.params.pictureId, function(error, picture) {
    if (error) {
		console.error("readPicture error: ", error);
		response.send(error);
	}
	console.log("readPicture request. response:", picture);
    response.json(picture);
  });
};
exports.listPicturesByTime = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listPicturesByTime error: ", error);
		response.send(error);
	}
	console.log("listPicturesByTime request. response:", picture);
    response.json(picture);
  })
  .where('createdDate').lt(request.params.timestamp)
  .sort({ createdDate: 'desc' })
  .limit(request.params.count);
};
exports.listPicturesByUser = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listPicturesByUser error: ", error);
		response.send(error);
	}
	console.log("listPicturesByUser request with username "
		+ request.params.username
		+ ". response:",
		picture);
    response.json(picture);
  })
  .where('playerName').equals(request.params.username)
  .sort({ createdDate: 'desc' });
};
exports.listPictures = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listPictures error: ", error);
		response.send(error);
	}
	console.log("listPictures request. response:", picture);
    response.json(picture);
  })
  .sort({ createdDate: 'desc' })
  .limit(parseInt(request.params.count, 10)); // Base 10
};
exports.listFeedbackNeededPictures = function(request, response) {
  Picture.find({}, function(error, picture) {
    if (error) {
		console.error("listFeedbackNeededPictures error: ", error);
		response.send(error);
	}
	console.log("listFeedbackNeededPictures request. response:", picture);
    response.json(picture);
  })
  .sort({ totalFeedback: 'asc' })
  .limit(parseInt(request.params.count, 10)); // Base 10
};

// POST
exports.createPicture = function(request, response) {
  let new_picture = new Picture(request.body);
  new_picture.save(function(error, picture) {
    if (error) {
		console.error("createPicture error: ", error);
		response.send(error);
	}
	console.log("createPicture request. response:", picture);
    response.json(picture);
  });
};

// UPDATE
exports.updatePicture = function(request, response) {
  Picture.findOneAndUpdate(
	{_id: request.params.pictureID},
	request.body,
	{new: true},
	function(error, picture) {
		if (error) {
			console.error("updatePicture error: ", error);
			response.send(error);
		}
		console.log("updatePicture request. response:", picture);
		response.json(picture);
	}
  );
};
exports.incrementLikes = function(request, response) {
	var userId = "";
	var localPictureId = "";
	Picture.findOneAndUpdate(
		{_id: request.params.pictureID},
		{$inc: {"likes": 1, "totalFeedback": 1}},
		function(error, picture) {
			if (error) {
				console.error("incrementLikes error: ", error);
				response.send(error);
				return;
			}
			console.log("incrementLikes request with id=" + request.params.pictureID);
			response.json(picture);
			userId = picture.playerId;
			localPictureId = picture.localPictureId;
			
			if (!error) {
				// User.find({ userId: userId })
				// Get userProperties and pass to notification
				notificationController.createFeedbackNotification(
					userId, request.params.userID, localPictureId, true);
			}
		}
	);
};
exports.incrementDislikes = function(request, response) {
	var userId = "";
	var localPictureId = "";
	Picture.findOneAndUpdate(
		{_id: request.params.pictureID},
		{$inc: {"dislikes": 1}},
		function(error, picture) {
			if (error) {
				console.error("incrementDislikes error: ", error);
				response.send(error);
			}
			console.log("incrementDislikes request with id=" + request.params.pictureID);
			response.json(picture);
			userId = picture.playerId;
			localPictureId = picture.localPictureId;
			
			if (!error) {
				// User.find({ userId: userId })
				// Get userProperties and pass to notification
				notificationController.createFeedbackNotification(
					userId, request.params.userID, localPictureId, false);
			}
		}
	);
};

// DELETE
exports.deletePicture = function(request, response) {
  Picture.remove({
    _id: request.params.pictureID
  }, function(error, picture) {
    if (error) {
		console.error("deletePicture error: ", error);
		response.send(error);
	}
	console.log("deletePicture request. response:", picture);
    response.json({ message: 'picture successfully deleted' });
  });
};