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
        [NotMapped]
        public bool StopRequest { get; set; }
        
        [NotMapped]
        public CancellationToken Token { get; set; }
        
        [NotMapped]
        public long ClientNumber { get; set; }
        
        [NotMapped]
        public List<Task> ServerTasks { get; set; }

        [NotMapped]
        public List<ServerTask> ClientsPool { get; set; }
        
        [NotMapped]
        public Signals CurrentServerSignal { get; set; }
        
        [NotMapped]
        private Dictionary<long, TcpClient> _tcpClients { get; set; }

        [NotMapped]
        private Dictionary<long, TcpClient> _jsonClients { get; set; }
        
        [NotMapped]
        private TcpListener _serverSocket { get; init; }
        
        public Server()
        {
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
                            Users.Add(new User()
                            {
                                UserName = clientType.ToString()
                            });
                            await LaunchMessageStream(currentClient, clientNetworkStream, clientType.ToString());
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
        }

        public override async Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public async Task LaunchJsonStream(TcpClient currentClient, NetworkStream clientNetworkStream)
        {
            var currentJsonRecieveTask = RecieveJsonAsync(currentClient, clientNetworkStream);
            ServerTasks.Add(currentJsonRecieveTask);
            ClientsPool.Add(new ServerTask()
            {
                Task = currentJsonRecieveTask,
                TaskType = TaskType.LISTEN,
                ClientId = currentClient.GetHashCode()
            });
            await Task.WhenAll(ServerTasks);
        }
        
        public async Task LaunchMessageStream(TcpClient currentClient, NetworkStream clientNetworkStream, string username)
        {
            var currentListenTask = ListenAsync(currentClient, clientNetworkStream, username);
            ServerTasks.Add(currentListenTask);
            ClientsPool.Add(new ServerTask()
            {
                Task = currentListenTask,
                TaskType = TaskType.LISTEN,
                ClientId = currentClient.GetHashCode()
            });
                            
            await Task.WhenAll(ServerTasks);
        }
        private async Task ListenAsync(TcpClient client, NetworkStream clientNetworkStream, string username)
        {
            try
            {
                _tcpClients.Add(ClientNumber, client);
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
                                Console.WriteLine($"Current username :{username.ToString()}");
                                
                                do // Start converting bytes to string
                                { 
                                    StringBuilder currentStringBuilder = new StringBuilder();

                                    int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                                    currentStringBuilder.AppendFormat("{0}",
                                        Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                                    if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                                    {
                                        string msg =
                                            $"\n{username.ToString()} said :{currentStringBuilder.ToString()}\n";
                                        await BroadcastAsync(msg);
                                    }
                                } while (clientNetworkStream.CanRead); // Until stream data is available

                                
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    } while (gameData.CurrentPlayerSignal != Signals.DISCONNECTED);
                };

                Task currentClientPool = Task.Run(currentClientListenAction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async Task RecieveJsonAsync(TcpClient client, NetworkStream clientNetworkStream)
        {
            try
            {
                _jsonClients.Add(ClientNumber, client);
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
                                int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                                username.AppendFormat("{0}",
                                    Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                                Console.WriteLine($"Current username :{username.ToString()}");
                                
                                do // Start converting bytes to string
                                { 
                                    StringBuilder currentStringBuilder = new StringBuilder();

                                    bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length, Token);
                                    currentStringBuilder.AppendFormat("{0}",
                                        Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                                    if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                                    {
                                        //TO DO
                                        
                                        string json = currentStringBuilder.ToString();
                                        MemoryStream mStrm= new MemoryStream( Encoding.UTF8.GetBytes( json ) );
                                        GameData gameData =
                                            await System.Text.Json.JsonSerializer.DeserializeAsync<GameData>(mStrm, cancellationToken:Token);
                                        GameDatas.Add(gameData);
                                        await SendJsonAsync(json);
                                    }
                                } while (clientNetworkStream.CanRead); // Until stream data is available

                                
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

        public async Task BroadcastAsync(string message)
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
        
        public async Task SendMessageAsync(string message)
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
        
        public async Task SendJsonAsync(object obj)
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

        private async IAsyncEnumerable<TcpClient> GetClientAsync(Dictionary<long, TcpClient> clients)
        {
            foreach (var client in clients)
            {
                yield return client.Value;
            }
        }
    }
}