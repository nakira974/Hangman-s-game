using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    public class Client : Host
    {
        private Host _hostImplementation;
        private TcpClient ClientSocket { get; init; }
        public IPAddress ServerAddress { get; set; }

        public Client()
        {
            DataRecieve = 0;
            GameDatas = new List<GameData>();
            MessageList = new List<string>();
            ClientStream = new Dictionary<int, NetworkStream>();
            GetCurrentIpAddress();
            ClientSocket = new TcpClient();
            ClientSocket.SendBufferSize = DefaultSendBufferSize;
            ClientSocket.ReceiveBufferSize = DefaultReceiveBufferSize;
            ClientSocket.ReceiveTimeout = DefaultReceiveTimeOut;
            ClientSocket.SendTimeout = DefaultSendTimeOut;

        }

        /// <summary>
        /// Constructeur public prenant en paramètre l'adresse du serveur à joindre
        /// </summary>
        /// <param name="senderName"></param>
        public Client(string senderName)
        {
            DataRecieve = 0;
            SenderName = senderName;
            Users = new List<User>();
            Users.Add(new User()
            {
                UserName = SenderName
            });
            GameDatas = new List<GameData>();
            MessageList = new List<string>();
            ClientStream = new Dictionary<int, NetworkStream>();
            GetCurrentIpAddress();
            ClientSocket = new TcpClient();
            ClientSocket.SendBufferSize = DefaultSendBufferSize;
            ClientSocket.ReceiveBufferSize = DefaultReceiveBufferSize;
            ClientSocket.ReceiveTimeout = DefaultReceiveTimeOut;
            ClientSocket.SendTimeout = DefaultSendTimeOut;

        }

        ~Client()
        {
#pragma warning disable 4014
            DisconnectAsync();
#pragma warning restore 4014
        }

        public static IHost<Host> Instance { get; } = new Client();

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            throw new NotImplementedException();
        }


        public override async Task Reload()
        {
            throw new NotImplementedException();
        }

        public override async Task Stop()
        {
            await DisconnectAsync();
        }

        /// <summary>
        /// Lance le flux d'envoi des messages et du json
        /// </summary>
        public async Task LaunchProcess()
        {
            List<Task> jobs = new List<Task>()
            {
                ListenAsync(),
                ReceiveJsonAsync()
            };

            await Task.WhenAny(jobs);
        }

        /// <summary>
        /// Se connecte à un serveur spécifié en lançant un stream pour les messages et un autre pour le json 
        /// </summary>
        /// <param name="ipAddress"></param>
        public async Task ConnectAsync(string ipAddress)
        {
            try
            {
                //Listen stream
                
                byte[] bytes = Encoding.Latin1.GetBytes(SenderName);
                await ClientSocket.ConnectAsync(ipAddress, DefaultPort);
                NetworkStream stream = ClientSocket.GetStream();
                await stream.WriteAsync(bytes, 0, bytes.Length);
                ClientStream.Add(1, stream);
                
                //Json stream
                var jsonClient = new TcpClient();
                await jsonClient.ConnectAsync(ipAddress, DefaultPort);
                bytes = Encoding.Latin1.GetBytes("application/json");
                NetworkStream jsonStream = jsonClient.GetStream();
                await jsonStream.WriteAsync(bytes, 0, bytes.Length);
                ClientStream.Add(2, jsonStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Envoi un message au serveur
        /// </summary>
        /// <param name="message"></param>
        public override async Task SendMessageAsync(string message)
        {
            byte[] bytes = new byte[] { };
            NetworkStream stream = ClientStream[1];
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo input;
            sb.Append(message);
            bytes = Encoding.Latin1.GetBytes(sb.ToString());
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// Envoi un objet json vers le serveur
        /// </summary>
        /// <param name="obj"></param>
        public override async Task SendJsonAsync(object obj)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            NetworkStream stream = ClientStream[2];
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public override async Task ConnectAsync()
        {
            try
            {
                string author = "Maxime";
                byte[] bytes = Encoding.ASCII.GetBytes(author);
                await ClientSocket.ConnectAsync(CurrentIpAddress.ToString(), DefaultPort);
                NetworkStream stream = ClientSocket.GetStream();
                stream.Write(bytes, 0, bytes.Length);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public override async Task DisconnectAsync()
        {
            try
            {
                await SendMessageAsync("0xffff");
                ClientStream[1].Socket.DisconnectAsync(new SocketAsyncEventArgs(false));
                ClientStream[1].Close();
                ClientStream[1].Socket.Close();
                await SendJsonAsync(new GameData()
                {
                    CurrentPlayerSignal = Signals.DISCONNECTED
                });
                ClientStream[2].Socket.DisconnectAsync(new SocketAsyncEventArgs(false));
                ClientStream[2].Close();
                ClientStream[2].Socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public override async Task ListenAsync()
        {
            byte[] currentBuffer = new byte[DefaultReceiveBufferSize];
            NetworkStream clientNetworkStream = ClientStream[1];
            do
            {
                if (ClientStream[1].CanRead)
                {
                    do // Start converting bytes to string
                    {
                        StringBuilder currentStringBuilder = new StringBuilder();

                        int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length);
                        currentStringBuilder.AppendFormat("{0}",
                            Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                        if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                        {
                            //TO DO
                            Console.WriteLine($"{currentStringBuilder.ToString()}");
                            MessageList.Add($"\n{currentStringBuilder.ToString()}\n");
                        }

                    } while (ClientStream[1].CanRead);

                }
            } while (ClientStream[1].CanRead);
        }
        
        public override async Task ReceiveJsonAsync()
        {
            byte[] currentBuffer = new byte[DefaultReceiveBufferSize];
            NetworkStream clientNetworkStream = ClientStream[2];
            do
            {
                if (ClientStream[2].CanRead)
                {
                    do // Start converting bytes to string
                    {
                        StringBuilder currentStringBuilder = new StringBuilder();

                        int bytesReaded = await clientNetworkStream.ReadAsync(currentBuffer, 0, currentBuffer.Length);
                        currentStringBuilder.AppendFormat("{0}",
                            Encoding.Latin1.GetString(currentBuffer, 0, bytesReaded));
                        if (!string.IsNullOrEmpty(currentStringBuilder.ToString()))
                        {
                            string json = currentStringBuilder.ToString();
                            MemoryStream mStrm= new MemoryStream( Encoding.UTF8.GetBytes( json ) );
                            GameData gameData =
                                await System.Text.Json.JsonSerializer.DeserializeAsync<GameData>(mStrm, cancellationToken:CancellationToken.None);
                            GameDatas.Add(gameData);
                            DataRecieve++;
                        }

                    } while (ClientStream[2].CanRead);

                }
            } while (ClientStream[2].CanRead);
        }
    }
}