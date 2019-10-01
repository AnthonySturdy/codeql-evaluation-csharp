using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer {
    class SimpleServer {
        TcpListener listener;

        public SimpleServer(string ipAddress, int port) {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start() {
            listener.Start();

            Socket socket = listener.AcceptSocket();

            Console.WriteLine("Socket accepted");

            SocketMethod(socket);
        }

        public void Stop() {
            listener.Stop();
        }

        void SocketMethod(Socket socket) {
            string receivedMessage;

            NetworkStream stream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            while((receivedMessage = reader.ReadLine()) != null) {
                string returnMsg = GetReturnMessage(receivedMessage);

                writer.WriteLine(returnMsg);
                writer.Flush();
            }

            socket.Close();
        }

        string GetReturnMessage(string code) {
            if(code == "Hi") {
                return "Hello";
            } else if (code == "Joke") {
                return "What do you call a zoo with only one dog? A shih-tzu.";
            } else {
                return "I don't know what to say to that.";
            }
        }

    }
}
