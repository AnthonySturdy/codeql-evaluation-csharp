using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
            Thread t = new Thread(new ParameterizedThreadStart(ClientMethod));  
            t.Start(c);
        }

        void ClientMethod(object clientObj) {
            //This function is ran on its own thread for each individual client

            //Cast object to Client object
            Client client = (Client)clientObj;

            bool exitLoop = false;
            int noOfIncomingBytes;
            while ((noOfIncomingBytes = client.reader.ReadInt32()) != 0) {
                //Retrieve and deserialize packet
                byte[] buffer = client.reader.ReadBytes(noOfIncomingBytes); //Read bytes to array
                MemoryStream ms = new MemoryStream(buffer);
                Packet p = binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet

                //Use packets depending on type
                switch (p.type) {
                    case PacketType.CHATMESSAGE:
                        ChatMessagePacket chatPacket = (ChatMessagePacket)p;
                        MessageAllClients(client.clientUsername + ": " + chatPacket.message);     
                        break;

                    case PacketType.USERINFO:
                        UserInfoPacket userPacket = (UserInfoPacket)p;
                        client.clientUsername = userPacket.username;
                        MessageAllClients(client.clientUsername + " joined.");
                        break;

                    case PacketType.DISCONNECT:
                        exitLoop = true;
                        break;
                }

                if (exitLoop)
                    break;  //This is checked here (instead of the loop condition) because running client.reader.ReadInt32() after user disconnects, crashes the server
            }

            //When program gets here, client has left so remove from client list
            client.Close();
            clients.Remove(client);

            //Announce quit to remaining clients
            MessageAllClients(client.clientUsername + " exited.");
        }

        void MessageAllClients(string message) {
            Console.WriteLine(message);

            Packet p = new ChatMessagePacket(message);

            for (int i = 0; i < clients.Count; i++) {
                Send(p, i);
            }
        }

        public void Send(Packet data, int clientIndex) {
            BinaryWriter _writer = new BinaryWriter(clients[clientIndex].stream);

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();

            memStream.SetLength(0);
        }

    }
}
