var fs = require('fs');
var http = require('http');
var url = require('url');
var qs = require('querystring');

var range = "abcdefghijklmnopqrstuvwxyz0123456789";
var block = function(size){
	var data = "";
	for(var i = 0; i < size; i++){
		data += range[parseInt(Math.random() * range.length, 10)];
	}
	return data;
};

http.createServer(function(req, res){
	console.log(req.url);
	console.log(req.headers);

	var path = url
		.parse(req.url)
		.pathname
		.split('/')
		.filter(function(itm){return itm.length > 0;});
	var query = qs.parse(url.parse(req.url).query);
	query.nosize = query.nosize !== undefined;

	var size = (path[0] ? parseInt(path[0], 10) : undefined) || 1024;
	var chunks = (path[1] ? parseInt(path[1], 10) : undefined) || 1;
	if (size === NaN || chunks === NaN){
		res.writeHead(400, "bad request");
		res.end();
		return;
	}
	var chunkSize = parseInt(size / chunks, 10);
	
	var chunked = function(left){
		var bsize = (left -= chunkSize) < 0 ? left + chunkSize : chunkSize;
		res.write(block(bsize));
		if(left <= 0){ res.end(); return; }
		setTimeout(function(){ chunked(left);}, 500);
	};

	res.writeHead(200, query.nosize ? {} : {'Content-Length':size});
	chunked(size);
}).listen(1337, '127.0.0.1');
console.log('listening on localhost:1337');
