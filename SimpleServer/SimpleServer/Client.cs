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
        public NetworkStream stream;
        public BinaryReader reader { get; private set; }
        //public StreamWriter writer { get; private set; }
        public int clientNumber;
        public string clientUsername;

        public Client(Socket _socket) {
            socket = _socket;

            stream = new NetworkStream(socket);
            reader = new BinaryReader(stream);
            //writer = new StreamWriter(stream);
        }

        public void Close() {
            socket.Close();
        }
    }
}
