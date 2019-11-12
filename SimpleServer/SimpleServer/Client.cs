using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SharedClassLibrary;

namespace SimpleServer {
    class Client {
        public Socket tcpSocket;
        public Socket udpSocket;
        public NetworkStream stream;
        public BinaryReader reader { get; private set; }

        public int clientNumber;
        public string clientUsername;
        public Image profilePicture;

        SimpleServer serverInstance;

        public Client(Socket _socket, SimpleServer inst) {
            tcpSocket = _socket;
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            stream = new NetworkStream(tcpSocket);
            reader = new BinaryReader(stream);

            serverInstance = inst;
        }

        void UDPConnect(EndPoint clientConnection) {
            udpSocket.Connect(clientConnection);
            serverInstance.Send(new LoginPacket(udpSocket.LocalEndPoint), serverInstance.GetClientIndex(this));
        }

        public void Close() {
            tcpSocket.Close();
        }
    }
}
