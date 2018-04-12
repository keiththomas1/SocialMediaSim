'use strict';


let mongoose = require('mongoose');
let User = mongoose.model('Users');

// POST
exports.createUser = function(request, response) {
  let newUser = new User(request.body);
  newUser.save(function(error, user) {
    if (error) {
		console.error("createUser error with: ", user);
		response.send(error);
	}
	console.log("createUser request. response:", user);
    response.json(user);
  });
};

// GET
exports.findUser = function(request, response) {
  User.findById(request.params.id, function(error, user) {
    if (error) {
		console.error("findUser error with: ", user);
		response.send(error);
	}
	console.log("findUser request. response:", user);
    response.json(user);
  });
};
