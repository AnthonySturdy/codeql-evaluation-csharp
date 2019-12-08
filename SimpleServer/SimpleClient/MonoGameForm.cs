using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SharedClassLibrary;

namespace SimpleClient {
    public partial class MonoGameForm : Form {
        SimpleClient sc;

        public MonoGameForm(SimpleClient _sc) {
            sc = _sc;
            InitializeComponent();
        }

        public void HandlePacket(Packet p) {
            drawTest1.HandlePacket(p);
        }

        private void DrawTest1_Click(object sender, EventArgs e) {

        }
    }

    public class DrawTest : MonoGame.Forms.Controls.MonoGameControl {
        SimpleClient simpleClientInst;

        //Car movement is handled on the client.
        //Game checkpoint logic is handled on the server.
        public struct Car {
            public Texture2D texture;
            public Microsoft.Xna.Framework.Color colour;
            public float posX, posY;
            public float rotation;
        }

        public Texture2D backgroundTexture;
        public Car playerCar;
        public Car opponentCar;
        public float rotationSpeed = 4.5f;
        public float moveSpeed = 5.0f;
        public float accelerationSpeed = 1.0f;

        public DrawTest(SimpleClient _simpleClientInst) {
            simpleClientInst = _simpleClientInst;
        }

        protected override void Initialize() {
            base.Initialize();

            FileStream backgroundFs = new FileStream("Textures/background.png", FileMode.Open, FileAccess.Read);
            backgroundTexture = Texture2D.FromStream(GraphicsDevice, backgroundFs);

            FileStream playerCarFs = new FileStream("Textures/RaceCar.png", FileMode.Open, FileAccess.Read);
            playerCar.texture = Texture2D.FromStream(GraphicsDevice, playerCarFs);
            playerCar.rotation = 0;

            FileStream opponentCarFs = new FileStream("Textures/RaceCar.png", FileMode.Open, FileAccess.Read);
            opponentCar.texture = Texture2D.FromStream(GraphicsDevice, opponentCarFs);
            opponentCar.rotation = 0;
            opponentCar.colour = (playerCar.colour == Microsoft.Xna.Framework.Color.Red ? Microsoft.Xna.Framework.Color.Blue : Microsoft.Xna.Framework.Color.Red);

        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);


            //Send this clients information to server
            simpleClientInst.UDPSend(new PlayerClientInformationPacket(playerCar.posX, playerCar.posY, playerCar.rotation));
            
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up)) {
                //Calculate direction vector from angle
                Vector2 moveDir = new Vector2((float)Math.Cos(playerCar.rotation), (float)Math.Sin(playerCar.rotation));
                playerCar.posX += moveDir.X * moveSpeed;
                playerCar.posY += moveDir.Y * moveSpeed;

                //Can only turn when moving
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right)) {
                    playerCar.rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * rotationSpeed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left)) {
                    playerCar.rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * rotationSpeed;
                }
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down)) {
                //Calculate direction vector from angle
                Vector2 moveDir = new Vector2((float)Math.Cos(playerCar.rotation), (float)Math.Sin(playerCar.rotation));
                playerCar.posX -= moveDir.X * moveSpeed;
                playerCar.posY -= moveDir.Y * moveSpeed;

                //Can only turn when moving
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right)) {
                    playerCar.rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * rotationSpeed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left)) {
                    playerCar.rotation -= (float)gameTime.ElapsedGameTime.TotalSeconds * rotationSpeed;
                }
            }
        }

        protected override void Draw() {
            base.Draw();

            Editor.spriteBatch.Begin();

            Editor.spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height), new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height), Microsoft.Xna.Framework.Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);

            Editor.spriteBatch.Draw(opponentCar.texture, new Rectangle((int)opponentCar.posX, (int)opponentCar.posY, opponentCar.texture.Width, opponentCar.texture.Height), new Rectangle(0, 0, opponentCar.texture.Width, opponentCar.texture.Height), opponentCar.colour, opponentCar.rotation + 1.5708f, new Vector2(opponentCar.texture.Width/2, opponentCar.texture.Height/2), SpriteEffects.None, 1f);
            Editor.spriteBatch.Draw(playerCar.texture, new Rectangle((int)playerCar.posX, (int)playerCar.posY, playerCar.texture.Width, playerCar.texture.Height), new Rectangle(0, 0, playerCar.texture.Width, playerCar.texture.Height), playerCar.colour, playerCar.rotation + 1.5708f, new Vector2(playerCar.texture.Width / 2, playerCar.texture.Height / 2), SpriteEffects.None, 1f);

            string s = playerCar.posX + ", " + playerCar.posY;
            Editor.spriteBatch.DrawString(Editor.Font, s, new Vector2(
                (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString(s).X / 2),
                (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                Microsoft.Xna.Framework.Color.White);

            Editor.spriteBatch.End();
        }

        public void HandlePacket(Packet p) {
            switch (p.type) {
                case PacketType.GAME_START:
                    GameStartPacket startPacket = (GameStartPacket)p;
                    playerCar.posX = startPacket.startPosX;
                    playerCar.posY = startPacket.startPosY;
                    playerCar.colour = new Microsoft.Xna.Framework.Color(startPacket.colour.R, startPacket.colour.G, startPacket.colour.B, startPacket.colour.A);
                    break;

                case PacketType.PLAYERCLIENTINFO:
                    PlayerClientInformationPacket clientInfoPacket = (PlayerClientInformationPacket)p;
                    opponentCar.posX = clientInfoPacket.posX;
                    opponentCar.posY = clientInfoPacket.posY;
                    opponentCar.rotation = clientInfoPacket.rotation;
                    break;
            }
        }
    }
}
