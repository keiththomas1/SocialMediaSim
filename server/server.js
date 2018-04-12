var express = require('express'),
  app = express(),
  port = process.env.PORT || 3000,
  mongoose = require('mongoose'),
  Task = require('./api/models/pictureModel'), //created model loading here
  Task = require('./api/models/userModel'), //created model loading here
  Task = require('./api/models/notificationModel'), //created model loading here
  bodyParser = require('body-parser'),
  RateLimit = require('express-rate-limit');
  
// mongoose instance connection url connection
mongoose.Promise = global.Promise;
mongoose.connect(
	'mongodb://kryzodoze:jx6X3bz7N8CyQ6n4@cluster0-shard-00-00-qdbyz.mongodb.net:27017,cluster0-shard-00-01-qdbyz.mongodb.net:27017,cluster0-shard-00-02-qdbyz.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin');

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.enable('trust proxy'); // Since we are behind a reverse proxy (Amazon EBS)

var postLimiter = new RateLimit({
  windowMs: 10*60*1000, // 15 minutes
  max: 100, // limit each IP to 10 requests per windowMs
  delayMs: 0 // disable delaying - full speed until the max limit is reached
});

var pictureRoutes = require('./api/routes/pictureRoutes'); //importing route
pictureRoutes(app); //register the route
var userRoutes = require('./api/routes/userRoutes'); //importing route
userRoutes(app); //register the route
var notificationRoutes = require('./api/routes/notificationRoutes'); //importing route
notificationRoutes(app); //register the route

app.listen(port);
app.use(postLimiter);

console.log('Delaygram\'s awesome server started on port ' + port);