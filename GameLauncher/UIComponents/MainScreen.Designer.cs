namespace GameLauncher.UIComponents
{
    partial class MainScreen
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
            this.multiPlayerTile = new MetroFramework.Controls.MetroTile();
            this.configTile = new MetroFramework.Controls.MetroTile();
            this.singlePlayerTile = new MetroFramework.Controls.MetroTile();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.multiPlayerTile);
            this.metroPanel1.Controls.Add(this.configTile);
            this.metroPanel1.Controls.Add(this.singlePlayerTile);
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(3, 3);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(534, 234);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // multiPlayerTile
            // 
            this.multiPlayerTile.ActiveControl = null;
            this.multiPlayerTile.Location = new System.Drawing.Point(267, 4);
            this.multiPlayerTile.Name = "multiPlayerTile";
            this.multiPlayerTile.Size = new System.Drawing.Size(264, 123);
            this.multiPlayerTile.TabIndex = 4;
            this.multiPlayerTile.Text = "Multi-Player";
            this.multiPlayerTile.UseSelectable = true;
            this.multiPlayerTile.Click += new System.EventHandler(this.multiPlayerTile_Click);
            // 
            // configTile
            // 
            this.configTile.ActiveControl = null;
            this.configTile.Location = new System.Drawing.Point(3, 133);
            this.configTile.Name = "configTile";
            this.configTile.Size = new System.Drawing.Size(528, 89);
            this.configTile.TabIndex = 3;
            this.configTile.Text = "Configurations";
            this.configTile.UseSelectable = true;
            this.configTile.Click += new System.EventHandler(this.configTile_Click);
            // 
            // singlePlayerTile
            // 
            this.singlePlayerTile.ActiveControl = null;
            this.singlePlayerTile.Location = new System.Drawing.Point(3, 3);
            this.singlePlayerTile.Name = "singlePlayerTile";
            this.singlePlayerTile.Size = new System.Drawing.Size(258, 124);
            this.singlePlayerTile.TabIndex = 2;
            this.singlePlayerTile.Text = "Single Player";
            this.singlePlayerTile.UseSelectable = true;
            this.singlePlayerTile.Click += new System.EventHandler(this.singlePlayerTile_Click);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Name = "MainScreen";
            this.Size = new System.Drawing.Size(538, 238);
            this.metroPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTile multiPlayerTile;
        private MetroFramework.Controls.MetroTile configTile;
        private MetroFramework.Controls.MetroTile singlePlayerTile;
    }
}
