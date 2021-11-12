using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLib;
using GameLib.rsc;
using Microsoft.EntityFrameworkCore;
using Tcp_Lib;

namespace JeuxDuPendu
{
    public partial class ConnectionForm : Form
    {
        private string _playerName { get; set; }
        private string _currentServerSelected { get; set; }

        public ConnectionForm()
        {
            InitializeComponent();
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            GameForm mainMenu = new GameForm();
            mainMenu.Closed += (s, args) => this.Close();
            mainMenu.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server server = new Server(_playerName);
            Task startTask = server.Start();
            this.Hide();
            Multiplayer multiplayer = new Multiplayer(server);
            multiplayer.Closed += (s, args) => this.Close();
            multiplayer.Show();
        }

        private void serverDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            FillServerDataGrid();
        }

        private Task FillServerDataGrid()
        {
            List<Player.ServerListView> registeredServers = new List<Player.ServerListView>();

            try
            {
                using (GameLib.rsc.ProgramDbContext context = new GameLib.rsc.ProgramDbContext())
                {
                    if (!context.Players.Any())
                    {
                        var player = new Player()
                        {
                            Name = "nakira974",
                            PlayerId = 1,
                            Severs = new List<Player.ServerListView>() { }
                        };
                        player.Severs.Add(new Player.ServerListView()
                        {
                            Player = player,
                            Id = 1,
                            IpAddress = "localhost",
                            IsOnline = false
                        });
                        context.Players.Add(player);
                        context.SaveChanges();
                    }

                    context.Players.Include(collection => collection.Severs);
                    var servers = context.Players.Select(x => x.Severs).FirstOrDefaultAsync().Result;
                    registeredServers = servers.ToList();
                }

                var bindingSource1 = new System.Windows.Forms.BindingSource
                    { DataSource = PingServers(registeredServers).Result };
                serverDataGrid.DataSource = bindingSource1;

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromException(e);
            }
        }

        private async Task<List<Player.ServerListView>> PingServers(List<Player.ServerListView> servers)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return servers;
        }

        private void connectionTextBox_TextChanged(object sender, EventArgs e)
        {
            _currentServerSelected = connectionTextBox.Text;
        }

        private async void connectionButton_Click(object sender, EventArgs e)
        {
            var tasks = new List<Task>();
            Client client = new Client(_playerName);
            await client.ConnectAsync(connectionTextBox.Text);
            if (client.ClientStream.Count > 0)
            {
                this.Hide();
                Multiplayer multiplayer = new Multiplayer(client);
                multiplayer.Closed += (s, args) => this.Close();
                multiplayer.Show();
            }

            await Task.Run(client.LaunchProcess);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _playerName = textBox1.Text;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await using (ProgramDbContext context = new ProgramDbContext())
            {
                context.Players.Include(collection => collection.Severs);
                await foreach (Player.ServerListView? server in GetServerAsync())
                {
                    var player = context.Players.FirstOrDefaultAsync().Result;
                    var currentServerToRemove = context.Servers.FirstOrDefaultAsync(x => x.Id == server!.Id).Result;
                    player.Severs.Remove(currentServerToRemove);
                }

                await context.SaveChangesAsync();
                await FillServerDataGrid();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                await using (ProgramDbContext context = new ProgramDbContext())
                {
                    context.Players.Include(collection => collection.Severs);
                    var server = new Player.ServerListView()
                    {
                        IpAddress = _currentServerSelected
                    };
                    var player = context.Players.FirstOrDefaultAsync().Result;
                    if (player.Severs == null)
                        player.Severs = new List<Player.ServerListView>();
                    player.Severs.Add(server);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }


            await FillServerDataGrid();
        }


        async IAsyncEnumerable<Player.ServerListView?> GetServerAsync()
        {
            foreach (DataGridViewRow selectedRow in serverDataGrid.SelectedRows)
            {
                yield return selectedRow.DataBoundItem is Player.ServerListView
                    ? (Player.ServerListView)selectedRow.DataBoundItem
                    : default;
            }
        }
    }
}