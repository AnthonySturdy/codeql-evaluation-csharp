using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClassLibrary;

namespace SimpleServer {
    class SimpleServer {
        TcpListener listener;
        MemoryStream memStream = new MemoryStream();    //Used to send packets
        BinaryFormatter binFormatter = new BinaryFormatter();

        List<Client> clients;
        List<Game> games = new List<Game>();

        public SimpleServer(string ipAddress, int port) {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new List<Client>();
        }

        public void Start() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("* SERVER START *");

            listener.Start();

            do {
                CreateClient();
            } while (clients.Count > 0);
        }

        public void Stop() {
            listener.Stop();
        }

        void CreateClient() {
            //Create socket
            Socket socket = listener.AcceptSocket();

            //Create client
            Client c = new Client(socket);
            if (clients.Count == 0) //Set up client number (for identification)
                c.clientNumber = 0;
            else
                c.clientNumber = clients[clients.Count - 1].clientNumber + 1;
            clients.Add(c);

            //Handle this client on a new thread
            Thread t = new Thread(new ParameterizedThreadStart(TCPClientMethod));  
            t.Start(c);
        }

        void TCPClientMethod(object clientObj) {
            //This function is ran on its own thread recieving messages from an individual client

            Client client = (Client)clientObj;

            while (client.tcpSocket.Connected) {
                HandlePacket(client.TCPRead(), client);
            }

            //CLIENT EXIT HANDLED HERE
            //When program gets here, client has left so remove from client list
            client.Close();
            clients.Remove(client);

            //Announce quit to remaining clients
            MessageAllClients(client.clientUsername + " exited.");

            //Create and send client list packets when Client disconnects
            List<Tuple<Image, string>> clientInfo = new List<Tuple<Image, string>>();
            for (int i = 0; i < clients.Count; i++) {
                clientInfo.Add(new Tuple<Image, string>(clients[i].profilePicture, clients[i].clientUsername));
            }
            SendPacketToAllClients(new ClientListPacket(clientInfo)); //Loop again to send packet to all clients
        }

        void UDPClientMethod(object clientObj) {
            Client client = (Client)clientObj;

            while (client.udpSocket.Connected) {
                HandlePacket(client.UDPRead(), client);
            }
        }

        void HandlePacket(Packet p, Client client) {
            switch (p.type) {
                case PacketType.CHATMESSAGE:
                    ChatMessagePacket chatPacket = (ChatMessagePacket)p;
                    if (chatPacket.message[0] == '@') {  //If first character is '@', treat as direct message
                        string recipient = chatPacket.message.Split(' ')[0];    //Get recipient name
                        string message;
                        try {
                            message = chatPacket.message.Remove(0, recipient.Length + 1);    //Get remaining message. +1 for space after username.
                        } catch {
                            message = "";
                        }
                        recipient = recipient.Remove(0, 1);     //Remove the @ from username
                        if (MessageIndividualClient(recipient, client.clientUsername + " -> You: " + message)) //If message succeeds (Succeeds if user is found)
                            client.TCPSend(new ChatMessagePacket("You -> " + recipient + ": " + message));  //Display message back to sender
                        else
                            client.TCPSend(new ChatMessagePacket("Could not find " + recipient));   //If message fails (couldn't find user), inform the sender.

                    } else if (chatPacket.message[0] == '/') {  //If first character is '/', treat as command
                        string command = chatPacket.message.Split(' ')[0];
                        List<string> parameters = chatPacket.message.Split(' ').ToList();
                        if(parameters.Count > 1)
                            parameters.RemoveAt(0);
                        ExecuteCommand(command, parameters, client);
                    } else {    //If first character none of above, treat as global message
                        MessageAllClients(client.clientUsername + ": " + chatPacket.message);
                    }
                    break;

                case PacketType.IMAGEMESSAGE:
                    ImageMessagePacket imgPacket = (ImageMessagePacket)p;
                    MessageAllClients(client.clientUsername + ": ");    //Send this so others know who sent the image
                    Console.WriteLine("IMAGE SENT");
                    SendPacketToAllClients(imgPacket);
                    MessageAllClients("");    //New line after image sent
                    break;

                case PacketType.USERINFO:
                    UserInfoPacket userPacket = (UserInfoPacket)p;
                    client.clientUsername = userPacket.username;
                    client.profilePicture = userPacket.profilePicture;

                    MessageAllClients(client.clientUsername + " joined.");  //Announce join

                    //Create client list packet when Client joins
                    List<Tuple<Image, string>> info = new List<Tuple<Image, string>>();
                    for (int i = 0; i < clients.Count; i++) {
                        info.Add(new Tuple<Image, string>(clients[i].profilePicture, clients[i].clientUsername));
                    }
                    SendPacketToAllClients(new ClientListPacket(info));
                    break;

                case PacketType.LOGINPACKET:
                    LoginPacket loginPacket = (LoginPacket)p;

                    client.UDPConnect(loginPacket.endpoint);

                    Thread t = new Thread(new ParameterizedThreadStart(UDPClientMethod));
                    t.Start(client);
                    break;

                case PacketType.PLAYERCLIENTINFO:
                    for (int i = 0; i < games.Count; i++) {      //Find which game packet is sent from
                        if (games[i].clientList.Contains(client)) {
                            games[i].ProcessPacket((PlayerClientInformationPacket)p, client);
                            break;
                        }
                    }
                    break;
            }
        }

