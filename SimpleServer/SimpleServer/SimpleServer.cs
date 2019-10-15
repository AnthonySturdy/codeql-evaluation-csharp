using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer {
    class SimpleServer {
        TcpListener listener;

        List<Client> clients;

        public SimpleServer(string ipAddress, int port) {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);

            clients = new List<Client>();
        }

        public void Start() {
            listener.Start();

            Socket socket = listener.AcceptSocket();

            Console.WriteLine("Socket accepted");

            Client c = new Client(socket);
            clients.Add(c);
            Thread t = new Thread(new ParameterizedThreadStart(ClientMethod));
            t.Start(c);
        }

        public void Stop() {
            listener.Stop();
        }

        void ClientMethod(object clientObj) {
            Client client = (Client)clientObj;

            string receivedMessage;

            client.writer.WriteLine("Welcome!");
            client.writer.Flush();

            while ((receivedMessage = client.reader.ReadLine()) != null) {
                string returnMsg = GetReturnMessage(receivedMessage);

                client.writer.WriteLine(returnMsg);
                client.writer.Flush();
            }

            client.Close();
            clients.Remove(client);
        }

        string GetReturnMessage(string code) {
            string capMsg = code.ToUpper();

            if (capMsg == "HI" || capMsg == "HELLO" || capMsg == "HEY") {
                return "Hello";
            } else if (capMsg == "JOKE" || capMsg == "TELL ME A JOKE") {
                return "What do you call a zoo with only one dog? A shih-tzu.";
            } else {
                return "I don't know what to say to that.";
            }
        }

    }
}
