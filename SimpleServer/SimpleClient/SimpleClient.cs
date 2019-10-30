using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Shared;

namespace SimpleClient {
    public class SimpleClient {
        TcpClient client;
        StreamWriter writer;
        BinaryReader reader;
        NetworkStream stream;

        ClientForm messageForm;

        Thread readerThread;

        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binFormatter = new BinaryFormatter();

        public SimpleClient() {
            messageForm = new ClientForm(this);
            Application.Run(messageForm);
        }

        public bool Connect(string ipAddress, int port) {
            try {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(ipAddress), port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new BinaryReader(stream);
                Run();
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

            return true;
        }

        public void Run() {
            readerThread = new Thread(ProcessServerResponse);
            readerThread.Start();
        }

        public void Stop() {
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

        public void SetUserInfo(string username) {
            Packet p = new UserInfoPacket(username);
            Send(p);
        }

        public void Send(Packet data) {
            BinaryWriter _writer = new BinaryWriter(stream);

            binFormatter.Serialize(memStream, data);
            byte[] buffer = memStream.GetBuffer();

            _writer.Write(buffer.Length);
            _writer.Write(buffer);
            _writer.Flush();

            memStream.Dispose();
        }

        void ProcessServerResponse() {
            int noOfIncomingBytes;

            while((noOfIncomingBytes = reader.ReadInt32()) != 0) {
                byte[] buffer = reader.ReadBytes(noOfIncomingBytes); //Read bytes to array
                MemoryStream ms = new MemoryStream(buffer);
                Packet p = binFormatter.Deserialize(ms) as Packet;   //Deserialize MemoryStream to Packet

                switch (p.type) {
                    case PacketType.CHATMESSAGE:
                        ChatMessagePacket _p = binFormatter.Deserialize(memStream) as ChatMessagePacket;
                        messageForm.UpdateChatWindow(_p.message);
                        break;
                }
            }

        }
    }
}
