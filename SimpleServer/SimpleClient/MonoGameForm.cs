using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace SimpleClient {
    public partial class MonoGameForm : Form {
        public MonoGameForm() {
            InitializeComponent();
        }

        private void DrawTest1_Click(object sender, EventArgs e) {

        }
    }

    public class DrawTest : MonoGame.Forms.Controls.MonoGameControl {
        protected override void Initialize() {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        protected override void Draw() {
            base.Draw();

            Editor.spriteBatch.Begin();

            Editor.spriteBatch.DrawString(Editor.Font, "HELLO WORLD", new Vector2(
                (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString("HELLO WORLD").X / 2),
                (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                Microsoft.Xna.Framework.Color.White);

            Editor.spriteBatch.End();
        }
    }
}
