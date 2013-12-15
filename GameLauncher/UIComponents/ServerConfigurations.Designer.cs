namespace GameLauncher.UIComponents
{
    partial class ServerConfigurations
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.goButton = new MetroFramework.Controls.MetroButton();
            this.portTextBox = new MetroFramework.Controls.MetroTextBox();
            this.ipTextBox = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.metroToolTip2 = new MetroFramework.Components.MetroToolTip();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 9);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(199, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Multi-Player - Friend Game Info.";
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.goButton);
            this.metroPanel1.Controls.Add(this.portTextBox);
            this.metroPanel1.Controls.Add(this.ipTextBox);
            this.metroPanel1.Controls.Add(this.metroLabel3);
            this.metroPanel1.Controls.Add(this.metroLabel2);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(534, 234);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(330, 166);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(100, 49);
            this.goButton.TabIndex = 7;
            this.goButton.Text = "Play with Friend!";
            this.goButton.UseSelectable = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Lines = new string[0];
            this.portTextBox.Location = new System.Drawing.Point(173, 128);
            this.portTextBox.MaxLength = 32767;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.PasswordChar = '\0';
            this.portTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.portTextBox.SelectedText = "";
            this.portTextBox.Size = new System.Drawing.Size(257, 23);
            this.portTextBox.TabIndex = 6;
            this.metroToolTip2.SetToolTip(this.portTextBox, "Ask you friend who is hosting the game for info!");
            this.metroToolTip1.SetToolTip(this.portTextBox, "Ask you friend who is hosting the game for info!");
            this.portTextBox.UseSelectable = true;
            // 
            // ipTextBox
            // 
            this.ipTextBox.Lines = new string[0];
            this.ipTextBox.Location = new System.Drawing.Point(173, 69);
            this.ipTextBox.MaxLength = 32767;
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.PasswordChar = '\0';
            this.ipTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ipTextBox.SelectedText = "";
            this.ipTextBox.Size = new System.Drawing.Size(257, 23);
            this.ipTextBox.TabIndex = 5;
            this.metroToolTip2.SetToolTip(this.ipTextBox, "Ask you friend who is hosting the game for info!");
            this.metroToolTip1.SetToolTip(this.ipTextBox, "Ask you friend who is hosting the game for info!\r\n");
            this.ipTextBox.UseSelectable = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(92, 128);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(79, 19);
            this.metroLabel3.TabIndex = 4;
            this.metroLabel3.Text = "Server Port:";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(92, 69);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(65, 19);
            this.metroLabel2.TabIndex = 3;
            this.metroLabel2.Text = "Server IP:";
            // 
            // metroToolTip1
            // 
            this.metroToolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroToolTip1.StyleManager = null;
            this.metroToolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.metroToolTip1_Popup);
            // 
            // metroToolTip2
            // 
            this.metroToolTip2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroToolTip2.StyleManager = null;
            this.metroToolTip2.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // ServerConfigurations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Name = "ServerConfigurations";
            this.Size = new System.Drawing.Size(534, 234);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTextBox portTextBox;
        private MetroFramework.Controls.MetroTextBox ipTextBox;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Components.MetroToolTip metroToolTip1;
        private MetroFramework.Components.MetroToolTip metroToolTip2;
        private MetroFramework.Controls.MetroButton goButton;

    }
}
