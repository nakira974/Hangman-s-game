using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLib.rsc;
using JeuxDuPendu.MyControls;
using Tcp_Lib;

namespace JeuxDuPendu
{
    public partial class Multiplayer : Form
    {
        public System.Windows.Forms.Timer ATimer = new System.Windows.Forms.Timer();
        private int MessageCount { get; set; }
        private int MessageBoxIndex { get; set; }

        private GameData? GameData { get; set; }
        private Client Client { get; init; }
        private Server Server { get; init; }
        HangmanViewer _hangmanViewer = new HangmanViewer();


        public Multiplayer(Client client)
        {
            InitializeComponent();
            InitializeGameComponent();
            Client = client;
            MessageCount = 0;
            MessageBoxIndex = 0;
            label2.Text = Client.CurrentIpAddress.ToString();

            ATimer.Tick += aTimer_Tick!;
            ATimer.Interval = 1000; //milisecunde
            ATimer.Enabled = true;
            GameData = new GameData();
            GameData.PlayersList = new List<Host.User>();
        }

        public Multiplayer(Server server)
        {
            InitializeComponent();
            InitializeGameComponent();
            Server = server;
            label2.Text = Server.CurrentIpAddress.ToString();

            MessageCount = 0;
            MessageBoxIndex = 0;

            ATimer.Tick += aTimer_Tick!;
            ATimer.Interval = 1000;
            ATimer.Enabled = true;

            var word = Word.SelectRandomWord().Result.Text;
            GameData = new GameData()
            {
                PlayersList = new List<Host.User>() { Server.Users.FirstOrDefault() },
                CurrentPlayer = Server.SenderName,
                CurrentTurn = 0,
                CurrentPlayerSignal = Signals.WAIT,
                CurrentLetterSet = char.MinValue,
                CurrentWordDiscovered = word
            };
            Server.GameDatas.Add(GameData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Client != null)
                Client.Stop();
            if (Server != null)
                Server.Stop();
            this.Hide();
            ConnectionForm connectionMenu = new ConnectionForm();
            connectionMenu.Closed += (s, args) => this.Close();
            connectionMenu.Show();
        }

        private void playerDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (Client != null)
            {
                await Client.SendMessageAsync(textBox2.Text);
            }
            else if (Server != null)
            {
                await Server.SendMessageAsync(textBox2.Text);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Multiplayer_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        async void aTimer_Tick(object sender, EventArgs e)
        {
            await Check();
        }

        private async Task Check()
        {
            lCrypedWord.Text = GameData!.CurrentWordDiscovered;
            try
            {
                if (Server != null)
                {
                    if (Server.GameDatas.Count > 1)
                        GameData = Server.GameDatas.LastOrDefault();
                    else
                        GameData!.PlayersList = Server.Users;

                    if (Server.Users.Count > 1)
                        await Server.SendJsonAsync(GameData);


                    var bindingSource1 = new System.Windows.Forms.BindingSource { DataSource = GameData!.PlayersList };
                    playerDataGrid.DataSource = bindingSource1;

                    if (Server.MessageList.Count > 0)
                    {
                        if (Server.MessageList.Count > MessageCount)
                        {
                            MessageCount++;
                            listBox1.Items.Add(Server.MessageList.Last());
                        }
                    }
                }
                else if (Client != null)
                {
                    if (Client.GameDatas.Count > 0)
                    {
                        GameData = Client.GameDatas.LastOrDefault();
                        var bindingSource1 = new System.Windows.Forms.BindingSource
                            { DataSource = GameData!.PlayersList };
                        playerDataGrid.DataSource = bindingSource1;
                    }


                    if (Client.MessageList.Count > 0)
                    {
                        if (Client.MessageList.Count > MessageCount)
                        {
                            MessageCount++;
                            listBox1.Items.Add(Client.MessageList.Last());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            GameData!.CurrentLetterSet = textBox1.Text.FirstOrDefault();

            if (Client != null)
            {
                if (GameData.CurrentPlayer == Client.SenderName)
                {
                    GameData.CurrentTurn++;
                    await Client.SendJsonAsync(GameData);
                }
            }
            else if (Server != null)
            {
                if (GameData.CurrentPlayer == Server.SenderName)
                {
                    GameData.CurrentTurn++;
                    await Server.SendJsonAsync(GameData);
                }
            }
        }


        public void StartNewGame()
        {
            // Methode de reinitialisation classe d'affichage du pendu.
            _hangmanViewer.Reset();

            //Affichage du mot à trouver dans le label.
            lCrypedWord.Text = "_____";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void InitializeGameComponent()
        {
            // On positionne le controle d'affichage du pendu dans panel1 : 
            panel1.Controls.Add(_hangmanViewer);

            // à la position 0,0
            _hangmanViewer.Location = new Point(0, 0);

            // et de la même taille que panel1
            _hangmanViewer.Size = panel1.Size;
        }
    }
}