using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLib;
using JeuxDuPendu.MyControls;
using Tcp_Lib;

namespace JeuxDuPendu
{
    public partial class Multiplayer : Form
    {
        public System.Windows.Forms.Timer ATimer = new System.Windows.Forms.Timer();
        private int MessageCount { get; set; }
        private int GameDataCount { get; set; }
        private int MessageBoxIndex { get; set; }

        private int LastTurn { get; set; }
        private GameData? GameData { get; set; }
        private Client Client { get; init; }
        private Server Server { get; init; }
        HangmanViewer _hangmanViewer = new HangmanViewer();
        private HangmanGame Ruler { get; init; }


        public Multiplayer(Client client)
        {
            InitializeComponent();
            InitializeGameComponent();
            Client = client;
            MessageCount = 0;
            MessageBoxIndex = 0;
            GameDataCount = 0;
            LastTurn = 0;
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
            LastTurn = 0;
            MessageCount = 0;
            MessageBoxIndex = 0;
            GameDataCount = 1;

            ATimer.Tick += aTimer_Tick!;
            ATimer.Interval = 1000;
            ATimer.Enabled = true;

            Ruler = new HangmanGame();
            GameData = new GameData()
            {
                PlayersList = new List<Host.User>() { Server.Users.FirstOrDefault() },
                CurrentPlayer = Server.SenderName,
                CurrentTurn = 0,
                CurrentPlayerSignal = Signals.WAIT,
                CurrentLetterSet = char.MinValue,
                CurrentWordDiscovered = Ruler.HashedWord.ToString(),
                CurrentHangmanState = 0
            };
            Server.GameDatas.Add(GameData);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (Client != null)
                await Client.Stop();
            if (Server != null)
                await Server.Stop();
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
                    await ServerCheck();
                }
                else if (Client != null)
                {
                    await ClientCheck();
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
                GameData.CurrentTurn++;
                await Client.SendJsonAsync(GameData);
            }
            else if (Server != null)
            {
                if (Ruler.TryAppendCharacter(GameData!.CurrentLetterSet).Result)
                {
                    GameData.CurrentWordDiscovered = Ruler.HashedWord.ToString();
                }
                else
                {
                    GameData.CurrentHangmanState++;
                    await _hangmanViewer.MoveNextStep(GameData.CurrentHangmanState);
                }

                GameData.CurrentTurn++;
                await Server.SendJsonAsync(GameData);
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

        private async Task ServerCheck()
        {
            if (_hangmanViewer.IsGameOver)
            {
                if (GameData!.CurrentHangmanState > 0)
                    MessageBox.Show("Vous avez perdu !");
                StartNewGame();
                Ruler.NewGame();
                GameData = new GameData()
                {
                    PlayersList = GameData!.PlayersList,
                    CurrentLetterSet = ' ',
                    CurrentPlayer = Server.SenderName,
                    CurrentTurn = 0,
                    CurrentHangmanState = 0,
                    CurrentPlayerSignal = Signals.WAIT,
                    CurrentWordDiscovered = Ruler.HashedWord.ToString()
                };

                await Server.SendJsonAsync(GameData);
            }
            else
            {
                if (Server.GameDatas.Count > 1)
                {
                    if (Server.GameDatas.Count > GameDataCount)
                    {
                        GameData = Server.GameDatas.LastOrDefault();
                        GameDataCount++;
                        if (GameData!.CurrentTurn > LastTurn)
                        {
                            LastTurn++;

                            if (Ruler.TryAppendCharacter(GameData!.CurrentLetterSet).Result)
                            {
                                GameData.CurrentWordDiscovered = Ruler.HashedWord.ToString();
                                lCrypedWord.Text = GameData!.CurrentWordDiscovered;
                            }
                            else
                            {
                                GameData.CurrentHangmanState++;
                                await _hangmanViewer.MoveNextStep(GameData.CurrentHangmanState);
                            }
                        }
                    }
                }

                GameData!.PlayersList = Server.Users;

                if (Server.Users.Count > 1)
                    await Server.SendJsonStreamAsync(GameData);
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
        }

        private async Task ClientCheck()
        {
            if (_hangmanViewer.IsGameOver)
            {
                if (GameData!.CurrentHangmanState > 0)
                    MessageBox.Show("Vous avez perdu !");
                GameData.CurrentHangmanState = 0;
                StartNewGame();
                Thread.Sleep(2000);
                GameData = Client.GameDatas.LastOrDefault();
            }
            else
            {
                if (Client.GameDatas.Count > 0)
                {
                    GameData = Client.GameDatas.LastOrDefault();
                    var bindingSource1 = new System.Windows.Forms.BindingSource
                        { DataSource = GameData!.PlayersList };
                    playerDataGrid.DataSource = bindingSource1;
                    if (Client.GameDatas.Count > GameDataCount)
                    {
                        GameDataCount++;
                        lCrypedWord.Text = GameData!.CurrentWordDiscovered;
                        await _hangmanViewer.MoveNextStep(GameData.CurrentHangmanState);
                    }
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
    }
}