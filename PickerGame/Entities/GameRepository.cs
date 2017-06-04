using PickerGameModel.Interfaces;
using System.Collections.Generic;
using PickerGameModel.Entities.Game;
using PickerGameModel.Interfaces.Game;

namespace PickerGameModel.Entities
{
    public class GameRepository : IGameRepository
    {
        public GameRepository()
        {
            Games = new List<IGame> {new DefaultGame()};
        }

        public List<IGame> Games { get; }
    }
}