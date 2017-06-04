using System;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Interfaces.Game
{
    public interface IGame
    {
        int GameId { get; }
        DateTime CreatedAt { get; }
        IGameSettings Settings { get; }

        GameState GameState { get; }
        IPlayer[] Participiants { get; }
        IPlayer[] Winners { get; }
        IPlayer Owner { get; }
        IPlayer PlayerMoving { get; }

        void Start(IPlayer owner);
        void Reset(IPlayer owner);
        int GetLifes(IPlayer player);
        void KickPlayer(IPlayer player);
        int Move(IPlayer caller, int move);
        void JoinPlayer(IPlayer player);
        int TellMeSecret(IPlayer player);
        void SkipPlayerMove(IPlayer player);
    }
}