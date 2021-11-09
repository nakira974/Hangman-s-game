using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Lib;

namespace GameLib
{
    public class Player
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required] [ForeignKey("PlayerId")] public virtual ICollection<Server> Severs { get; set; }

        [Required] public string Name { get; init; }

        public Player()
        {
        }

        public Player(string name)
        {
            Name = name;
        }
    }
}