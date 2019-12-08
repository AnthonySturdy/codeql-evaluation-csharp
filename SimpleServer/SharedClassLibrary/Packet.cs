using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SharedClassLibrary {
    [Serializable]
    public enum PacketType {
        EMPTY,
        CHATMESSAGE,
        IMAGEMESSAGE,
        USERINFO,
        DISCONNECT,
        CLIENTLIST,
        LOGINPACKET,
        GAME_REQUEST
    }

    //Packet parent object
    [Serializable]
    public class Packet {
        public PacketType type = PacketType.EMPTY;
    }

    //User info packet sent on connection
    [Serializable]
    public class UserInfoPacket : Packet {
        public string username = "";
        public Image profilePicture;

        public UserInfoPacket(string username, Image profilePic) {
            this.type = PacketType.USERINFO;
            this.username = username;
            this.profilePicture = profilePic;
        }
    }

    [Serializable]
    public class LoginPacket : Packet {
        public EndPoint endpoint;

        public LoginPacket(EndPoint _endpoint) {
            this.type = PacketType.LOGINPACKET;
            this.endpoint = _endpoint;
        }
    }

    //Chat message information packet
    [Serializable]
    public class ChatMessagePacket : Packet {
        public string message = "";

        public ChatMessagePacket(string message) {
            this.type = PacketType.CHATMESSAGE;
            this.message = message;
        }
    }

    [Serializable]
    public class ImageMessagePacket : Packet {
        public Image image;

        public ImageMessagePacket(Image img) {
            this.type = PacketType.IMAGEMESSAGE;
            this.image = img;
        }
    }

    [Serializable]
    public class DisconnectPacket : Packet {
        public DisconnectPacket() {
            this.type = PacketType.DISCONNECT;
        }
    }

    [Serializable]
    public class ClientListPacket : Packet {
        public List<Tuple<Image, string>> clientInformation;    //List of client profile pictures and usernames

        public ClientListPacket(List<Tuple<Image, string>> clientList) {
            this.type = PacketType.CLIENTLIST;
            this.clientInformation = clientList;
        }
    }

    [Serializable]
    public class GameRequestPacket : Packet {
        public string senderUsername;

        public GameRequestPacket(string sender) {
            this.type = PacketType.GAME_REQUEST;
            this.senderUsername = sender;
        }
    }
}