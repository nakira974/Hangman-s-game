using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLib
{
    internal abstract class GameRules<T> : IGameRules<T>
    {
        public IEnumerable<Player> Players { get; init; }

        protected GameRules()
        {
        }

        protected GameRules(IEnumerable<Player> players)
        {
            Players = players;
        }

        public int GetHashcode()
        {
            throw new NotImplementedException();
        }
    }
}