using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    public abstract class Host : IHost<Host>
    {
        protected const int DefaultSendTimeOut = 60000;
        protected const int DefaultReceiveTimeOut = 60000;
        protected const int DefaultSendBufferSize = 1024;
        protected const int DefaultReceiveBufferSize = 4096;
        protected const int DefaultPort = 9001;
        [NotMapped] public List<User> Users { get; set; }
        [NotMapped] public  Dictionary<int, NetworkStream> ClientStream { get; set; }
        [NotMapped] public List<string> MessageList { get; set; }
        [NotMapped] public string SenderName { get; init; }
        [NotMapped] public List<GameData> GameDatas { get; set; }
        [NotMapped] public GameData CurrentGameData { get; set; }
        public IPAddress CurrentIpAddress { get; set; }

        
        protected string GetCurrentHostName()
        {
            return Dns.GetHostName();
        }
        protected void GetCurrentIpAddress()
        {
            string hostName = Dns.GetHostName();
#pragma warning disable 618
            var addressLit = Dns.GetHostByName(hostName).AddressList.ToList();
            CurrentIpAddress = addressLit[addressLit.Count()-1];
#pragma warning restore 618
        }
        public abstract void Dispose();
        public abstract Task Start();
        public abstract Task Reload();
        public abstract Task Stop();
        public async Task<IAsyncResult> SendByteAsync(string jsonContent)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReceiveByteAsync()
        {
            throw new NotImplementedException();
        }
        public abstract Task ConnectAsync();
        public abstract Task DisconnectAsync();

        public struct User
        {
            public string UserName { get; init; }
        }
    }
}