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

namespace SimpleClient {
    public class SimpleClient {
        TcpClient client;
        ClientForm messageForm;
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;
        Thread readerThread;

        public SimpleClient() {
            client = new TcpClient();
            messageForm = new ClientForm(this);
        }

        public bool Connect(string ipAddress, int port) {
            try {
                client.Connect(IPAddress.Parse(ipAddress), port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                Application.Run(messageForm);
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
            readerThread.Abort();
            client.Close();
        }

        public void SendMessage(string message) {
            writer.WriteLine(message);
            writer.Flush();
        }

        void ProcessServerResponse() {
            string serverMessage;

            while((serverMessage = reader.ReadLine()) != null) {
                messageForm.UpdateChatWindow(serverMessage);
            }
        }
    }
}
