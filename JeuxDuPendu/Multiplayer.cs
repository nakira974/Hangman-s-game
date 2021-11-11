using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tcp_Lib;

namespace JeuxDuPendu
{
    public partial class Multiplayer : Form
    {
        public System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
        private int _messageCount { get; set; }
        private int _messageBoxIndex { get; set; }

        private GameData _gameData { get; set; }
        private Client Client { get; init; }
        private Server Server { get; init; }

        public Multiplayer()
        {
            InitializeComponent();
        }

        public Multiplayer(Client client)
        {
            InitializeComponent();
            Client = client;
            _messageCount = 0;
            _messageBoxIndex = 0;
            label2.Text = Client.CurrentIpAddress.ToString();

            aTimer.Tick += aTimer_Tick!;
            aTimer.Interval = 1000; //milisecunde
            aTimer.Enabled = true;
        }

        public Multiplayer(Server server)
        {
            InitializeComponent();
            Server = server;
            label2.Text = Server.CurrentIpAddress.ToString();

            _messageCount = 0;
            _messageBoxIndex = 0;

            aTimer.Tick += aTimer_Tick!;
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            _gameData = new GameData()
            {
                PlayersList = new List<Host.User>() { Server.Users.FirstOrDefault() },
                CurrentPlayer = Server.SenderName,
                CurrentTurn = 0,
                CurrentPlayerSignal = Signals.WAIT,
                CurrentLetterSet = char.MinValue,
                CurrentWordDiscovered = string.Empty
            };
            Server.GameDatas.Add(_gameData);
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
            _gameData = new GameData();
            _gameData.PlayersList = new List<Host.User>();
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
            try
            {
                if (Server != null)
                {
                    _gameData.PlayersList = Server.Users;

                    if (Server.Users.Count > 0)
                    {
                        await Server.SendJsonAsync(_gameData);
                    }

                    var bindingSource1 = new System.Windows.Forms.BindingSource { DataSource = _gameData.PlayersList };
                    playerDataGrid.DataSource = bindingSource1;

                    if (Server.MessageList.Count > 0)
                    {
                        if (Server.MessageList.Count > _messageCount)
                        {
                            _messageCount++;
                            listBox1.Items.Add(Server.MessageList.Last());
                        }
                    }
                }
                else if (Client != null)
                {
                    if (Client.GameDatas.Count > 0)
                    {
                        var bindingSource1 = new System.Windows.Forms.BindingSource
                            { DataSource = Client.GameDatas.FirstOrDefault()!.PlayersList };
                        playerDataGrid.DataSource = bindingSource1;
                    }
                    /*
                    playerDataGrid.DataSource = _gameData.PlayersList;
                    */

                    if (Client.MessageList.Count > 0)
                    {
                        if (Client.MessageList.Count > _messageCount)
                        {
                            _messageCount++;
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
            _gameData.CurrentLetterSet = textBox1.Text.FirstOrDefault();

            if (Client != null)
            {
                if (_gameData.CurrentPlayer == Client.SenderName)
                {
                    _gameData.CurrentTurn++;
                    await Client.SendJsonAsync(_gameData);
                }
            }
            else if (Server != null)
            {
                if (_gameData.CurrentPlayer == Server.SenderName)
                {
                    _gameData.CurrentTurn++;
                    await Server.SendJsonAsync(_gameData);
                }
            }
        }
    }
}