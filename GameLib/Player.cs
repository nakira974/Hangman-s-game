using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLib
{
    public class Player
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; init; }

        [Required] [ForeignKey("Id")] public virtual ICollection<ServerListView> Severs { get; set; }

        [Required] public string Name { get; init; }

        public Player()
        {
        }

        public Player(string name)
        {
            Name = name;
        }

        public sealed class ServerListView
        {
            [ForeignKey("PlayerId")] public Player Player { get; set; }
            public string IpAddress { get; set; }
            public int Id { get; set; }

            public bool IsOnline { get; set; }
        }
    }
}