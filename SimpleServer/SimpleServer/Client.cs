using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer {
    class Client {
        Socket socket;
        NetworkStream stream;
        public StreamReader reader { get; private set; }
        public StreamWriter writer { get; private set; }

        public Client(Socket _socket) {
            socket = _socket;

            stream = new NetworkStream(socket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);


        }

        public void Close() {
            socket.Close();
        }
    }
}
