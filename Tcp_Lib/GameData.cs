using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tcp_Lib
{
    public record GameData()
    {
        [JsonPropertyName("CurrentClientSignal")]
        public Signals CurrentPlayerSignal { get; set; }
        [JsonPropertyName("ClientName")]
        public string CurrentPlayer { get; set; }
        
        [JsonPropertyName("CurrentTurn")]
        public int CurrentTurn { get; set; }
        
        [JsonPropertyName("PlayersList")]
        public List<Host.User> PlayersList { get; set; }
        
        [JsonPropertyName("CurrentLetterSet")]
        public char CurrentLetterSet { get; set; }
        
        [JsonPropertyName("CurrentWordDiscovered")]
        public string CurrentWordDiscovered { get; set; }
        
        [JsonPropertyName("CurrentHangmanState")]
        public short CurrentHangmanState { get; set; }
        
    };
}