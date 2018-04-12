'use strict';

let mongoose = require('mongoose');
let Notification = mongoose.model('Notifications');

// POST
exports.createFeedbackNotification = function(userId, otherUserId, pictureId, liked, characterProperties) {
  let newNotification = new Notification();
  newNotification.userId = userId;
  newNotification.otherUserId = otherUserId;
  newNotification.pictureId = pictureId;
  newNotification.liked = liked;
  if (characterProperties) {
	newNotification.characterProperties = characterProperties;
  }
  
  newNotification.save(function(error, notification) {
    if (error) {
		console.error("createFeedbackNotification error: ", error);
	}
  });
};

exports.getNotificationsByUserId = function(request, response) {
  // First find any notifications and return them to user
  Notification.find({ userId: request.params.userId }, function(error, picture) {
    if (error) {
		console.error("getNotificationsByUserId error: ", error);
		response.send(error);
	}
	console.log("getNotificationsByUserId request with userId "
		+ request.params.userId
		+ ". response:",
		picture);
    response.json(picture);
	
    // Now delete them all since they are no longer needed
	Notification.remove({ userId: request.params.userId}, function(error, picture) {
		if (error) {
			console.error("getNotificationsByUserId removal error: ", error);
		}
	});
  })
  .sort({ createdDate: 'desc' });
}