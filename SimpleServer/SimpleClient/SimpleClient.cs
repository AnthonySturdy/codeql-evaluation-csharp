using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO;
using System.Net;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClassLibrary;

namespace SimpleClient {
    public class SimpleClient {
        IPAddress ip;
        int port;

        TcpClient tcpClient;
        UdpClient udpClient;
        StreamWriter writer;
        BinaryReader reader;
        NetworkStream stream;

        ClientForm messageForm;
        MonoGameForm gameForm;

        Thread readerThread;

        MemoryStream memStream = new MemoryStream();    //Used to send packets
        BinaryFormatter binFormatter = new BinaryFormatter();

        public SimpleClient() {
            messageForm = new ClientForm(this);
            Application.Run(messageForm);
        }

        public bool Connect(string _ipAddress, int _port, string username, Image profilePic) {
            try {
                ip = IPAddress.Parse(_ipAddress);
                port = _port;

                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);

                stream = tcpClient.GetStream();
                writer = new StreamWriter(stream);
                reader = new BinaryReader(stream);

                udpClient = new UdpClient();
                udpClient.Connect(ip, port);
                TCPSend(new LoginPacket(udpClient.Client.LocalEndPoint));

                Run(username, profilePic);
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

            return true;
        }

        public void Run(string username, Image profilePic) {
            readerThread = new Thread(TCPRead);
            readerThread.Start();

            Packet p = new UserInfoPacket(username, profilePic);
            TCPSend(p);
        }

        public void Stop() {
            TCPSend(new DisconnectPacket());   //Let the server know the client is about to disconnect

            if(readerThread != null)
                readerThread.Abort();

            if(tcpClient != null)
                tcpClient.Close();
        }

        public void SendMessage(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                Packet p = new ChatMessagePacket(message);
                TCPSend(p);
            }
        }

        public void TCPSend(Packet data) {
            if (tcpClient == null)
                return;

            if (!tcpClient.Connected) 
                return;

            BinaryWriter _writer = new BinaryWriter(stream);

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();

            memStream.SetLength(0);
        }

        public void UDPSend(Packet data) {
            if (udpClient == null)
                return;

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            udpClient.Send(buffer, buffer.Length);

            memStream.SetLength(0);
        }

        Packet DeserialisePacket(byte[] buffer) {
            MemoryStream ms = new MemoryStream(buffer);
            return binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet
        }

        void TCPRead() {
            int noOfIncomingBytes;

            while((noOfIncomingBytes = reader.ReadInt32()) != 0) {
                byte[] buffer = reader.ReadBytes(noOfIncomingBytes); //Read bytes to array
                Packet p = DeserialisePacket(buffer);
                HandlePacket(p);
            }

        }

        void UDPRead() {
            while (true) {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = udpClient.Receive(ref endpoint);
                Packet p = DeserialisePacket(buffer);
                HandlePacket(p);
            }
        }

        void HandlePacket(Packet p) {
            switch (p.type) {
                case PacketType.CHATMESSAGE:
                    ChatMessagePacket chatPacket = (ChatMessagePacket)p;
                    messageForm.UpdateChatWindow(chatPacket.message);
                    break;

                case PacketType.IMAGEMESSAGE:
                    ImageMessagePacket imgPacket = (ImageMessagePacket)p;
                    messageForm.AddImageToChatWindow(imgPacket.image);
                    break;

                case PacketType.CLIENTLIST:
                    ClientListPacket clientListPacket = (ClientListPacket)p;
                    messageForm.PopulateClientList(clientListPacket.clientInformation);
                    break;

                case PacketType.LOGINPACKET:
                    LoginPacket loginPacket = (LoginPacket)p;
                    udpClient.Connect((IPEndPoint)loginPacket.endpoint);
                    Thread t = new Thread(UDPRead);
                    t.Start();
                    break;

                case PacketType.GAME_REQUEST:
                    GameRequestPacket gamePacket = (GameRequestPacket)p;
                    messageForm.UpdateChatWindow("- GAME REQUEST FROM USER " + gamePacket.senderUsername + ". TYPE /game " + gamePacket.senderUsername + " TO START -");
                    break;

                case PacketType.GAME_START:
                    gameForm = new MonoGameForm(this);
                    gameForm.HandlePacket(p);
                    gameForm.ShowDialog();
                    break;

                case PacketType.PLAYERCLIENTINFO:
                    gameForm.HandlePacket(p);
                    break;
            }
        }
    }
}
