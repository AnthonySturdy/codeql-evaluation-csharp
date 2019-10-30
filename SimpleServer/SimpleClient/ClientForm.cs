using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;


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
                    //Split string (to extract username)
                    string[] messageSplit = message.Split(':');

                    if(messageSplit.Length == 1) {
                        //If only 1, it's a server message
                        OutputBox.SelectionColor = Color.Navy;
                        OutputBox.AppendText(message + "\n");
                    } else {
                        //Print username in colour
                        OutputBox.SelectionColor = Color.Crimson;
                        OutputBox.AppendText(messageSplit[0] + ":");

                        //Join rest of split string (if more than 1 ':')
                        string msg = "";
                        for (int i = 1; i < messageSplit.Length; i++)
                            msg += messageSplit[i];

                        //Write rest of string in black
                        OutputBox.SelectionColor = Color.Black;
                        OutputBox.AppendText(msg + "\n");
                    }

                    //Change selection start and scroll to it
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

        private void ClientForm_Load(object sender, EventArgs e) {
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e) {
            client.Stop();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.Enter) {
                SendButton.PerformClick();  //Send message
                e.SuppressKeyPress = true;  //Stop annoying sound
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e) {
            //Username check
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text)) {
                UpdateChatWindow("Please input a valid username");
                return;
            }

            //Attempt connection
            if(client.Connect(IPTextBox.Text, (int)PortNumberBox.Value)) {
                InputBox.Enabled = true;
                SendButton.Enabled = true;
                DisconnectButton.Enabled = true;
                ConnectButton.Enabled = false;
                UsernameTextBox.Enabled = false;
                IPTextBox.Enabled = false;
                PortNumberBox.Enabled = false;
            } else {
                UpdateChatWindow("Failed to connect to " + IPTextBox.Text + ":" + PortNumberBox.Value);
            }

        }

        private void DisconnectButton_Click(object sender, EventArgs e) {
            client.Stop();
            InputBox.Enabled = false;
            SendButton.Enabled = false;
            DisconnectButton.Enabled = false;
            ConnectButton.Enabled = true;
            UsernameTextBox.Enabled = true;
            IPTextBox.Enabled = true;
            PortNumberBox.Enabled = true;
        }
    }
}
