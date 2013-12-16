namespace GameLauncher.UIComponents
{
    partial class GeneralConfiguration
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
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.GraphicPage = new System.Windows.Forms.TabPage();
            this.fullscreenToggle = new MetroFramework.Controls.MetroToggle();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.resolutionComboBox = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.AudioPage = new System.Windows.Forms.TabPage();
            this.metroTrackBar1 = new MetroFramework.Controls.MetroTrackBar();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.GraphicPage.SuspendLayout();
            this.AudioPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.tabControl);
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
            // tabControl
            // 
            this.tabControl.Controls.Add(this.GraphicPage);
            this.tabControl.Controls.Add(this.AudioPage);
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 1;
            this.tabControl.Size = new System.Drawing.Size(528, 228);
            this.tabControl.TabIndex = 2;
            this.tabControl.UseSelectable = true;
            // 
            // GraphicPage
            // 
            this.GraphicPage.Controls.Add(this.fullscreenToggle);
            this.GraphicPage.Controls.Add(this.metroLabel2);
            this.GraphicPage.Controls.Add(this.resolutionComboBox);
            this.GraphicPage.Controls.Add(this.metroLabel1);
            this.GraphicPage.Location = new System.Drawing.Point(4, 38);
            this.GraphicPage.Name = "GraphicPage";
            this.GraphicPage.Padding = new System.Windows.Forms.Padding(3);
            this.GraphicPage.Size = new System.Drawing.Size(520, 186);
            this.GraphicPage.TabIndex = 0;
            this.GraphicPage.Text = "Graphics";
            this.GraphicPage.ToolTipText = "dasda";
            this.GraphicPage.UseVisualStyleBackColor = true;
            // 
            // fullscreenToggle
            // 
            this.fullscreenToggle.AutoSize = true;
            this.fullscreenToggle.Location = new System.Drawing.Point(228, 116);
            this.fullscreenToggle.Name = "fullscreenToggle";
            this.fullscreenToggle.Size = new System.Drawing.Size(80, 17);
            this.fullscreenToggle.TabIndex = 3;
            this.fullscreenToggle.Text = "Off";
            this.fullscreenToggle.UseSelectable = true;
            this.fullscreenToggle.CheckedChanged += new System.EventHandler(this.fullscreenToggle_CheckedChanged);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(124, 114);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(68, 19);
            this.metroLabel2.TabIndex = 2;
            this.metroLabel2.Text = "FullScreen";
            // 
            // resolutionComboBox
            // 
            this.resolutionComboBox.FormattingEnabled = true;
            this.resolutionComboBox.ItemHeight = 23;
            this.resolutionComboBox.Location = new System.Drawing.Point(228, 51);
            this.resolutionComboBox.Name = "resolutionComboBox";
            this.resolutionComboBox.Size = new System.Drawing.Size(174, 29);
            this.resolutionComboBox.TabIndex = 1;
            this.resolutionComboBox.UseSelectable = true;
            this.resolutionComboBox.SelectedValueChanged += new System.EventHandler(this.resolutionComboBox_SelectedValueChanged);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(124, 51);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(69, 19);
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "Resolution";
            // 
            // AudioPage
            // 
            this.AudioPage.Controls.Add(this.metroTrackBar1);
            this.AudioPage.Controls.Add(this.metroLabel3);
            this.AudioPage.Location = new System.Drawing.Point(4, 38);
            this.AudioPage.Name = "AudioPage";
            this.AudioPage.Padding = new System.Windows.Forms.Padding(3);
            this.AudioPage.Size = new System.Drawing.Size(520, 186);
            this.AudioPage.TabIndex = 1;
            this.AudioPage.Text = "Audio";
            this.AudioPage.UseVisualStyleBackColor = true;
            // 
            // metroTrackBar1
            // 
            this.metroTrackBar1.BackColor = System.Drawing.Color.Transparent;
            this.metroTrackBar1.Location = new System.Drawing.Point(238, 50);
            this.metroTrackBar1.Name = "metroTrackBar1";
            this.metroTrackBar1.Size = new System.Drawing.Size(164, 23);
            this.metroTrackBar1.TabIndex = 1;
            this.metroTrackBar1.Text = "volumeTrackBar";
            this.metroTrackBar1.ValueChanged += new System.EventHandler(this.metroTrackBar1_ValueChanged);
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(120, 50);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(97, 19);
            this.metroLabel3.TabIndex = 0;
            this.metroLabel3.Text = "Master Volume";
            // 
            // GeneralConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Name = "GeneralConfiguration";
            this.Size = new System.Drawing.Size(534, 234);
            this.metroPanel1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.GraphicPage.ResumeLayout(false);
            this.GraphicPage.PerformLayout();
            this.AudioPage.ResumeLayout(false);
            this.AudioPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTabControl tabControl;
        private System.Windows.Forms.TabPage GraphicPage;
        private System.Windows.Forms.TabPage AudioPage;
        private MetroFramework.Controls.MetroToggle fullscreenToggle;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroComboBox resolutionComboBox;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTrackBar metroTrackBar1;
        private MetroFramework.Controls.MetroLabel metroLabel3;
    }
}
