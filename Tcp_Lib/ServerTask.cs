using System.Threading.Tasks;

namespace Tcp_Lib
{
    /// <summary>
    /// Type de la tâche crée
    /// </summary>
    public enum TaskType
    {
        LISTEN,
        MONOCAST,
        BROADCAST
        
    }

    /// <summary>
    /// Tâche associée à process sur le serveur
    /// </summary>
    public class ServerTask
    {
        public Task Task { get; set; }
        public TaskType TaskType { get; set; }
        public int ClientId { get; set; }
    }
    
}