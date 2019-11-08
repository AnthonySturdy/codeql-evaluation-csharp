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
        TcpClient client;
        StreamWriter writer;
        BinaryReader reader;
        NetworkStream stream;

        ClientForm messageForm;

        Thread readerThread;

        MemoryStream memStream = new MemoryStream();    //Used to send packets
        BinaryFormatter binFormatter = new BinaryFormatter();

        public SimpleClient() {
            messageForm = new ClientForm(this);
            Application.Run(messageForm);
            
        }

        public bool Connect(string ipAddress, int port, string username, Image profilePic) {
            try {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(ipAddress), port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new BinaryReader(stream);
                Run(username, profilePic);
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

            return true;
        }

        public void Run(string username, Image profilePic) {
            readerThread = new Thread(ProcessServerResponse);
            readerThread.Start();

            Packet p = new UserInfoPacket(username, profilePic);
            Send(p);
        }

        public void Stop() {
            Send(new DisconnectPacket());   //Let the server know the client is about to disconnect

            if(readerThread != null)
                readerThread.Abort();

            if(client != null)
                client.Close();
        }

        public void SendMessage(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                Packet p = new ChatMessagePacket(message);
                Send(p);
            }
        }

        public void Send(Packet data) {
            if (client == null)
                return;

            if (!client.Connected) 
                return;

            BinaryWriter _writer = new BinaryWriter(stream);

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();

            memStream.SetLength(0);
        }

        void ProcessServerResponse() {
            int noOfIncomingBytes;

            while((noOfIncomingBytes = reader.ReadInt32()) != 0) {
                byte[] buffer = reader.ReadBytes(noOfIncomingBytes); //Read bytes to array
                MemoryStream ms = new MemoryStream(buffer);
                Packet p = binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet

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
                }
            }

        }
    }
}
