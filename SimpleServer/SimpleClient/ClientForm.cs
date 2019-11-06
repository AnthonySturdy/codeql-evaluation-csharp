using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


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

        public void PopulateClientList(List<Tuple<Image, string>> clientList) {
            ListView listView = new ListView();

            ImageList images = new ImageList();
            images.ImageSize = new Size(32, 32); //Shouldn't be able to be different to this anyway, just a precaution
            
            for(int i = 0; i < clientList.Count; i++) {
                images.Images.Add(clientList[i].Item1);
            }

            ClientsListView.SmallImageList = images;

            for (int i = 0; i < clientList.Count; i++) {
                ListViewItem item = new ListViewItem();
                item.Text = clientList[i].Item2;
                item.ImageIndex = i;
                ClientsListView.Items.Add(item);
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
            if (client.Connect(IPTextBox.Text, (int)PortNumberBox.Value, UsernameTextBox.Text, ProfilePictureBox.Image)) {
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

        private void ProfilePictureBox_Click(object sender, EventArgs e) {
            //Has to be on a thread to avoid 'Current thread must be set to single thread apartment' error.
            Thread t = new Thread(OpenProfilePictureSelectionDialogue);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
        void OpenProfilePictureSelectionDialogue() {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Choose Image(*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK) {
                ProfilePictureBox.Image = Image.FromFile(ofd.FileName);
                ProfilePictureBox.Image = (Image)(new Bitmap(ProfilePictureBox.Image, new Size(32, 32)));   //Resize to 32x32, this is the maximum it will be displayed at will be quicker than sending an unnecassarily big image
            }
        }
    }
}
