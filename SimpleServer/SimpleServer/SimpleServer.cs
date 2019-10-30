using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Shared;

namespace SimpleServer {
    class SimpleServer {
        TcpListener listener;
        MemoryStream memStream = new MemoryStream();
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
            //Cast object to Client object
            Client client = (Client)clientObj;


            Console.WriteLine("Client " + client.clientNumber + " joined.");
            MessageAllClients("Client " + client.clientNumber + " joined.");

            int noOfIncomingBytes;
            while ((noOfIncomingBytes = client.reader.ReadInt32()) != 0) {
                byte[] buffer = client.reader.ReadBytes(noOfIncomingBytes); //Read bytes to array
                MemoryStream ms = new MemoryStream(buffer);
                Packet p = binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet

                switch (p.type) {
                    case PacketType.CHATMESSAGE:
                        ChatMessagePacket _p = binFormatter.Deserialize(memStream) as ChatMessagePacket;
                        Console.WriteLine("Client " + client.clientNumber + ": " + _p.message);    //Write incoming messages to server window
                        MessageAllClients("Client " + client.clientNumber + ": " + _p.message);     
                        break;
                }
                
            }

            int clientExitNum = client.clientNumber;

            client.Close();
            clients.Remove(client);

            Console.WriteLine("Client " + client.clientNumber + " exited.");
            MessageAllClients("Client " + client.clientNumber + " exited.");
        }

        string GetReturnMessage(string code) {
            string capMsg = code.ToUpper();

            if (capMsg == "HI" || capMsg == "HELLO" || capMsg == "HEY") {
                return "Hello";
            } else if (capMsg == "JOKE" || capMsg == "TELL ME A JOKE") {
                return "What do you call a zoo with only one dog? A shih-tzu.";
            } else if (capMsg == "BYE") {
                return "Bye";
            } else {
                return "I don't know what to say to that.";
            }
        }

        void MessageAllClients(string message) {
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

            memStream.Dispose();
        }

    }
}
