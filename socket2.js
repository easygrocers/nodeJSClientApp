// Sample based on: http://elegantcode.com/2011/05/04/taking-baby-steps-with-node-js-websockets/
// Changed for sockets.io 6.x => 7.x
// Sung: found this at http://stackoverflow.com/questions/7189849/socket-io-issue-with-control-chars
var express = require('express');
var app = express();
var htp = require('http').createServer(app);
var socketIO = require('socket.io').listen(htp);

var events = require('events');
var eventEmitter = new events.EventEmitter();

var static = require('node-static');

// code for client stuff
htp.listen(8000);

app.use("/", express.static(__dirname + '/public'));


// code for handling socket stuff
var lines = require('lines-adapter');
var net = require('net');

// prevents the server from crashing
process.on('uncaughtException', function (err) {
  console.error(err);
  console.log("Node NOT Exiting...");
});

// sends message to clients
socketIO.sockets.on('connection', function (socket) {    
    socket.on('msg', function (data) {
        socketIO.sockets.emit('new', data);
  });
});


// code below was used for testing other stuff
/*
var server = net.createServer(function (socket) {
  
    var name = '';
  
    //utf16le
    socket.setEncoding('utf8'); 
    socket.on('connect', function() {
        console.log("Connected!");
        socket.emit('partInfo' , "hello");
    }); 
    
*/
    
/*    socket.on('data', function(data) {
        if (name !== '')   
            console.log("name " + name) 
        else {
            name = data.toString('ascii').substring(0, 12).trim().replace(/\s/g, '_');
            console.log('> You have joined as ' + name + '\r\n');
        }
        
        console.log(data + ' received from the client');
        socketIO.sockets.emit('new', data); 
        socket.write("hello");
        socket.write(data, 'utf8', function() {
            console.log(data + "data sent to client " + socket.address().address + " " + socket.bytesWritten);
            socket.end();});
        
    });
	socket.on('close', function() {
        console.log('server disconnected');
  });
    
});

server.listen(1337);*/
 