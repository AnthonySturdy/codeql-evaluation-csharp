using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClassLibrary;

namespace SimpleServer {
    class SimpleServer {
        TcpListener listener;
        MemoryStream memStream = new MemoryStream();    //Used to send packets
        BinaryFormatter binFormatter = new BinaryFormatter();

        List<Client> clients;

        public SimpleServer(string ipAddress, int port) {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new List<Client>();
        }

        public void Start() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("* SERVER START *");

            listener.Start();

            do {
                CreateClient();
            } while (clients.Count > 0);
        }

        public void Stop() {
            listener.Stop();
        }

        void CreateClient() {
            //Create socket
            Socket socket = listener.AcceptSocket();

            //Create client
            Client c = new Client(socket);
            if (clients.Count == 0) //Set up client number (for identification)
                c.clientNumber = 0;
            else
                c.clientNumber = clients[clients.Count - 1].clientNumber + 1;
            clients.Add(c);

            //Handle this client on a new thread
            Thread t = new Thread(new ParameterizedThreadStart(TCPClientMethod));  
            t.Start(c);
        }

        void TCPClientMethod(object clientObj) {
            //This function is ran on its own thread recieving messages from an individual client

            Client client = (Client)clientObj;

            while (client.tcpSocket.Connected) {
                HandlePacket(client.TCPRead(), client);
            }

            //CLIENT EXIT HANDLED HERE
            //When program gets here, client has left so remove from client list
            client.Close();
            clients.Remove(client);

            //Announce quit to remaining clients
            MessageAllClients(client.clientUsername + " exited.");

            //Create and send client list packets when Client disconnects
            List<Tuple<Image, string>> clientInfo = new List<Tuple<Image, string>>();
            for (int i = 0; i < clients.Count; i++) {
                clientInfo.Add(new Tuple<Image, string>(clients[i].profilePicture, clients[i].clientUsername));
            }
            SendPacketToAllClients(new ClientListPacket(clientInfo)); //Loop again to send packet to all clients
        }

        void UDPClientMethod(object clientObj) {
            Client client = (Client)clientObj;

            while (client.udpSocket.Connected) {
                HandlePacket(client.UDPRead(), client);
            }
        }

        void HandlePacket(Packet p, Client client) {
            switch (p.type) {
                case PacketType.CHATMESSAGE:
                    ChatMessagePacket chatPacket = (ChatMessagePacket)p;
                    MessageAllClients(client.clientUsername + ": " + chatPacket.message);
                    break;

                case PacketType.IMAGEMESSAGE:
                    ImageMessagePacket imgPacket = (ImageMessagePacket)p;
                    MessageAllClients(client.clientUsername + ": ");    //Send this so others know who sent the image
                    Console.WriteLine("IMAGE SENT");
                    SendPacketToAllClients(imgPacket);
                    MessageAllClients("");    //New line after image sent
                    break;

                case PacketType.USERINFO:
                    UserInfoPacket userPacket = (UserInfoPacket)p;
                    client.clientUsername = userPacket.username;
                    client.profilePicture = userPacket.profilePicture;

                    MessageAllClients(client.clientUsername + " joined.");  //Announce join

                    //Create client list packet when Client joins
                    List<Tuple<Image, string>> info = new List<Tuple<Image, string>>();
                    for (int i = 0; i < clients.Count; i++) {
                        info.Add(new Tuple<Image, string>(clients[i].profilePicture, clients[i].clientUsername));
                    }
                    SendPacketToAllClients(new ClientListPacket(info));
                    break;

                case PacketType.LOGINPACKET:
                    LoginPacket loginPacket = (LoginPacket)p;

                    client.UDPConnect(loginPacket.endpoint);

                    Thread t = new Thread(new ParameterizedThreadStart(UDPClientMethod));
                    t.Start(client);
                    break;
            }
        }

        void MessageAllClients(string message) {
            if (message.Contains(':'))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);

            Packet p = new ChatMessagePacket(message);

            for (int i = 0; i < clients.Count; i++) {
                clients[i].UDPSend(p);
            }
        }

        void SendPacketToAllClients(Packet p) {
            for(int i = 0; i < clients.Count; i++) {
                clients[i].TCPSend(p);
            }
        }

        public int GetClientIndex(Client c) {
            for(int i = 0; i < clients.Count; i++) {
                if (clients[i] == c)
                    return i;
            }

            return -1;
        }
        public int GetClientIndex(string username) {
            for (int i = 0; i < clients.Count; i++) {
                if (clients[i].clientUsername == username)
                    return i;
            }

            return -1;
        }

    }
}
