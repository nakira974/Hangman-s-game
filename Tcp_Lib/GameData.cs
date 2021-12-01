using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tcp_Lib
{
    /// <summary>
    /// Record contenu les données du jeu envoyé à tous les clients et au serveur depuis les clients
    /// </summary>
    public record GameData()
    {
        /// <summary>
        /// Enum qui permet au client d'annoncer sa deconnexion au serveur
        /// </summary>
        [JsonPropertyName("CurrentClientSignal")]
        public Signals CurrentPlayerSignal { get; set; }
        /// <summary>
        /// Nom du joueur envoyant la donnée
        /// </summary>
        [JsonPropertyName("ClientName")]
        public string CurrentPlayer { get; set; }
        /// <summary>
        /// Numéro du tour actuellement joué
        /// </summary>
        [JsonPropertyName("CurrentTurn")]
        public int CurrentTurn { get; set; }
        /// <summary>
        /// Liste des joueurs connectés sur le serveur 
        /// </summary>
        
        [JsonPropertyName("PlayersList")]
        public List<Host.User> PlayersList { get; set; }
        /// <summary>
        /// Dernière lettre jouée
        /// </summary>
        
        [JsonPropertyName("CurrentLetterSet")]
        public char CurrentLetterSet { get; set; }
        /// <summary>
        /// Mot découvert par les joueurs
        /// </summary>
        
        [JsonPropertyName("CurrentWordDiscovered")]
        public string CurrentWordDiscovered { get; set; }
        /// <summary>
        /// Numéro de l'état du dessin du pendu
        /// </summary>
        [JsonPropertyName("CurrentHangmanState")]
        public short CurrentHangmanState { get; set; }
        /// <summary>
        /// Attribut booléen pour savoir si la partie est gagnée ou non
        /// </summary>
        [JsonPropertyName("IsGameWon")]
        public bool IsGameWon { get; set; }

        [JsonPropertyName("DiscoveredChars")]
        public List<char> DiscoveredChars { get; set; }

    };
}