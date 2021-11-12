using System;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    public interface IHost<T> : IDisposable
    {      
        public Task Start();
        public Task Reload();
        public Task Stop();
        public Task<IAsyncResult> SendByteAsync(string jsonContent);
        public Task<string> ReceiveByteAsync();
        public Task ConnectAsync();
        public Task DisconnectAsync();
        public Task SendMessageAsync(string message);
        public Task SendJsonAsync(object obj);
        public Task ReceiveJsonAsync();
        public Task ListenAsync();



    }
}