using System.Threading.Tasks;

namespace Tcp_Lib
{
    public enum TaskType
    {
        LISTEN,
        MONOCAST,
        BROADCAST
        
    }

    public class ServerTask
    {
        public Task Task { get; set; }
        public TaskType TaskType { get; set; }
        public int ClientId { get; set; }
    }
    
}