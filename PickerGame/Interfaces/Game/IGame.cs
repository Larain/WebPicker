using System;
using System.Collections.Generic;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Interfaces.Game
{
    public interface IGame
    {
        int GameId { get; }
        DateTime CreatedAt { get; }
        IGameSettings Settings { get; set; }
        int PlayersAmount { get; }
        List<IPlayer> Participiants { get; }
        List<IPlayer> Winners { get; }
        int SecretNumber { get; set; }
        GameState GameState { get; set; }
        IPlayer Owner { get; set; }
        Dictionary<IPlayer, int> AvaibleTurns { get; set; }
    }
}