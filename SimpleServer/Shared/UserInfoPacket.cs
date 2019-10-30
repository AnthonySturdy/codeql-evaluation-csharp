using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    [Serializable]
    public class UserInfoPacket : Packet {
        public string username = "";

        public UserInfoPacket(string username) {
            this.type = PacketType.USERINFO;
            this.username = username;
        }
    }
}
