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
            messageForm = new ClientForm(this);
            Application.Run(messageForm);
        }

        public bool Connect(string ipAddress, int port) {
            try {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(ipAddress), port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
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
                writer.WriteLine(message);
                writer.Flush();
            }
        }

        void ProcessServerResponse() {
            string serverMessage;

            while((serverMessage = reader.ReadLine()) != null) {
                messageForm.UpdateChatWindow(serverMessage);
            }
        }
    }
}
