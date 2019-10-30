using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    [Serializable]
    public enum PacketType {
        EMPTY,
        CHATMESSAGE,
        USERINFO
    }

    [Serializable]
    public class Packet {
        public PacketType type = PacketType.EMPTY;
    }
}
