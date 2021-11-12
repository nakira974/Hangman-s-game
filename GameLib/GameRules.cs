using System;
using System.Collections.Generic;

namespace GameLib
{
    public abstract class GameRules<T> : IGameRules<T>
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