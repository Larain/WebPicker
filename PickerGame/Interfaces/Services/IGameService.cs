using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;

namespace PickerGameModel.Interfaces.Services
{
    public interface IGameService
    {
        IGame Game { get; set; }
        IPlayer PlayerMoving { get; }
        void JoinPlayer(IPlayer player);
        void Start(IPlayer owner);
        int GetLifes(IPlayer player);
        void KickPlayer(IPlayer player);
        int Move(IPlayer caller, int move);
        void Reset(IPlayer owner);
        int TellMeSecret(IPlayer player);
        void SkipPlayerMove(IPlayer player);
    }
}