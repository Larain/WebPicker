using System;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;

namespace WebPicker.Models.LobbyViewModels
{
    public class DetailsViewModel
    {
        public int GameId { get; set; }
        public DateTime CreatedAt { get; set; }
        public GameState GameState { get; set; }
        public int MaxPlayersLimit { get; set; }
        public int PlayersAmout { get; set; }
        public IPlayer Owner { get; set; }
        public bool IsUserOwner { get; set; }
    }
}
