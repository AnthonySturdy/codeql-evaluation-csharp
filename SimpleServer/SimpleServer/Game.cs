using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharedClassLibrary;

namespace SimpleServer {
    class Game {
        public struct Player {
            public float PosX, PosY;
            public float rotation;
            public Color colour;
            public int currentCheckpoint;
        }

        struct Vec2 {
            public float x, y;

            public Vec2(float _x, float _y) {
                x = _x;
                y = _y;
            }
        }

        Vec2[] checkpoints = {
        new Vec2(829, 560), //Lap 1
        new Vec2(1009,423),
        new Vec2(974, 126),
        new Vec2(813, 69 ),
        new Vec2(729, 219),
        new Vec2(656, 306),
        new Vec2(515, 287),
        new Vec2(383, 114),
        new Vec2(150, 100),
        new Vec2(140, 235),
        new Vec2(241, 321),
        new Vec2(204, 460),
        new Vec2(220, 580),
        new Vec2(552, 579),

        new Vec2(829, 560), //Lap 2
        new Vec2(1009,423),
        new Vec2(974, 126),
        new Vec2(813, 69 ),
        new Vec2(729, 219),
        new Vec2(656, 306),
        new Vec2(515, 287),
        new Vec2(383, 114),
        new Vec2(150, 100),
        new Vec2(140, 235),
        new Vec2(241, 321),
        new Vec2(204, 460),
        new Vec2(220, 580),
        new Vec2(552, 579),

        new Vec2(829, 560), //Lap3
        new Vec2(1009,423),
        new Vec2(974, 126),
        new Vec2(813, 69 ),
        new Vec2(729, 219),
        new Vec2(656, 306),
        new Vec2(515, 287),
        new Vec2(383, 114),
        new Vec2(150, 100),
        new Vec2(140, 235),
        new Vec2(241, 321),
        new Vec2(204, 460),
        new Vec2(220, 580),
        new Vec2(552, 579) };

        public List<Client> clientList = new List<Client>();
        public List<Player> playerList = new List<Player>();

        public void Start() {
            for(int i = 0; i < clientList.Count; i++) {
                playerList.Add(new Player());
                Player p = playerList[i];
                p.PosX = 590;
                p.PosY = 545 + (45 * i);
                p.rotation = 0;
                p.colour = (i == 0 ? Color.Red : Color.Blue);
                p.currentCheckpoint = 0;

                clientList[i].TCPSend(new GameStartPacket(p.PosX, p.PosY, p.colour));
            }
        }

        public void ProcessPacket(PlayerClientInformationPacket packet, Client sender) {
            for(int i = 0; i < clientList.Count; i++) {
                if(clientList[i] != sender) {   //Send info to other players
                    clientList[i].UDPSend(packet);
                } else {
                    Vec2 playerPos = new Vec2(packet.posX, packet.posY);
                    Vec2 checkPointPos = checkpoints[playerList[i].currentCheckpoint];
                    float dist = (float)Math.Sqrt(Math.Pow(playerPos.x - checkPointPos.x, 2) + Math.Pow(playerPos.y - checkPointPos.y, 2));
                    if(dist < 40) {
                        Player p = playerList[i];
                        p.currentCheckpoint++;
                        clientList[i].TCPSend(new CheckpointPacket(checkpoints[p.currentCheckpoint].x, checkpoints[p.currentCheckpoint].y));
                    }
                }
            }
        }
    }
}
