//Create a chat module to use.
(function () {
  window.Chat = {
    socket : null,
  
    initialize : function(socketURL) {
      this.socket = io.connect(socketURL);

      //Send message on button click or enter
      $('#send').click(function() {
        Chat.send();
      });

      $('#msg').keyup(function(evt) {
        if ((evt.keyCode || evt.which) == 13) {
          Chat.send();
          return false;
        }
      });

      //Process any incoming messages
      this.socket.on('new', this.add);
    },

    //Adds a new message to the chat.
    add : function(data) {
      var name = '<h5>'+data.name+'</h5>' || '<h5>anonymous</h5>';
      var msg = $('<div class="msg">' + name + ': </div>')
        .append('<span class="text">' + data.msg + '</span>');

      $('#messages')
        .append(msg)
        .animate({scrollTop: $('#messages').prop('scrollHeight')}, 0);
    },
 
    //Sends a message to the server,
    //then clears it from the textarea
    send : function() {
      this.socket.emit('msg', {
        name: $('#name').val(),
        msg: $('#msg').val()
      });

      $('#msg').val('');

      return false;
    }
  };
}());