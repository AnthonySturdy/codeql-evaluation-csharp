using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClassLibrary {
    [Serializable]
    public enum PacketType {
        EMPTY,
        CHATMESSAGE,
        USERINFO,
        DISCONNECT
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

        public UserInfoPacket(string username) {
            this.type = PacketType.USERINFO;
            this.username = username;
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
    public class DisconnectPacket : Packet {
        public DisconnectPacket() {
            this.type = PacketType.DISCONNECT;
        }
    }
}