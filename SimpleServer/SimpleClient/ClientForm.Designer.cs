﻿namespace SimpleClient {
    partial class ClientForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.SendButton = new System.Windows.Forms.Button();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.PortNumberBox = new System.Windows.Forms.NumericUpDown();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.ClientsListView = new System.Windows.Forms.ListView();
            this.AttachImageButton = new System.Windows.Forms.Button();
            this.ProfilePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PortNumberBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProfilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SendButton
            // 
            this.SendButton.Enabled = false;
            this.SendButton.Location = new System.Drawing.Point(477, 382);
            this.SendButton.Margin = new System.Windows.Forms.Padding(2);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(96, 48);
            this.SendButton.TabIndex = 0;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_click);
            // 
            // InputBox
            // 
            this.InputBox.Enabled = false;
            this.InputBox.Location = new System.Drawing.Point(62, 382);
            this.InputBox.Margin = new System.Windows.Forms.Padding(2);
            this.InputBox.Multiline = true;
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(411, 48);
            this.InputBox.TabIndex = 1;
            this.InputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputBox_KeyDown);
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(50, 24);
            this.UsernameTextBox.MaxLength = 30;
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(175, 20);
            this.UsernameTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(228, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "IP Address";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(231, 25);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(120, 20);
            this.IPTextBox.TabIndex = 5;
            this.IPTextBox.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(354, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(420, 25);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(68, 21);
            this.ConnectButton.TabIndex = 9;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // PortNumberBox
            // 
            this.PortNumberBox.Location = new System.Drawing.Point(357, 25);
            this.PortNumberBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.PortNumberBox.Name = "PortNumberBox";
            this.PortNumberBox.Size = new System.Drawing.Size(58, 20);
            this.PortNumberBox.TabIndex = 10;
            this.PortNumberBox.Value = new decimal(new int[] {
            4444,
            0,
            0,
            0});
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Enabled = false;
            this.DisconnectButton.Location = new System.Drawing.Point(492, 25);
            this.DisconnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(81, 21);
            this.DisconnectButton.TabIndex = 11;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // OutputBox
            // 
            this.OutputBox.Location = new System.Drawing.Point(12, 51);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ReadOnly = true;
            this.OutputBox.Size = new System.Drawing.Size(560, 326);
            this.OutputBox.TabIndex = 12;
            this.OutputBox.Text = "";
            // 
            // ClientsListView
            // 
            this.ClientsListView.HideSelection = false;
            this.ClientsListView.Location = new System.Drawing.Point(578, 25);
            this.ClientsListView.MultiSelect = false;
            this.ClientsListView.Name = "ClientsListView";
            this.ClientsListView.Size = new System.Drawing.Size(193, 405);
            this.ClientsListView.TabIndex = 13;
            this.ClientsListView.UseCompatibleStateImageBehavior = false;
            this.ClientsListView.View = System.Windows.Forms.View.List;
            this.ClientsListView.DoubleClick += new System.EventHandler(this.ClientsListView_DoubleClick);
            // 
            // AttachImageButton
            // 
            this.AttachImageButton.Enabled = false;
            this.AttachImageButton.Image = global::SimpleClient.Properties.Resources.file_attachment_clip_file_paperclip_icon_1320190556905027615;
            this.AttachImageButton.Location = new System.Drawing.Point(12, 382);
            this.AttachImageButton.Margin = new System.Windows.Forms.Padding(0);
            this.AttachImageButton.Name = "AttachImageButton";
            this.AttachImageButton.Size = new System.Drawing.Size(48, 48);
            this.AttachImageButton.TabIndex = 15;
            this.AttachImageButton.UseVisualStyleBackColor = true;
            this.AttachImageButton.Click += new System.EventHandler(this.AttachImageButton_Click);
            // 
            // ProfilePictureBox
            // 
            this.ProfilePictureBox.BackColor = System.Drawing.Color.DarkRed;
            this.ProfilePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ProfilePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProfilePictureBox.Image = global::SimpleClient.Properties.Resources.DefaultProfilePicture;
            this.ProfilePictureBox.Location = new System.Drawing.Point(12, 12);
            this.ProfilePictureBox.Name = "ProfilePictureBox";
            this.ProfilePictureBox.Size = new System.Drawing.Size(32, 32);
            this.ProfilePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ProfilePictureBox.TabIndex = 14;
            this.ProfilePictureBox.TabStop = false;
            this.ProfilePictureBox.Click += new System.EventHandler(this.ProfilePictureBox_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 438);
            this.Controls.Add(this.AttachImageButton);
            this.Controls.Add(this.ProfilePictureBox);
            this.Controls.Add(this.ClientsListView);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.PortNumberBox);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IPTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UsernameTextBox);
            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.SendButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.Text = "Chat Room / Anthony Sturdy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClientForm_FormClosed);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PortNumberBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProfilePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.NumericUpDown PortNumberBox;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.RichTextBox OutputBox;
        private System.Windows.Forms.ListView ClientsListView;
        private System.Windows.Forms.PictureBox ProfilePictureBox;
        private System.Windows.Forms.Button AttachImageButton;
    }
}