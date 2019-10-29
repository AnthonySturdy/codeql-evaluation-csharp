using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SimpleClient {
    public partial class ClientForm : Form {
        delegate void UpdateChatWindowDelegate(string message);
        UpdateChatWindowDelegate _updateChatWindowDelegate;

        SimpleClient client;

        public ClientForm(SimpleClient c) {
            InitializeComponent();
            _updateChatWindowDelegate = new UpdateChatWindowDelegate(UpdateChatWindow);
            client = c;
            InputBox.Select();
        }

        public void UpdateChatWindow(string message) {
            try {
                if (OutputBox.InvokeRequired) {
                    Invoke(_updateChatWindowDelegate, message);
                } else {
                    OutputBox.Text += message;
                    OutputBox.SelectionStart = OutputBox.Text.Length;
                    OutputBox.ScrollToCaret();
                }
            } catch(System.InvalidOperationException e) {
                Console.WriteLine(e.Message);
            }
        }

        private void SendButton_click(object sender, EventArgs e) {
            client.SendMessage(InputBox.Text);
            InputBox.Text = "";
            InputBox.Select();
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e) {
            client.Stop();
        }

        private void ClientForm_Load(object sender, EventArgs e) {
            client.Run();
        }
    }
}
