const ipc=require('node-ipc');
const ns = require('./src/ns');
const NsArguement = require('./src/nsArguement');

function add(x, y) {
    return x + y;
}

function subtract(x, y) {
    return x - y;
}

console.log("Hello world");

ipc.config.id = 'world';
ipc.config.retry= 1500;
ipc.config.rawBuffer=true;

ipc.serve(
    function(){
        ipc.server.on(
            'connect',
            function(socket){
                console.log("Client connected");
            }
        );

        ipc.server.on(
            'data',
            function(data,socket){
                ipc.log('got a message', data,data.toString());
                var result;
                //read message to obj
                var jsonData = JSON.parse(data.toString());

                switch(jsonData.type) {
                    case 'test': 
                        result = "Good test";
                        break;
                    case 'function':
                        result = `${ns.resolve(jsonData.data)}`;
                        break;
                    default:
                        result = "ERROR! Unkown type, got " + jsonData.type;
                }

                ipc.server.emit(
                    socket,
                    result
                );
            }
        );
    }
);

console.log("Server running");

ipc.server.start();

ns.bind(add, (nsf) => {
    nsf.arguements = [ new NsArguement('int'), new NsArguement('int') ];
    nsf.returns = new NsArguement('int');
});

ns.bind(subtract, (nsf) => {
    nsf.arguements = [ new NsArguement('int'), new NsArguement('int') ];
    nsf.returns = new NsArguement('int');
})