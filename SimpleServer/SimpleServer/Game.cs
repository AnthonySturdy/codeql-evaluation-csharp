using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharedClassLibrary;

namespace SimpleServer {
    class Game {
        SimpleServer serverInst;

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

        bool gameOver = false;

        public void Start(SimpleServer server) {
            serverInst = server;

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
                if (clientList[i] != sender) {   //Send info to other player
                    PlayerClientInformationPacket cpPacket = packet;

                    //Check this clients current checkpoint
                    Player senderPlayer = playerList[(i + 1) % playerList.Count];  //Sender
                    Vec2 playerPos = new Vec2(packet.posX, packet.posY);
                    Vec2 checkPointPos = checkpoints[senderPlayer.currentCheckpoint];
                    float dist = (float)Math.Sqrt(Math.Pow(playerPos.x - checkPointPos.x, 2) + Math.Pow(playerPos.y - checkPointPos.y, 2));
                    if (dist < 40) {
                        if(senderPlayer.currentCheckpoint == checkpoints.Length - 1) {
                            if(gameOver == false)
                                serverInst.MessageAllClients("- User \"" + sender.clientUsername + "\" has won a race! -");
                            gameOver = true;
                        } else {
                            senderPlayer.currentCheckpoint++;
                        }
                    }

                    playerList[(i + 1) % playerList.Count] = senderPlayer;

                    //Add clients checkpoint position to information packet
                    //-100, -100 is how client knows game is over
                    if (gameOver) {
                        cpPacket.checkpointPosX = -100;
                        cpPacket.checkpointPosY = -100;
                    } else {
                        cpPacket.checkpointPosX = checkpoints[playerList[i].currentCheckpoint].x;
                        cpPacket.checkpointPosY = checkpoints[playerList[i].currentCheckpoint].y;
                    }
                    

                    clientList[i].UDPSend(packet);
                }
            }
        }
    }
}