        public void MessageAllClients(string message) {
            if (message.Contains(':'))
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);

            Packet p = new ChatMessagePacket(message);

            for (int i = 0; i < clients.Count; i++) {
                clients[i].TCPSend(p);
            }
        }

        bool MessageIndividualClient(string recipient, string message) {
            int clientIndex = GetClientIndex(recipient);
            if (clientIndex == -1)  //If can't find user
                return false;

            Client c = clients[clientIndex];
            c.TCPSend(new ChatMessagePacket(message));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            return true;
        }

        void SendPacketToAllClients(Packet p) {
            for(int i = 0; i < clients.Count; i++) {
                clients[i].TCPSend(p);
            }
        }

        public int GetClientIndex(Client c) {
            for(int i = 0; i < clients.Count; i++) {
                if (clients[i] == c)
                    return i;
            }

            return -1;
        }
        public int GetClientIndex(string username) {
            for (int i = 0; i < clients.Count; i++) {
                if (clients[i].clientUsername == username)
                    return i;
            }

            return -1;
        }

        void ExecuteCommand(string command, List<string> parameters, Client client) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(client.clientUsername + ": " + command);

            if (command.ToLower() == "/help") {
                string helpString = "--- HELP ---\n" +
                    "- Send global chat messages by typing a message and pressing Send.\n" +
                    "- Send private messages by double clicking a user on the list, typing your message then pressing Send.\n" +
                    "- Send picture messages by pressing the button to the left of the input box and selecting an image.\n" +
                    "- Send a game request by typing /game followed by the username you would like to challenge. E.g \"/game Sturdy\".\n" +
                    "- Change your profile picture by disconnecting, clicking the profile picture icon, selecting a picture and reconnecting.";
                client.TCPSend(new ChatMessagePacket(helpString));
            } else if(command.ToLower() == "/game") {
                if (parameters[0] == client.clientUsername)
                    return;

                int recipientIndex = GetClientIndex(parameters[0]);
                if (recipientIndex == -1)
                    return;

                Client c = clients[recipientIndex];
                for(int i = 0; i < games.Count; i++) {      //Check if challenged player is already in an empty game
                    if (games[i].clientList.Contains(c) && games[i].clientList.Count == 1) {  //If is in an empty game, add client to game and start game
                        games[i].clientList.Add(client);
                        games[i].Start(this);
                        return;
                    }
                }
                c.TCPSend(new GameRequestPacket(client.clientUsername));
                Game g = new Game();
                g.clientList.Add(client);
                games.Add(g);
            }
        }

    }
}
