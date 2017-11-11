var express = require('express'),
  app = express(),
  port = process.env.PORT || 3000,
  mongoose = require('mongoose'),
  Task = require('./api/models/pictureModel'), //created model loading here
  bodyParser = require('body-parser');
  
// mongoose instance connection url connection
mongoose.Promise = global.Promise;
mongoose.connect(
	'mongodb://kryzodoze:jx6X3bz7N8CyQ6n4@cluster0-shard-00-00-qdbyz.mongodb.net:27017,cluster0-shard-00-01-qdbyz.mongodb.net:27017,cluster0-shard-00-02-qdbyz.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin');

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var routes = require('./api/routes/pictureRoutes'); //importing route
routes(app); //register the route

app.listen(port);

console.log('Delaygram\'s awesome server started on port ' + port);