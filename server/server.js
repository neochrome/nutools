var fs = require('fs');
var http = require('http');
var url = require('url');

http.createServer(function(req, res){
	console.log(req.url);
	console.log(req.headers);

	var path = url.parse(req.url).pathname;
	switch(path){
		default:
			res.writeHead(200, {'Content-Type':'text/plain'});
			res.write('Usage:\n');
			res.write('/bin => a binary resource\n');
			res.write('/text => a plain text resource\n');
			res.write('/html => a html resource\n');
			res.write('/xml => a xml resource\n');
			res.write('/slow => a slow (5sek) request\n');
			res.end();
			break;
		case '/bin':
		case '/text':
		case '/html':
		case '/xml':
			fs.readFile('.' + path, function(err, data){
				if(err){
					console.log(err);
					res.writeHead(500, err.toString());
					res.end();
					return;
				}
				res.writeHead(200, {'Content-Length':data.length});
				res.end(data);
			});
			break;
		case '/slow':
			res.writeHead(200, {'Content-Type':'text/plain','Content-Length':5});
			setTimeout(function(){
				res.write('#');
				setTimeout(function(){
					res.write('#');
					setTimeout(function(){
						res.write('#');
						setTimeout(function(){
							res.write('#');
							setTimeout(function(){
								res.end('#');
							}, 1000);
						}, 1000);
					}, 1000);
				}, 1000);
			}, 1000);
			break;
	}
}).listen(1337, '127.0.0.1');
console.log('listening on localhost:1337');
