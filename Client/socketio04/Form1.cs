using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using socketio04;
using Newtonsoft;
using SocketIOClient;
using SocketIOClient.Messages;
using WebSocket4Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;

namespace socketio04
{
    public partial class Form1 : Form
    {
        Client socket;
        delegate void SetTextCallback(string text);
        private Thread txtUpdateThread = null;

        public void Execute()
        {
            Console.WriteLine("Starting TestSocketIOClient Example...");

            socket = new Client("http://127.0.0.1:8000/"); // url to nodejs server 

            socket.SocketConnectionClosed += SocketConnectionClosed;
            socket.Error += SocketError;

            // register for 'connect' event with io server
            socket.On("connect", (fn) =>
            {
                Console.WriteLine("\r\nConnected event...\r\n");
            });

            
            // received new message from the server
            socket.On("new", (data) =>
            {
                // json returned is in a wieird format ex: {"name":"new","args":[{"name":"sung","msg":"wefwef"}]} grab the right string from that (second part is what we're interested in)
                string jsonstr = data.Json.Args[0].ToString();
                
                // convert json string to message object
                Message msg = JsonConvert.DeserializeObject<Message>(jsonstr);
                
                // start a new thread to safely append the textbox text
                this.txtUpdateThread = new Thread(() => this.SetText(msg.name + ": " + msg.msg + "\r\n"));
                this.txtUpdateThread.Start();
                
            });

            // make the socket.io connection
            socket.Connect();
        }

        // thread safe way of appeneding text
        // http://msdn.microsoft.com/en-us/library/ms171728%28VS.80%29.aspx
        // http://stackoverflow.com/questions/3360555/how-to-pass-parameters-to-threadstart-method-in-thread
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text);
            }
        }

        public Form1()
        {
            InitializeComponent();
            Execute();
            this.AcceptButton = button1;
        }

        // send the messsage to the server
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("You must enter a name");
            }
            else
            {
                Console.WriteLine("Message Sent");
                Message msg = new Message();
                msg.name = textBox2.Text;
                msg.msg = textBox3.Text;
                socket.Emit("msg", msg);
                textBox3.Text = "";
            }
        }


        // code below doesn't really do anything
        void SocketError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("socket client error:");
            Console.WriteLine(e.Message);
        }

        void SocketConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocketConnection was terminated!");
        }


    }
}
