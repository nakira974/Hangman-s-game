using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tcp_Lib;
using GameLib;
using Microsoft.EntityFrameworkCore;

namespace JeuxDuPendu
{
    public partial class ConnectionForm : Form
    {
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
            Server srv = new Server();
            Task startTask = srv.Start();
            this.Hide();
            Multiplayer multiplayer = new Multiplayer(srv);
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
            List<Server> registeredServers = new List<Server>();

            try
            {
                using (GameLib.rsc.ProgramDbContext context = new GameLib.rsc.ProgramDbContext())
                {
                    context.Players.Include(collection => collection.Severs);
                    var servers = context.Players.Select(x => x.Severs).FirstOrDefaultAsync().Result;
                    foreach (var server in servers)
                    {
                        registeredServers.Add(server);
                    }
                }

                serverDataGrid.DataSource = PingServers(registeredServers).Result;

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromException(e);
            }
        }

        private async Task<List<ServerListView>> PingServers(List<Server> servers)
        {
            List<ServerListView> result = new List<ServerListView>();

            try
            {
                foreach (var server in servers)
                {
                    result.Add(new ServerListView()
                    {
                        Id = server.Id,
                        IpAddress = server.CurrentIpAddress.ToString(),
                        IsOnline = false
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        private void connectionTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private async void connectionButton_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            await client.ConnectAsync(connectionTextBox.Text);
            if (client.ClientStream.Count > 0)
            {
                this.Hide();
                Multiplayer multiplayer = new Multiplayer(client);
                multiplayer.Closed += (s, args) => this.Close();
                multiplayer.Show();
            }

            await Task.Run(client.ListenAsync);
        }
    }

    public class ServerListView
    {
        public string IpAddress { get; set; }
        public int Id { get; set; }

        public bool IsOnline { get; set; }
    }
}