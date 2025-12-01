using System;
using System.Collections.Concurrent;
using Game.Domain.Models;

namespace Game.Api.Services
{
    public interface IGameStore
    {
        GameState CreateGame();
        bool TryGet(Guid id, out GameState state);
    }

    public class InMemoryGameStore : IGameStore
    {
        private readonly ConcurrentDictionary<Guid, GameState> _games = new();

        public GameState CreateGame()
        {
            var state = new GameState();
            _games[state.Id] = state;
            return state;
        }

        public bool TryGet(Guid id, out GameState state)
        {
            return _games.TryGetValue(id, out state);
        }
    }
}
