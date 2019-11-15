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
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleServer {
    class Client {
        public Socket tcpSocket;
        public Socket udpSocket;
        public NetworkStream stream;
        public BinaryReader reader { get; private set; }
        public MemoryStream memStream;
        public BinaryFormatter binFormatter;

        public int clientNumber;
        public string clientUsername;
        public Image profilePicture;

        public Client(Socket _socket) {
            tcpSocket = _socket;
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            stream = new NetworkStream(tcpSocket);
            reader = new BinaryReader(stream);
            memStream = new MemoryStream();
            binFormatter = new BinaryFormatter();
        }

        public void UDPConnect(EndPoint clientConnection) {
            udpSocket.Connect(clientConnection);
            TCPSend(new LoginPacket(udpSocket.LocalEndPoint));
        }

        public void TCPSend(Packet data) {
            BinaryWriter _writer = new BinaryWriter(stream);

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();

            memStream.SetLength(0);
        }

        public void UDPSend(Packet data) {
            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            udpSocket.Send(buffer);
        }

        public Packet TCPRead() {
            int noOfIncomingBytes;
            while ((noOfIncomingBytes = reader.ReadInt32()) != 0) {
                //Retrieve and deserialize packet
                Packet p = DeserialisePacket(reader.ReadBytes(noOfIncomingBytes));
                if (p.type == PacketType.DISCONNECT)
                    Close();

                return p; 
            }

            return null;
        }

        public Packet UDPRead() {
            byte[] bytes = new byte[256];
            int noOfIncomingBytes;
            try {
                if ((noOfIncomingBytes = udpSocket.Receive(bytes)) != 0) {
                    Packet p = DeserialisePacket(reader.ReadBytes(noOfIncomingBytes));
                    if (p.type == PacketType.DISCONNECT)
                        Close();

                    return p;
                }
            } catch (SocketException e) {
                Console.WriteLine(e.Message); //Get an error when user disconnects, this removes it
                return new Packet();
            }

            return new Packet();
        }

        public Packet DeserialisePacket(byte[] buffer) {
            MemoryStream ms = new MemoryStream(buffer);
            return binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet
        }

        public void Close() {
            udpSocket.Close();
            tcpSocket.Close();
        }
    }
}
