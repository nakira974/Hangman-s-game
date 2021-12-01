namespace Tcp_Lib
{
    /// <summary>
    /// Signaux annoncant l'activité des clients
    /// </summary>
    public enum Signals
    {
        START,
        STOP,
        RELOAD,
        ACCEPT,
        DENY,
        WAIT,
        RUNNING,
        CONNECTED,
        DISCONNECTED
    }
}