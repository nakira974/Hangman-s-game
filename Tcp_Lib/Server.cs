using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    public class Server : Host
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }
        [NotMapped] private bool StopRequest { get; set; }
        
        [NotMapped] private CancellationToken Token { get; set; }
        
        [NotMapped] private long ClientNumber { get; set; }
        
        [NotMapped] private List<Task> ServerTasks { get; set; }

        [NotMapped] private List<ServerTask> ClientsPool { get; set; }
        
        [NotMapped] private Signals CurrentServerSignal { get; set; }
        
        [NotMapped]
        private Dictionary<long, TcpClient> _tcpClients { get; set; }

        [NotMapped]
        private Dictionary<long, TcpClient> _jsonClients { get; set; }
        
        [NotMapped]
        private TcpListener _serverSocket { get; init; }
        
        public Server()
        {
            DataRecieve = 0;
            GameDatas = new List<GameData>();
            MessageList = new List<string>();
            _tcpClients = new Dictionary<long, TcpClient>();
            _jsonClients = new Dictionary<long, TcpClient>();
            StopRequest = false;
            Token = new CancellationToken(StopRequest);
            ClientNumber = 0;
            ServerTasks = new List<Task>();
            ClientsPool = new List<ServerTask>();
            GetCurrentIpAddress();
            _serverSocket = new TcpListener(CurrentIpAddress, Host.DefaultPort);
            _serverSocket.Server.ReceiveBufferSize = DefaultReceiveBufferSize;
            _serverSocket.Server.SendBufferSize = DefaultReceiveBufferSize;
            _serverSocket.Server.ReceiveTimeout = DefaultReceiveTimeOut;
            _serverSocket.Server.SendTimeout = DefaultSendTimeOut;
        }

        public Server(string senderName)
        {
            DataRecieve = 0;
            GameDatas = new List<GameData>();
            SenderName = senderName;
            Users = new List<User>();
            Users.Add(new User()
            {
                UserName = SenderName
            });
            MessageList = new List<string>();
            _tcpClients = new Dictionary<long, TcpClient>();
            _jsonClients = new Dictionary<long, TcpClient>();
            StopRequest = false;
            Token = new CancellationToken(StopRequest);
            ClientNumber = 0;
            ServerTasks = new List<Task>();
            ClientsPool = new List<ServerTask>();
            GetCurrentIpAddress();
            _serverSocket = new TcpListener(CurrentIpAddress, Host.DefaultPort);
            _serverSocket.Server.ReceiveBufferSize = DefaultReceiveBufferSize;
            _serverSocket.Server.SendBufferSize = DefaultReceiveBufferSize;
            _serverSocket.Server.ReceiveTimeout = DefaultReceiveTimeOut;
            _serverSocket.Server.SendTimeout = DefaultSendTimeOut;
        }
        ~Server()
        {
#pragma warning disable 4014
            DisconnectAsync();
#pragma warning restore 4014
        }

        public static IHost<Host> Instance { get; } = new Server();

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            CurrentServerSignal = Signals.RUNNING;
            
            try
            {
                _serverSocket.Start();
                do
                {
                    byte[] currentBuffer = new byte[DefaultReceiveBufferSize];
                    
                    var currentClient = await _serverSocket.AcceptTcpClientAsync();
                    NetworkStream clientNetworkStream = currentClient.GetStream();
                    if (clientNetworkStream.CanRead)
                    {
                        StringBuilder clientType = new StringBuilder();
                        int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                        clientType.AppendFormat("{0}",
                            Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                        
                        if (clientType.ToString().Contains("application/json"))
                        {
                            await LaunchJsonStream(currentClient, clientNetworkStream);
                        }
                        else
                        {
                            User currentUser = new User()
                            {
                                UserName = clientType.ToString()
                            };
                            Users.Add(currentUser);
                            await LaunchMessageStream(currentClient, clientNetworkStream, currentUser);
                        }
                    }
                   
                    Console.WriteLine($"Accepting client {currentClient.GetHashCode()}");
                    
                } while (!StopRequest);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            throw new NotImplementedException();
        }

        public override async Task Reload()
        {
            throw new NotImplementedException();
        }

        public override async Task Stop()
        {
            StopRequest = true;
            await DisconnectAsync();
        }

        public override async Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task DisconnectAsync()
        {
            await foreach (var client in GetClientAsync(_jsonClients).WithCancellation(Token))
            {
                client.Client.DisconnectAsync(new SocketAsyncEventArgs(true));
            }

            await foreach (var client in GetClientAsync(_tcpClients).WithCancellation(Token))
            {
                client.Client.DisconnectAsync(new SocketAsyncEventArgs(true));

            }
            _serverSocket.Server.Close();
        }

        private async Task LaunchJsonStream(TcpClient currentClient, NetworkStream clientNetworkStream)
        {
            var currentJsonRecieveTask = ReceiveJsonAsync(currentClient, clientNetworkStream);
            ServerTasks.Add(currentJsonRecieveTask);
            ClientsPool.Add(new ServerTask()
            {
                Task = currentJsonRecieveTask,
                TaskType = TaskType.LISTEN,
                ClientId = currentClient.GetHashCode()
            });
            await Task.WhenAll(ServerTasks);
        }
        
        public async Task LaunchMessageStream(TcpClient currentClient, NetworkStream clientNetworkStream, User user)
        {
            var currentListenTask = ListenAsync(currentClient, clientNetworkStream, user);
            ServerTasks.Add(currentListenTask);
            ClientsPool.Add(new ServerTask()
            {
                Task = currentListenTask,
                TaskType = TaskType.LISTEN,
                ClientId = currentClient.GetHashCode()
            });
                            
            await Task.WhenAll(ServerTasks);
        }
        private async Task ListenAsync(TcpClient client, NetworkStream clientNetworkStream, User user)
        {
            try
            {
                _tcpClients.Add(ClientNumber, client);
                long currentClientId = ClientNumber;
                ClientNumber++;
                GameData gameData = new GameData();
                Action currentClientListenAction = async () =>
                {
                    byte[] currentBuffer = new byte[DefaultReceiveBufferSize];
                    Console.WriteLine($"New listen task for client {client.GetHashCode()}");

                    do
                    {
                        try
                        {

                            if (clientNetworkStream.CanRead)
                            {
                                Console.WriteLine($"Current username :{user.UserName}");
                                
                                do // Start converting bytes to string
                                { 
                                    StringBuilder currentStringBuilder = new StringBuilder();

                                    if (clientNetworkStream.Socket.Connected)
                                    {
                                        int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                                        currentStringBuilder.AppendFormat("{0}",
                                            Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                                        if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                                        {
                                            string msg =
                                                $"\n{user.UserName} said :{currentStringBuilder.ToString()}\n";
                                            if (currentStringBuilder.ToString() == "0xffff")
                                            {
                                                _tcpClients[currentClientId].Dispose();
                                                _tcpClients.Remove(currentClientId);
                                                Users.Remove(user);
                                            }
                                            else
                                            {
                                                await BroadcastAsync(msg);
                                            }
                                        }
                                    }
                                   
                                } while (clientNetworkStream.Socket.Connected); // Until stream data is available

                                
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                        
                    } while (clientNetworkStream.Socket.Connected);

                    
                };

                Task currentClientPool = Task.Run(currentClientListenAction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task ReceiveJsonAsync(TcpClient client, NetworkStream clientNetworkStream)
        {
            try
            {
                _jsonClients.Add(ClientNumber, client);
                long currentClientId = ClientNumber;
                ClientNumber++;
                GameData gameData = new GameData();
                Action currentClientListenAction = async () =>
                {
                    byte[] currentBuffer = new byte[DefaultReceiveBufferSize];
                    Console.WriteLine($"New listen task for client {client.GetHashCode()}");

                    do
                    {
                        try
                        {
                            

                            StringBuilder username = new StringBuilder();

                            if (clientNetworkStream.CanRead)
                            {
                                do // Start converting bytes to string
                                { 
                                    StringBuilder currentStringBuilder = new StringBuilder();

                                    if (clientNetworkStream.Socket.Connected)
                                    {
                                        int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                                        currentStringBuilder.AppendFormat("{0}",
                                            Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                                        if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                                        {
                                            //TO DO
                                        
                                            string json = currentStringBuilder.ToString();
                                            MemoryStream mStrm= new MemoryStream( Encoding.UTF8.GetBytes( json ) );
                                            GameData gameData =
                                                await System.Text.Json.JsonSerializer.DeserializeAsync<GameData>(mStrm, cancellationToken:Token);
                                            DataRecieve++;
                                            if (gameData.CurrentPlayerSignal == Signals.DISCONNECTED)
                                            {
                                                _jsonClients[currentClientId].Dispose();
                                                _jsonClients.Remove(currentClientId);
                                            }
                                            else
                                            {
                                                await SendJsonAsync(gameData);
                                            }
                                        }
                                    }

                                } while (clientNetworkStream.Socket.Connected); // Until stream data is available

                                
                            }
                            
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    } while (gameData.CurrentPlayerSignal != Signals.DISCONNECTED);
                };

                Task currentClientPool = Task.Run(currentClientListenAction, Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task BroadcastAsync(string message)
        {

            try
            {
                
                MessageList.Add(message);
                byte[] bytes = Encoding.Latin1.GetBytes(message);

                await foreach (var client in GetClientAsync(_tcpClients).WithCancellation(Token))
                {
                    await client.Client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        
        public override async Task SendMessageAsync(string message)
        {

            try
            {
                message = SenderName + " " + "said :" + message;
                MessageList.Add(message);
                byte[] bytes = Encoding.Latin1.GetBytes(message);

                await foreach (var client in GetClientAsync(_tcpClients).WithCancellation(Token))
                {
                    await client.Client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        
        public override async Task SendJsonAsync(object obj)
        {

            try
            {
                GameDatas.Add(obj as GameData);
                string json = System.Text.Json.JsonSerializer.Serialize(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                await foreach (var client in GetClientAsync(_jsonClients).WithCancellation(Token))
                {
                    await client.Client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        public override async Task ReceiveJsonAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task ListenAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SendJsonStreamAsync(object obj)
        {

            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                await foreach (var client in GetClientAsync(_jsonClients).WithCancellation(Token))
                {
                    await client.Client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        
    }
}