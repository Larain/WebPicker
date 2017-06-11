using System;
using System.Collections.Generic;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Entities.Game.Base
{
    public abstract class Game : IGame
    {
        public Dictionary<IPlayer, int> AvaibleTurns { get; set; }

        protected Game()
        {
            GameId = GetHashCode();

            CreatedAt = DateTime.Now;

            AvaibleTurns = new Dictionary<IPlayer, int>();
            Participiants = new List<IPlayer>();
            Winners = new List<IPlayer>();

            GameState = GameState.WaitinForPlayers;
        }

        public abstract IGameSettings Settings { get; set; }
        public int PlayersAmount => Participiants.Count;
        public int GameId { get; }
        public DateTime CreatedAt { get; }
        public List<IPlayer> Participiants { get; }
        public List<IPlayer> Winners { get; }
        public IPlayer Owner { get; set; }
        public int SecretNumber { get; set; }
        public GameState GameState { get; set; }
    }
}