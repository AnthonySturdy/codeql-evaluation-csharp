using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleClient {
    class SimpleClient {
        TcpClient client;
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;

        public SimpleClient() {
            client = new TcpClient();
        }

        public bool Connect(string ipAddress, int port) {
            try {
                client.Connect(IPAddress.Parse(ipAddress), port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

            return true;
        }

        public void Run() {
            Thread t = new Thread(ClientWrite);
            t.Start();

            Thread t1 = new Thread(ProcessServerResponse);
            t1.Start();
        }

        void ClientWrite() {
            string userInput;

            while ((userInput = Console.ReadLine()) != null) {
                writer.WriteLine(userInput);
                writer.Flush();

                if (userInput == "Exit") {
                    break;
                }
            }
        }

        void ProcessServerResponse() {
            string serverMessage;

            while((serverMessage = reader.ReadLine()) != null) {
                Console.WriteLine(serverMessage);

                if (serverMessage == "Bye") {
                    client.Close();
                    break;
                }
            }
        }
    }
}
