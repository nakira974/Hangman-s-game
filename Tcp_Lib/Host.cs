using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tcp_Lib
{
    /// <summary>
    /// Classe abstraite contenant les élements communs au serveur et au client
    /// </summary>
    public abstract class Host : IHost<Host>
    {
        /// <summary>
        ///Temps de timeout d'envoi par défaut
        /// </summary>
        protected const int DefaultSendTimeOut = 60000;
        /// <summary>
        /// Temps de timeout de reception par défaut
        /// </summary>
        protected const int DefaultReceiveTimeOut = 60000;
        /// <summary>
        /// Taille par défaut du buffer d'envoi
        /// </summary>
        protected const int DefaultSendBufferSize = 1024;
        /// <summary>
        /// Taille par défaut du buffer de réception
        /// </summary>
        protected const int DefaultReceiveBufferSize = 4096;
        /// <summary>
        /// Port par défaut de l'application
        /// </summary>
        protected const int DefaultPort = 9001;
        /// <summary>
        /// Nombre de données reçues
        /// </summary>
        public int DataRecieve { get; set; }
        /// <summary>
        /// Liste des utilisateurs connectés
        /// </summary>
        [NotMapped] public List<User> Users { get; set; }
        /// <summary>
        /// Liste des streams TCP pour l'envoi de texte
        /// </summary>
        [NotMapped] public  Dictionary<int, NetworkStream> ClientStream { get; set; }
        /// <summary>
        /// Liste des message envoyés
        /// </summary>
        [NotMapped] public List<string> MessageList { get; set; }
        /// <summary>
        /// Nom de l'envoyeur du message
        /// </summary>
        [NotMapped] public string SenderName { get; init; }
        /// <summary>
        /// Liste des données de jeu
        /// </summary>
        [NotMapped] public List<GameData> GameDatas { get; set; }
        /// <summary>
        /// Dernière donnée de jeu
        /// </summary>
        [NotMapped] public GameData CurrentGameData { get; set; }
        /// <summary>
        /// Adresse IP de l'hôte
        /// </summary>
        public IPAddress CurrentIpAddress { get; set; }

        
        protected string GetCurrentHostName()
        {
            return Dns.GetHostName();
        }
        /// <summary>
        /// Récupère l'adresse IP de l'hôte
        /// </summary>
        protected void GetCurrentIpAddress()
        {
            string hostName = Dns.GetHostName();
#pragma warning disable 618
            var addressLit = Dns.GetHostByName(hostName).AddressList.ToList();
            CurrentIpAddress = addressLit[addressLit.Count()-1];
#pragma warning restore 618
        }
        public abstract void Dispose();
        /// <summary>
        /// Démarre le service hôte
        /// </summary>
        /// <returns></returns>
        public abstract Task Start();
        public abstract Task Reload();
        /// <summary>
        /// Stop l'hôte en fermant les connexion entrantes/sortantes
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Envoi un message texte à un ou plusieurs hôtes
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task SendMessageAsync(string message);
        /// <summary>
        /// Envoi un objet json à un ou plusieurs hôte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract Task SendJsonAsync(object obj);
        public abstract Task ReceiveJsonAsync();
        public abstract Task ListenAsync();

        /// <summary>
        /// Retourne de manière asynchrone les clients
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        protected async IAsyncEnumerable<TcpClient> GetClientAsync(Dictionary<long, TcpClient> clients)
        {
            foreach (var client in clients)
            {
                yield return client.Value;
            }
        }
        
        /// <summary>
        /// Struture des utilisateurs de l'application
        /// </summary>
        public struct User
        {
            public string UserName { get; init; }
        }
    }
}