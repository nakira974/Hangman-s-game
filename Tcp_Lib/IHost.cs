using System;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    /// <summary>
    /// Implémentation des méthodes communes au client et au serveur
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHost<T> : IDisposable
    {      
        /// <summary>
        /// Lance le service hôte
        /// </summary>
        /// <returns></returns>
        public Task Start();
        /// <summary>
        /// Relance le service hôte
        /// </summary>
        /// <returns></returns>
        public Task Reload();
        /// <summary>
        /// Stoppe le service hôte
        /// </summary>
        /// <returns></returns>
        public Task Stop();
        /// <summary>
        /// Envoi en ArraySegement[Byte] un objet json serialisé
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public Task<IAsyncResult> SendByteAsync(string jsonContent);
        /// <summary>
        /// Reçoit un objet json sous forme de byte 
        /// </summary>
        /// <returns></returns>
        public Task<string> ReceiveByteAsync();
        /// <summary>
        /// Méthode de connexion par défaut à un serveur
        /// </summary>
        /// <returns></returns>
        public Task ConnectAsync();
        /// <summary>
        /// Méthode de deconnexion asynchrone par défaut
        /// </summary>
        /// <returns></returns>
        public Task DisconnectAsync();
        /// <summary>
        /// Méthode d'envoi d'un message par défaut
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendMessageAsync(string message);
        /// <summary>
        /// Méthode d'envoi d'un objet générique par défaut
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task SendJsonAsync(object obj);
        /// <summary>
        /// Méthode de réception d'un objet par défaut
        /// </summary>
        /// <returns></returns>
        public Task ReceiveJsonAsync();
        /// <summary>
        /// Méthode d'écoute 
        /// </summary>
        /// <returns></returns>
        public Task ListenAsync();



    }
}