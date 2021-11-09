
namespace JeuxDuPendu
{
    partial class ConnectionForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backButton = new System.Windows.Forms.Button();
            this.connectionTextBox = new System.Windows.Forms.TextBox();
            this.connectionButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.serverDataGrid = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.serverDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(24, 43);
            this.backButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(86, 31);
            this.backButton.TabIndex = 1;
            this.backButton.Text = "Retour";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // connectionTextBox
            // 
            this.connectionTextBox.Location = new System.Drawing.Point(24, 363);
            this.connectionTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.connectionTextBox.Name = "connectionTextBox";
            this.connectionTextBox.PlaceholderText = "Saisir l\'adresse ou le nom d\'un serveur";
            this.connectionTextBox.Size = new System.Drawing.Size(334, 27);
            this.connectionTextBox.TabIndex = 6;
            this.connectionTextBox.TextChanged += new System.EventHandler(this.connectionTextBox_TextChanged);
            // 
            // connectionButton
            // 
            this.connectionButton.Location = new System.Drawing.Point(387, 363);
            this.connectionButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.connectionButton.Name = "connectionButton";
            this.connectionButton.Size = new System.Drawing.Size(103, 31);
            this.connectionButton.TabIndex = 5;
            this.connectionButton.Text = "Connexion";
            this.connectionButton.UseVisualStyleBackColor = true;
            this.connectionButton.Click += new System.EventHandler(this.connectionButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(362, 431);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 31);
            this.button1.TabIndex = 7;
            this.button1.Text = "Créer un serveur";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(24, 431);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(143, 31);
            this.button2.TabIndex = 8;
            this.button2.Text = "Ajouter un serveur";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // serverDataGrid
            // 
            this.serverDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.serverDataGrid.Location = new System.Drawing.Point(24, 121);
            this.serverDataGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.serverDataGrid.Name = "serverDataGrid";
            this.serverDataGrid.RowHeadersWidth = 51;
            this.serverDataGrid.RowTemplate.Height = 25;
            this.serverDataGrid.Size = new System.Drawing.Size(449, 200);
            this.serverDataGrid.TabIndex = 9;
            this.serverDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.serverDataGrid_CellContentClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(305, 47);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.PlaceholderText = "Nom du joueur";
            this.textBox1.Size = new System.Drawing.Size(168, 27);
            this.textBox1.TabIndex = 10;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 621);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.serverDataGrid);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.connectionTextBox);
            this.Controls.Add(this.connectionButton);
            this.Controls.Add(this.backButton);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ConnectionForm";
            this.Text = "ConnectionForm";
            this.Load += new System.EventHandler(this.ConnectionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.serverDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.TextBox connectionTextBox;
        private System.Windows.Forms.Button connectionButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView serverDataGrid;
        private System.Windows.Forms.TextBox textBox1;
    }
}