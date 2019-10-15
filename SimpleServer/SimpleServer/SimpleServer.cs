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
            Console.WriteLine("* SERVER START *");

            listener.Start();

            CreateClient();
        }

        public void Stop() {
            listener.Stop();
        }

        void CreateClient() {
            //Create socket
            Socket socket = listener.AcceptSocket();
            Console.WriteLine("New socket accepted");

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

            string receivedMessage;

            client.writer.WriteLine("Welcome client " + client.clientNumber);
            client.writer.Flush();

            while ((receivedMessage = client.reader.ReadLine()) != null) {
                if (receivedMessage == "Exit") {
                    Console.WriteLine("Client " + client.clientNumber + " exited.");
                    break;
                }

                Console.WriteLine("Client " + client.clientNumber + ": " + receivedMessage);    //Write incoming messages to server window
                
                string returnMsg = GetReturnMessage(receivedMessage);   //Process message and get response
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
