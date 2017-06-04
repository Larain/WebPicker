using System.Collections.Generic;
using PickerGameModel.Interfaces.Game;

namespace PickerGameModel.Interfaces
{
    public interface IGameRepository
    {
        List<IGame> Games { get; }
    }
}