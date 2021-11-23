using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        public System.Windows.Forms.Timer ATimer = new System.Windows.Forms.Timer();
        private string? _playerName { get; set; }
        private string? _currentServerSelected { get; set; }

        
        public ConnectionForm()
        {
            InitializeComponent();
            ATimer.Tick += aTimer_Tick!;
            ATimer.Interval = 30000;
            ATimer.Enabled = true;
        }

        /// <summary>
        /// Timer qui appelle la fonction d'update de l'interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void aTimer_Tick(object sender, EventArgs e)
        {
            await Check();
        }

        private async Task Check()
        {
            await FillServerDataGrid();
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

            foreach (DataGridViewCell cell in serverDataGrid.SelectedCells)
            {
                _currentServerSelected = cell.Value.ToString();
                break;
            }

        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            FillServerDataGrid();
        }

        /// <summary>
        /// Se connecte à la db et rempli la datagridview et ping les serveurs en ligne.
        /// </summary>
        /// <returns></returns>
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

                serverDataGrid.Columns[0].Visible = false;

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromException(e);
            }
        }

        /// <summary>
        /// Ping les serveurs connectés et met leur variable isConnected à true si oui
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        private async Task<List<Player.ServerListView>> PingServers(List<Player.ServerListView> servers)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                servers.ForEach(srv =>
                {
                    PingReply reply = pinger.Send(srv.IpAddress);
                    pingable = reply.Status == IPStatus.Success;
                    srv.IsOnline = pingable;
                });
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return servers;
        }

        private void connectionTextBox_TextChanged(object sender, EventArgs e)
        {
            _currentServerSelected = connectionTextBox.Text;
        }

        /// <summary>
        /// Update de l'interface toute les 30 secondes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Permet de supprimer un serveur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Permet d'ajouter un serveur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Itère les serveurs et récupère l'enumrable en async
        /// </summary>
        /// <returns></returns>
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