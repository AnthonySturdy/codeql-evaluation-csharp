using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string userInput;

            ProcessServerResponse();

            while ((userInput = Console.ReadLine()) != null) {
                writer.WriteLine(userInput);
                writer.Flush();

                ProcessServerResponse();

                if (userInput == "Exit") {
                    break;
                }
            }

            client.Close();
        }

        void ProcessServerResponse() {
            Console.WriteLine("Server says: " + reader.ReadLine());
            Console.WriteLine();
        }
    }
}
