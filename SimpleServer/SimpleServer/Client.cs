using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer {
    class Client {
        public Socket socket;
        public NetworkStream stream;
        public BinaryReader reader { get; private set; }

        public int clientNumber;
        public string clientUsername;
        public Image profilePicture;

        public Client(Socket _socket) {
            socket = _socket;

            stream = new NetworkStream(socket);
            reader = new BinaryReader(stream);
        }

        public void Close() {
            socket.Close();
        }
    }
}
